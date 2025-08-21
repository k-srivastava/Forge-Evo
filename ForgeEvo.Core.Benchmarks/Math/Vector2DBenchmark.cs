using System.Numerics;
using BenchmarkDotNet.Attributes;
using ForgeEvo.Core.Math;

namespace ForgeEvo.Core.Benchmarks.Math;

[MemoryDiagnoser]
public class Vector2DBenchmark
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
    public float System_Dot() => Vector2.Dot(_systemA, _systemB);

    [Benchmark]
    public float Forge_Dot() => _forgeA * _forgeB;

    [Benchmark]
    public Vector2 System_Scale() => _systemA * 5f;

    [Benchmark]
    public Vector2D Forge_Scale() => _forgeA * 5f;

    [Benchmark]
    public float System_Length() => _systemA.Length();

    [Benchmark]
    public float Forge_Length() => _forgeA.Length();
}