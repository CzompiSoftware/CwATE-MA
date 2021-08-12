using System.Reflection;
using System;
using System.Text;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using System.Linq;

namespace Markdig.CWCTMA.XMD
{
    internal class CodeHelper
    {
        private static string CreateExecuteInlineMethodTemplate(string content)
        {
            var builder = new StringBuilder();

            builder.Append($"using System;");
            builder.Append($"\r\nnamespace CodeHelper");
            builder.Append($"\r\n{{");
            builder.Append($"\r\n    public sealed class CSharp");
            builder.Append($"\r\n    {{");
            builder.Append($"\r\n        public static void Main(string[] args) {{ }}");
            builder.Append($"\r\n        public static object Execute()");
            builder.Append($"\r\n        {{");
            builder.Append($"\r\n            return {content};");
            builder.Append($"\r\n        }}");
            builder.Append($"\r\n    }}");
            builder.Append($"\r\n}}");
            return builder.ToString();
        }
        private static string CreateExecuteBlockMethodTemplate(string content)
        {
            var builder = new StringBuilder();

            builder.Append($"using System;");
            builder.Append($"\r\nnamespace CodeHelper");
            builder.Append($"\r\n{{");
            builder.Append($"\r\n    public sealed class CSharp");
            builder.Append($"\r\n    {{");
            builder.Append($"\r\n        public static void Main(string[] args) {{ }}");
            builder.Append($"\r\n        public static object Execute()");
            builder.Append($"\r\n        {{");
            builder.Append($"\r\n            {content.Replace("\r\n", "\r\n            ")}");
            builder.Append($"\r\n        }}");
            builder.Append($"\r\n    }}");
            builder.Append($"\r\n}}");
            return builder.ToString();
        }

        internal static object Execute(string sourceCode)
        {
            Debug.WriteLine($"CreateExecuteBlockMethodTemplate\r\n{sourceCode}");

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var compilation = CSharpCompilation.Create("CodeHelper", new[] { syntaxTree }, MetadataReferences);
            using var memoryStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memoryStream);


            if (!emitResult.Success)
            {
                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    if (diagnostic.Severity != DiagnosticSeverity.Error || diagnostic.IsWarningAsError)
                        continue;

                    Debug.WriteLine($"Compilation failed: {diagnostic.Location} {diagnostic.GetMessage()}");
                }
                return null;
            }
            else
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                var loadContext = new CodeLoadContext("CodeHelper");
                Assembly assembly = loadContext.LoadFromStream(memoryStream);

                Type type = assembly.GetType("CodeHelper.CSharp");
                MethodInfo methodInfo = type.GetMethod("Execute");

                return methodInfo.Invoke(null, null);
            }


        }

        private static WeakReference<PortableExecutableReference[]> _metadataReferences = new WeakReference<PortableExecutableReference[]>(null);
        internal static PortableExecutableReference[] MetadataReferences
        {
            get
            {
                if (_metadataReferences.TryGetTarget(out var value) && value != null)
                    return value;
                var references = GetPortableExecutableReferences();
                _metadataReferences.SetTarget(references);
                return references;
            }
        }
        private static PortableExecutableReference[] GetPortableExecutableReferences()
        {
            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            return trustedAssembliesPaths.Select(reference => MetadataReference.CreateFromFile(reference)).ToArray();
        }
        internal static object ExecuteInline(string content) => Execute(CreateExecuteInlineMethodTemplate(content));
        internal static object ExecuteBlock(string content) => Execute(CreateExecuteBlockMethodTemplate(content));
    }
}