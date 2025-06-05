using genapi_api.Services.GeneratorServices.Generators.Interfaces;

namespace genapi_api.Services.GeneratorServices.Factories
{
    public interface IApiGeneratorFactory
    {
        IProjectGenerator CreateProjectGenerator();
        IControllerGenerator CreateControllerGenerator();
        IDatabaseContextGenerator CreateDbContextGenerator();
        IModelGenerator CreateModelGenerator();
    }
}