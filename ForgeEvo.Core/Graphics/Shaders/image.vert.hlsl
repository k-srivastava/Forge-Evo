cbuffer TransformBuffer : register(b0)
{
    matrix transforms[128];
};

struct vertex_input
{
    float2 position : POSITION;
    float2 texture_coordinate : TEXCOORD0;
    uint instance_id : SV_InstanceID;
};

struct vertex_output
{
    float4 position : SV_POSITION;
    float2 texture_coordinate : TEXCOORD0;
};

vertex_output main(vertex_input input)
{
    vertex_output output;

    matrix transform = transforms[input.instance_id];
    output.position = mul(transform, float4(input.position, 0.0, 1.0));
    output.texture_coordinate = input.texture_coordinate;
    return output;
}
