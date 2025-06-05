using genapi_api.Data.GeneratorData;
using genapi_api.Services.GeneratorServices.Generators.Concrete;
using genapi_api.Services.GeneratorServices.Generators.Interfaces;
using genapi_api.Services.GeneratorServices.Strategies;

namespace genapi_api.Services.GeneratorServices.Factories
{
    public class AzureSqlApiGeneratorFactory : IApiGeneratorFactory
    {
        private readonly ICodeGenerationStrategy _strategy;
        private readonly CodeGenerationOptions _options;

        public AzureSqlApiGeneratorFactory(CodeGenerationOptions options)
        {
            _options = options;
            _strategy = new AzureSqlCodeStrategy();
        }
        public IControllerGenerator CreateControllerGenerator() => new DefaultControllerGenerator(_strategy, _options);

        public IDatabaseContextGenerator CreateDbContextGenerator() => new DefaultDbContextGenerator(_strategy, _options);

        public IModelGenerator CreateModelGenerator() => new DefaultModelGenerator(_strategy, _options);

        public IProjectGenerator CreateProjectGenerator() => new DefaultProjectGenerator(_strategy, _options);
    }
}