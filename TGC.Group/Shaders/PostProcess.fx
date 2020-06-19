//Textura
texture mainSceneTexture;
sampler2D mainSceneSampler = sampler_state
{
    Texture = (mainSceneTexture);
    ADDRESSU = MIRROR;
    ADDRESSV = WRAP;
	MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture gogleViewTexture;
sampler2D gogleViewSampler = sampler_state
{
    Texture = (mainSceneTexture);
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
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;

	output.Texcoord = input.Texcoord;

	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
    float4 mainSceneColor = tex2D(mainSceneSampler, input.Texcoord);
    float4 gogleViewColor = tex2D(gogleViewSampler, input.Texcoord);
    return mainSceneColor + gogleViewColor;
}


technique Default
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}
