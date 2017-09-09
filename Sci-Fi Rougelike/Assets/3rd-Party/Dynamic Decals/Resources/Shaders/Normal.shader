Shader "Projection/Decal/Normal"
{
	Properties
	{
		_BumpScale("Normal Strength", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpFlip("Invert Normals", Range(0.0, 1.0)) = 0.0

		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		//Deferred
		Pass
		{
			Name "DEFERRED"
			Tags{ "LightMode" = "Deferred" }

			Blend 0 Zero One
			Blend 1 Zero One
			Blend 2 SrcAlpha OneMinusSrcAlpha, Zero One
			Blend 3 Zero One

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
			#pragma multi_compile ____ _AlphaTest
			#pragma multi_compile _ _Omni

			#include "Cginc/DeferredProjections.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragDeferred

			void fragDeferred(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
			{
				//Setup Instance Data
				UNITY_SETUP_INSTANCE_ID(i);

				//Generate projection
				Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

				//Calculate normals
				float3x3 surface2WorldTranspose = Surface2WorldTranspose(i.worldUp, projection.normal);
				half3 normalWorld = WorldNormal(projection.localUV, surface2WorldTranspose, 1);

				//Normal output
				half4 n = half4(normalWorld, 1.0);
				outNormal = NormalOutput(n);

				//Other outputs
				outAlbedo = half4(0,0,0,0);
				outSmoothSpec = half4(0,0,0,0);
				outEmission = half4(0,0,0,0);
			}
			ENDCG
		}
	}
	Fallback Off
}