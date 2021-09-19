using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.RequestModels;
using Contracts.Models.ResponseModels;
using Persistence.Repositories;
using Persistence.Models.ReadModels;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("apiKeys")]
    public class ApiKeysController : ControllerBase
    {
        private readonly IUserRepository _usersRepository;
        private readonly IApiKeysRepository _apiKeysRepository;
        public ApiKeysController(IUserRepository usersRepository, IApiKeysRepository apiKeysRepository)
        {
            _usersRepository = usersRepository;
            _apiKeysRepository = apiKeysRepository;
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiKeyResponse>> Create(ApiKeyRequest request)
        {
            var user = await _usersRepository.GetAsync(request.Username);
            if (user is null)
            {
                return Conflict($"User with Username: '{request.Username}' already exists!");
            }
            if (!user.Password.Equals(request.Password))
            {
                return BadRequest($"Wrong password for user: '{user.Username}'");
            }

            var apiKey = new ApiKeyReadModel
            {
                Id = Guid.NewGuid(),
                ApiKey = Guid.NewGuid().ToString("N"),
                UserId = user.Id,
                IsActive = true,
                DateCreated = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(2)
            };
            await _apiKeysRepository.SaveAsync(apiKey);
            return new ApiKeyResponse
            {
                Id = apiKey.Id,
                ApiKey = apiKey.ApiKey,
                UserId = apiKey.UserId,
                IsActive = apiKey.IsActive,
                DateCreated = apiKey.DateCreated,
                ExpirationDate = apiKey.ExpirationDate
            };
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiKeyResponse>>> GetAllKeys(string username, string password)
        {
            var user = await _usersRepository.GetAsync(username);
            if (user is null)
            {
                return Conflict($"User with Username: '{username}' already exists!");
            }
            if (!user.Password.Equals(password))
            {
                return BadRequest($"Wrong password for user: '{user.Username}'");
            }
           var apiKeys = await _apiKeysRepository.GetByUserIdAsync(user.Id);

            return Ok(apiKeys.Select(apiKey => new ApiKeyResponse 
            {
                Id = apiKey.Id,
                ApiKey = apiKey.ApiKey,
                UserId = apiKey.UserId,
                IsActive = apiKey.IsActive,
                DateCreated = apiKey.DateCreated,
                ExpirationDate = apiKey.ExpirationDate

            }));
        }
        [HttpPut]
        [Route("{id}/isActive")]
        public async Task<ActionResult<ApiKeyResponse>> UpdateKeyState(Guid id, UpdateKeyStateRequest request)
        {
            var apiKey = await _apiKeysRepository.GetByApiKeyIdAsync(id);

            if (apiKey is null)
            {
                return NotFound($"Api key with Id: '{id}' does not exists");
            }

            await _apiKeysRepository.UpdateIsActive(id, request.IsActive);

            return new ApiKeyResponse
            {
                Id = apiKey.Id,
                ApiKey = apiKey.ApiKey,
                UserId = apiKey.UserId,
                IsActive = request.IsActive,
                DateCreated = apiKey.DateCreated,
                ExpirationDate = apiKey.ExpirationDate
            };
        }
    }
}
