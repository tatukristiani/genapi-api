using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;

namespace genapi_api.Controllers
{
    [ApiController]
    [Route("api/v1/interfaces")]
    public class InterfaceController : ControllerBase
    {
        private readonly ILogger<InterfaceController> _logger;
        private static IConfiguration _config;

        public InterfaceController(ILogger<InterfaceController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        public IActionResult CreateInterface()
        {
            Log.Information($"{HttpContext.GetEndpoint()?.DisplayName}: Request received");
            return CreateInitialInterface() ? Created() : BadRequest();
        }

        private bool CreateInitialInterface()
        {
            string apiProjectName = $"Genapi-{Guid.NewGuid()}";
            string apiDirectory = Path.Combine(Directory.GetCurrentDirectory(), apiProjectName);

            // Step 1: Create a new directory
            if (Directory.Exists(apiDirectory))
                Directory.Delete(apiDirectory, true);
            Directory.CreateDirectory(apiDirectory);

            // Step 2: Create a new .NET Web API project
            RunCommand($"dotnet new webapi -n {apiProjectName}", Directory.GetCurrentDirectory());

            // Step 3: Modify Program.cs to add a simple status endpoint
            string programCsPath = Path.Combine(apiDirectory, "Program.cs");
            System.IO.File.WriteAllText(programCsPath, @"
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet(""/status"", () => ""Server is online"");
app.Run();
");

            // Step 4: Add a .gitignore file
            System.IO.File.WriteAllText(Path.Combine(apiDirectory, ".gitignore"), "bin/\nobj/\n");

            // Step 5: Initialize Git and push to GitHub
            InitializeGitRepo(apiDirectory);
            return PushToGitHub(apiDirectory);
        }

        static void InitializeGitRepo(string repoPath)
        {
            LibGit2Sharp.Repository.Init(repoPath); // Initializes a new Git repo
            using var repo = new LibGit2Sharp.Repository(repoPath);

            // Stage all files
            Commands.Stage(repo, "*");

            // Commit changes
            LibGit2Sharp.Signature author = new LibGit2Sharp.Signature("tatukristiani", "tatukristian@gmail.com", DateTimeOffset.Now);
            repo.Commit("Initial commit", author, author);

        }

        static bool PushToGitHub(string repoPath)
        {
            try
            {
                using var repo = new LibGit2Sharp.Repository(repoPath);

                string githubUsername = _config["GITHUB_USERNAME"];
                string githubToken = _config["GITHUB_TOKEN"];

                var branch = repo.CreateBranch("main");
                Commands.Checkout(repo, branch);
                repo.Network.Remotes.Add("origin", "https://github.com/tatukristiani/testrepo.git");
                repo.Branches.Update(branch, b => b.Remote = "origin", b => b.UpstreamBranch = branch.CanonicalName);

                var pushOptions = new PushOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = githubUsername,
                        Password = githubToken
                    }
                };

                repo.Network.Push(branch, pushOptions);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        static void RunCommand(string command, string workingDirectory)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            process.WaitForExit();
        }
    }
}
