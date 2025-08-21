using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Core display class for the game.
/// </summary>
public class Display
{
    /// <summary>
    ///     Veldrid command list.
    /// </summary>
    private readonly CommandList _commandList;

    /// <summary>
    ///     Veldrid graphics device.
    /// </summary>
    private readonly GraphicsDevice _device;

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
        _device = device;
        _commandList = device.ResourceFactory.CreateCommandList();
    }

    /// <summary>
    ///     Raw SDL2 window for the display.
    /// </summary>
    internal Sdl2Window Window { get; }

    /// <summary>
    ///     Clears the display to the specified background color.
    /// </summary>
    /// <param name="color">The color used to clear the display.</param>
    public void Clear(RgbaFloat color)
    {
        _commandList.Begin();
        _commandList.SetFramebuffer(_device.MainSwapchain.Framebuffer);
        _commandList.ClearColorTarget(0, color);
        _commandList.End();

        _device.SubmitCommands(_commandList);
        _device.SwapBuffers(_device.MainSwapchain);
    }
}