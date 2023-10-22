using System.Collections.Immutable;

namespace XCodeAnalysis.Test;



[TestFixture]
public class XMethodAnalyzerTests
{
    [Test]
    public void XMethodAnalyzer_NoError_WhenMethodIsValid()
    {
        // Arrange
        var code = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    [XMethod]
    public void ValidMethod(IReadOnlyXBag bag)
    {
        // method implementation
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("Test", new[] { syntaxTree }, references);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new XMethodAttributeAnalyzer()));

        // Act
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        // Assert
        Assert.IsEmpty(diagnostics);
    }

    [Test]
    public void XMethodAnalyzer_Error_WhenMethodIsInvalid()
    {
        // Arrange
        var code = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    [XMethod]
    public void InvalidMethod()
    {
        // method implementation
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("Test", new[] { syntaxTree }, references);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new XMethodAttributeAnalyzer()));

        // Act
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        // Assert
        Assert.IsNotEmpty(diagnostics);
    }
    
    [Test]
    public void XMethodAnalyzer_NoError_WhenMethodIsMissingAttribute()
    {
        // Arrange
        var code = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    public void MethodWithoutAttribute(IReadOnlyXBag bag)
    {
        // method implementation
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("Test", new[] { syntaxTree }, references);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new XMethodAttributeAnalyzer()));

        // Act
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        // Assert
        Assert.IsEmpty(diagnostics);
    }
    
    [Test]
    public void XMethodAnalyzer_Error_WhenMethodIsInvalid_MultipleParameters()
    {
        // Arrange
        var code = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    [XMethod]
    public void InvalidMethod(IReadOnlyXBag bag, int additionalParameter)
    {
        // method implementation
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("Test", new[] { syntaxTree }, references);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new XMethodAttributeAnalyzer()));

        // Act
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        // Assert
        Assert.IsNotEmpty(diagnostics);
    }

    [Test]
    public void XMethodAnalyzer_Error_WhenMethodIsInvalid_WrongParameterType()
    {
        // Arrange
        var code = @"
using System;
using System.Collections.Generic;

public class TestClass
{
    [XMethod]
    public void InvalidMethod(string bag)
    {
        // method implementation
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };
        var compilation = CSharpCompilation.Create("Test", new[] { syntaxTree }, references);
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new XMethodAttributeAnalyzer()));

        // Act
        var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

        // Assert
        Assert.IsNotEmpty(diagnostics);
    }
}