using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ForgeEvo.Core;

public class Display : IDisposable
{
    private readonly Sdl2Window _window;
    private readonly GraphicsDevice _device;

    public Display(uint width = 800, uint height = 600, string title = "Forge Evo")
    {
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new(50, 50, (int) width, (int) height, WindowState.Normal, title),
            new(),
            out _window,
            out _device
        );
    }

    public void Run()
    {
        while (_window.Exists)
        {
            _window.PumpEvents();
            _device.SwapBuffers();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        _device.Dispose();
        _window.Close();
    }
}