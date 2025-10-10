using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Graphics;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Demo;

public class StressTestGame() : Game(title: "Stress Test Demo")
{
    private readonly Color _backgroundColor = new(100, 149, 237);
    private readonly List<Circle> _circles = new((int)CircleRenderer.MaxRenderQueueSize);
    private readonly List<Image> _images = new((int)ImageRenderer.MaxRenderQueueSize);

    private Sprite _sprite = null!;
    private bool _isRunning = true;

    private readonly Random _random = new(DateTime.Now.Millisecond);

    protected override void Initialize()
    {
        _sprite = new("Assets/Images/flower.png");

        GenerateRandomShapes();
        GenerateRandomImages();

        EventBus.GetByName(InternalEvent.GameUpdated.ToName()).Subscribe(GenerateRandomShapes);
        EventBus.GetByName(InternalEvent.GameUpdated.ToName()).Subscribe(GenerateRandomImages);
    }

    protected override void Update(float deltaTime)
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        if (InputHandler.IsKeyPressed(Key.Space))
            _isRunning = !_isRunning;
    }

    protected override void Render()
    {
        foreach (Circle circle in _circles)
            circle.Enqueue();

        foreach (Image image in _images)
            image.Enqueue();

        Display.Render(_backgroundColor);
    }

    private void GenerateRandomShapes()
    {
        if (!_isRunning)
            return;

        _circles.Clear();

        for (var i = 0; i < _circles.Capacity; i++)
        {
            Vector2D position = new(
                (float)_random.NextDouble() * Display.Size.Width,
                (float)_random.NextDouble() * Display.Size.Height
            );

            float radius = (float)_random.NextDouble() * 200;
            var colorBytes = new byte[3];
            _random.NextBytes(colorBytes);
            Color color = new Color(colorBytes[0], colorBytes[1], colorBytes[2]).WithAlpha(100);

            _circles.Add(new(position, radius, color));
        }
    }

    private void GenerateRandomImages()
    {
        if (!_isRunning)
            return;

        _images.Clear();

        for (var i = 0; i < _images.Capacity; i++)
        {
            Vector2D position = new(
                (float)_random.NextDouble() * Display.Size.Width,
                (float)_random.NextDouble() * Display.Size.Height
            );

            float scale = (float)_random.NextDouble() * 0.2F;

            _images.Add(new(_sprite, position, Vector2D.One * scale));
        }
    }
}