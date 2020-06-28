
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float time = 0;

// variable de fogs
float4 ColorFog;
float WaterLevel;

//Textura
texture texDiffuseMap;
sampler2D textureSampler = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = MIRROR;
    ADDRESSV = WRAP;
	MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : NORMAL0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 MeshPosition : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;

    output.Position = mul(input.Position, matWorldViewProj);

	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;
	output.WorldPosition = mul(input.Position, matWorld);

	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
	
    float4 textureColor = tex2D(textureSampler, input.Texcoord);
	
	// no hacer fog encima del mar
	float proportion = input.WorldPosition.y > WaterLevel + 60;
	
		
	
    textureColor = lerp(ColorFog, textureColor,  proportion);
	
    return textureColor;
//return float4(abs(Nn.x), 0, abs(Nn.z), 1) * 3;
}



technique Default
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}