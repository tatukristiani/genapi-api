using Newtonsoft.Json.Linq;
using System.Text;

namespace genapi_api.CodeGeneration
{
    public static class ModelCodeGenerator
    {
        public static void GenerateModels(JArray models, string modelsPath, string projectName)
        {
            foreach (var model in models)
            {
                string modelCode = GenerateModel(model, projectName);
                string modelFilename = String.Concat(model.Value<string>("name"), ".cs");
                File.WriteAllText(Path.Combine(modelsPath, modelFilename), modelCode);
            }
        }

        private static string GenerateModel(JToken model, string projectName)
        {
            StringBuilder sb = new StringBuilder();

            // Add namespace
            sb.Append($"namespace {projectName}.Models\r\n{{\r\n    ");

            // Add class name
            sb.Append($"public class {model["name"]}\r\n    {{\r\n        ");

            // Add all properties (NOT READY)
            JArray properties = model.Value<JArray>("properties");
            for (int i = 0; i < properties.Count; i++)
            {
                string type = properties[i].Value<string>("type");
                string name = properties[i].Value<string>("name");
                if (i == properties.Count - 1)
                {
                    sb.Append($"public {type} {name} {{ get; set; }}\r\n    ");
                }
                else
                {
                    sb.Append($"public {type} {name} {{ get; set; }}\r\n        ");
                }
            }

            // Finally add closing brackets
            sb.Append("}\r\n}");

            return sb.ToString();
        }
    }
}
