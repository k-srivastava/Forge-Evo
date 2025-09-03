using System;
using System.Numerics;
using ForgeEvo.Core.Math;
using JetBrains.Annotations;
using Xunit;

namespace ForgeEvo.Core.Tests.Math;

[TestSubject(typeof(Vector2D))]
public class Vector2DTest
{
    [Fact]
    public void ImplicitConversion_ToAndFromVector2_Works()
    {
        Vector2 systemVector = new(3F, -4F);
        Vector2D forgeVector = systemVector;
        Vector2 back = forgeVector;

        Assert.Equal(systemVector.X, forgeVector.X);
        Assert.Equal(systemVector.Y, forgeVector.Y);
        Assert.Equal(systemVector, back);
    }

    [Fact]
    public void Length_And_LengthSquared_AreConsistent()
    {
        Vector2D vector = new(3F, 4F);
        Assert.Equal(5F, vector.Length(), 5);
        Assert.Equal(25F, vector.LengthSquared(), 5);
    }

    [Fact]
    public void Normal_ProducesUnitLengthVector()
    {
        Vector2D vector = new(10F, 0F);
        Vector2D normal = vector.Normal();

        Assert.True(normal.IsNormalized());
    }

    [Fact]
    public void Normal_ZeroVector_Throws()
    {
        Assert.Throws<DivideByZeroException>(() => Vector2D.Zero.Normal());
    }

    [Fact]
    public void Orthogonal_IsPerpendicular()
    {
        Vector2D vector = new(1F, 0F);
        Vector2D orthogonal = vector.Orthogonal();
        float dot = vector * orthogonal;

        Assert.Equal(0F, dot, 5);
    }

    [Fact]
    public void ReflectTo_WithNormalizedNormal_Works()
    {
        Vector2D vector = new(1F, -1F);
        Vector2D normal = new Vector2D(0F, 1F).Normal();
        Vector2D reflected = vector.ReflectTo(normal);

        Assert.Equal(new(1, 1), reflected);
    }

    [Fact]
    public void ReflectTo_WithNonNormalizedNormal_Throws()
    {
        Vector2D vector = new(1F, -1F);
        Vector2D invalidNormal = new(0F, 5F);

        Assert.Throws<ArgumentException>(() => vector.ReflectTo(invalidNormal));
    }

    [Fact]
    public void Angle_BetweenSameVector_IsZero()
    {
        Vector2D vector = Vector2D.One;
        Assert.Equal(0f, vector.Angle(vector), 5);
    }

    [Fact]
    public void Distance_Euclidean_And_Manhattan_Work()
    {
        Vector2D a = new(1F, 2F);
        Vector2D b = new(4F, 6F);

        Assert.Equal(5f, Vector2D.Distance(a, b), 5);
        Assert.Equal(7f, Vector2D.Distance(a, b, Distance.Manhattan), 5);
    }

    [Fact]
    public void LinearInterpolation_HalfwayPoint()
    {
        Vector2D start = Vector2D.Zero;
        Vector2D end = new(10F, 0F);
        Vector2D mid = Vector2D.LinearInterpolation(start, end, 0.5F);

        Assert.Equal(new(5F, 0F), mid);
    }

    [Fact]
    public void Operators_DotAndCross_Work()
    {
        Vector2D a = new(1F, 0F);
        Vector2D b = new(0F, 1F);

        Assert.Equal(0F, a * b);
        Assert.Equal(1F, a ^ b);
    }

    [Fact]
    public void Equality_Works()
    {
        Vector2D a = new(1F, 2F);
        Vector2D b = new(1F, 2F);
        Vector2D c = new(2F, 3F);

        Assert.True(a == b);
        Assert.True(a != c);
    }
}