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
            Log.Information($"GenerateDefaultProgramCS: Program.cs file location is {programCsPath}");

            // Default content for Program.cs
            string content = $@"using Microsoft.EntityFrameworkCore;
            using {projectName}.Data;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure DbContext with Azure SQL connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(""DefaultConnection"")));

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {{
                app.UseSwagger();
                app.UseSwaggerUI();
            }}

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Enable controller routing
            app.MapControllers();

            app.Run();
            ";

            // Write the content to Program.cs
            File.WriteAllText(programCsPath, content, Encoding.UTF8);
            Log.Information($"GenerateDefaultProgramCS: Program.cs file generated.");
        }

        public static void AddAzureSqlPackagesToProjectFile(string projectPath, string projectName)
        {
            Log.Information($"AddAzureSqlPackagesToProjectFile: Addition started...");
            string csprojPath = Path.Combine(projectPath, $"{projectName}.csproj");

            try
            {
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
                Log.Information($"AddAzureSqlPackagesToProjectFile:  Package references added successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"AddAzureSqlPackagesToProjectFile: Error updating project file: {ex.Message}");
            }
        }

        public static void GenerateDefaultWebAPI(string projectName, string workingDirectory)
        {
            Log.Information($"GenerateDefaultWebAPI: Generating default .NET Web API...");
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
                    return;
                }

                Log.Information($"GenerateDefaultWebAPI: Generation of .NET Web API ready.");
            }
        }

        public static void GenerateGitignore(string projectPath)
        {
            Log.Information($"GenerateGitignore: Generating .gitignore file...");
            string content = "bin/\nobj/\n";
            File.WriteAllText(Path.Combine(projectPath, ".gitignore"), content);
            Log.Information($"GenerateGitignore: Generation of .gitignore file ready.");
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
