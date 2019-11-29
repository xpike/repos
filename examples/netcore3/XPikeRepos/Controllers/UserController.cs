using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Example.Library.DataStores;
using Example.Library.Models;
using Example.Library.Repositories;
using Microsoft.AspNetCore.Mvc;
using XPike.Logging;

namespace XPikeRepos.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController
        : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDataStore _dataStore;

        private readonly ILog<UserController> _logger;

        public UserController(ILog<UserController> logger,
            IUserRepository userRepository,
            IUserDataStore dataStore)
        {
            _logger = logger;
            _userRepository = userRepository;
            _dataStore = dataStore;
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            return Ok((await _dataStore.GetAllUsersAsync()).ToList());
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserAsync([FromRoute] int userId)
        {
            var user = await _userRepository.GetUserAsync(userId);
            if (user != null)
                return Ok(user);

            return NotFound();
        }

        [HttpPost("user")]
        [ProducesResponseType(typeof(User), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            var id = await _userRepository.CreateUserAsync(user);
            if (id.HasValue)
                return Ok(user);

            return Problem("Failed to add to DataStore!");
        }

        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] int userId, [FromBody] User user)
        {
            if (userId != user.UserId)
                return BadRequest();

            var success = await _userRepository.UpdateUserAsync(user);
            if (success)
                return Ok();

            return Problem("Failed to update DataStore!");
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int userId)
        {
            var success = await _userRepository.DeleteUserAsync(userId);
            if (success)
                return Ok();

            return Problem("Failed to delete from DataStore!");
        }
    }
}