namespace genapi_api.Data
{
    public class CodeGenerationOptions(string projectPath, string projectName, string projectParentPath, Configurations configurations)
    {

        public string ProjectPath { get; set; } = projectPath;
        public string ProjectName { get; set; } = projectName;
        public string ProjectParentPath { get; set; } = projectParentPath;
        public Configurations Configurations { get; set; } = configurations;
    }
}
