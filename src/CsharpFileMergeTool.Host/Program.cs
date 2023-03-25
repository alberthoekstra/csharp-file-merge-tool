using System;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: mergecs [output_file] [input_file1] [input_file2] ...");
            return;
        }

        var directory = args[0];
        var outputFile = args[1];

        await CsharpFileMerger.MergeAsync(directory, outputFile);
    }
}
