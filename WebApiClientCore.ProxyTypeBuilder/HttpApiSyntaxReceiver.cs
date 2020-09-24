﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.ProxyTypeBuilder
{
    /// <summary>
    /// httpApi语法接收器
    /// </summary>
    class HttpApiSyntaxReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// 接口标记名称
        /// </summary>
        private const string ihttpApiTypeName = "WebApiClientCore.IHttpApi";

        /// <summary>
        /// 接口列表
        /// </summary>
        private readonly List<InterfaceDeclarationSyntax> interfaceList = new List<InterfaceDeclarationSyntax>();

        /// <summary>
        /// 访问语法树 
        /// </summary>
        /// <param name="syntaxNode"></param>
        void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax syntax)
            {
                this.interfaceList.Add(syntax);
            }
        }

        /// <summary>
        /// 获取所有HttpApi符号
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public IEnumerable<INamedTypeSymbol> GetHttpApiTypes(Compilation compilation)
        {
            var iHttpApiType = compilation.GetTypeByMetadataName(ihttpApiTypeName);
            if (iHttpApiType == null)
            {
                yield break;
            }

            foreach (var @interface in this.interfaceList)
            {
                var model = compilation.GetSemanticModel(@interface.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(@interface);
                if (symbol != null)
                {
                    if (symbol.AllInterfaces.Any(item => item.Equals(iHttpApiType)))
                    {
                        yield return symbol;
                    }
                }
            }
        }
    }
}