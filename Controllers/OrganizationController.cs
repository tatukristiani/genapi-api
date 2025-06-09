using AutoMapper;
using genapi_api.Data.GenapiData;
using genapi_api.Data.GenapiData.Entities;
using genapi_api.Data.GenapiData.Models;
using Microsoft.AspNetCore.Mvc;

namespace genapi_api.Controllers
{
    [ApiController]
    [Route("api/v1/organizations")]
    public class OrganizationController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<OrganizationController> _logger;
        private static IConfiguration _config;
        private readonly IMapper _mapper;
        public OrganizationController(IDatabaseAgent agent, IMapper mapper, ILogger<OrganizationController> logger, IConfiguration config)
        {
            _agent = agent;
            _logger = logger;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrganizationById([FromRoute] Guid id, [FromHeader(Name = "X-Api-Key")] string apiKeyValue)
        {
            if (string.IsNullOrWhiteSpace(apiKeyValue))
                return Unauthorized("API key is missing.");

            ApiKey? apiKey = await _agent.GetValidApiKey(Guid.Parse(apiKeyValue));

            if (apiKey == null)
                return Unauthorized("Invalid or expired API key.");

            var usage = await _agent.GetApiKeyUsage(apiKey.Id);


            if (usage != null && usage.RequestCount >= apiKey.DailyLimit)
                return StatusCode(429, "Daily API limit exceeded.");

            if (usage == null)
            {
                usage = new ApiKeyUsage
                {
                    ApiKeyId = apiKey.Id,
                    Date = DateTime.UtcNow.Date,
                    RequestCount = 1
                };
                await _agent.AddEntity(usage);
            }
            else
            {
                usage.RequestCount++;
                await _agent.UpdateEntity(usage);
            }

            var org = await _agent.GetOrganizationById(id);
            if (org == null) return NotFound();
            return Ok(_mapper.Map<OrganizationDTO>(org));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationCreateDTO org)
        {
            // Confirm if user exists
            if (await _agent.OrganizationExists(org.Name))
            {
                return BadRequest($"Failed to create organization. Organization name already taken!");
            }

            if (org.Editors.Count == 0) return BadRequest("Failed to create organization. One existing users username must be provided.");

            // Create Organization entity and add user to both editors and users if not already added
            var newOrg = new Organization();
            newOrg.Name = org.Name;
            newOrg.Created = DateTime.UtcNow;
            newOrg.Id = Guid.NewGuid();
            var editors = new List<User>();
            var users = new List<User>();

            foreach (var editor in org.Editors)
            {
                var user = await _agent.GetUserByUsername(editor);
                if (user == null) return BadRequest($"Failed to create organization. Couldn't find user with username '{editor}'.");
                editors.Add(user);
            }

            foreach (var orgUser in org.Users)
            {
                if (editors.Any(u => u.Username != orgUser))
                {
                    var orgUserFromDb = await _agent.GetUserByUsername(orgUser);
                    if (orgUserFromDb == null) return BadRequest($"Failed to create organization. Couldn't find user with username '{orgUser}'.");
                    users.Add(orgUserFromDb);
                }
            }

            newOrg.Users = users.Union(editors).ToList();
            newOrg.Editors = editors;

            var result = await _agent.AddEntity(newOrg);

            return result ? CreatedAtAction(nameof(GetOrganizationById), new { id = newOrg.Id }, _mapper.Map<OrganizationDTO>(newOrg)) : StatusCode(500);
        }
    }
}
