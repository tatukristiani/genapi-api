using genapi_api.Services.Factories;

namespace genapi_api.Services.Builders
{
    public class ApiBuilder
    {
        private readonly IApiGeneratorFactory _factory;

        public ApiBuilder(IApiGeneratorFactory factory)
        {
            _factory = factory;
        }

        public void Build()
        {
            _factory.CreateProjectGenerator().Generate();
            _factory.CreateDbContextGenerator().Generate();
            _factory.CreateModelGenerator().Generate();
            _factory.CreateControllerGenerator().Generate();
        }
    }
}