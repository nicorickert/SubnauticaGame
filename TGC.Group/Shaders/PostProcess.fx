/* POSTPROCESS */


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
texture mainSceneTexture;
sampler mainSceneSampler = sampler_state
{
    Texture = (mainSceneTexture);
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
};

texture gogleViewTexture;
sampler gogleViewSampler = sampler_state
{
    Texture = (gogleViewTexture);
};


//Input del Vertex Shader
struct VS_INPUT_GOGLEVIEW
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_GOGLEVIEW
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_GOGLEVIEW vsGogleView(VS_INPUT_GOGLEVIEW input)
{
    VS_OUTPUT_GOGLEVIEW output;

    output.Position = float4(input.Position.xy, 0, 1);
	
    output.Texcoord = input.Texcoord;

    return output;
}

struct PS_INPUT_GOGLEVIEW
{
    float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 psGogleView(PS_INPUT_GOGLEVIEW input) : COLOR0
{
    float4 mainSceneColor = tex2D(mainSceneSampler, input.Texcoord);
    float4 gogleViewColor = tex2D(gogleViewSampler, input.Texcoord);
	
    float4 finalColor = (gogleViewColor.a <= 0.005) ? mainSceneColor : gogleViewColor;
    return finalColor;
}


technique GogleView
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vsGogleView();
        PixelShader = compile ps_3_0 psGogleView();
    }
}


float4 psNoGogles(PS_INPUT_GOGLEVIEW input) : COLOR0
{
    return tex2D(mainSceneSampler, input.Texcoord);
}


technique NoGogles
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vsGogleView();
        PixelShader = compile ps_3_0 psNoGogles();
    }
}
