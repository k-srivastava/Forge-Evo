using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Core.Benchmarks.Math;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class Vector2DMicroBenchmarks
{
    private readonly Vector2D _forgeA = new(3f, 4f);
    private readonly Vector2D _forgeB = new(5f, 6f);

    private readonly Vector2 _systemA = new(3f, 4f);
    private readonly Vector2 _systemB = new(5f, 6f);

    [Benchmark(Baseline = true)]
    public Vector2 System_Add() => _systemA + _systemB;

    [Benchmark]
    public Vector2D Forge_Add() => _forgeA + _forgeB;

    [Benchmark]
    public Vector2 System_Sub() => _systemA - _systemB;

    [Benchmark]
    public Vector2D Forge_Sub() => _forgeA - _forgeB;

    [Benchmark]
    public Vector2 System_Scale() => _systemA * 5F;

    [Benchmark]
    public Vector2D Forge_Scale() => _forgeA * 5F;

    [Benchmark]
    public float System_Dot() => Vector2.Dot(_systemA, _systemB);

    [Benchmark]
    public float Forge_Dot() => _forgeA * _forgeB;

    [Benchmark]
    public float System_Length() => _systemA.Length();

    [Benchmark]
    public float Forge_Length() => _forgeA.Length();

    [Benchmark]
    public float System_LengthSquared() => _systemA.LengthSquared();

    [Benchmark]
    public float Forge_LengthSquared() => _forgeA.LengthSquared();

    [Benchmark]
    public Vector2D System_Lerp() => Vector2.Lerp(_systemA, _systemB, 0.25F);

    [Benchmark]
    public Vector2D Forge_Lerp() => Vector2D.LinearInterpolation(_forgeA, _forgeB, 0.25F);

    [Benchmark]
    public Vector2D Forge_Orthogonal() => _forgeA.Orthogonal();

    [Benchmark]
    public Vector2D Forge_Normal() => _forgeA.Normal();

    [Benchmark]
    public Vector2D Forge_ReflectTo()
    {
        Vector2D normal = new Vector2D(0.3F, 1F).Normal();
        return _forgeA.ReflectTo(normal);
    }

    [Benchmark]
    public float Forge_Angle() => _forgeA.Angle(_forgeB);
}

[MemoryDiagnoser]
[DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class Vector2DBatchBenchmarks
{
    [Params(1_024, 16_384, 1_000_000)] public int Length;
    private Vector2D[] _forgeA = null!;
    private Vector2D[] _forgeB = null!;

    private float[] _scalars = null!;

    private Vector2[] _systemA = null!;
    private Vector2[] _systemB = null!;

    [GlobalSetup]
    public void Setup()
    {
        Random rng = new(1293846);

        _systemA = new Vector2[Length];
        _systemB = new Vector2[Length];

        _forgeA = new Vector2D[Length];
        _forgeB = new Vector2D[Length];

        _scalars = new float[Length];

        for (var i = 0; i < Length; i++)
        {
            float x1 = (float)rng.NextDouble() * 100F - 50F;
            float y1 = (float)rng.NextDouble() * 100F - 50F;

            float x2 = (float)rng.NextDouble() * 100F - 50F;
            float y2 = (float)rng.NextDouble() * 100F - 50F;

            _systemA[i] = new(x1, y1);
            _systemB[i] = new(x2, y2);

            _forgeA[i] = new(x1, y1);
            _forgeB[i] = new(x2, y2);

            _scalars[i] = (float)rng.NextDouble() * 2F - 1F;
        }
    }

    [Benchmark(Baseline = true)]
    public Vector2 System_Add_Array()
    {
        Vector2 total = default;

        for (var i = 0; i < Length; i++)
            total += _systemA[i] + _systemB[i];

        return total;
    }

    [Benchmark]
    public Vector2D Forge_Add_Array()
    {
        Vector2D total = default;
        for (var i = 0; i < Length; i++)
            total += _forgeA[i] + _forgeB[i];

        return total;
    }

    [Benchmark]
    public Vector2 System_Sub_Array()
    {
        Vector2 difference = default;
        for (var i = 0; i < Length; i++)
            difference += _systemA[i] - _systemB[i];

        return difference;
    }

    [Benchmark]
    public Vector2D Forge_Sub_Array()
    {
        Vector2D difference = default;
        for (var i = 0; i < Length; i++)
            difference += _forgeA[i] - _forgeB[i];

        return difference;
    }

    [Benchmark]
    public Vector2 System_Scale_Array()
    {
        Vector2 scaled = default;
        for (var i = 0; i < Length; i++)
            scaled += _systemA[i] * _scalars[i];

        return scaled;
    }

    [Benchmark]
    public Vector2D Forge_Scale_Array()
    {
        Vector2D scaled = default;
        for (var i = 0; i < Length; i++)
            scaled += _forgeA[i] * _scalars[i];

        return scaled;
    }

    [Benchmark]
    public float System_Dot_Array()
    {
        var sum = 0F;
        for (var i = 0; i < Length; i++)
            sum += Vector2.Dot(_systemA[i], _systemB[i]);

        return sum;
    }

    [Benchmark]
    public float Forge_Dot_Array()
    {
        var sum = 0F;
        for (var i = 0; i < Length; i++)
            sum += _forgeA[i] * _forgeB[i];

        return sum;
    }

    [Benchmark]
    public float System_Length_Array()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += _systemA[i].Length();

        return total;
    }

    [Benchmark]
    public float Forge_Length_Array()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += _forgeA[i].Length();

        return total;
    }

    [Benchmark]
    public float System_Length_Squared_Array()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += _systemA[i].LengthSquared();

        return total;
    }

    [Benchmark]
    public float Forge_Length_Squared_Array()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += _forgeA[i].LengthSquared();

        return total;
    }

    [Benchmark]
    public Vector2 System_Normal_Array()
    {
        Vector2 total = default;
        for (var i = 0; i < Length; i++)
        {
            Vector2 vector = _systemA[i];
            float length = vector.Length();

            if (length > 1e-12F)
                total += vector / length;
        }

        return total;
    }

    [Benchmark]
    public Vector2D Forge_Normal_Array()
    {
        Vector2D total = default;
        for (var i = 0; i < Length; i++)
        {
            Vector2D vector = _forgeA[i];
            float length = vector.Length();

            if (length > 1e-12F)
                total += vector / length;
        }

        return total;
    }
}

[MemoryDiagnoser]
[DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class Vector2DMathBenchmarks
{
    [Params(4_096, 65_536)] public int Length;

    private Vector2D[] _systemA = null!;
    private Vector2D[] _systemB = null!;

    private Vector2D[] _forgeA = null!;
    private Vector2D[] _forgeB = null!;

    private Vector2D[] _normals = null!;

    [GlobalSetup]
    public void Setup()
    {
        Random rng = new(1293846);

        _systemA = new Vector2D[Length];
        _systemB = new Vector2D[Length];

        _forgeA = new Vector2D[Length];
        _forgeB = new Vector2D[Length];

        _normals = new Vector2D[Length];

        for (var i = 0; i < Length; i++)
        {
            float x1 = (float)rng.NextDouble() * 100F - 50F;
            float y1 = (float)rng.NextDouble() * 100F - 50F;

            float x2 = (float)rng.NextDouble() * 100F - 50F;
            float y2 = (float)rng.NextDouble() * 100F - 50F;

            _systemA[i] = new(x1, y1);
            _systemB[i] = new(x2, y2);

            _forgeA[i] = new(x1, y1);
            _forgeB[i] = new(x2, y2);

            float normalX = (float)rng.NextDouble() * 2F - 1F;
            float normalY = (float)rng.NextDouble() * 2F - 1F;

            Vector2D normal = new(normalX, normalY);
            _normals[i] = normal.Length() > 1e-12F ? normal.Normal() : new(1F, 0F);
        }
    }

    [Benchmark(Baseline = true)]
    public float System_Distance_Euclidean()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += Vector2.Distance(_systemA[i], _systemB[i]);

        return total;
    }

    [Benchmark]
    public float Forge_Distance_Euclidean()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += Vector2D.Distance(_forgeA[i], _forgeB[i]);

        return total;
    }

    [Benchmark]
    public float System_Distance_Squared_Euclidean()
    {
        var total = 0F;
        for (var i = 0; i < Length; i++)
            total += Vector2.DistanceSquared(_systemA[i], _systemB[i]);

        return total;
    }

    [Benchmark]
    public float Forge_Distance_Squared_Euclidean()
    {
        var total = 0f;
        for (var i = 0; i < Length; i++)
            total += Vector2D.DistanceSquared(_forgeA[i], _forgeB[i]);

        return total;
    }

    [Benchmark]
    public float Forge_Distance_Manhattan()
    {
        var total = 0f;
        for (var i = 0; i < Length; i++)
            total += Vector2D.Distance(_forgeA[i], _forgeB[i], Distance.Manhattan);

        return total;
    }

    [Benchmark]
    public float Forge_Angle_Array()
    {
        var total = 0f;
        for (var i = 0; i < Length; i++)
            total += _forgeA[i].Angle(_forgeB[i]);

        return total;
    }

    [Benchmark]
    public Vector2D Forge_Reflect_To_Array()
    {
        Vector2D total = default;
        for (var i = 0; i < Length; i++)
            total += _forgeA[i].ReflectTo(_normals[i]);

        return total;
    }

    [Benchmark]
    public Vector2D Forge_Lerp_Array()
    {
        Vector2D total = default;
        for (var i = 0; i < Length; i++)
        {
            float t = (i & 15) * (1f / 15f);
            total += Vector2D.LinearInterpolation(_forgeA[i], _forgeB[i], t);
        }

        return total;
    }

    [Benchmark]
    public int Forge_IsNormalized_Checks()
    {
        var count = 0;
        for (var i = 0; i < Length; i++)
            if (_normals[i].IsNormalized())
                count++;

        return count;
    }
}