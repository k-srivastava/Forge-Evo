using BenchmarkDotNet.Running;

namespace ForgeEvo.Core.Benchmarks;

public static class Program
{
    public static void Main()
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
    }
}