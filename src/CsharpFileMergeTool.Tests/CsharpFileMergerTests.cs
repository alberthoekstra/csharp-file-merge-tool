using FluentAssertions;
using Xunit;

namespace CsharpFileMergeTool.Tests;

public class CsharpFileMergerTests
{
    [Fact]
    public void Should_place_usings_within_namespace()
    {
        const string csharpContent = "using System; namespace Test {}";
        const string exceptedOutcome = "namespace Test { using System; }";

        var changedCsharpContent = CsharpFileMerger.MoveUsingStatementsInsideNamespace(csharpContent);
        changedCsharpContent.Should().Be(exceptedOutcome);
    }
    
    [Fact]
    public void Should_place_usings_within_namespace_with_class()
    {
        const string csharpContent = "using System; namespace Test { public class TestClass {} }";
        const string exceptedOutcome = "namespace Test { using System; public class TestClass { } }";

        var changedCsharpContent = CsharpFileMerger.MoveUsingStatementsInsideNamespace(csharpContent);
        changedCsharpContent.Should().Be(exceptedOutcome);
    }
    
    [Fact]
    public void Should_place_usings_within_namespace_with_class_and_method()
    {
        const string csharpContent = "using System; namespace Test { public class TestClass { public void TestMethod() { } } }";
        const string exceptedOutcome = "namespace Test { using System; public class TestClass { public void TestMethod() { } } }";

        var changedCsharpContent = CsharpFileMerger.MoveUsingStatementsInsideNamespace(csharpContent);
        changedCsharpContent.Should().Be(exceptedOutcome);
    }
}