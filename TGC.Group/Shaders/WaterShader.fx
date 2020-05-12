
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float time = 0;

//Textura
texture textureExample;
sampler2D textureSampler = sampler_state
{
    Texture = (textureExample);
    ADDRESSU = MIRROR;
    ADDRESSV = MIRROR;
	MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 MeshPosition : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;

    output.Position = mul(input.Position, matWorldViewProj);
	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;

	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.Texcoord + float4(0.01,0.01,0.01,0) * time);
	textureColor = textureColor;
	
	return float4(textureColor.xyz, 0.9f);
}



technique Default
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}