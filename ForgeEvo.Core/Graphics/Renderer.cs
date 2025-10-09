using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ForgeEvo.Core.Math;
using Veldrid;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Generic interface for a Forge renderer.
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>
    ///     Render the contents of the command list to its display of given size.
    /// </summary>
    /// <param name="commandList">Command list to be followed during the render.</param>
    /// <param name="displaySize">Size of the display.</param>
    void Render(CommandList commandList, Size2D displaySize);
}

/// <summary>
///     Base class for a Forge renderer with <see cref="Veldrid" /> interop.
/// </summary>
/// <param name="device">Graphics device of the renderer's parent.</param>
/// <param name="pipeline">Render pipeline to be followed.</param>
/// <param name="resourceLayout">Resouce layout of the pipeline.</param>
/// <param name="sampler">Texture sampler of the renderer.</param>
/// <param name="vertexBuffer">Vertex object buffer to hold vertex positions.</param>
/// <param name="indexBuffer">Index object buffer to hold vertex order.</param>
/// <param name="transformBuffer">Transform buffer to hold transform data.</param>
/// <typeparam name="TDrawItem">Datatype to be rendered by this renderer.</typeparam>
public abstract class RendererBase<TDrawItem>(
    GraphicsDevice device,
    Pipeline pipeline,
    ResourceLayout resourceLayout,
    Sampler sampler,
    DeviceBuffer vertexBuffer,
    DeviceBuffer indexBuffer,
    DeviceBuffer transformBuffer
) : IRenderer where TDrawItem : struct
{
    /// <summary>
    ///     Graphics device of the renderer's parent.
    /// </summary>
    protected readonly GraphicsDevice Device = device;

    /// <summary>
    ///     List of draw items to be rendererd.
    /// </summary>
    protected readonly List<TDrawItem> DrawList = new(128);

    /// <summary>
    ///     Index buffer object to hold vertex order.
    /// </summary>
    protected readonly DeviceBuffer IndexBuffer = indexBuffer;

    /// <summary>
    ///     Render pipeline to be followed.
    /// </summary>
    protected readonly Pipeline Pipeline = pipeline;

    /// <summary>
    ///     Resource layout of the pipeline.s
    /// </summary>
    protected readonly ResourceLayout ResourceLayout = resourceLayout;

    /// <summary>
    ///     Texture sampler.
    /// </summary>
    protected readonly Sampler Sampler = sampler;

    /// <summary>
    ///     Transform buffer object to hold transform data.
    /// </summary>
    protected readonly DeviceBuffer TransformBuffer = transformBuffer;

    /// <summary>
    ///     Vertex buffer object to hold vertex positions.
    /// </summary>
    protected readonly DeviceBuffer VertexBuffer = vertexBuffer;

    #region IRenderer Members

    public abstract void Render(CommandList commandList, Size2D displaySize);

    public virtual void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
        TransformBuffer.Dispose();

        Pipeline.Dispose();
        ResourceLayout.Dispose();
        Sampler.Dispose();
    }

    #endregion

    /// <summary>
    ///     Add a draw item to the renderer's draw list.
    /// </summary>
    /// <param name="item">Item to add to the draw list.</param>
    public void AddToDrawList(TDrawItem item)
    {
        DrawList.Add(item);
    }

    /// <summary>
    ///     Load an HLSL shader from a file.
    /// </summary>
    /// <param name="device">Graphics device to load the shader into.</param>
    /// <param name="sourcePath">Source path of the shader to load.</param>
    /// <param name="stage">Stage of the shader: vertex, fragment, compute, etc.</param>
    /// <param name="entryPoint">Execution entry point of the shader, usually the main function.</param>
    /// <returns>Loaded shader attached to the <c>device</c>.</returns>
    /// <exception cref="FileNotFoundException">The <c>sourcePath</c> must lead to a valid exisiting file.</exception>
    protected static Shader LoadShader(
        GraphicsDevice device, string sourcePath, ShaderStages stage, string entryPoint = "main"
    )
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Shader file not found.", sourcePath);

        string shaderSource = File.ReadAllText(sourcePath);
        return device.ResourceFactory.CreateShader(new(stage, Encoding.UTF8.GetBytes(shaderSource), entryPoint));
    }
}

/// <summary>
///     Sprite renderer that renders raw <see cref="Sprite" />s from <see cref="Image" />s.
/// </summary>
/// <param name="device">Graphics device of the renderer's parent.</param>
/// <param name="pipeline">Render pipeline to be followed.</param>
/// <param name="resourceLayout">Resouce layout of the pipeline.</param>
/// <param name="sampler">Texture sampler of the renderer.</param>
/// <param name="vertexBuffer">Vertex object buffer to hold vertex positions.</param>
/// <param name="indexBuffer">Index object buffer to hold vertex order.</param>
/// <param name="transformBuffer">Transform buffer to hold transform data.</param>
public sealed class SpriteRenderer(
    GraphicsDevice device,
    Pipeline pipeline,
    ResourceLayout resourceLayout,
    Sampler sampler,
    DeviceBuffer vertexBuffer,
    DeviceBuffer indexBuffer,
    DeviceBuffer transformBuffer
) : RendererBase<Image>(device, pipeline, resourceLayout, sampler, vertexBuffer, indexBuffer, transformBuffer)
{
    /// <summary>
    ///     Create a default 2D sprite renderer attached to the given <c>device</c>.
    /// </summary>
    /// <param name="device">Graphics device to attach the renderer to.</param>
    /// <returns>New instance of a sprite renderer.</returns>
    public static SpriteRenderer CreateDefault(GraphicsDevice device)
    {
        Vertex[] vertices =
        [
            new(new(-1, -1), new(0, 1)),
            new(new(1, -1), new(1, 1)),
            new(new(1, 1), new(1, 0)),
            new(new(-1, 1), new(0, 0))
        ];

        DeviceBuffer vertexBuffer = device.ResourceFactory.CreateBuffer(new(
            (uint)(Unsafe.SizeOf<Vertex>() * vertices.Length),
            BufferUsage.VertexBuffer
        ));

        device.UpdateBuffer(vertexBuffer, 0, vertices);

        ushort[] indices = [0, 1, 2, 2, 3, 0];

        DeviceBuffer indexBuffer = device.ResourceFactory.CreateBuffer(new(
            (uint)(sizeof(ushort) * indices.Length),
            BufferUsage.IndexBuffer
        ));

        device.UpdateBuffer(indexBuffer, 0, indices);

        DeviceBuffer transformBuffer = device.ResourceFactory.CreateBuffer(new(
            sizeof(float) * 4 * 4,
            BufferUsage.UniformBuffer | BufferUsage.Dynamic
        ));

        Sampler sampler = device.ResourceFactory.CreateSampler(new(
            SamplerAddressMode.Clamp, SamplerAddressMode.Clamp, SamplerAddressMode.Clamp,
            SamplerFilter.MinLinear_MagLinear_MipLinear, null, 1, 0, 0, 0, SamplerBorderColor.TransparentBlack
        ));

        ResourceLayout resourceLayout = device.ResourceFactory.CreateResourceLayout(new(
            new ResourceLayoutElementDescription("transform", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("texture2d", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("current_sampler_state", ResourceKind.Sampler, ShaderStages.Fragment)
        ));

        Shader vertexShader = LoadShader(
            device, "Graphics/Shaders/image.vert.hlsl", ShaderStages.Vertex
        );

        Shader fragmentShader = LoadShader(
            device, "Graphics/Shaders/image.frag.hlsl", ShaderStages.Fragment
        );

        var pipelineDescription = new GraphicsPipelineDescription
        {
            BlendState = BlendStateDescription.SingleAlphaBlend,
            DepthStencilState = DepthStencilStateDescription.Disabled,
            RasterizerState = RasterizerStateDescription.CullNone,
            PrimitiveTopology = PrimitiveTopology.TriangleList,
            ResourceLayouts = [resourceLayout],
            ShaderSet = new(
                [
                    new(
                        new VertexElementDescription(
                            "position", VertexElementSemantic.Position, VertexElementFormat.Float2
                        ),
                        new VertexElementDescription(
                            "texture_coordinate", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2
                        )
                    )
                ],
                [vertexShader, fragmentShader]
            ),
            Outputs = device.MainSwapchain.Framebuffer.OutputDescription
        };

        Pipeline pipeline = device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);

        return new(device, pipeline, resourceLayout, sampler, vertexBuffer, indexBuffer, transformBuffer);
    }

    /// <summary>
    ///     Render all the sprites in the draw list.
    /// </summary>
    /// <param name="commandList">Command list to follow during rendering.</param>
    /// <param name="displaySize">Size of the display to which all the sprites are to be rendered.</param>
    public override void Render(CommandList commandList, Size2D displaySize)
    {
        if (DrawList.Count == 0)
            return;

        commandList.SetPipeline(Pipeline);
        commandList.SetVertexBuffer(0, VertexBuffer);
        commandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);

        foreach (Image image in DrawList)
        {
            Size2D size = image.Size;

            // Compute the scale of the image taking inversions along both axes into account.
            Vector2D scale = new(
                size.Width * (image.Scale.X < 0 ? -1 : 1),
                size.Height * (image.Scale.Y < 0 ? -1 : 1)
            );

            Matrix4x4 transformMatrix = CreateSpriteTransform(
                image.Position,
                scale,
                displaySize
            );

            commandList.UpdateBuffer(TransformBuffer, 0, ref transformMatrix);

            using ResourceSet resourceSet = Device.ResourceFactory.CreateResourceSet(new(
                ResourceLayout, TransformBuffer, image.Sprite.TextureView, Sampler
            ));

            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed(6, 1, 0, 0, 0);
        }

        DrawList.Clear();
    }

    /// <summary>
    ///     Calculate the transform matrix for a given sprite.
    /// </summary>
    /// <param name="position">Position of the sprite on the display.</param>
    /// <param name="scale">Scale of the sprite.</param>
    /// <param name="displaySize">Size of the display to which the sprite is rendered.</param>
    /// <returns>Transform matrix of the sprite used in the fragment shader.</returns>
    private static Matrix4x4 CreateSpriteTransform(Vector2D position, Vector2D scale, Size2D displaySize)
    {
        Vector2D translation = new(
            position.X / displaySize.Width * 2 - 1 + scale.X / displaySize.Width,
            1 - position.Y / displaySize.Height * 2 - scale.Y / displaySize.Height
        );

        var scaleMatrix = Matrix4x4.CreateScale(scale.X / displaySize.Width, scale.Y / displaySize.Height, 1);
        var translationMatrix = Matrix4x4.CreateTranslation(translation.X, translation.Y, 0);

        return scaleMatrix * translationMatrix;
    }

    #region Nested type: Vertex

    // ReSharper disable NotAccessedPositionalProperty.Local
    private readonly record struct Vertex(Vector2D Position, Vector2D TextureCoordinate);
    // ReSharper restore NotAccessedPositionalProperty.Local

    #endregion
}