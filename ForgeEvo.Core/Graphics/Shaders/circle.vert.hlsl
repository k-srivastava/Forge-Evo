cbuffer TransformBuffer : register(b0)
{
    float4x4 transform;
}

struct vertex_input
{
    float2 position : POSITION;
    float4 color : COLOR0;
};

struct vertex_output
{
    float4 position : SV_POSITION;
    float4 color : COLOR0;
};

vertex_output main(vertex_input input)
{
    vertex_output output;
    output.position = mul(transform, float4(input.position, 0, 1));
    output.color = input.color;
    return output;
}
