using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DnDFightTool.Analyzers;

/// <summary>
///     Flags any property or field access on a local variable that was passed as the
///     first argument to <c>SendAsSubCommandAsync</c>.
///     <para>
///         After dispatch the sub-command is owned by the mediator.  Callers must read
///         output through the typed response (<c>ICommandResponse&lt;T&gt;.Response</c>)
///         instead of reaching back into the sub-command object.
///     </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SubCommandPropertyAccessAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Diagnostic identifier emitted by this analyzer.</summary>
    public const string DiagnosticId = "DNDFA001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Do not access sub-command properties after dispatch",
        messageFormat: "'{0}' was dispatched as a sub-command; do not read its properties after dispatch — use the typed response instead",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
            "After calling SendAsSubCommandAsync the sub-command object must not be " +
            "inspected.  Return data through the typed response (CommandBase<T>) instead.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            return ImmutableArray.Create(Rule);
        }
    }

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationBlockAction(AnalyzeBlock);
    }

    private static void AnalyzeBlock(OperationBlockAnalysisContext context)
    {
        foreach (var block in context.OperationBlocks)
        {
            new SubCommandWalker(context).Visit(block);
        }
    }

    // -------------------------------------------------------------------------

    private sealed class SubCommandWalker : OperationWalker
    {
        private readonly OperationBlockAnalysisContext _context;

        /// <summary>
        ///     Locals that have been passed to <c>SendAsSubCommandAsync</c> as the
        ///     sub-command argument.  Any property / field access on these locals is
        ///     flagged regardless of which command type they hold.
        /// </summary>
        private readonly HashSet<ILocalSymbol> _dispatchedLocals =
            new HashSet<ILocalSymbol>(SymbolEqualityComparer.Default);

        public SubCommandWalker(OperationBlockAnalysisContext context)
        {
            _context = context;
        }

        // ---- Track dispatch -------------------------------------------------

        public override void VisitInvocation(IInvocationOperation operation)
        {
            if (IsSendAsSubCommandAsync(operation) && operation.Arguments.Length >= 1)
            {
                var firstArg = Unwrap(operation.Arguments[0].Value);
                if (firstArg is ILocalReferenceOperation localRef)
                {
                    _dispatchedLocals.Add(localRef.Local);
                }
            }

            // Visit children AFTER recording the dispatch so that any property
            // accesses nested inside the call arguments are evaluated with the
            // up-to-date set.
            base.VisitInvocation(operation);
        }

        // ---- Flag post-dispatch access --------------------------------------

        public override void VisitPropertyReference(IPropertyReferenceOperation operation)
        {
            CheckMemberAccess(operation.Instance, operation.Syntax);
            base.VisitPropertyReference(operation);
        }

        public override void VisitFieldReference(IFieldReferenceOperation operation)
        {
            CheckMemberAccess(operation.Instance, operation.Syntax);
            base.VisitFieldReference(operation);
        }

        // ---- Helpers --------------------------------------------------------

        private void CheckMemberAccess(IOperation? instance, SyntaxNode syntax)
        {
            if (instance is null)
            {
                return;
            }

            if (Unwrap(instance) is ILocalReferenceOperation localRef &&
                _dispatchedLocals.Contains(localRef.Local))
            {
                _context.ReportDiagnostic(
                    Diagnostic.Create(Rule, syntax.GetLocation(), localRef.Local.Name));
            }
        }

        private static bool IsSendAsSubCommandAsync(IInvocationOperation operation)
        {
            return operation.TargetMethod.Name == "SendAsSubCommandAsync";
        }

        /// <summary>Strips implicit conversions to reach the underlying operation.</summary>
        private static IOperation Unwrap(IOperation operation)
        {
            while (operation is IConversionOperation conversion && conversion.IsImplicit)
            {
                operation = conversion.Operand;
            }

            return operation;
        }
    }
}
