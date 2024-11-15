Shader "Hidden/Distortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	_Distortion ("Distortion Texture", 2D) = "black" {}
    	_Warp ("Warp Texture", 2D) = "black" {}
    }
    SubShader 
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		
		Pass
		{
			HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
			
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_Distortion);
            SAMPLER(sampler_Distortion);

            TEXTURE2D(_Warp);
            SAMPLER(sampler_Warp);
			
            
			float4 _MainTex_TexelSize;

            float _Intensity;
            float _Glow;

            #define _SCREEN_SPACE_OCCLUSION
            
            struct Attributes
            {
                float4 vertex       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
            	UNITY_VERTEX_OUTPUT_STEREO
			};

            Varyings vert(Attributes v)
			{
				Varyings o;
            	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = v.uv;
				

				return o;
			}

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
			// Combines the top and bottom colors using normal blending.
			// https://en.wikipedia.org/wiki/Blend_modes#Normal_blend_mode
			// This performs the same operation as Blend SrcAlpha OneMinusSrcAlpha.
			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

			//#define _HigherFidelity
			float4 frag(Varyings i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				// Get Noise
				float4 warp = SAMPLE_TEXTURE2D(_Warp, sampler_Warp, i.uv + _Time.x);

				// Get Warped Distortion
				float4 distort = SAMPLE_TEXTURE2D(_Distortion, sampler_Distortion, i.uv + (0.02f * warp - 0.005));

				float tentacleMask = distort.g;
				float shatterMask = distort.r;

				tentacleMask = 1 - tentacleMask;
				//tentacleMask = smoothstep(0, 1, _Intensity - tentacleMask);
				float tentacleMask1 = smoothstep(0, 1, _Intensity - tentacleMask);
				tentacleMask = tentacleMask< _Intensity ? 1 : 0;
				
				tentacleMask = tentacleMask * 0.5f + tentacleMask1;
				shatterMask = shatterMask < _Intensity ? 1 : 0;

				// Get Main Texture and blend in
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, lerp(i.uv, (0.5f-(i.uv - 0.5f)), tentacleMask));

				color = alphaBlend(float4((warp.rgb * 0.015f), saturate(shatterMask - tentacleMask)), color);

				//	tentacleMask = tentacleMask - tentacleMask1;
				color = alphaBlend(float4(_Glow.rrr, saturate(tentacleMask - 0.1f)*0.1f), color);
				//return tentacleMask;
				return color;
			}
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}