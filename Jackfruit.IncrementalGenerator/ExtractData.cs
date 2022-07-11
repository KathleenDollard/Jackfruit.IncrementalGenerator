using Jackfruit.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public class ExtractData
    {
        private const string defineName = "Define";
        private const string addSubCommandName = "AddSubCommands";
        private const string setDelegateName = "SetAction";

        public static CommandDetail GetDetails(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(node, cancellationToken);
            return symbolInfo.Symbol switch
            {
                ITypeSymbol typeSymbol => DetailFromType(typeSymbol, semanticModel, node.GetLocation(), cancellationToken),
                _ => CommandDetail.Empty.InternalError("Root is not a type", node.GetLocation())
            };
        }

        private static CommandDetail DetailFromType(ITypeSymbol typeSymbol, SemanticModel semanticModel, Location location, CancellationToken cancellationToken)
        {
            var commandDetail = new CommandDetail(typeSymbol.Name, "Root","int",typeSymbol.ContainingNamespace.ToString());
            commandDetail.XmlDocs = typeSymbol.GetDocumentationCommentXml();

            var method = typeSymbol.GetTypeMembers("Define", 0)
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault();
            if (method is null)
            { return CommandDetail.Empty.InternalError("Define method not found", location); }

            if (!(method.DeclaringSyntaxReferences.SingleOrDefault()?.GetSyntax() is MethodDeclarationSyntax methodSyntax))
            { return CommandDetail.Empty.InternalError("Issue getting method syntax", location); }
            var addSubCommandSyntaxes = methodSyntax.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(x => RoslynHelpers.GetName(x.Expression).methodName == addSubCommandName);
            var setDelegateSyntax = methodSyntax.ChildNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(x => RoslynHelpers.GetName(x.Expression).methodName == setDelegateName)
                    .FirstOrDefault();
            if (!addSubCommandSyntaxes.Any() && setDelegateSyntax is null)
            { commandDetail.UserWarning(Error.NoActionOrSubCommandsId, Error.NoActionOrSubCommandsMessage, location); }

            foreach (var subCommand in addSubCommandSyntaxes)
            {

            }

            if (setDelegateSyntax is not null)
            {
                var setDelegateOp = semanticModel.GetOperation(setDelegateSyntax);
                if (!(setDelegateOp is IInvocationOperation setDelegateInvocation))
                { return commandDetail.InternalError("SetDelegate is not an invocation", setDelegateSyntax.GetLocation()); }
                commandDetail.MemberDetails = MemberDetails(setDelegateInvocation, commandDetail, setDelegateSyntax.GetLocation());


        
        }

            return commandDetail;
        }

        private static List<Detail> MemberDetails(IInvocationOperation setDelegateInvocation,
                                                         CommandDetail commandDetail,
                                                         Location location)
        {
            if (setDelegateInvocation.Arguments.Length != 1)
            { return new List<Detail>(); } // This scenario will be reported as a syntax error

            var handlerMethodSymbol = RoslynHelpers.MethodFromArg(setDelegateInvocation.Arguments[0]);
            if (handlerMethodSymbol == null)
            {
                commandDetail.UserWarning(Error.SetDelegateNotMethodGroupId,
                                          Error.SetDelegateNotMethodGroupMessage,
                                          location);
                return new List<Detail>()   ;
            }

            var memberDetails = new List<Detail>();

            foreach (var param in handlerMethodSymbol.Parameters)
            {
                //param.GetAttributes
                var detail = new Detail(param.Name, param.Name, param.Type.ToString());
                if (param.Name.EndsWith("Arg"))
                {
                    detail.MemberKind = MemberKind.Argument;
                    detail.Name = detail.Name.Substring(0, param.Name.Length - 3);
                }
                else if (param.Type.IsAbstract)  // Test that this is true for interfaces
                {
                    detail.MemberKind = MemberKind.Service;
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
