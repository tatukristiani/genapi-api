using AutoMapper;
using genapi_api.Data.GenapiData;
using genapi_api.Data.GenapiData.Entities;
using Microsoft.AspNetCore.Mvc;

namespace genapi_api.Controllers
{
    [ApiController]
    [Route("api/v1/api-keys")]
    public class ApiKeyController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<ApiKeyController> _logger;
        private static IConfiguration _config;
        private readonly IMapper _mapper;

        public ApiKeyController(IDatabaseAgent agent, IMapper mapper, ILogger<ApiKeyController> logger, IConfiguration config)
        {
            _agent = agent;
            _logger = logger;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetApiKeys()
        {
            return Ok(await _agent.GetApiKeys());
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateApiKey([FromBody] ApiKeyDTO dto)
        {
            ApiKey apiKey = _mapper.Map<ApiKey>(dto);
            apiKey.Id = Guid.NewGuid();
            apiKey.Key = Guid.NewGuid();
            apiKey.CreatedAt = DateTime.UtcNow;
            apiKey.IsActive = true;

            return await _agent.AddEntity(apiKey) ? StatusCode(201) : StatusCode(500);
        }
    }
}
