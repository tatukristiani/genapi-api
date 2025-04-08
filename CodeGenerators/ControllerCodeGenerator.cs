using Serilog;
using System.Text;

namespace genapi_api.CodeGenerators
{
    public static class ControllerCodeGenerator
    {
        public static void GenerateControllers(List<string> resourceNames, string projectName, string controllersPath)
        {
            Log.Information($"GenerateControllers: Generating Controller classes...");
            foreach (string resourceName in resourceNames)
            {
                string resourcePluralName = String.Concat(resourceName, "s");
                string resourceControllerFilename = String.Concat(resourcePluralName, "Controller.cs");

                string content = GenerateController(resourceName, resourcePluralName, projectName);
                File.WriteAllText(Path.Combine(controllersPath, resourceControllerFilename), content);
            }
        }

        private static string GenerateController(string resourceName, string resourcePluralName, string projectName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {projectName}.Data;");
            sb.AppendLine($"using {projectName}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {projectName}.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine($"    public class {resourcePluralName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly ApplicationDbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {resourcePluralName}Controller(ApplicationDbContext context)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = context;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        // GET: api/{resourcePluralName}");
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<IEnumerable<{resourceName}>>> Get{resourcePluralName}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _context.{resourcePluralName}.ToListAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        // GET: api/{resourcePluralName}/5");
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{resourceName}>> Get{resourceName}(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var {resourceName.ToLower()} = await _context.{resourcePluralName}.FindAsync(id);");
            sb.AppendLine();
            sb.AppendLine($"            if ({resourceName.ToLower()} == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                return NotFound();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            return {resourceName.ToLower()};");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        // POST: api/{resourcePluralName}");
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{resourceName}>> Create{resourceName}({resourceName} {resourceName.ToLower()})");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.{resourcePluralName}.Add({resourceName.ToLower()});");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine();
            sb.AppendLine($"            return CreatedAtAction(nameof(Get{resourceName}), new {{ id = {resourceName.ToLower()}.Id }}, {resourceName.ToLower()});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        // PUT: api/{resourcePluralName}/5");
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Update{resourceName}(int id, {resourceName} {resourceName.ToLower()})");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (id != {resourceName.ToLower()}.Id)");
            sb.AppendLine("            {");
            sb.AppendLine("                return BadRequest();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            _context.Entry({resourceName.ToLower()}).State = EntityState.Modified;");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                await _context.SaveChangesAsync();");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (DbUpdateConcurrencyException)");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (!{resourceName}Exists(id))");
            sb.AppendLine("                {");
            sb.AppendLine("                    return NotFound();");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            sb.AppendLine("                    throw;");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        // DELETE: api/{resourcePluralName}/5");
            sb.AppendLine("        [HttpDelete(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Delete{resourceName}(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var {resourceName.ToLower()} = await _context.{resourcePluralName}.FindAsync(id);");
            sb.AppendLine($"            if ({resourceName.ToLower()} == null)");
            sb.AppendLine("            {");
            sb.AppendLine("                return NotFound();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            _context.{resourcePluralName}.Remove({resourceName.ToLower()});");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine();
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private bool {resourceName}Exists(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return _context.{resourcePluralName}.Any(e => e.Id == id);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
