
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float time = 0;

float ka = 0.1;
float kd = 1000000;
float ks = 0;
    
float3 lightPosition;
float3 eyePosition;

// variable de fogs
float4 ColorFog;
float StartFogDistance;
float EndFogDistance;

//Textura
texture textureExample;
sampler2D textureSampler = sampler_state
{
    Texture = (textureExample);
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
    float3 WorldNormal : TEXCOORD3;
    float3 ViewPosition : TEXCOORD4;
};

//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;

    output.Position = mul(input.Position, matWorldViewProj);

	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;
	output.WorldPosition = mul(input.Position, matWorld);
	output.WorldNormal = mul(float4(input.Normal, 1.0), matInverseTransposeWorld);
    output.ViewPosition = mul(input.Position, matWorldView);

	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
    float textureScale = 200;
    float4 texelColor = tex2D(textureSampler, textureScale * input.Texcoord);
    
    float3 ambientColor = float3(0.4, 0.1, 0.5);
    float3 diffuseColor = float3(1, 1, 1);
    //float3 specularColor = float3(1, 1, 1);
    //float shininess = 1;
    
    float3 l = normalize(lightPosition - input.WorldPosition.xyz);
    //float3 v = normalize(eyePosition - input.WorldPosition.xyz);
    //float3 h = normalize(v + l);
    
    float n_dot_l = max(0, dot(input.WorldNormal, l));
    //float n_dot_h = max(0, dot(input.WorldNormal, h));
    
    float3 ambientLight = ambientColor * ka;
    float3 diffuseLight = diffuseColor * kd * n_dot_l;
    //float3 specularLight = ks * specularColor * pow(n_dot_h, shininess);
	
    float3 finalColorRGB = saturate(ambientLight + diffuseLight) * texelColor.rgb;//   +specularLight;
    float4 textureColor = float4(finalColorRGB, 1);
    
	// FOG
	float zNear = StartFogDistance;
    float zFar = EndFogDistance;
    float zPos = input.ViewPosition.z; // Distancia relativa a la camara
	
    float distPlanes = zFar - zNear;
    float distPosToNear = zPos - zNear;
	
    float proportion = clamp(distPosToNear / distPlanes, 0, 1);	// clamp = minimo 0 maximo 1 o sino la division
	
    textureColor = lerp(textureColor, ColorFog, proportion);
	
	return textureColor;
    return float4(n_dot_l, 0,0, 1);
}



technique Default
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}