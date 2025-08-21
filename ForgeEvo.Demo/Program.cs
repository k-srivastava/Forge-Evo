using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Math;
using Veldrid;

var game = new DemoGame();
game.Run();

internal class DemoGame() : Game(title: "Forge Evo Demo")
{
    protected override void Update()
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        if (InputHandler.MouseDelta.Length() > 0)
        {
            Vector2D mousePosition = InputHandler.MousePosition;
            Console.WriteLine($"Mouse moved to X: {mousePosition.X}, Y: {mousePosition.Y}.");
        }

        if (InputHandler.IsMousePressed(MouseButton.Left)) Console.WriteLine("Pressed left mouse button.");

        if (InputHandler.ScrollDelta != 0) Console.WriteLine("Mouse scrolled.");
    }

    protected override void Render()
    {
        Display.Clear(RgbaFloat.CornflowerBlue);
    }
}