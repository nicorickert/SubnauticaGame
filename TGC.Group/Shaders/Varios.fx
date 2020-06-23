/*
* Shader con tecnicas varias utilizadas por diversas herramientas del framework,
* como: TgcBox, TgcArrow, TgcPlaneWall, TgcBoundingBox, TgcBoundingSphere, etc.
* Hay varias Techniques, una para cada combinacion utilizada en el framework de formato de vertice:
*	- PositionColoredTextured
*	- PositionTextured
*	- PositionColored
*	- PositionColoredAlpha
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Factor de translucidez
float alphaValue = 1;

/**************************************************************************************/
/* PositionColoredTextured */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_PositionColoredTextured
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_PositionColoredTextured
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_PositionColoredTextured vs_PositionColoredTextured(VS_INPUT_PositionColoredTextured input)
{
	VS_OUTPUT_PositionColoredTextured output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_PositionColoredTextured
{
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_PositionColoredTextured(PS_INPUT_PositionColoredTextured input) : COLOR0
{
	return input.Color * tex2D(diffuseMap, input.Texcoord);
}

/*
* Technique PositionColoredTextured
*/
technique PositionColoredTextured
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_PositionColoredTextured();
		PixelShader = compile ps_3_0 ps_PositionColoredTextured();
	}
}

/**************************************************************************************/
/* PositionTextured */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_PositionTextured
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_PositionTextured
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_PositionTextured vs_PositionTextured(VS_INPUT_PositionTextured input)
{
	VS_OUTPUT_PositionTextured output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_PositionTextured
{
	float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_PositionTextured(PS_INPUT_PositionTextured input) : COLOR0
{
	return tex2D(diffuseMap, input.Texcoord);
}

/*
* Technique PositionTextured
*/
technique PositionTextured
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_PositionTextured();
		PixelShader = compile ps_3_0 ps_PositionTextured();
	}
}

/**************************************************************************************/
/* PositionColored */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_PositionColored
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

//Output del Vertex Shader
struct VS_OUTPUT_PositionColored
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT_PositionColored vs_PositionColored(VS_INPUT_PositionColored input)
{
	VS_OUTPUT_PositionColored output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_PositionColored
{
	float4 Color : COLOR0;
};

//Pixel Shader
float4 ps_PositionColored(PS_INPUT_PositionColored input) : COLOR0
{
	return input.Color;
}

/*
* Technique PositionColored
*/
technique PositionColored
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_PositionColored();
		PixelShader = compile ps_3_0 ps_PositionColored();
	}
}

/**************************************************************************************/
/* PositionColoredAlpha */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_PositionColoredAlpha
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

//Output del Vertex Shader
struct VS_OUTPUT_PositionColoredAlpha
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT_PositionColoredAlpha vs_PositionColoredAlpha(VS_INPUT_PositionColoredAlpha input)
{
	VS_OUTPUT_PositionColoredAlpha output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_PositionColoredAlpha
{
	float4 Color : COLOR0;
};

//Pixel Shader
float4 ps_PositionColoredAlpha(PS_INPUT_PositionColoredAlpha input) : COLOR0
{
	return float4(input.Color.rgb, alphaValue);
}

/*
* Technique PositionColoredAlpha
*/
technique PositionColoredAlpha
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_PositionColoredAlpha();
		PixelShader = compile ps_3_0 ps_PositionColoredAlpha();
	}
}




/* POSTPROCESS */


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



/* ---------------------------------------*/
/*              Blinn-Phong               */
/* ---------------------------------------*/

float ka;
float kd;
float ks;
    
float3 lightPosition;
float3 eyePosition;

// variable de fogs
float4 ColorFog;
float StartFogDistance;
float EndFogDistance;

struct VS_INPUT_BlinnPhong
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float3 Normal : NORMAL0;
};

struct VS_OUTPUT_BlinnPhong
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 ViewPosition : TEXCOORD3;
};

VS_OUTPUT_BlinnPhong vs_BlinnPhong(VS_INPUT_BlinnPhong input)
{
    VS_OUTPUT_BlinnPhong output;
    
    output.Position = mul(input.Position, matWorldViewProj);
    
    output.TextureCoordinates = input.TextureCoordinates;
    
    output.WorldPosition = mul(input.Position, matWorld);
    
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    
    output.ViewPosition = mul(input.Position, matWorldView);
	
    return output;
}

float4 ps_BlinnPhong(VS_OUTPUT_BlinnPhong input) : COLOR0
{
    float4 texelColor = tex2D(diffuseMap, input.TextureCoordinates);
    
    float3 ambientColor = float3(1, 1, 1);
    float3 diffuseColor = texelColor.xyz;
    float3 specularColor = float3(1, 1, 1);
    float shininess = 10;
    
    float3 l = normalize(lightPosition - input.WorldPosition.xyz);
    float3 v = normalize(eyePosition - input.WorldPosition.xyz);
    float3 h = normalize(v + l);
    
    float n_dot_l = max(0, dot(input.WorldNormal, l));
    float n_dot_h = max(0, dot(input.WorldNormal, h));
    
    float3 ambientLight = ambientColor * ka;
    float3 diffuseLight = diffuseColor * kd * n_dot_l;
    float3 specularLight = ks * specularColor * pow(n_dot_h, shininess);
	
    float3 finalColorRGB = saturate(ambientLight + diffuseLight) * texelColor.rgb + specularLight;
    float4 finalColor = float4(finalColorRGB, texelColor.a);
	
	// FOG
    
    float zNear = StartFogDistance;
    float zFar = EndFogDistance;
    float zPos = input.ViewPosition.z; // Distancia relativa a la camara
	
    float distPlanes = zFar - zNear;
    float distPosToNear = zPos - zNear;
	
    float proportion = clamp(distPosToNear / distPlanes, 0, 1); // clamp = minimo 0 maximo 1 o sino la division
	
    finalColor = lerp(finalColor, ColorFog, proportion);
	
    return finalColor;
}


technique BlinnPhongTextured
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_BlinnPhong();
        PixelShader = compile ps_3_0 ps_BlinnPhong();
    }
}