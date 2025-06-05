using genapi_api.Data.GeneratorData;
using genapi_api.Utilities;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace genapi_api.Services.GeneratorServices.Strategies
{
    public class AzureSqlCodeStrategy : ICodeGenerationStrategy
    {
        public void GenerateProjectContext(CodeGenerationOptions options)
        {
            GenerateNETProject(options.ProjectName, options.ProjectParentPath);
            AddDependencies(options.ProjectPath, options.ProjectName);
            GenerateProgramCS(options.ProjectPath, options.ProjectName);
            GithubService.GenerateDefaultGitignore(options.ProjectPath);
        }
        private void GenerateNETProject(string projectName, string projectParentPath)
        {
            Log.Information($"GenerateNETProject: Generating .NET Web API project...");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new webapi -n {projectName}",
                WorkingDirectory = projectParentPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };


            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Log.Error($"GenerateNETProject: Error while generating .NET Web API! Error: {error}");
                    throw new Exception(error);
                }
            }
        }
        private void AddDependencies(string projectPath, string projectName)
        {
            Log.Information($"AddDependencies: Adding dependecies...");
            string csprojPath = Path.Combine(projectPath, $"{projectName}.csproj");

            // Load the project file
            XDocument doc = XDocument.Load(csprojPath);

            // Find the ItemGroup where PackageReference elements should be added
            // or create a new one if it doesn't exist
            XElement? itemGroup = doc.Root.Elements("ItemGroup")
                .FirstOrDefault(e => e.Elements("PackageReference").Any());

            if (itemGroup == null)
            {
                itemGroup = new XElement("ItemGroup");
                doc.Root.Add(itemGroup);
            }

            // Add the required package references if they don't already exist
            AddPackageIfNotExists(itemGroup, "Microsoft.EntityFrameworkCore", "8.0.0");
            AddPackageIfNotExists(itemGroup, "Microsoft.EntityFrameworkCore.SqlServer", "8.0.0");
            AddPackageIfNotExists(itemGroup, "Microsoft.EntityFrameworkCore.Design", "8.0.0");

            // Save the modified project file
            doc.Save(csprojPath);
        }
        private void AddPackageIfNotExists(XElement itemGroup, string packageName, string version)
        {
            // Check if the package reference already exists
            if (!itemGroup.Elements("PackageReference")
                .Any(e => e.Attribute("Include")?.Value == packageName))
            {
                // Add the package reference
                itemGroup.Add(new XElement("PackageReference",
                    new XAttribute("Include", packageName),
                    new XAttribute("Version", version)));
            }
        }
        private void GenerateProgramCS(string projectPath, string projectName)
        {
            Log.Information($"GenerateProgramCS: Generating Program.cs file...");
            string programCsPath = Path.Combine(projectPath, "Program.cs");

            var sb = new StringBuilder();

            // Add using statements
            sb.AppendLine($"using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {projectName}.Data;");
            sb.AppendLine();

            // Add builder configuration
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine();

            // Add services
            sb.AppendLine("// Add services to the container");
            sb.AppendLine("builder.Services.AddControllers();");
            sb.AppendLine("builder.Services.AddEndpointsApiExplorer();");
            sb.AppendLine("builder.Services.AddSwaggerGen();");
            sb.AppendLine();

            // Add DbContext configuration
            sb.AppendLine("// Configure DbContext with Azure SQL connection");
            sb.AppendLine("builder.Services.AddDbContext<ApplicationDbContext>(options =>");
            sb.AppendLine("    options.UseSqlServer(builder.Configuration.GetConnectionString(\"DefaultConnection\")));");
            sb.AppendLine();

            // Build the app
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine();

            // Database creation code
            sb.AppendLine("// Create database and schema during startup");
            sb.AppendLine("using (var scope = app.Services.CreateScope())");
            sb.AppendLine("{");
            sb.AppendLine("    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();");
            sb.AppendLine("    context.EnsureDatabaseCreated();");
            sb.AppendLine("}");
            sb.AppendLine();

            // Configure HTTP pipeline
            sb.AppendLine("// Configure the HTTP request pipeline");
            sb.AppendLine("if (app.Environment.IsDevelopment())");
            sb.AppendLine("{");
            sb.AppendLine("    app.UseSwagger();");
            sb.AppendLine("    app.UseSwaggerUI();");
            sb.AppendLine("}");
            sb.AppendLine();

            // Add middleware
            sb.AppendLine("app.UseHttpsRedirection();");
            sb.AppendLine("app.UseAuthorization();");
            sb.AppendLine();

            // Enable routing and run
            sb.AppendLine("// Enable controller routing");
            sb.AppendLine("app.MapControllers();");
            sb.AppendLine();
            sb.AppendLine("app.Run();");

            // Write the content to Program.cs
            File.WriteAllText(programCsPath, sb.ToString(), Encoding.UTF8);
        }
        public void GenerateDbContext(CodeGenerationOptions options)
        {
            Log.Information($"GenerateApplicationDbContext: Generating ApplicationDbContext...");

            string dataPath = Path.Combine(options.ProjectPath, "Data");
            Directory.CreateDirectory(dataPath);
            string projectName = options.ProjectName;
            List<Resource> resources = options.Configurations.Resources;
            StringBuilder sb = new StringBuilder();

            // Add using statements + namespace
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {projectName}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {projectName}.Data");
            sb.AppendLine("{");

            // Add class name + constructor
            sb.AppendLine("    public class ApplicationDbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine("        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");

            // Add DbSets from given models
            for (int i = 0; i < resources.Count; i++)
            {
                sb.AppendLine($"        public DbSet<{resources[i].Name}> {resources[i].Name}s {{ get; set; }}");
            }

            // Add OnModelCreating
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnModelCreating(modelBuilder);");
            sb.AppendLine("        }");

            // Add EnsureDatabaseCreated
            sb.AppendLine("        public void EnsureDatabaseCreated()");
            sb.AppendLine("        {");
            sb.AppendLine("            this.Database.EnsureCreated();");
            sb.AppendLine("        }");

            // Add closing brackets
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(dataPath, "ApplicationDbContext.cs"), sb.ToString());
        }
        public void GenerateModels(CodeGenerationOptions options)
        {
            Log.Information($"GenerateModels: Generating Model classes...");
            string modelsPath = Path.Combine(options.ProjectPath, "Models");
            Directory.CreateDirectory(modelsPath);

            foreach (var resource in options.Configurations.Resources)
            {
                string resourceCode = GenerateModel(resource, options.ProjectName);
                string resourceFilename = string.Concat(resource.Name, ".cs");
                File.WriteAllText(Path.Combine(modelsPath, resourceFilename), resourceCode);
            }
        }
        private string GenerateModel(Resource resource, string projectName)
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
        public void GenerateControllers(CodeGenerationOptions options)
        {
            Log.Information($"GenerateControllers: Generating Controller classes...");
            string controllersPath = Path.Combine(options.ProjectPath, "Controllers");
            Directory.CreateDirectory(controllersPath);

            foreach (string resourceName in options.Configurations.Resources.Select(r => r.Name).ToList())
            {
                string resourcePluralName = string.Concat(resourceName, "s");
                string resourceControllerFilename = string.Concat(resourcePluralName, "Controller.cs");

                string content = GenerateController(resourceName, resourcePluralName, options.ProjectName);
                File.WriteAllText(Path.Combine(controllersPath, resourceControllerFilename), content);
            }
        }
        private string GenerateController(string resourceName, string resourcePluralName, string projectName)
        {

            StringBuilder sb = new StringBuilder();

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