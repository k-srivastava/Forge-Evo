using Veldrid;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Base game class for Forge.
/// </summary>
public abstract class Game
{
    /// <summary>
    ///     Display used by the game for rendering and managing the window. Provides functionality to create and configure a
    ///     game window, handle window events, and render graphics to the screen.
    /// </summary>
    protected readonly Display Display;

    /// <summary>
    ///     Whether the game is running.
    /// </summary>
    private bool _running = true;

    /// <summary>
    ///     Create a new game. Provides core functionality to manage the game loop, rendering, and updating logic.
    /// </summary>
    protected Game(uint width = 800, uint height = 600, string title = "Forge Evo")
    {
        Display = new(width, height, title);
        Display.Window.Closed += () => _running = false;
    }

    /// <summary>
    ///     Starts the main game loop and keeps the game running until explicitly stopped.
    /// </summary>
    public void Run()
    {
        while (_running)
        {
            InputSnapshot input = Display.Window.PumpEvents();
            InputHandler.Update(input);

            Update();
            Render();
        }
    }

    /// <summary>
    ///     Update method for the game.
    /// </summary>
    protected abstract void Update();

    /// <summary>
    ///     Render method for the game.
    /// </summary>
    protected abstract void Render();
}