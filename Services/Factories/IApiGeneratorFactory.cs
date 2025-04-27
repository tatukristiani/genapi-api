using genapi_api.Services.Generators.Interfaces;

namespace genapi_api.Services.Factories
{
    public interface IApiGeneratorFactory
    {
        IProjectGenerator CreateProjectGenerator();
        IControllerGenerator CreateControllerGenerator();
        IDatabaseContextGenerator CreateDbContextGenerator();
        IModelGenerator CreateModelGenerator();
    }
}