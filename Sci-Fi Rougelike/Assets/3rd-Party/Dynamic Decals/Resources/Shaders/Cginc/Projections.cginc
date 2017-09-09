//Includes
#include "UnityCG.cginc"
#include "UnityInstancing.cginc"

#include "UnityStandardUtils.cginc"
#include "UnityPBSLighting.cginc"

#include "AutoLight.cginc"

//Unity screenspace samplers
uniform sampler2D_float _CameraDepthTexture;
uniform sampler2D_float _CameraDepthNormalsTexture;

//Custom screenspace samplers
uniform sampler2D _CustomDepthTexture;
uniform sampler2D _CustomNormalTexture;

//Masking screenspace sampler
uniform sampler2D _MaskBuffer_0;

//Instanced parameters
UNITY_INSTANCING_CBUFFER_START (MyProperties)
UNITY_DEFINE_INSTANCED_PROP (float4, _Color)
UNITY_DEFINE_INSTANCED_PROP (float, _Multiplier)
UNITY_DEFINE_INSTANCED_PROP (float4, _EmissionColor)
UNITY_DEFINE_INSTANCED_PROP (float4, _TilingOffset)
UNITY_DEFINE_INSTANCED_PROP (float4, _MaskLayers)
UNITY_DEFINE_INSTANCED_PROP (float, _MaskBase)
UNITY_INSTANCING_CBUFFER_END

//Shader parameters
sampler2D _MainTex;

float _Glossiness;

float _Metallic;
sampler2D _MetallicGlossMap;

//half4 _SpecColor;
sampler2D _SpecGlossMap;

float _BumpScale;
sampler2D _BumpMap;
float _BumpFlip;

sampler2D _EmissionMap;

float _Cutoff;
float _NormalCutoff;

//Vertex function
struct VertexInput
{
	float4 vertex	: POSITION;
	half3 normal	: NORMAL;
	float2 uv0		: TEXCOORD0;
	float2 uv1		: TEXCOORD1;

	#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
	float2 uv2		: TEXCOORD2;
	#endif

	#ifdef _TANGENT_TO_WORLD
	half4 tangent	: TANGENT;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

//Projection struct
struct Projection
{
	float2 screenPos;
	float3 posWorld;
	float2 localUV;

	float depth;
	float3 normal;
};

//Depth & Normal input
float3 MobileDepthNormal(float2 ScreenPos, out float Depth)
{
	float3 surfaceNormal;

	//Grab our view space normal & depth
	DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, ScreenPos), Depth, surfaceNormal);

	//Convert to worldspace normal
	return mul(float4(surfaceNormal, 1.0), UNITY_MATRIX_V).xyz;
}
float3 PrecisionDepthNormal(float2 ScreenPos, out float Depth)
{
	//Orthographic depth
	float orthDepth = tex2D(_CameraDepthTexture, ScreenPos).r;
	#ifdef UNITY_REVERSED_Z
	orthDepth = 1 - orthDepth;
	#endif
	orthDepth += _ProjectionParams.y / (_ProjectionParams.z - _ProjectionParams.y);

	//Perspective depth
	float perpDepth = Linear01Depth(tex2D(_CameraDepthTexture, ScreenPos).r);

	//Blended depth
	Depth = (UNITY_MATRIX_P[3][3] * orthDepth) + ((1 - UNITY_MATRIX_P[3][3]) * perpDepth);

	//Grab our view space normal
	float3 surfaceNormal = (tex2D(_CustomNormalTexture, ScreenPos).xyz - 0.5) * 2;

	//Convert to worldspace normal
	return mul(float4(surfaceNormal, 1.0), UNITY_MATRIX_V).xyz;
}
float3 CustomDepthNormal(float2 ScreenPos, out float Depth)
{
	//Orthographic depth
	float orthDepth = tex2D(_CustomDepthTexture, ScreenPos).r;
	#ifdef UNITY_REVERSED_Z
	orthDepth = 1 - orthDepth;
	#endif
	orthDepth += _ProjectionParams.y / (_ProjectionParams.z - _ProjectionParams.y);
	
	//Perspective depth
	float perpDepth = Linear01Depth(tex2D(_CustomDepthTexture, ScreenPos).r);
	
	//Blended depth
	Depth = (UNITY_MATRIX_P[3][3] * orthDepth) + ((1 - UNITY_MATRIX_P[3][3]) * perpDepth);

	//Grab our view space normal
	float3 surfaceNormal = (tex2D(_CustomNormalTexture, ScreenPos).xyz - 0.5) * 2;

	//Convert to worldspace normal
    return mul(float4(surfaceNormal, 1.0), UNITY_MATRIX_V).xyz;
}

float3 DepthNormal(float2 ScreenPos, out float Depth)
{
    float3 normalWorld;

	normalWorld = MobileDepthNormal(ScreenPos, Depth);

	#if defined (_CustomDepthNormals)
	normalWorld = CustomDepthNormal(ScreenPos, Depth);
	#endif

	#if defined (_PrecisionDepthNormals)
	normalWorld = PrecisionDepthNormal(ScreenPos, Depth);
	#endif

    return normalWorld;
}

//World pos from depth
float3 CalculatePerspectiveWorldPos(float3 Ray, float Depth)
{
	//Set magnitude of ray to far clipping plane
	Ray = Ray * (_ProjectionParams.z / Ray.z);

	//Calculate view position
	float4 viewpos = float4(Ray * Depth, 1);

	//Calculate & return world position
	return mul(unity_CameraToWorld, viewpos).xyz;
}
float3 CalculateOrthographicWorldPos(float2 ScreenPos, float Depth)
{
	//Calculate position in view space
	float4 viewpos = float4(unity_OrthoParams.x * ((ScreenPos.x * 2) - 1), unity_OrthoParams.y * ((ScreenPos.y * 2) - 1), (Depth * (_ProjectionParams.z - _ProjectionParams.y)), 1);
	
	//Calculate & return world position
	return mul(unity_CameraToWorld, viewpos).xyz;
}

//Clip pixels & return local uvs
half2 DecalUVs(float3 WorldPos)
{
	//Calculate position in object space
	float3 opos = mul(unity_WorldToObject, float4(WorldPos, 1)).xyz;

	//Remove pixels outside the bounds of our object
	clip(float3(0.5, 0.5, 0.5) - abs(opos.xyz));

	//Calculate local uvs, projecting along Z, so using xy position as co-ordinates
	float2 UnscaledUVs = float2(opos.xy + 0.5);

	float4 tilingOffset = UNITY_ACCESS_INSTANCED_PROP(_TilingOffset);

	return (UnscaledUVs * tilingOffset.xy) + tilingOffset.zw;
}
half2 OmniUVs(float3 WorldPos)
{
	//Calculate position in object space
	float3 opos = mul(unity_WorldToObject, float4(WorldPos, 1)).xyz;

	//Remove pixels outside the bounds of our object
	clip(float3(0.5, 0.5, 0.5) - abs(opos.xyz));

	//Calculate local uvs, projecting along Z, so using xy position as co-ordinates
	float magnitude = length(opos) / 0.5;

	//Remove pixels outside the range of our OmniDecal
	clip(1 - magnitude);

	return float2(magnitude, 0.5);
}

//Clip masking
void ClipMasking(float2 uv)
{
	half mask = UNITY_ACCESS_INSTANCED_PROP(_MaskBase);
	half4 maskLayers = UNITY_ACCESS_INSTANCED_PROP(_MaskLayers);
	half4 maskBuffer = tex2D(_MaskBuffer_0, uv);

	mask += maskBuffer.x * (maskLayers.x * 2 - 1);
	mask += maskBuffer.y * (maskLayers.y * 2 - 1);
	mask += maskBuffer.z * (maskLayers.z * 2 - 1);
	mask += maskBuffer.w * (maskLayers.w * 2 - 1);

	clip(mask - 0.1);
}
//Clip normals
void ClipNormal(float3 surfaceNormal, half3 worldForward)
{
	//Use dot product to determine angle & clip
	float d = dot(surfaceNormal, normalize(-worldForward));
	clip(d - _NormalCutoff);
}

//Calculate projection
Projection CalculateProjection(float4 i_screenPos, float3 i_ray, half3 i_worldForward)
{
	Projection proj = (Projection)0;

	//Calculate screenspace position
	proj.screenPos = i_screenPos.xy / i_screenPos.w;

	//Calculate depth & normals
	proj.normal = DepthNormal(proj.screenPos, proj.depth);

	//Calculate world position - Blended
	proj.posWorld = (UNITY_MATRIX_P[3][3] * CalculateOrthographicWorldPos(proj.screenPos, proj.depth)) + ((1 - UNITY_MATRIX_P[3][3]) * CalculatePerspectiveWorldPos(i_ray, proj.depth));

	//Calculate local uvs
	#ifdef _Omni
	proj.localUV = OmniUVs(proj.posWorld);
	#else
	proj.localUV = DecalUVs(proj.posWorld);
	ClipNormal(proj.normal, i_worldForward);
	#endif

	//Clip masking
	ClipMasking(proj.screenPos);

	return proj;
}

//Fragment common data (Modified Unity Struct)
struct FragmentCommonData
{
	half occlusion;
	half oneMinusReflectivity;
	half oneMinusRoughness;
	
	half3 diffColor;
	half3 specColor;
	half3 normalWorld; 
	half3 eyeVec;

	half3 posWorld;
};

//GI
inline UnityGI FragmentGI(FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
{
	UnityGIInput d;

	d.light = light;
	d.worldPos = s.posWorld;
	d.worldViewDir = -s.eyeVec;
	d.atten = atten;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	d.ambient = 0;
	d.lightmapUV = i_ambientOrLightmapUV;
#else
	d.ambient = i_ambientOrLightmapUV.rgb;
	d.lightmapUV = 0;
#endif

	d.probeHDR[0] = unity_SpecCube0_HDR;
	d.probeHDR[1] = unity_SpecCube1_HDR;
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
	d.boxMax[0] = unity_SpecCube0_BoxMax;
	d.probePosition[0] = unity_SpecCube0_ProbePosition;
	d.boxMax[1] = unity_SpecCube1_BoxMax;
	d.boxMin[1] = unity_SpecCube1_BoxMin;
	d.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif

	if (reflections)
	{
		Unity_GlossyEnvironmentData g;
		g.roughness = 1 - s.oneMinusRoughness;
		g.reflUVW = reflect(s.eyeVec, s.normalWorld);
		return UnityGlobalIllumination(d, occlusion, s.normalWorld, g);
	}
	else
	{
		return UnityGlobalIllumination(d, occlusion, s.normalWorld);
	}
}

//Input
half AlbedoOcclusion(float2 localUvs)
{
	//Calculate alpha
	float alpha = UNITY_ACCESS_INSTANCED_PROP(_Color).a;

	#if !defined(SHADER_API_D3D11_9X)
	alpha *= tex2Dlod(_MainTex, float4(localUvs, 0, 0)).a;
	#else
	alpha *= tex2D(_MainTex, localUvs).a;
	#endif

	//Clip alpha
	clip(alpha - _Cutoff);

	//Alpha test
	#if defined(_AlphaTest)
	alpha = 1.0;
	#endif

	return alpha;
}
half ShapeOcclusion(float2 localUvs)
{
	//Calculate alpha
	float alpha = UNITY_ACCESS_INSTANCED_PROP(_Multiplier);

	#if !defined(SHADER_API_D3D11_9X)
	alpha *= tex2Dlod(_MainTex, float4(localUvs, 0, 0)).r;
	#else
	alpha *= tex2D(_MainTex, localUvs).r;
	#endif

	//Clip alpha
	clip(alpha - _Cutoff);

	//Alpha test
	#if defined(_AlphaTest)
	alpha = 1.0;
	#endif

	return alpha;
}

half3 Albedo(float2 localUvs)
{
	half3 color = UNITY_ACCESS_INSTANCED_PROP (_Color).rgb;
	return color * tex2D(_MainTex, localUvs).rgb;
}
half4 SpecGloss(float2 localUvs)
{
	half4 sg = tex2D(_SpecGlossMap, localUvs);
	sg.rgb *= _SpecColor.rgb;
	sg.a *= _Glossiness;

	return sg;
}
half2 MetalGloss(float2 localUvs)
{
	half2 mg = tex2D(_MetallicGlossMap, localUvs).ra;
	mg.r *= _Metallic;
	mg.g *= _Glossiness;
	return mg;
}
float3x3 Surface2WorldTranspose(float3 WorldUp, float3 surfaceNormal)
{
	//Calculate bi-normal
	float3 binormalWorld = normalize(cross(WorldUp, surfaceNormal));

	//Calculate tangent
	float3 tangentWorld = normalize(cross(surfaceNormal, binormalWorld));

	//Object 2 World Matrix
	return float3x3(tangentWorld, binormalWorld, surfaceNormal);
}
half3 WorldNormal(float2 localUvs, float3x3 Surface2WorldTranspose, float scale)
{
	//Grab & Scale Normal Map
	float3 normalMap = UnpackNormal(tex2D(_BumpMap, localUvs));
	normalMap.z /= clamp(_BumpScale, 0.1, 4) * scale;
	normalMap = normalize(normalMap);

	normalMap.y = lerp(normalMap.y, -normalMap.y, _BumpFlip);

	//Transform normal from tangent space into view space
	half3 normal = mul(normalMap, Surface2WorldTranspose);
	return normalize(normal);
}
half3 EmissionAlpha(float2 localUvs)
{
	half4 color = UNITY_ACCESS_INSTANCED_PROP(_EmissionColor);
	half4 Emission = tex2D(_EmissionMap, localUvs) * color;
	return Emission.rgb;
}

//Fragment setups
inline FragmentCommonData FragmentUnlit(Projection i_projection, half3 i_worldUp, half3 i_eyeVec)
{
	FragmentCommonData o = (FragmentCommonData)0;

	//Occlusion
	o.occlusion = AlbedoOcclusion(i_projection.localUV);

	//Calculate world normals
	#if SHADER_TARGET < 30
	//Don't use normal maps when using shader model 2.0
	#else
	float3x3 surface2WorldTranspose = Surface2WorldTranspose(i_worldUp, i_projection.normal);
	o.normalWorld = WorldNormal(i_projection.localUV, surface2WorldTranspose, o.occlusion);
	#endif

	//Pass in variables
	o.eyeVec = normalize(i_eyeVec);
	o.posWorld = i_projection.posWorld;

	//Set default fragment values
	o.diffColor = Albedo(i_projection.localUV);
	o.specColor = half3(0, 0, 0);
	o.oneMinusReflectivity = 1;
	o.oneMinusRoughness = 0;
	return o;
}
inline FragmentCommonData FragmentSpecular(Projection i_projection, half3 i_worldUp, half3 i_eyeVec)
{
	//Grab base data
	FragmentCommonData o = FragmentUnlit(i_projection, i_worldUp, i_eyeVec);

	half4 specGloss = SpecGloss(i_projection.localUV);
	half3 specColor = specGloss.rgb;
	half oneMinusRoughness = specGloss.a;

	half oneMinusReflectivity;
	half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular(Albedo(i_projection.localUV), specColor, oneMinusReflectivity);

	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
}
inline FragmentCommonData FragmentMetallic(Projection i_projection, half3 i_worldUp, half3 i_eyeVec)
{
	//Grab base data
	FragmentCommonData o = FragmentUnlit(i_projection, i_worldUp, i_eyeVec);

	half2 metallicGloss = MetalGloss(i_projection.localUV);
	half metallic = metallicGloss.x;
	half oneMinusRoughness = metallicGloss.y;

	half oneMinusReflectivity;
	half3 specColor;
	half3 diffColor = DiffuseAndSpecularFromMetallic(Albedo(i_projection.localUV), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.oneMinusRoughness = oneMinusRoughness;
	return o;
}