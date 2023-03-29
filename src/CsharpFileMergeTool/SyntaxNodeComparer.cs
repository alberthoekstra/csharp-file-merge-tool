using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CsharpFileMergeTool;

public static class SyntaxNodeComparer
{
    public static readonly UsingDirectiveComparerType UsingDirectiveComparer = new UsingDirectiveComparerType();

    public class UsingDirectiveComparerType : IEqualityComparer<UsingDirectiveSyntax>
    {
        public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
        {
            return x.Name.ToString() == y.Name.ToString();
        }

        public int GetHashCode(UsingDirectiveSyntax obj)
        {
            return obj.Name.ToString().GetHashCode();
        }
    }
}