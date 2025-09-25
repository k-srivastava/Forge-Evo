using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Core display class for the game.
/// </summary>
public class Display : IDisposable
{
    /// <summary>
    ///     Veldrid command list.
    /// </summary>
    private readonly CommandList _commandList;

    /// <summary>
    ///     Veldrid graphics device.
    /// </summary>
    internal readonly GraphicsDevice Device;

    /// <summary>
    ///     Create a new display using the SLD2 window.
    /// </summary>
    /// <param name="width">Width of the display in pixels.</param>
    /// <param name="height">Height of the display in pixels.</param>
    /// <param name="title">Title of the display.</param>
    internal Display(uint width = 800, uint height = 600, string title = "Forge Evo")
    {
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new(50, 50, (int)width, (int)height, WindowState.Normal, title),
            new(),
            out Sdl2Window window,
            out GraphicsDevice device
        );

        Window = window;
        Device = device;
        _commandList = device.ResourceFactory.CreateCommandList();

        Window.Resized += () =>
        {
            if (Device.MainSwapchain.Framebuffer.Width != window.Width ||
                Device.MainSwapchain.Framebuffer.Height != window.Height)
                Device.ResizeMainWindow((uint)Window.Width, (uint)Window.Height);
        };
    }

    /// <summary>
    ///     Raw SDL2 window for the display.
    /// </summary>
    internal Sdl2Window Window { get; }

    public void Dispose()
    {
        _commandList.Dispose();
        Device.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Clears the display to the specified background color.
    /// </summary>
    /// <param name="color">The color used to clear the display.</param>
    public void Clear(Color color)
    {
        _commandList.Begin();

        _commandList.SetFramebuffer(Device.MainSwapchain.Framebuffer);
        _commandList.ClearColorTarget(0, color.ToRgbaFloat());
        _commandList.End();

        Device.SubmitCommands(_commandList);
        Device.SwapBuffers(Device.MainSwapchain);
    }
}