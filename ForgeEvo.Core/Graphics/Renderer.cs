using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ForgeEvo.Core.Math;
using JetBrains.Annotations;
using Veldrid;

namespace ForgeEvo.Core.Graphics;

/// <summary>
///     Generic interface for a renderable item.
/// </summary>
public interface IRenderable
{
    /// <summary>
    ///     Enqueue the item to its corresponding renderer's render queue.
    /// </summary>
    void Enqueue();
}

/// <summary>
///     Generic interface for a Forge renderer.
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>
    ///     Render the contents of the render queue to its display of a given size.
    /// </summary>
    /// <param name="commandList">List of commands to be followed during the render.</param>
    /// <param name="display">Size of the display.</param>
    void Render(CommandList commandList, Size2D display);

    /// <summary>
    ///     Create a default renderer of the given type.
    /// </summary>
    /// <param name="device">Graphics device of the renderer's parent.</param>
    /// <typeparam name="TRenderer">Renderer type to instantiate.</typeparam>
    /// <typeparam name="TItem">Renderer's corresponding render item type.</typeparam>
    /// <returns>Default instance of the specified renderer.</returns>
    static TRenderer CreateDefault<TRenderer, TItem>(GraphicsDevice device)
        where TRenderer : IRenderer<TItem, TRenderer> where TItem : IRenderable => TRenderer.CreateDefault(device);
}

/// <summary>
///     Generic interface for a Forge renderer with its given render item type.
/// </summary>
/// <typeparam name="TRenderItem">Type of the item the renderer renders.</typeparam>
/// <typeparam name="TSelf">Type of the renderer itself.</typeparam>
/// <remarks>
///     The interface requires <c>TSelf</c> for reflection to enforce <c>CreateDefault</c> as an interface method at
///     compile-time.
/// </remarks>
public interface IRenderer<in TRenderItem, out TSelf> : IRenderer
    where TRenderItem : IRenderable
    where TSelf : IRenderer<TRenderItem, TSelf>
{
    /// <summary>
    ///     Maximum queue size of the renderer.
    /// </summary>
    static abstract uint MaxRenderQueueSize { get; }

    /// <summary>
    ///     Create a default instance of the renderer.
    /// </summary>
    /// <param name="device">Graphics device of the renderer's parent.</param>
    /// <returns>Default instance of the renderer.</returns>
    static abstract TSelf CreateDefault(GraphicsDevice device);

    /// <summary>
    ///     Enqueue a render item to the renderer's render queue.
    /// </summary>
    /// <param name="item">Item to render.</param>
    void Enqueue(TRenderItem item);
}

/// <summary>
///     Master renderer containing all the active renderers for the game.
/// </summary>
public sealed class MasterRenderer : IRenderer
{
    /// <summary>
    ///     Image renderer.
    /// </summary>
    private readonly ImageRenderer? _imageRenderer;

    /// <summary>
    ///     List of all the renderers.
    /// </summary>
    private readonly IRenderer[] _renderers;

    /// <summary>
    ///     Create a new primary renderer with the given renderers.
    /// </summary>
    /// <param name="renderers">List of all child renderers.</param>
    public MasterRenderer(params IRenderer[] renderers)
    {
        _renderers = renderers;

        foreach (IRenderer renderer in renderers)
        {
            switch (renderer)
            {
                case ImageRenderer imageRenderer:
                    _imageRenderer = imageRenderer;
                    break;
            }
        }
    }

    #region IRenderer Members

    public void Render(CommandList commandList, Size2D display)
    {
        foreach (IRenderer renderer in _renderers)
            renderer.Render(commandList, display);
    }

    public void Dispose()
    {
        foreach (IRenderer renderer in _renderers)
            renderer.Dispose();
    }

    #endregion

    /// <summary>
    ///     Create a default primary renderer with default instances of every renderer.
    /// </summary>
    /// <param name="device">Graphics device of the renderer's parent.</param>
    /// <returns>New master renderer.</returns>
    public static MasterRenderer CreateDefault(GraphicsDevice device) => new(ImageRenderer.CreateDefault(device));

    /// <summary>
    ///     Attempt to enqueue a renderable item to the renderer's render queue by dispatching it to the relevant child
    ///     renderer if it exists.
    /// </summary>
    /// <param name="renderable">Render item to enqueue.</param>
    /// <returns><c>true</c> if the item could be added to the relevant renderer, otherwise <c>false</c>.</returns>
    public bool TryEnqueue(IRenderable renderable)
    {
        switch (renderable)
        {
            case Image image:
                _imageRenderer?.Enqueue(image);
                return true;
        }

        return false;
    }
}

/// <summary>
///     Image renderer that renders raw <see cref="Sprite" />s from <see cref="Image" />s.
/// </summary>
/// <param name="device">Graphics device of the renderer's parent.</param>
/// <param name="pipeline">Render pipeline to be followed.</param>
/// <param name="resourceLayout">Resource layout of the <c>pipeline</c>.</param>
/// <param name="sampler">Texture sampler of the renderer.</param>
/// <param name="vertexBuffer">Vertex object buffer to hold vertex positions.</param>
/// <param name="indexBuffer">Index object buffer to hold vertex order.</param>
/// <param name="transformBuffer">Transform object buffer to hold transform data.</param>
public sealed class ImageRenderer(
    GraphicsDevice device,
    Pipeline pipeline,
    ResourceLayout resourceLayout,
    Sampler sampler,
    DeviceBuffer vertexBuffer,
    DeviceBuffer indexBuffer,
    DeviceBuffer transformBuffer
) : IRenderer<Image, ImageRenderer>
{
    /// <summary>
    ///     Queue of images to be rendered.
    /// </summary>
    private readonly Queue<Image> _renderQueue = new();

    #region IRenderer<Image,ImageRenderer> Members

    public static uint MaxRenderQueueSize => 128;

    public static ImageRenderer CreateDefault(GraphicsDevice device)
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

        Shader vertexShader = RendererUtils.LoadShader(
            device, "Graphics/Shaders/image.vert.hlsl", ShaderStages.Vertex
        );

        Shader fragmentShader = RendererUtils.LoadShader(
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

    public void Render(CommandList commandList, Size2D display)
    {
        if (_renderQueue.Count == 0)
            return;

        commandList.SetPipeline(pipeline);
        commandList.SetVertexBuffer(0, vertexBuffer);
        commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);

        while (_renderQueue.Count > 0)
        {
            Image image = _renderQueue.Dequeue();
            Size2D size = image.Size;

            // Compute the scale of the image taking inversions along both axes into account.
            Vector2D scale = new(
                size.Width * (image.Scale.X < 0 ? -1 : 1),
                size.Height * (image.Scale.Y < 0 ? -1 : 1)
            );

            Matrix4x4 transformMatrix = CreateImageTransform(image.Position, scale, display);
            commandList.UpdateBuffer(transformBuffer, 0, ref transformMatrix);

            using ResourceSet resourceSet = device.ResourceFactory.CreateResourceSet(new(
                resourceLayout, transformBuffer, image.Sprite.TextureView, sampler
            ));

            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed(6, 1, 0, 0, 0);
        }
    }

    public void Enqueue(Image item)
    {
        if (_renderQueue.Count >= MaxRenderQueueSize)
            throw new InvalidOperationException("Image render queue is full.");

        _renderQueue.Enqueue(item);
    }

    public void Dispose()
    {
        pipeline.Dispose();
        resourceLayout.Dispose();
        sampler.Dispose();
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        transformBuffer.Dispose();
    }

    #endregion

    /// <summary>
    ///     Calculate the transform matrix for a given <see cref="Sprite" />.
    /// </summary>
    /// <param name="position">Position of the sprite on the display.</param>
    /// <param name="scale">Scale of the sprite.</param>
    /// <param name="display">Size of the display to which the sprite is rendered.</param>
    /// <returns>Transform matrix of the sprite used in the fragment shader.</returns>
    private static Matrix4x4 CreateImageTransform(Vector2D position, Vector2D scale, Size2D display)
    {
        Vector2D translation = new(
            position.X / display.Width * 2 - 1 + scale.X / display.Width,
            1 - position.Y / display.Height * 2 - scale.Y / display.Height
        );

        var scaleMatrix = Matrix4x4.CreateScale(scale.X / display.Width, scale.Y / display.Height, 1);
        var translationMatrix = Matrix4x4.CreateTranslation(translation.X, translation.Y, 0);

        return scaleMatrix * translationMatrix;
    }

    #region Nested type: Vertex

    /// <summary>
    ///     Vertex of each <see cref="Image" /> rendered to the display.
    /// </summary>
    /// <param name="Position">Position of the vertex.</param>
    /// <param name="TextureCoordinate">UV texture coordinate of the vertex.</param>
    private readonly record struct Vertex([UsedImplicitly] Vector2D Position, Vector2D TextureCoordinate);

    #endregion
}

/// <summary>
///     Collection of utility functions for rendering.
/// </summary>
file static class RendererUtils
{
    /// <summary>
    ///     Load an HLSL shader from a file.
    /// </summary>
    /// <param name="device">Graphics device to load the shader into.</param>
    /// <param name="sourcePath">Source path of the shader to load.</param>
    /// <param name="stage">Stage of the shader: vertex, fragment, compute, etc.</param>
    /// <param name="entryPoint">Execution entry point of the shader, usually the <c>main</c> function.</param>
    /// <returns>Loaded shader attached to the <c>device</c>.</returns>
    /// <exception cref="FileNotFoundException">The <c>sourcePath</c> must lead to a valid existing file.</exception>
    internal static Shader LoadShader(
        GraphicsDevice device, string sourcePath, ShaderStages stage, string entryPoint = "main"
    )
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Shader file not found.", sourcePath);

        string shaderSource = File.ReadAllText(sourcePath);
        return device.ResourceFactory.CreateShader(new(stage, Encoding.UTF8.GetBytes(shaderSource), entryPoint));
    }
}