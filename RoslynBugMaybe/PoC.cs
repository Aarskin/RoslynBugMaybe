using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Collections.Generic;

namespace RoslynBugMaybe
{
	[TestFixture]
	public class PoC
	{
		string fixtureSource = @"using NUnit.Framework;

								namespace Foo.NUnit
								{
									public class ThingTests
									{
										[Test]
										public void Test()
										{
										}
									}
								}";

		[Test]
		public void Test_VisitMethodOnly_VisitsMethods()
		{
			SyntaxTree fixtureTree = CSharpSyntaxTree.ParseText(fixtureSource);
			SyntaxNode roslynRoot = fixtureTree.GetRoot();

			MethodCollector methodCollector = new MethodCollector(roslynRoot);
			IEnumerable<string> methodNames = methodCollector.GetMethodNames();

			Assert.That(methodNames, Is.Not.Null.And.Not.Empty);
		}

		[Test]
		public void Test_VisitMethodAndNamespace_VisitsMethods()
		{
			SyntaxTree fixtureTree = CSharpSyntaxTree.ParseText(fixtureSource);
			SyntaxNode roslynRoot = fixtureTree.GetRoot();

			NamespaceAndMethodCollector methodCollector = new NamespaceAndMethodCollector(roslynRoot);
			IEnumerable<string> methodNames = methodCollector.GetMethodNames();

			Assert.That(methodNames, Is.Not.Null.And.Not.Empty);
		}
	}

	internal class MethodCollector : CSharpSyntaxWalker
	{
		private SyntaxNode roslynRoot;
		private List<string> _methodNames;

		public MethodCollector(SyntaxNode roslynRoot)
		{
			this.roslynRoot = roslynRoot;
			_methodNames = new List<string>();
		}

		internal IEnumerable<string> GetMethodNames()
		{
			this.Visit(roslynRoot);

			return _methodNames;
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			_methodNames.Add(node.Identifier.ToString());
		}
	}

	internal class NamespaceAndMethodCollector : CSharpSyntaxWalker
	{
		private SyntaxNode roslynRoot;
		private List<string> _methodNames;
		private string _namespace;

		public NamespaceAndMethodCollector(SyntaxNode roslynRoot)
		{
			this.roslynRoot = roslynRoot;
			_methodNames = new List<string>();
		}

		internal IEnumerable<string> GetMethodNames()
		{
			this.Visit(roslynRoot);

			return _methodNames;
		}

		internal string GetNamespace()
		{
			this.Visit(roslynRoot);

			return _namespace;
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			_methodNames.Add(node.Identifier.ToString());
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			_namespace = node.Name.ToString();
		}
	}
}
