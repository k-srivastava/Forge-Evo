using ForgeEvo.Core.Math;
using JetBrains.Annotations;
using Xunit;

namespace ForgeEvo.Core.Tests.Math;

[TestSubject(typeof(Size2D))]
public class Size2DTest
{
    [Fact]
    public void ExplicitConversion_ToAndFromVector2D_Works()
    {
        Vector2D vector = new(3F, 4F);
        var size = (Size2D)vector;
        var back = (Vector2D)size;

        Assert.Equal(vector.X, size.Width);
        Assert.Equal(vector.Y, size.Height);
        Assert.Equal(vector, back);
    }

    [Fact]
    public void Equality_Works()
    {
        Size2D a = new(1, 2);
        Size2D b = new(1, 2);
        Size2D c = new(2, 3);

        Assert.True(a == b);
        Assert.True(a != c);
    }
}