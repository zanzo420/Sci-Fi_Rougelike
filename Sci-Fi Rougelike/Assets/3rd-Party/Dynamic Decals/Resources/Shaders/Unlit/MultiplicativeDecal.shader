Shader "Projection/Decal/Multiplicative"
{
	Properties
	{
		_Color("Albedo", Color) = (1,1,1,1)
		_MainTex("Albedo Map", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_NormalCutoff("Normal Cutoff", Range(0.0, 1.0)) = 0.5
		
		_TilingOffset("Tiling / Offset", Vector) = (1, 1, 0, 0)

		_MaskBase("Mask Base", Range(0.0, 1.0)) = 0.0
		_MaskLayers("Layers", Color) = (0.5, 0.5, 0.5, 0.5)
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest+1" "DisableBatching" = "True"  "IgnoreProjector" = "True" }
		ZWrite Off ZTest Always Cull Front

		//Forward Base
		Pass
		{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			Blend DstColor Zero

			CGPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma glsl

			#pragma multi_compile __ _PrecisionDepthNormals _CustomDepthNormals
			#pragma multi_compile _ _AlphaTest
			#pragma multi_compile ___ _Omni

			#include "../Cginc/ForwardProjections.cginc"

			#pragma vertex vertProjection
			#pragma fragment fragForward

			half4 fragForward(ProjectionInput i) : SV_Target
			{
				//Setup Instance Data
				UNITY_SETUP_INSTANCE_ID(i);

				//Generate projection
				Projection projection = CalculateProjection(i.screenPos, i.ray, i.worldForward);

				//Generate base data
				FragmentCommonData fragment = FragmentUnlit(projection, i.worldUp, i.eyeVec);

				//Grab color
				half3 c = fragment.diffColor;
				c *= fragment.occlusion;

				//Apply Fog
				UNITY_APPLY_FOG(i.fogCoord, c);
				return Output(c, 1);
			}
			ENDCG
		}
	}
	Fallback Off
}