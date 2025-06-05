using genapi_api.Data.GeneratorData;

namespace genapi_api.Services.GeneratorServices.Strategies
{
    public interface ICodeGenerationStrategy
    {
        void GenerateProjectContext(CodeGenerationOptions options);
        void GenerateDbContext(CodeGenerationOptions options);
        void GenerateModels(CodeGenerationOptions options);
        void GenerateControllers(CodeGenerationOptions options);
    }
}