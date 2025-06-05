using genapi_api.Data.GeneratorData;
using genapi_api.Services.GeneratorServices.Generators.Interfaces;
using genapi_api.Services.GeneratorServices.Strategies;

namespace genapi_api.Services.GeneratorServices.Generators.Concrete
{
    public class DefaultControllerGenerator : IControllerGenerator
    {
        private readonly ICodeGenerationStrategy _strategy;
        private readonly CodeGenerationOptions _options;
        public DefaultControllerGenerator(ICodeGenerationStrategy strategy, CodeGenerationOptions options)
        {
            _strategy = strategy;
            _options = options;
        }
        public void Generate() => _strategy.GenerateControllers(_options);
    }
}