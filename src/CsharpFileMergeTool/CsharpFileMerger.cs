using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace CsharpFileMergeTool;

public static class CsharpFileMerger
{
    /// <summary>
    /// Merges all .cs files into one single file
    /// </summary>
    /// <param name="directory">Directory to search the .cs files in, including sub directories</param>
    /// <param name="outputFile">Path to output file</param>
    /// <exception cref="ArgumentException"></exception>
    public static async Task MergeAsync(string directory, string outputFile)
    {
        if (!Directory.Exists(directory))
        {
            throw new ArgumentException("Directory does not exist.");
        }

        var files = Directory.GetFiles(directory, "*.cs");

        if (files.Length == 0)
        {
            throw new ArgumentException("No C# files found in directory.");
        }

        using var writer = new StreamWriter(outputFile);

        foreach (var file in files)
        {
            var fileContents = File.ReadAllText(file);
            var changedFileContent = MoveUsingStatementsInsideNamespace(fileContents);
            await writer.WriteAsync(changedFileContent);
        }
    }

    /// <summary>
    /// Moves all using statements from outside the namespace declaration into the namespace declaration to prevent duplicate usings
    /// </summary>
    /// <param name="code">File content of a cs file</param>
    /// <returns></returns>
    public static string MoveUsingStatementsInsideNamespace(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetCompilationUnitRoot();

        var usings = root.Usings.Select(u => u.WithTrailingTrivia()).ToArray();
        var namespaceDeclaration = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        var newRoot = namespaceDeclaration.WithUsings(SyntaxFactory.List(usings));
        var newSyntaxTree = syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options);

        var workspace = new AdhocWorkspace();
        var formatted = Formatter.Format(newSyntaxTree.GetRoot(), workspace);

        return formatted.ToFullString();
    }
}