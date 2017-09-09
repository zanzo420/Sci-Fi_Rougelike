﻿Shader "Projection/Decal/Metallic/DeferredOpaque"
{
	Properties
	{
		_Color("Albedo", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic Gloss Map", 2D) = "white" {}

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
		Tags{ "Queue" = "AlphaTest+40" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		//Deferred
		Pass
		{
			Name "DEFERRED"
			Tags{ "LightMode" = "Deferred" }

			Blend One Zero

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
			
			#include "../Cginc/DeferredProjections.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragMetallic

			void fragMetallic(ProjectionInput i, out half4 outAlbedo : SV_Target, out half4 outSmoothSpec : SV_Target1, out half4 outNormal : SV_Target2, out half4 outEmission : SV_Target3)
			{
				//Setup Instance Data
				UNITY_SETUP_INSTANCE_ID (i);

				//Generate projection
				Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

				//Generate base data
				FragmentCommonData fragment = FragmentMetallic(projection, i.worldUp, i.eyeVec);

				//Calculate ambient & reflections
				half3 a = Ambient(fragment);

				//Emission
				a += EmissionAlpha(projection.localUV);

				//Albedo output
				half3 c = fragment.diffColor;
				outAlbedo = AlbedoOutput(c, fragment.occlusion);
				//Specsmooth output
				half4 s = half4(fragment.specColor, fragment.oneMinusRoughness);
				outSmoothSpec = SpecSmoothOutput(s, fragment.occlusion);
				//Normal output
				half4 n = half4(fragment.normalWorld, 1.0);
				outNormal = NormalOutput(n);
				//Emission output
				outEmission = EmissionOutput(half4(a, 1.0), fragment.occlusion);
			}
			ENDCG
		}
	}
	Fallback Off
}