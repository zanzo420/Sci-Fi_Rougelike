Shader "Projection/Decal/Dry"
{
	Properties
	{
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMap("Gloss Map", 2D) = "white" {}

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

			BlendOp Min

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

				//Specsmooth output
				float w = 1 - (_Glossiness * tex2D(_GlossMap, projection.localUV).r);
				outSmoothSpec = half4(100,100,100, w);

				//Other outputs
				outAlbedo = half4(100,100,100,100);
				outNormal = half4(100,100,100,100);
				outEmission = half4(100,100,100,100);
			}
			ENDCG
		}
	}
	Fallback Off
}