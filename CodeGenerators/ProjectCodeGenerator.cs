using genapi_api.Data;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace genapi_api.CodeGeneration
{
    public static class ProjectCodeGenerator
    {
        public static void GenerateDefaultProgramCS(string projectPath, string projectName)
        {
            Log.Information($"GenerateDefaultProgramCS: Generating Program.cs file...");
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

        public static void AddAzureSqlPackagesToProjectFile(string projectPath, string projectName)
        {
            Log.Information($"AddAzureSqlPackagesToProjectFile: Addition started...");
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
            Log.Information($"AddAzureSqlPackagesToProjectFile: Package references DONE.");
        }

        public static void GenerateDefaultWebAPI(string projectName, string workingDirectory)
        {
            Log.Information($"GenerateDefaultWebAPI: Generating default .NET Web API project...");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new webapi -n {projectName}",
                WorkingDirectory = workingDirectory,
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
                    Log.Error($"GenerateDefaultWebAPI: Error while generating .NET Web API! Error: {error}");
                    throw new Exception(error);
                }
            }
        }

        public static void GenerateGitignore(string projectPath)
        {
            Log.Information($"GenerateGitignore: Generating .gitignore file...");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new gitignore",
                WorkingDirectory = projectPath,
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
                    Log.Error($"GenerateGitignore: Error while generating .gitignore. Error: {error}");
                    throw new Exception(error);
                }
            }
        }

        public static void GenerateApplicationDbContext(List<Resource> resources, string projectName, string workingDirectory)
        {
            Log.Information($"GenerateApplicationDbContext: Generating ApplicationDbContext...");
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

            File.WriteAllText(Path.Combine(workingDirectory, "ApplicationDbContext.cs"), sb.ToString());
        }

        /************ Other helper methods ***************/
        private static void AddPackageIfNotExists(XElement itemGroup, string packageName, string version)
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

    }
}
