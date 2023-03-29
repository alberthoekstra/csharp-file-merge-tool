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

        var fileContents = files.Select(File.ReadAllText).ToArray();
        var result = MergeFileContent(fileContents);
        
        File.WriteAllText(outputFile, result);
    }
    
    /// <summary>
    /// Merges all .cs file contents into one single string
    /// </summary>
    /// <param name="fileContents"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string MergeFileContent(params string[] fileContents)
    {
        if (fileContents == null || fileContents.Length == 0)
        {
            throw new ArgumentNullException(nameof(fileContents));
        }

        var mergedMembers = new SyntaxList<MemberDeclarationSyntax>();
        var mergedUsings = new SyntaxList<UsingDirectiveSyntax>();
        
        foreach (var fileContent in fileContents)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            
            var root = syntaxTree.GetCompilationUnitRoot();

            mergedUsings = mergedUsings.AddRange(root.Usings);
            mergedMembers = mergedMembers.AddRange(root.Members);
        }

        var uniqueUsings = mergedUsings.Distinct(SyntaxNodeComparer.UsingDirectiveComparer).ToArray();
        
        var orderedMembers = mergedMembers
            .OrderBy(member => member is NamespaceDeclarationSyntax ? 1 : 0)
            .ToArray();
        
        var mergedRoot = SyntaxFactory.CompilationUnit()
            .WithUsings(SyntaxFactory.List(uniqueUsings))
            .WithMembers(SyntaxFactory.List(orderedMembers))
            .NormalizeWhitespace();

        var result = mergedRoot.ToFullString();
        return result;
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