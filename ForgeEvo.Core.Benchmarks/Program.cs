using BenchmarkDotNet.Running;
using ForgeEvo.Core.Benchmarks.Math;

namespace ForgeEvo.Core.Benchmarks;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Vector2DBenchmark>();
    }
}