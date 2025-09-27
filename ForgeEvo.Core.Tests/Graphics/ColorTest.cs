using ForgeEvo.Core.Engine;
using JetBrains.Annotations;
using Xunit;

namespace ForgeEvo.Core.Tests.Engine;

[TestSubject(typeof(Color))]
public class ColorTest
{
    [Fact]
    public void AdditionClamp_Works()
    {
        Color first = new(100, 100, 100, 100);
        Color second = new(200, 200, 200, 200);

        Assert.Equal(Color.White, first + second);
    }

    [Fact]
    public void SubtractionClamp_Works()
    {
        Color first = new(100, 100, 100, 100);
        Color second = new(200, 200, 200, 200);

        Assert.Equal(Color.Transparent, first - second);
    }

    [Fact]
    public void MultiplicationClamp_Works()
    {
        Color first = new(100, 100, 100, 100);
        const float scalar = 5F;

        Assert.Equal(Color.White, first * scalar);
    }
}