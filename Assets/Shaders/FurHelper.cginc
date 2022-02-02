#pragma target 3.0
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half2 uv1 : TEXCOORD1;
	fixed4 diff : COLOR;
	float3 lightDir : TEXCOORD2;
	float3 normal : TEXCOORD3;
};


float _FurLength;
sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _FurTex;
float4 _FurTex_ST;
float _Blur;

uniform fixed3 _Gravity;
uniform fixed _GravityStrength;
float _Ambient;
float _Emission;
sampler2D _WindTex;
float4 _WindTex_ST;
float _WindFrequency;
float _WindStrength;

 

v2f vert(appdata_base v) {
	v2f o;
	//v.vertex.xyz += v.normal * _FurLength * FURSTEP;
	fixed3 direction = lerp(v.normal, _Gravity * _GravityStrength + v.normal * (1-_GravityStrength), FURSTEP);
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

	float2 windTex = tex2Dlod(_WindTex, float4(worldPos.xz + v.normal * _WindTex_ST.xy + _Time.y * _WindFrequency, 0.0, 0.0)).xy;
	float2 wind = (windTex * 2 - 1) * _WindStrength;
	float4 windVertex = v.vertex + float4(wind.x, 0.0, 0.0, wind.y);

	if (FURSTEP > 0.2)
	{
		direction += float3(wind.x, 0.0, wind.y);
	}
	v.vertex.xyz += direction * _FurLength * FURSTEP;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	o.uv1 = TRANSFORM_TEX(v.texcoord, _FurTex);
	o.lightDir = normalize(_WorldSpaceLightPos0).xyz;
	o.normal = mul(unity_ObjectToWorld, v.normal);
	float3 worldNormal = normalize(mul(v.normal, (float3x3) unity_WorldToObject));
	o.diff = LambertDiffuse(worldNormal);
	
	//if (FURSTEP > 0.5)
	//{

		

	
	////o.pos = UnityObjectToClipPos(windVertex);
	//}


	o.diff.a = 1 - (FURSTEP * FURSTEP);
	o.diff.a += dot(normalize(_WorldSpaceCameraPos.xyz - worldPos), worldNormal) - _Blur;
	return o;
}

float _CutOff;
float _Thickness;
float _Shininess;
float _Specular;
float _Dif2;

fixed4 frag(v2f i) : SV_Target {
	
	

	fixed4 col = tex2D(_MainTex, i.uv) + (i.diff * 0.5);
	fixed alpha = tex2D(_FurTex, i.uv1).r;
    
	float3 L = normalize(i.lightDir.xyz);
	float3 V = normalize(_WorldSpaceCameraPos - i.pos);
    float H = normalize(L + V);
	float4 intensity = 0.05;
	float power = 8;
    //col += intensity * pow(saturate(dot(i.normal, H)), power);
	
	MaterialParameters lightingMat;
    lightingMat.Ke = float3(0, 0, 0) + _Emission;
    lightingMat.Ka = float3(1, 1, 1) * _Ambient;
    lightingMat.Kd = float3(1, 1, 1) * _Dif2;
    lightingMat.Ks = float3(1, 1, 1) * _Specular;
    lightingMat.shininess = 1.0 * _Shininess;

    LightParameters light;
	
    light.position = i.lightDir.xyz;
    float3 lightColor = _LightColor0.rgb;
    light.color = lightColor;
    float3 globAmbient = float3 (1, 1, 1);
    //globAmbient.rgb *= _Ambient;
    col.rgb *= lighting(lightingMat, light, globAmbient, i.pos, normalize(i.normal), _WorldSpaceCameraPos);
	
	col.a *= step(lerp(_CutOff, _CutOff + _Thickness, FURSTEP), alpha);
	return col;
}