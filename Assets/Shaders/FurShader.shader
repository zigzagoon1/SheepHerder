Shader "Custom/FurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FurTex ("Fur pattern", 2D) = "white" {}
        _Diffuse ("Diffuse value", Range(0, 3)) = 1
 
        _FurLength ("Fur length", Range(0.0, 1)) = 0.5
        _CutOff ("Alpha cutoff", Range(0, 1)) = 0.5
        _Blur ("Blur", Range(0, 1)) = 0.5
        _Thickness ("Thickness", Range(0, 1)) = 0

        _Gravity ("Gravity Direction", Vector) = (0, 0.1, 0)
        _GravityStrength ("Gravity Strength", Range(0,1)) = 0.25

        _Ambient ("Ambient Reflection", Range(0, 1)) = 0.1
        _Emission ("Emissive Reflection", Range(0,1)) = 0.1
        _Shininess ("Shininess", Range(0, 100)) = 1
        _Specular ("Specular", Range(0, 1)) = 0.1
        _Dif2 ("Diffuse2", Range(0,2)) = 0.5

        _WindTex ("Wind Texture", 2D) = "white" {}
        _WindFrequency ("Wind Frequency/Speed", float) = 0.1
        _WindStrength ("Wind Strength", float) = 0.1
    }
 
    CGINCLUDE
 
        fixed _Diffuse;
                    struct MaterialParameters 
            {
                float3 Ke;
                float3 Ka;
                float3 Kd;
                float3 Ks;
                float shininess;
            };
            struct LightParameters
            {
                float3 position;
                float3 color;
            };

        float3 lighting(MaterialParameters material, LightParameters light, float3 globalAmbient, float3 P, float3 N, float3 eyePosition)

{
   float3 color;
  // Compute the emissive term

  float3 emissive = material.Ke;



  // Compute the ambient term

  float3 ambient = material.Ka * globalAmbient;



  // Compute the diffuse term

  float3 L = normalize(light.position.xyz - P);

  float diffuseLight = max(dot(N, L), 0);

  float3 diffuse = material.Kd * light.color.rgb * diffuseLight;



  // Compute the specular term

  float3 V = normalize(eyePosition - P);

  float3 H = normalize(L + V);

  float specularLight = pow(max(dot(N, H), 0),

                            material.shininess);

  if (diffuseLight <= 0) specularLight = 0;

  float3 specular = material.Ks * light.color.rgb * specularLight;



  color.xyz = emissive + ambient + diffuse + specular;

  
  return color;
}

        inline fixed4 LambertDiffuse(float3 worldNormal)
        {
            float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = max(0, dot(worldNormal, lightDir));
            return NdotL * _Diffuse;
        }
 
    ENDCG
 
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
 
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD2; 
                float3 normal : TEXCOORD3;
                fixed4 dif : COLOR;
                float3 lightDir : TEXCOORD4;
            };


        
            

            float4 Specular(v2f i) : SV_TARGET
        {
            float4 diffuse = float4(1.0, 0.0, 0.0, 1.0);
            float4 finalColor = diffuse * 0.1;
            float3 eye = _WorldSpaceCameraPos;
            float4 intensity = 0.2; 
            float power = 4;

            float3 L = -normalize(i.lightDir.xyz);
            float3 V = normalize(_WorldSpaceCameraPos - i.pos);
            float H = normalize(L + V);
            float R = reflect(i.lightDir, normalize(i.normal));
            finalColor += intensity * pow(saturate(dot(i.normal, H)), power);
            return finalColor;
        }
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Ambient;
            float _Emission;
            float _Specular;
            float _Dif2;
            float _Shininess;
 
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.dif = LambertDiffuse(v.normal);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.lightDir = normalize(_WorldSpaceLightPos0).xyz;
                return o;
            }
    
            fixed4 frag (v2f i) : SV_Target
            {
                float3 V = normalize(_WorldSpaceCameraPos - i.normal);
                fixed4 col = tex2D(_MainTex, i.uv);
                //col.rgb *= Specular(i);


                MaterialParameters lightingMat;
                lightingMat.Ke = float3 (0.0, 0.0, 0.0) * _Emission;
                lightingMat.Ka = float3(0.1, 0.1, 0.1) * _Ambient;
                lightingMat.Kd = float3(0.2, 0.4, 0.2) * _Dif2;
                lightingMat.Ks = float3(0.8, 0.8, 0.8) * _Specular;
                lightingMat.shininess = 1 * _Shininess;

                LightParameters light;
                light.position = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                light.color = lightColor;
                float3 globAmbient = float3 (1, 1, 1);
                globAmbient.rgb *= _Ambient;
                col.rgb *= lighting(lightingMat, light, globAmbient, i.pos, i.normal, _WorldSpaceCameraPos);
                col.a = 1;
                return col;
            }
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.05
            #define index 2
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.10
            #define index 3
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.15
            #define index 4
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.20
            #define index 5
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.25
            #define index 6
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.30
            #define index 7
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.35
            #define index 8
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.40
            #define index 9
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.45
            #define index 10
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.50
            #define index 11
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.55
            #define index 12
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.60
            #define index 13
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.65
            #define index 14
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.70
            #define index 15
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.75
            #define index 16
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.80
            #define index 17
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.85
            #define index 18
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.90
            #define index 19
            #include "FurHelper.cginc"
            ENDCG
        }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define FURSTEP 0.95
            #define index 20
            #include "FurHelper.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo
            #include "UnityCG.cginc"
            #include "Lighting.cginc"


            float _FurLength;
            sampler2D _MainTex;
            sampler2D _FurTex;
            float _Emission;
            float _Ambient;
            float _Dif2;
            float _Specular;
            float _Shininess;
            struct v2g 
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;

            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };


            v2g vert(appdata_base vertexInput)
            {
                v2g output;
                output.pos = vertexInput.vertex;
                output.normal = vertexInput.normal;
                output.texcoord = vertexInput.texcoord;
                return output;
            }
                        
            void CreateFinVertex(inout TriangleStream<g2f> triStream, float3 position, float3 normal, float2 texcoord, float2 texcoord1)
            {
                g2f o;
                
                float4 worldPos = mul(unity_ObjectToWorld, float4(position, 1.0));
                float3 gravity = float3((float) sin(_Time * 1.5) * 1.2, 2, 0);
                //gravity.x = (float) sin(_Time * 1.5) * 1.2;
                //gravity.y *= 2;
                //gravity.z = 0;
                worldPos.xyz += gravity;

                o.pos = worldPos;
                o.normal = mul(unity_ObjectToWorld, normal);
                o.texcoord = texcoord;
                o.texcoord1 = texcoord1;
                triStream.Append(o);
            }

            [maxvertexcount(4)]
            void geo(line v2g input[2], inout TriangleStream<g2f> triStream)
            {
                float3 midPoint = (input[0].pos + input[1].pos);
                midPoint = midPoint / 2;
                float3 avgNormal = (input[0].normal + input[1].normal);
                avgNormal = avgNormal / 2;
                float3 viewDir = WorldSpaceViewDir(float4(midPoint.xyz, 1));
                viewDir = viewDir.xyz;

                float dotProd = dot(avgNormal, -viewDir);
                float threshHold = 0.1;
                if (dotProd < threshHold && dotProd > -threshHold)
                {
                    CreateFinVertex(triStream, input[0].pos, input[0].normal, input[0].texcoord, float2(0,1));
                    CreateFinVertex(triStream, input[1].pos, input[1].normal, input[1].texcoord, float2(1,1));

                    CreateFinVertex(triStream, input[0].pos + input[0].normal * _FurLength, input[0].normal, input[0].texcoord, float2(0,0));
                    CreateFinVertex(triStream, input[1].pos + input[1].normal * _FurLength, input[1].normal, input[1].texcoord, float2(1,0));
                }

            }


            fixed4 frag(g2f i) : SV_Target
            {
             //   fixed4 col = tex2D(_MainTex, i.texcoord);
	            //fixed alpha = tex2D(_FurTex, i.texcoord1).r;

             //   return col;

                float3 V = normalize(_WorldSpaceCameraPos - i.normal);
                fixed4 col = tex2D(_MainTex, i.texcoord);
                //col.rgb *= Specular(i);


                MaterialParameters lightingMat;
                lightingMat.Ke = float3 (0.0, 0.0, 0.0) * _Emission;
                lightingMat.Ka = float3(0.1, 0.1, 0.1) * _Ambient;
                lightingMat.Kd = float3(0.2, 0.4, 0.2) * _Dif2;
                lightingMat.Ks = float3(0.8, 0.8, 0.8) * _Specular;
                lightingMat.shininess = 1 * _Shininess;

                LightParameters light;
                light.position = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                light.color = lightColor;
                float3 globAmbient = float3 (1, 1, 1);
                globAmbient.rgb *= _Ambient;
                col.rgb *= lighting(lightingMat, light, globAmbient, i.pos, i.normal, _WorldSpaceCameraPos);
                col.a = 1;
                return col;

            }
            ENDCG
        }
    }
}