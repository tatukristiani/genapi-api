using AutoMapper;
using genapi_api.Data.GenapiData;
using genapi_api.Data.GenapiData.Entities;
using genapi_api.Data.GenapiData.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace genapi_api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<UserController> _logger;
        private static IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController(IDatabaseAgent agent, IMapper mapper, ILogger<UserController> logger, IConfiguration config)
        {
            _agent = agent;
            _logger = logger;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser([FromQuery] string username)
        {
            var user = await _agent.GetUserByUsername(username);

            return user != null ? Ok(_mapper.Map<UserDTO>(user)) : NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var user = await _agent.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO user)
        {
            // Confirm if user exists
            if (_agent.UserExists(user.Username, user.Email))
            {
                return BadRequest($"Failed to create user.");
            }

            // Craeate User entity
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            User newUser = _mapper.Map<User>(user);
            newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            newUser.PasswordSalt = hmac.Key;
            newUser.Created = DateTime.UtcNow;

            var result = await _agent.AddEntity(newUser);

            return result ? CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, _mapper.Map<UserDTO>(newUser)) : BadRequest();
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Confirm if user exists
            var user = await _agent.GetUserByUsername(request.Username);
            if (user == null) return BadRequest();

            // Confirm password
            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            return computedHash.SequenceEqual(user.PasswordHash) ? Ok(_mapper.Map<UserDTO>(user)) : BadRequest();
        }
    }
}
