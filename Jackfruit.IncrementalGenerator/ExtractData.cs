using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Jackfruit.IncrementalGenerator
{
    public class ExtractData
    {
        private const string defineName = "Define";
        private const string addSubCommandName = "AddSubCommands";
        private const string setDelegateName = "SetAction";

        public static CommandDetail GetDetails(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);
            return symbol switch
            {
                ITypeSymbol typeSymbol => DetailFromType(typeSymbol, semanticModel, node.GetLocation(), cancellationToken),
                _ => CommandDetail.Empty.InternalError("Root is not a type", node.GetLocation())
            };
        }

        private static CommandDetail DetailFromType(ITypeSymbol typeSymbol, SemanticModel semanticModel, Location location, CancellationToken cancellationToken)
        {
            var commandDetail = new CommandDetail(typeSymbol.Name, "Root", "int", typeSymbol.ContainingNamespace.ToString());
            commandDetail.XmlDocs = typeSymbol.GetDocumentationCommentXml();

            var method = typeSymbol.GetMembers("Define")
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault();
            if (method is null)
            { return CommandDetail.Empty.InternalError("Define method not found", location); }

            if (!(method.DeclaringSyntaxReferences.SingleOrDefault()?.GetSyntax() is MethodDeclarationSyntax methodSyntax))
            { return CommandDetail.Empty.InternalError("Issue getting method syntax", location); }
            var addSubCommandSyntaxes = SubCommandSyntaxes(methodSyntax);
         
            var setDelegateSyntax = SetActionSyntax(methodSyntax);
            if (!addSubCommandSyntaxes.Any() && setDelegateSyntax is null)
            { commandDetail.UserWarning(Error.NoActionOrSubCommandsId, Error.NoActionOrSubCommandsMessage, location); }

            foreach (var subCommandSyntax in addSubCommandSyntaxes)
            {
                var subCommandOp = semanticModel.GetOperation(subCommandSyntax);
                if (subCommandOp is not IInvocationOperation subCommandDelegateInvocation)
                { return commandDetail.InternalError("AddSubCommand is not an invocation", subCommandSyntax.GetLocation()); }
                var handlerMethodSymbol = HandlerMethodFromInvocation(subCommandDelegateInvocation, commandDetail, subCommandSyntax.GetLocation());

                if (handlerMethodSymbol is not null)
                {
                    var subCommandDetail = new CommandDetail(handlerMethodSymbol.Name,
                                                             handlerMethodSymbol.Name,
                                                             handlerMethodSymbol.ReturnType.ToString(),
                                                             handlerMethodSymbol.ContainingNamespace.ToString())
                    {
                        MemberDetails = MemberDetails(handlerMethodSymbol, commandDetail, subCommandSyntax.GetLocation())
                    };
                    commandDetail.SubCommandDetails.Add(subCommandDetail);
                }
            }

            if (setDelegateSyntax is not null)
            {
                var setDelegateOp = semanticModel.GetOperation(setDelegateSyntax);
                if (!(setDelegateOp is IInvocationOperation setDelegateInvocation))
                { return commandDetail.InternalError("SetDelegate is not an invocation", setDelegateSyntax.GetLocation()); }
                var handlerMethodSymbol = HandlerMethodFromInvocation(setDelegateInvocation, commandDetail, setDelegateSyntax.GetLocation());
                if (handlerMethodSymbol is not null)
                { commandDetail.MemberDetails = MemberDetails(handlerMethodSymbol, commandDetail, setDelegateSyntax.GetLocation()); }
            }

            return commandDetail;

            static InvocationExpressionSyntax? SetActionSyntax(MethodDeclarationSyntax methodSyntax)
            {
                var expressionStatements = methodSyntax.Body?
                    .Statements
                    .OfType<ExpressionStatementSyntax>();
                var invocations = expressionStatements
                    .Select(x => x.Expression)
                    .OfType<InvocationExpressionSyntax>();
                var invocation = invocations
                    .FirstOrDefault(x => RoslynHelpers.GetName(x.Expression).methodName == setDelegateName);
                return invocation;
            }

            static IEnumerable<InvocationExpressionSyntax?> SubCommandSyntaxes(MethodDeclarationSyntax methodSyntax)
            {
                var expressionStatements = methodSyntax.Body?
                    .Statements
                    .OfType<ExpressionStatementSyntax>();
                var invocations = expressionStatements
                    .Select(x => x.Expression)
                    .OfType<InvocationExpressionSyntax>()
                    .Where(x => RoslynHelpers.GetName(x.Expression).methodName == addSubCommandName);
                return invocations;
            }
        }

        private static IMethodSymbol? HandlerMethodFromInvocation(IInvocationOperation setDelegateInvocation,
                                                         CommandDetail commandDetail,
                                                         Location location)
        {
            if (setDelegateInvocation.Arguments.Length != 1)
            { return null; } // This scenario will be reported as a syntax error

            var handlerMethodSymbol = RoslynHelpers.MethodFromArg(setDelegateInvocation.Arguments[0]);
            if (handlerMethodSymbol == null)
            {
                commandDetail.UserWarning(Error.SetDelegateNotMethodGroupId,
                                          Error.SetDelegateNotMethodGroupMessage,
                                          location);
                return null;
            }
            return handlerMethodSymbol;

        }

        private static List<Detail> MemberDetails(IMethodSymbol handlerMethodSymbol,
                                                         CommandDetail commandDetail,
                                                         Location location)
        {
            var memberDetails = new List<Detail>();

            foreach (var param in handlerMethodSymbol.Parameters)
            {
                var attributes = param.GetAttributes();
                var detail = new Detail(param.Name, param.Name, param.Type.ToString());
                if (param.Name.EndsWith("Arg") || attributes.Any(x => x.AttributeClass?.Name == "Argument"))
                {
                    detail.MemberKind = MemberKind.Argument;
                    detail.Name = detail.Name.Substring(0, param.Name.Length - 3);
                }
                else if (param.Type.IsAbstract)  // Test that this is true for interfaces
                {
                    detail.MemberKind = MemberKind.Service;
                }
                var descAttribute = attributes.Where(x => x.AttributeClass?.Name == "Description").FirstOrDefault();
                if (descAttribute is not null)
                {
                    detail.Description = descAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                }
                memberDetails.Add(detail);
            }
            return memberDetails;
        }






        //    => context.SemanticModel.GetOperation(node, cancellationToken) switch
        //    {
        //        IInvocationOperation rootCommandCreateInvocation => CommandDefFromInvocation(rootCommandCreateInvocation, cancellationToken),
        //        IInvalidOperation invalidOp => CommandDetail.ErrorsOnly(
        //            new Error
        //            {
        //                Message = "Invalid root op",
        //                DefaultSeverity = DiagnosticSeverity.Error,
        //                Location = invalidOp.Syntax.GetLocation()
        //            }),
        //        _ => null
        //    };

        //static CommandDefNode? CommandDefFromInvocation(IInvocationOperation rootCommandCreateInvocation, CancellationToken cancellationToken)
        //    => GetCommandDefNode(new string[] { }, null, Enumerable.Empty<MemberDef>(), rootCommandCreateInvocation, cancellationToken);

        //static CommandDefNode? CommandDefWithParams(IInvalidOperation invalidOp, CancellationToken cancellationToken)
        //{
        //    Location
        //    var invocationOp = invalidOp.ChildOperations.OfType<IInvocationOperation>().FirstOrDefault();
        //    return invocationOp is null
        //        ? null
        //        : GetCommandDefNode(new string[] { }, null, Enumerable.Empty<MemberDef>(), invocationOp, cancellationToken);
        //}
    }
}
