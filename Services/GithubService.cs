using LibGit2Sharp;
using Serilog;
using System.Diagnostics;

namespace genapi_api.VersionControl
{
    public static class GithubService
    {
        public static void GenerateDefaultGitignore(string projectPath)
        {
            Log.Information($"GenerateDefaultGitignore: Generating .gitignore file...");
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

        public static void InitializeGitRepo(string projectPath)
        {
            Log.Information($"InitializeGitRepo: Initializing Git repository...");

            Repository.Init(projectPath); // Initializes a new Git repo
            using var repo = new Repository(projectPath);
            Commands.Stage(repo, "*");
            Signature author = new Signature("tatukristiani", "tatukristian@gmail.com", DateTimeOffset.Now);
            repo.Commit("Initial commit", author, author);
        }

        public static void PushToGitHub(string projectPath, string remoteUrl, string username, string token)
        {
            Log.Information($"PushToGitHub: Pushing to Git...");

            using var repo = new Repository(projectPath);

            var branch = repo.CreateBranch("main");
            Commands.Checkout(repo, branch);
            repo.Network.Remotes.Add("origin", remoteUrl);
            repo.Branches.Update(branch, b => b.Remote = "origin", b => b.UpstreamBranch = branch.CanonicalName);

            var pushOptions = new PushOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = username,
                    Password = token
                }
            };

            repo.Network.Push(branch, pushOptions);
        }
    }
}
