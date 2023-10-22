using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XCodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class XMethodAttributeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "XMethodAttribute";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "XMethodAttribute must have only one parameter of type IReadOnlyXBag",
        "Method '{0}' has more than one parameter or has a parameter type other than IReadOnlyXBag",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var methodSymbol = (IMethodSymbol)context.Symbol;

        if (!methodSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "XMethodAttribute" || a.AttributeClass?.Name == "XMethod")) return;
        if (methodSymbol.Parameters.Length == 1 && methodSymbol.Parameters[0].Type.Name == "IReadOnlyXBag") return;
        var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
}