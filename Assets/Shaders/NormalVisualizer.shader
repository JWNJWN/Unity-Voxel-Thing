Shader "Custom/NormalVisualizer" {
	Properties
	{
	}
	SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  

	struct vertexInput
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD0;
	};

	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;
		output.normal = input.normal;
		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		return output;
	}

	float4 frag(vertexOutput input) : COLOR
	{
		return float4((input.normal * 0.5) + 0.5, 1);
	}
		ENDCG
	}
	}
}