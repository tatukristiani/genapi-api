using genapi_api.Data;

namespace genapi_api.Services.Strategies
{
    public interface ICodeGenerationStrategy
    {
        void GenerateProjectContext(CodeGenerationOptions options);
        void GenerateDbContext(CodeGenerationOptions options);
        void GenerateModels(CodeGenerationOptions options);
        void GenerateControllers(CodeGenerationOptions options);
    }
}