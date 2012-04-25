/*

% Diffuse lighting shader, created based on the tutorial on www.gamecamp.no:
% http://gamecamp.no/blogs/tutorials/archive/2007/12/16/xna-shader-programming-tutorial-2.aspx

keywords: diffuse lighting

date: 30/05/08

*/

float4x4    matWorldViewProj    : WorldViewProjection;
float4x4    matWorld            : World;  
float4      vLightPos           : LightPosition;

Texture ballTex;
sampler TextureSampler = sampler_state { texture = <ballTex>;};

struct OUT 
{ 
    float4 Pos              : POSITION; 
    float3 L                : TEXCOORD0; 
    float3 N                : TEXCOORD1;
    float2 TextureCoords    : TEXCOORD2;
};

struct PS_TO_SCREENT
{
    float4 Color : COLOR0;
};

OUT vs_vertexshader( float4 Pos: POSITION, float3 N: NORMAL, float2 inputText: TEXCOORD0 ) 
{ 
    OUT Out = (OUT) 0; 
    Out.Pos = mul(Pos, matWorldViewProj);   // Transform the vertex to worldspace
    Out.N = normalize(N);		// Calculate the vertex normal
    Out.TextureCoords = inputText;
    
    // Calculate and normalize the light vector
    Out.L = normalize(float3(vLightPos[0] - Pos.x, vLightPos[1] - Pos.y, vLightPos[2] - Pos.z));
    
    return Out; 
}

PS_TO_SCREENT ps_pixelshader(OUT input)
{ 
    PS_TO_SCREENT Out = (PS_TO_SCREENT)0;
    
    // Apply the texture to the object
    Out.Color = tex2D(TextureSampler, input.TextureCoords);
    
    // Ambient lighting
    float Ai = 1.0f;    // Light intensity
    float4 Ac = float4(0.075, 0.075, 0.075, 1.0); // Light color
   
    // Diffuse lighting    
    float Di = 1.0f; 
    float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
    
    // Multiply the existing texture/color with the diffuse lighting
    Out.Color *= Ai * Ac + Di * Dc * saturate(clamp(dot(input.L, input.N), 0, 1));
    
    return Out;
}

technique DiffuseLight 
{ 
    pass P0 
    { 
        VertexShader = compile vs_2_0 vs_vertexshader(); 
        PixelShader = compile ps_2_0 ps_pixelshader(); 
    } 
}