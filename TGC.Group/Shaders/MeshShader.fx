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

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
    Texture = (texLightMap);
};

//Parametros de la Luz
const float3 ambientColor = float3(1, 1, 1); //Color RGB para Ambient de la luz
const float3 specularColor = float3(1, 1, 1); //Color RGB para Ambient de la luz
const float shininess = 0.5; //Exponente de specular

float3 diffuseColor; //Color RGB para Ambient de la luz

float ka;
float kd;
float ks;
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara



//Input del Vertex Shader
struct BlinnVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

//Output del Vertex Shader
struct BlinnVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
    float3 WorldPosition : TEXCOORD2;
};



//Vertex Shader
BlinnVertexShaderOutput vsBlinn(BlinnVertexShaderInput input)
{
    BlinnVertexShaderOutput output;

	// Proyectamos la position
    output.Position = mul(input.Position, matWorldViewProj);

	// Propagamos las coordenadas de textura
    output.TextureCoordinates = input.TextureCoordinates;

	// Usamos la matriz normal para proyectar el vector normal
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	// Usamos la matriz de world para proyectar la posicion
    output.WorldPosition = mul(input.Position, matWorld);

    return output;
}

//Pixel Shader
float4 psBlinn(BlinnVertexShaderOutput input) : COLOR0
{
    input.WorldNormal = normalize(input.WorldNormal);

    float3 lightDirection = normalize(lightPosition.xyz - input.WorldPosition);
    float3 viewDirection = normalize(eyePosition.xyz - input.WorldPosition);
    float3 halfVector = normalize(lightDirection + viewDirection);

	// Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.TextureCoordinates);

	//Componente Diffuse: N dot L
    float3 NdotL = dot(input.WorldNormal, lightDirection);
    diffuseColor = texelColor.rgb;
    float3 diffuseLight = kd * diffuseColor * max(0.0, NdotL);

	//Componente Specular: (N dot H)^shininess
    float3 NdotH = dot(input.WorldNormal, halfVector);
    float3 specularLight = ((NdotL <= 0.0) ? 0.0 : ks) * specularColor * pow(max(0.0, NdotH), shininess);

    float4 finalColor = texelColor; //float4(ka * ambientColor + diffuseLight + specularLight, texelColor.a); //float4(saturate(ambientColor * ka + diffuseLight) * texelColor + specularLight, texelColor.a);
    return finalColor;
}

technique Default
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vsBlinn();
        PixelShader = compile ps_3_0 psBlinn();
    }
}


struct NoTextureVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Color : COLOR;
};

//Output del Vertex Shader
struct NoTextureVertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Color : COLOR;
    float3 WorldNormal : TEXCOORD1;
    float3 WorldPosition : TEXCOORD2;
};



//Vertex Shader
NoTextureVertexShaderOutput vsNoTexture(NoTextureVertexShaderInput input)
{
    NoTextureVertexShaderOutput output;

	// Proyectamos la position
    output.Position = mul(input.Position, matWorldViewProj);

    output.Color = input.Color;

	// Usamos la matriz normal para proyectar el vector normal
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	// Usamos la matriz de world para proyectar la posicion
    output.WorldPosition = mul(input.Position, matWorld);
    
    return output;
}

//Pixel Shader
float4 psNoTexture(NoTextureVertexShaderOutput input) : COLOR0
{
    input.WorldNormal = normalize(input.WorldNormal);

    float3 lightDirection = normalize(lightPosition.xyz - input.WorldPosition);
    float3 viewDirection = normalize(eyePosition.xyz - input.WorldPosition);
    float3 halfVector = normalize(lightDirection + viewDirection);

	// Obtener texel de la textura
    float4 texelColor = input.Color;

	//Componente Diffuse: N dot L
    float3 NdotL = dot(input.WorldNormal, lightDirection);
    diffuseColor = texelColor.rgb;
    float3 diffuseLight = kd * diffuseColor * max(0.0, NdotL);

	//Componente Specular: (N dot H)^shininess
    float3 NdotH = dot(input.WorldNormal, halfVector);
    float3 specularLight = ((NdotL <= 0.0) ? 0.0 : ks) * specularColor * pow(max(0.0, NdotH), shininess);

    float4 finalColor = texelColor; //float4(ka * ambientColor + diffuseLight + specularLight, texelColor.a); //float4(saturate(ambientColor * ka + diffuseLight) * texelColor + specularLight, texelColor.a);
    return finalColor;
}

technique NoTexture
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vsNoTexture();
        PixelShader = compile ps_3_0 psNoTexture();
    }
}