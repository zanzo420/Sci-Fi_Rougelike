Shader "Projection/Decal/Specular/DeferredAddTransparent"
{
	Properties
	{
		_Color("Albedo", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular Gloss Map", 2D) = "white" {}

		_BumpScale("Normal Strength", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Emission", Color) = (0,0,0)
		_EmissionMap("Emission Map", 2D) = "white" {}
		
		_TilingOffset("Tiling / Offset", Vector) = (1, 1, 0, 0)

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		Pass
		{
			Name "DEFERRED"
			Tags{ "LightMode" = "Deferred" }

			BlendOp 0 Max
			BlendOp 1 Max
			BlendOp 2 Max
			BlendOp 3 Max

			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile_instancing
			#pragma exclude_renderers nomrt
			#pragma glsl

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile ___ DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma multi_compile __ _PrecisionDepthNormals _CustomDepthNormals
			#pragma multi_compile _ _Omni
			
			#include "../Cginc/DeferredProjections.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragGloss

			void fragGloss(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
			{
				//Setup Instance Data
				UNITY_SETUP_INSTANCE_ID (i);

				//Generate projection
				Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

				//Generate base data
				FragmentCommonData fragment = FragmentSpecular(projection, i.worldUp, i.eyeVec);

				//Calculate ambient & reflections
				half3 a = Ambient(fragment);

				//Specsmooth output
				half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);

				outAlbedo = half4(0,0,0,0);
				outSmoothSpec = SpecSmoothOutputPassTwo(s, fragment.occlusion);
				outNormal = half4(0,0,0,0);
				outEmission = half4(0,0,0,0);
			}
			ENDCG
		}
	}
	Fallback Off
}