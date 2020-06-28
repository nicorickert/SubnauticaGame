
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

const float PI = 3.14159265f;
float time = 0;

float ka = 1;
float kd = 1;
float ks = 1;
    
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
};



// Funciones adicionales

float waveGenerator(float3 p, inout float derivadaX, inout float derivadaZ, float2 direction, float waveLenght, float k, float amplitude)
{
    float speed = 500;
    
    float2 d = normalize(direction);
    float w = 2 / waveLenght;
    float phase = speed * w;

    float F = dot(d, p.xz) * w + time * phase ;
    float G = (sin(F) + 1) / 2;
    // ola
    //p.x = d.x * sinF * amp; // X(p.x, p.y)
    float y = 2 * amplitude * pow(G, k); // Y(p.x, p.y)
    //p.z = d.y * sinF * amp; // Z(p.x, p.y)
	
    float derivada = k * w * amplitude * pow(G, k - 1) * cos(F);
    
    derivadaX += d.x * derivada;
    derivadaZ += d.y * derivada;
    
    return y;
}


//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;
	
    output.WorldPosition = mul(input.Position, matWorld);
    float derivadaX = 0;
    float derivadaZ = 0;

    float3 position = input.Position.xyz;
    float y = position.y;
    y += waveGenerator(position, derivadaX, derivadaZ, float2(1, -1), 1200, 2, 15);
    y += waveGenerator(position, derivadaX, derivadaZ, float2(3,5), 2000, 1.8, 15);
    y += waveGenerator(position, derivadaX, derivadaZ, float2(-5, 1.7), 800, 2.3, 15);
    y += waveGenerator(position, derivadaX, derivadaZ, float2(-1, -2), 700, 2, 15);
    
    input.Position.y = y;
    input.Normal = normalize(float3(-derivadaX, 1, -derivadaZ));

    output.Position = mul(input.Position, matWorldViewProj);
	
	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;
    
    output.WorldNormal = mul(float4(input.Normal, 1.0), matInverseTransposeWorld);
	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
	
	float textureScale = 100;
    float4 texelColor = tex2D(textureSampler, textureScale * input.Texcoord);
	
    float3 ambientColor = float3(1, 1, 1);
    float3 diffuseColor = texelColor.xyz;
    float3 specularColor = float3(1, 1, 1);
    float shininess = 50;
    
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
	
    //return float4(n_dot_l, 0, 0, 1);
    //return float4(abs(input.WorldNormal), 1);
    return finalColor;
}



technique Default
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vsDefault();
		PixelShader = compile ps_3_0 psDefault();
	}
}