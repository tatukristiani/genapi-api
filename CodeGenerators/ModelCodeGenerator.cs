using genapi_api.Data;
using genapi_api.Utilities;
using Serilog;
using System.Text;

namespace genapi_api.CodeGeneration
{
    public static class ModelCodeGenerator
    {
        public static void GenerateModels(List<Resource> resources, string modelsPath, string projectName)
        {
            Log.Information($"GenerateModels: Generating Model classes...");
            foreach (var resource in resources)
            {
                string resourceCode = GenerateModel(resource, projectName);
                string resourceFilename = String.Concat(resource.Name, ".cs");
                File.WriteAllText(Path.Combine(modelsPath, resourceFilename), resourceCode);
            }
        }

        private static string GenerateModel(Resource resource, string projectName)
        {
            StringBuilder sb = new StringBuilder();

            // Add namespace
            sb.AppendLine($"namespace {projectName}.Models");
            sb.AppendLine("{");

            // Add class name
            sb.AppendLine($"    public class {resource.Name}");
            sb.AppendLine("    {");

            // Add all properties
            List<Property> properties = resource.Properties;
            for (int i = 0; i < properties.Count; i++)
            {
                string type = DataTypeMapper.MapDataType(properties[i].Type);
                string name = properties[i].Name;
                sb.AppendLine($"        public {type} {name} {{ get; set; }}");

                if (type == "string") sb.Append($" = String.Empty;"); // Initialize String variables
            }

            // Add closing brackets
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
