using ForgeEvo.Core.Engine;
using ForgeEvo.Core.Graphics;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Demo;

public class PongGame() : Game(title: "Pong Demo")
{
    private readonly Color _backgroundColor = Color.Black;

    private Image _ballImage;
    private MutableVector2D _ballDirection;
    private readonly float _ballSpeed = 300;

    private Image _playerPaddleImage1;
    private Image _playerPaddleImage2;

    private readonly float _paddleSpeed = 400;

    protected override void Initialize()
    {
        var ballSourcePath = "Assets/Images/ball.png";
        var paddleSourcePath = "Assets/Images/paddle.png";

        _ballDirection = new Vector2D(0.5F, 0.5F).Normal();

        _ballImage = new(new Sprite(ballSourcePath), Vector2D.Zero, Vector2D.One);
        _ballImage.Position = new(
            Display.Size.Width / 2F - _ballImage.Size.Width, Display.Size.Height / 2F - _ballImage.Size.Height
        );

        Sprite paddleSprite = new(paddleSourcePath);

        _playerPaddleImage1 = new(paddleSprite, Vector2D.Zero, new(0.5F, 2F));
        _playerPaddleImage1.Position = new(20F, (Display.Size.Height - _playerPaddleImage1.Size.Height) / 2F);

        _playerPaddleImage2 = new(paddleSprite, Vector2D.Zero, new(0.5F, 2F));
        _playerPaddleImage2.Position = new(
            Display.Size.Width - (_playerPaddleImage2.Size.Width + 20F),
            (Display.Size.Height - _playerPaddleImage2.Size.Height) / 2F
        );
    }

    protected override void Update(float deltaTime)
    {
        if (InputHandler.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("Escape pressed, quitting...");
            Environment.Exit(0);
        }

        _ballImage.Position += _ballDirection * _ballSpeed * deltaTime;

        ResolvePaddleMovement(deltaTime);

        ResolvePaddleBounds();
        ResolveBallBounds();

        ResolveCollisions();
    }

    protected override void Render()
    {
        _ballImage.Render();
        _playerPaddleImage1.Render();
        _playerPaddleImage2.Render();

        Display.Render(_backgroundColor);
    }

    private void ResolvePaddleMovement(float deltaTime)
    {
        if (InputHandler.IsKeyDown(Key.W))
            _playerPaddleImage1.Position += Vector2D.Up * _paddleSpeed * deltaTime;

        if (InputHandler.IsKeyDown(Key.S))
            _playerPaddleImage1.Position += Vector2D.Down * _paddleSpeed * deltaTime;

        if (InputHandler.IsKeyDown(Key.Up))
            _playerPaddleImage2.Position += Vector2D.Up * _paddleSpeed * deltaTime;

        if (InputHandler.IsKeyDown(Key.Down))
            _playerPaddleImage2.Position += Vector2D.Down * _paddleSpeed * deltaTime;
    }

    private void ResolvePaddleBounds()
    {
        if (_playerPaddleImage1.Position.Y < 0)
            _playerPaddleImage1.Position.Y = 0;

        if (_playerPaddleImage1.Position.Y + _playerPaddleImage1.Size.Height > Display.Size.Height)
            _playerPaddleImage1.Position.Y = Display.Size.Height - _playerPaddleImage1.Size.Height;

        if (_playerPaddleImage2.Position.Y < 0)
            _playerPaddleImage2.Position.Y = 0;

        if (_playerPaddleImage2.Position.Y + _playerPaddleImage2.Size.Height > Display.Size.Height)
            _playerPaddleImage2.Position.Y = Display.Size.Height - _playerPaddleImage2.Size.Height;
    }

    private void ResolveBallBounds()
    {
        if (_ballImage.Position.X < 0 || _ballImage.Position.X + _ballImage.Size.Width > Display.Size.Width)
        {
            if (_ballImage.Position.X < 0)
            {
                _ballDirection.Reflect(Vector2D.Right);
                Console.WriteLine("Player 2 point!");
            }

            else
            {
                _ballDirection.Reflect(Vector2D.Left);
                Console.WriteLine("Player 1 point!");
            }

            _ballImage.Position = new(
                Display.Size.Width / 2F - _ballImage.Size.Width, Display.Size.Height / 2F - _ballImage.Size.Height
            );
        }

        if (_ballImage.Position.Y < 0)
            _ballDirection.Reflect(Vector2D.Down);

        else if (_ballImage.Position.Y + _ballImage.Size.Height > Display.Size.Height)
            _ballDirection.Reflect(Vector2D.Up);
    }

    private void ResolveCollisions()
    {
        float ballLeft = _ballImage.Position.X;
        float player1PaddleRight = _playerPaddleImage1.Position.X + _playerPaddleImage1.Size.Width;

        if (ballLeft <= player1PaddleRight)
        {
            float paddleTop = _playerPaddleImage1.Position.Y;
            float paddleBottom = _playerPaddleImage1.Position.Y + _playerPaddleImage1.Size.Height;

            float ballTop = _ballImage.Position.Y;
            float ballBottom = _ballImage.Position.Y + _ballImage.Size.Height;

            if (ballTop >= paddleTop && ballBottom <= paddleBottom)
            {
                _ballImage.Position.X = player1PaddleRight + 5;
                _ballDirection.Reflect(Vector2D.Right);
            }
        }

        float ballRight = _ballImage.Position.X + _ballImage.Size.Width;
        float player2PaddleLeft = _playerPaddleImage2.Position.X;

        if (ballRight >= player2PaddleLeft)
        {
            float paddleTop = _playerPaddleImage2.Position.Y;
            float paddleBottom = _playerPaddleImage2.Position.Y + _playerPaddleImage2.Size.Height;

            float ballTop = _ballImage.Position.Y;
            float ballBottom = _ballImage.Position.Y + _ballImage.Size.Height;

            if (ballTop >= paddleTop && ballBottom <= paddleBottom)
            {
                _ballImage.Position.X = player2PaddleLeft - (_ballImage.Size.Width + 5);
                _ballDirection.Reflect(Vector2D.Left);
            }
        }
    }
}