struct pixel_input
{
    float4 position : SV_POSITION;
    float2 texture_coordinate : TEXCOORD0;
};

Texture2D texture2d : register(t0);
SamplerState current_sampler_state : register(s0);

float4 main(pixel_input input) : SV_Target
{
    return texture2d.Sample(current_sampler_state, input.texture_coordinate);
}
