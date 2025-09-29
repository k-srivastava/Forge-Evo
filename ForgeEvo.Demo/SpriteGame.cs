using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Graphics;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Demo;

internal class SpriteGame() : Game(title: "Image Demo")
{
    private readonly Color _background = new(100, 149, 237);
    private readonly float _movementSpeed = 0.5F;
    private readonly float _scaleSpeed = 0.0005F;

    private Image _flowerImage;
    private Image _staticImage;

    protected override void Initialize()
    {
        var spriteSourcePath = "Assets/Images/flower.png";

        Sprite flowerSprite = new(Display, spriteSourcePath);
        _flowerImage = new(flowerSprite, Vector2D.Zero, new(0.2F, 0.2F));
        _staticImage = new(flowerSprite, new(100, 100), new(0.2F, 0.2F));
    }

    protected override void Update()
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        ResolveMovement();
        ResolveImageBounds();

        _staticImage.Scale = _flowerImage.Scale;
    }

    protected override void Render()
    {
        _flowerImage.Render();
        _staticImage.Render();

        Display.Clear(_background);
    }

    private void ResolveMovement()
    {
        if (InputHandler.IsKeyDown(Key.W))
            _flowerImage.Position += Vector2D.Up * _movementSpeed;

        if (InputHandler.IsKeyDown(Key.S))
            _flowerImage.Position += Vector2D.Down * _movementSpeed;

        if (InputHandler.IsKeyDown(Key.A))
            _flowerImage.Position += Vector2D.Left * _movementSpeed;

        if (InputHandler.IsKeyDown(Key.D))
            _flowerImage.Position += Vector2D.Right * _movementSpeed;

        if (InputHandler.IsKeyDown(Key.Up))
            _flowerImage.Scale += Vector2D.One * _scaleSpeed;

        if (InputHandler.IsKeyDown(Key.Down))
            _flowerImage.Scale -= Vector2D.One * _scaleSpeed;
    }

    private void ResolveImageBounds()
    {
        if (_flowerImage.Position.X < 0)
            _flowerImage.Position.X = 0;

        if (_flowerImage.Position.X + _flowerImage.Size.Width > Display.Size.Width)
            _flowerImage.Position.X = Display.Size.Width - _flowerImage.Size.Width;

        if (_flowerImage.Position.Y < 0)
            _flowerImage.Position.Y = 0;

        if (_flowerImage.Position.Y + _flowerImage.Size.Height > Display.Size.Height)
            _flowerImage.Position.Y = Display.Size.Height - _flowerImage.Size.Height;
    }
}