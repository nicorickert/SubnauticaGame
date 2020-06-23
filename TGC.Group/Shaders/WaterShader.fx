
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

float3 waveGenerator(float3 p, inout float3 tangentX, inout float3 tangentZ, float2 direction, float waveLenght, float stepness, float heightFactor)
{
    
    float2 d = normalize(direction);
    float k = 2 * PI / waveLenght;
    
    
    float c = sqrt(9.8 / k);
    float f = k * (dot(d, p.xz) - c * time);    // f = k (d.x * p.x + d.y * p.z - c * time)
    float amp = stepness / k * heightFactor;
    
    float cosF = cos(f);
    float sinF = sin(f);

    // ola
    p.x = d.x * sinF * amp; // X(p.x, p.y)
    p.y = cosF * amp; // Y(p.x, p.y)
    p.z = d.y * sinF * amp; // Z(p.x, p.y)
	
    // tangentes
    // f'x = k * d.x                f'z= k * d.z                amp * k = stepness
    tangentX += float3(
                    d.x * d.x * cosF * stepness * heightFactor,
                    - d.x * sinF * stepness * heightFactor,
                    d.x * d.y * cosF * stepness * heightFactor
                );
    
    
    tangentZ += float3(
				d.x * d.y * cosF * stepness,
				- d.y * sinF * stepness,
				d.y * d.y * cosF * stepness
			);
    
    return p;
}


//Vertex Shader
VS_OUTPUT vsDefault(VS_INPUT input)
{
	VS_OUTPUT output;
	
    output.WorldPosition = mul(input.Position, matWorld);
    float3 tangentX = float3(1, 0, 0);
    float3 tangentZ = float3(0,0, -1);

    float3 position = input.Position.xyz;
    
    position += waveGenerator(position, tangentX, tangentZ, float2(1, 1), 25, 0.01, 40);
    position += waveGenerator(position, tangentX, tangentZ, float2(1, 1.4), 30, 0.01, 40);
    position += waveGenerator(position, tangentX, tangentZ, float2(1, 0.8), 15, 0.02, 20);
    
    input.Position.xyz = position;
    input.Normal = normalize(cross(tangentX, tangentZ));

    output.Position = mul(input.Position, matWorldViewProj);
	
	output.Texcoord = input.Texcoord;
	output.MeshPosition = input.Position;
    
    output.WorldNormal = mul(float4(input.Normal, 1.0), matInverseTransposeWorld);
	return output;
}

//Pixel Shader
float4 psDefault(VS_OUTPUT input) : COLOR0
{
	
	float textureScale = 60;
    float4 texelColor = tex2D(textureSampler, textureScale * input.Texcoord);
	
    float3 ambientColor = float3(1, 1, 1);
    float3 diffuseColor = texelColor.xyz;
    float3 specularColor = float3(1, 1, 1);
    float shininess = 15;
    
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
    //return float4((input.WorldNormal.x),0,input.WorldNormal.z, 1);
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