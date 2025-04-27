using genapi_api.Data;
using genapi_api.Services.Generators.Interfaces;
using genapi_api.Services.Strategies;

namespace genapi_api.Services.Generators.Concrete
{
    public class DefaultProjectGenerator : IProjectGenerator
    {
        private readonly ICodeGenerationStrategy _strategy;
        private readonly CodeGenerationOptions _options;
        public DefaultProjectGenerator(ICodeGenerationStrategy strategy, CodeGenerationOptions options)
        {
            _strategy = strategy;
            _options = options;
        }
        public void Generate() => _strategy.GenerateProjectContext(_options);
    }
}
