using genapi_api.Data;
using genapi_api.Services.Generators.Interfaces;
using genapi_api.Services.Strategies;

namespace genapi_api.Services.Generators.Concrete
{
    public class DefaultDbContextGenerator : IDatabaseContextGenerator
    {
        private readonly ICodeGenerationStrategy _strategy;
        private readonly CodeGenerationOptions _options;
        public DefaultDbContextGenerator(ICodeGenerationStrategy strategy, CodeGenerationOptions options)
        {
            _strategy = strategy;
            _options = options;
        }
        public void Generate() => _strategy.GenerateDbContext(_options);
    }
}