using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Extensions;
using UnityEditor;
using UnityEngine;

namespace Mediator
{
    public class DataMediatorHelperGenerator : EditorWindow
    {
        private const string USER_CODE_ASSEMBLY = "Assembly-CSharp";

        [MenuItem("Tools/Generate DataMediator Helper")]
        public static void GenerateCode()
        {
            var outputPath = Path.Combine(Application.dataPath, "Code", "Mediator", "DataMediatorHelper.cs");
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            // Reflection logic here
            var methods = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.FullName.StartsWith(USER_CODE_ASSEMBLY)) 
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsPublic) // Skip interfaces, abstracts, and non-public types
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(method => method.GetCustomAttribute<MediatorHandlerAttribute>() != null);

            // Code generation logic
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using Monads;");
                writer.WriteLine("using Features.Obstacles;");

                writer.WriteLine("namespace Mediator");
                writer.WriteLine("{");
                writer.WriteLine("    public static class DM");
                writer.WriteLine("    {");

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1)
                        throw new InvalidOperationException($"Handler '{method.Name}' must have exactly one parameter.");

                    var requestType = parameters[0].ParameterType;
                    var responseType = method.ReturnType;

                    if (!requestType.IsValueType || !responseType.IsValueType)
                        throw new InvalidOperationException($"Handler '{method.Name}' must use structs for both request and response types.");

                    if (responseType == typeof(void))
                    {
                        // Multicast handler registration (for Publish)
                        writer.WriteLine($"        public static void {method.Name}({requestType.GetConstructorParametersString()})");
                        writer.WriteLine( "        {");
                        writer.WriteLine($"            return DataMediator.Instance.Publish<{requestType.Name}>(new {requestType.Name}({requestType.GetConstructorParameterNamesString()}));");
                        writer.WriteLine( "        }");
                    }
                    else
                    {
                        // Single handler registration (for Send)
                        writer.WriteLine($"        public static {responseType.Name} {method.Name}({requestType.GetConstructorParametersString()})");
                        writer.WriteLine( "        {");
                        writer.WriteLine($"            return DataMediator.Instance.Send<{requestType.Name}, {responseType.Name}>(new {requestType.Name}({requestType.GetConstructorParameterNamesString()}));");
                        writer.WriteLine( "        }");
                    }
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log($"Code generated at: {outputPath}");
            AssetDatabase.Refresh(); // Refresh the Unity Editor to detect the new script
        }
    }
}