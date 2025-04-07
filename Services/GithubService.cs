using LibGit2Sharp;

namespace genapi_api.VersionControl
{
    public static class GithubService
    {
        public static void InitializeGitRepo(string projectPath)
        {
            Repository.Init(projectPath); // Initializes a new Git repo
            using var repo = new Repository(projectPath);

            // Stage all files
            Commands.Stage(repo, "*");

            // Commit changes
            Signature author = new Signature("tatukristiani", "tatukristian@gmail.com", DateTimeOffset.Now);
            repo.Commit("Initial commit", author, author);
        }

        public static bool PushToGitHub(string projectPath, string remoteUrl, string username, string token)
        {
            try
            {
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
