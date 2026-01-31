Shader "Custom/Snow Interactive" {
	Properties{
		[Header(Main)]
	
		
		_Noise("Snow Noise", 2D) = "gray" {}
		
		_NoiseScale("Noise Scale", Range(0,2)) = 0.1
		_NoiseWeight("Noise Weight", Range(0,2)) = 0.1
		[HDR]_ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)

		[Space]
		[Header(Tesselation)]
		_MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
		_Tess("Tessellation", Range(1,500)) = 20
	

		[Space]
		[Header(Snow)]
		[HDR]_Color("Snow Color", Color) = (0.5,0.5,0.5,1)
		[HDR]_PathColorIn("Snow Path Color In", Color) = (0.5,0.5,0.7,1)
		[HDR]_PathColorOut("Snow Path Color Out", Color) = (0.5,0.5,0.7,1)
		_PathBlending("Snow Path Blending", Range(0,3)) = 0.3
		_MainTex("Snow Texture", 2D) = "white" {}
		_SnowHeight("Snow Height", Range(0,15)) = 0.3
		_SnowDepth("Snow Path Depth", Range(-2,2)) = 0.3
		_SnowTextureOpacity("Snow Texture Opacity", Range(0,1)) = 0.3
		_SnowTextureScale("Snow Texture Scale", Range(0,2)) = 0.3
		_Normal("Snow Normal", 2D) = "bump" {}
		_SnowNormalStrength("Snow Normal Strength", Range(0,1)) = 0.3

		[Space]
		[Header(Sparkles)]
		_SparkleScale("Sparkle Scale", Range(0,10)) = 10
		_SparkCutoff("Sparkle Cutoff", Range(0,2)) = 0.8
		_SparkleNoise("Sparkle Noise", 2D) = "gray" {}
		_SparkleTriplanarBlend("Sparkle triplanar blend", Range(0,1)) = 1

		[Space]
		[Header(Rim)]
		_RimPower("Rim Power", Range(1,20)) = 20
		[HDR]_RimColor("Rim Color Snow", Color) = (0.5,0.5,0.5,1)
		
		_NormalSampleThickness ("Normal sampling thickness", Range(0,2)) = 0.3 
	}
	HLSLINCLUDE

	// Includes
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
	#include "SnowTessellation.hlsl"
	#pragma vertex TessellationVertexProgram
	#pragma hull hull
	#pragma domain domain
	#pragma instancing_options renderinglayer
	// Keywords
	
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
	#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
	#pragma multi_compile _ _SHADOWS_SOFT
	#pragma multi_compile_fragment _ _LIGHT_LAYERS
	#pragma multi_compile_fog
	#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

	ControlPoint TessellationVertexProgram(Attributes2 v)
	{
		ControlPoint p;
		p.vertex = v.vertex;
		p.uv = v.uv;
		p.normal = v.normal;
		p.tangent = v.tangent;
		return p;
	}
	ENDHLSL

	SubShader{
		Tags
		{
			"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
		}

		Pass{
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			// vertex happens in snowtessellation.hlsl
			#pragma require tessellation tessHW
			#pragma fragment frag
			#pragma target 4.0
			

			sampler2D _MainTex, _SparkleNoise;
			float4 _Color, _RimColor;
			float _RimPower;
			float4 _PathColorIn, _PathColorOut;
			float _PathBlending;
			float _SparkleScale, _SparkCutoff;
			float _SnowTextureOpacity, _SnowTextureScale;
			float4 _ShadowColor;
			float _SnowNormalStrength;
			float _SparkleTriplanarBlend;

			half4 triplanar(Varyings2 IN, sampler2D tex, float blend, float scale)
			{
				float3 uvTri = IN.worldPos * scale;
				float3 triBlend = pow(abs(IN.normal), blend);
				triBlend /= dot(triBlend, 1.0);
				float4 x = tex2D(tex, uvTri.zy);
				float4 y = tex2D(tex, uvTri.xz);
				float4 z = tex2D(tex, uvTri.xy);
				return x * triBlend.x + y * triBlend.y + z * triBlend.z;
			}

			half4 frag(Varyings2 IN) : SV_Target{

				// Effects RenderTexture Reading
				float3 worldPosition = mul(unity_ObjectToWorld, IN.vertex).xyz;
				
				float2 uv = IN.worldPos.xz - _Position.xz;
				uv /= (_OrthographicCamSize * 2);
				uv += 0.5;
				uv.x = 1 - uv.x;

				// effects texture				
				float4 effect = tex2D(_GlobalEffectRT, uv);				

				// mask to prevent bleeding
				effect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
				effect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
				
				// worldspace Snow texture
				//float3 snowtexture = tex2D(_MainTex, IN.worldPos.xz * _SnowTextureScale).rgb;
				float4 snowtexture = triplanar(IN, _MainTex, 1, _SnowTextureScale);

				// snow normal
				float3 snownormal = UnpackNormal(
					tex2D(_Normal, IN.worldPos.xz * _NoiseScale)).rgb;
				snownormal = snownormal.r * IN.tangent + snownormal.g * IN.
					bitangent + snownormal.b * IN.normal;


				//lerp between snow color and snow texture
				float3 snowTex = lerp(_Color.rgb, snowtexture * _Color.rgb, _SnowTextureOpacity);
				
				//lerp the colors using the RT effect path 
				float3 path = lerp(_PathColorOut.rgb * IN.snowDepthT, _PathColorIn.rgb, saturate(IN.snowDepthT * _PathBlending));
				float3 mainColors = lerp(snowTex,path, saturate(IN.snowDepthT));

				// lighting and shadow information
				float shadow = 0;
				half4 shadowCoord = TransformWorldToShadowCoord(IN.worldPos);
				
				#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
					Light mainLight = GetMainLight(shadowCoord);
					shadow = mainLight.shadowAttenuation;
				#else
					Light mainLight = GetMainLight();
				#endif

				// extra point lights support
				float3 extraLights;
				int pixelLightCount = GetAdditionalLightsCount();
				for (int j = 0; j < pixelLightCount; ++j) {
					Light light = GetAdditionalLight(j, IN.worldPos, half4(1, 1, 1, 1));
					float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
					extraLights += attenuatedLightColor;			
				}

				float4 litMainColors = float4(mainColors,1) ;
				extraLights *= litMainColors.rgb;
				// add in the sparkles

				float4 sparkle = triplanar(IN, _SparkleNoise, _SparkleTriplanarBlend, _SparkleScale);
				
				//float sparklesStatic = tex2D(_SparkleNoise, IN.worldPos.xz * _SparkleScale).r;
				//float cutoffSparkles = step(_SparkCutoff,sparklesStatic);

				float3 refl = normalize(reflect(-mainLight.direction, snownormal));
				//float3 toCam = normalize(GetCameraPositionWS() - IN.worldPos);
				float3 toCam = normalize(GetWorldSpaceViewDir(IN.worldPos));

				//return half4(refl,1);
				//return half4(snownormal, 1);

				float rDot = dot(refl, toCam);
				//return half4(rDot, 0,0,1);

				float nDot = dot(snownormal, -mainLight.direction);
				//return half4(-nDot, 0,0,1);

				float sparkleStr = saturate(rDot * -nDot);
				//return half4(sparkleStr, 0,0,1);

				float cutoffSparkles = step(_SparkCutoff, sparkle * step(0.01, sparkleStr));

				cutoffSparkles *= saturate(
					sign(dot(-mainLight.direction, float3(0, -1, 0))));
				//cutoffSparkles *= saturate(sign(-nDot));

				//return half4(cutoffSparkles, 0, 0, 1);

				litMainColors += lerp(cutoffSparkles * 4, 0,
				                      saturate(IN.snowDepthT * 2));
				
				// add rim light
				half rim = 1.0 - dot(normalize(lerp(snownormal, IN.normal, _SnowNormalStrength)), normalize(IN.viewDir));
				// no rim inside of the path
				rim = lerp(rim, 0, saturate(IN.snowDepthT));
				litMainColors += _RimColor * pow(rim, _RimPower);

				// ambient and mainlight colors added
				half4 extraColors;
				extraColors.rgb = litMainColors.rgb * mainLight.color.rgb * (shadow + unity_AmbientSky.rgb);
				extraColors.a = 1;

				// colored shadows
				float3 coloredShadows = shadow + lerp(_ShadowColor, 0, shadow);
				litMainColors.rgb = litMainColors.rgb * mainLight.color * (coloredShadows);
				
				// everything together
				float4 final = litMainColors+ extraColors + float4(extraLights,0);
				// add in fog
				final.rgb = MixFog(final.rgb, IN.fogFactor);
				return final;

			}
			ENDHLSL

		}


		// depth only pass to fix invisiblity when turning on Depth Priming mode
		   Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask RGB
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0
            
			float _NormalSampleThickness;

            half4 frag(Varyings2 IN) : SV_Target
            {
            	float3 snownormal = UnpackNormal(tex2D(_Normal, IN.worldPos.xz * _NoiseScale)).rgb;
			    snownormal = snownormal.r * IN.tangent + snownormal.g * IN.bitangent + snownormal.b * IN.normal;
	           //float3 normal = mul(unity_ObjectToWorld, IN.normal).xyz;
            	float3 normal = mul(unity_ObjectToWorld, snownormal).xyz;
	            return float4(normal, 1);
            }

            // -------------------------------------
            // Shader Stages
            //#pragma vertex DepthVertex
            #pragma fragment frag

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

		// casting shadows is a little glitchy, I've turned it off, but maybe in future urp versions it works better?
		// Shadow Casting Pass
		Pass
		{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
			    ZWrite On
            ZTest LEqual
            ColorMask 0
				HLSLPROGRAM
				#pragma target 3.0
			
				// Support all the various light  ypes and shadow paths
				#pragma multi_compile_shadowcaster
			    // Unity defined keywords
				#pragma multi_compile _ LOD_FADE_CROSSFADE
				   // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
				   #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
				// Register our functions
			
				#pragma fragment frag
				// A custom keyword to modify logic during the shadow caster pass

				half4 frag(Varyings2 IN) : SV_Target{
						return 0;
				}
			
				ENDHLSL
		}
	}
}