using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.RequestModels;
using Contracts.Models.ResponseModels;
using Persistence.Repositories;
using Persistence.Models.ReadModels;
using RestAPI.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("apiKeys")]
    public class ApiKeysController : ControllerBase // I am connected to AApiKeyRepository, not ApiKeyServices!!
    {
        private readonly IUserRepository _usersRepository;
        private readonly IApiKeysRepository _apiKeysRepository;
        private readonly ApiKeySettings _apiKeySettings; // this is a place where everything is different from the tutor
        public ApiKeysController(IUserRepository usersRepository, 
                IApiKeysRepository apiKeysRepository,
                IOptions<ApiKeySettings> apiKeySettings) // here as well
        {
            _usersRepository = usersRepository;
            _apiKeysRepository = apiKeysRepository;
            _apiKeySettings = apiKeySettings.Value; //here as well
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
            var allKeys = await _apiKeysRepository.GetByUserIdAsync(user.Id);

            if (_apiKeySettings.MaxNumberOfKeys < allKeys.Count() + 1)
            {
                throw new BadHttpRequestException($"Api key limit is reached", 200);
            }
            var apiKey = new ApiKeyReadModel
            {
                Id = Guid.NewGuid(),
                ApiKey = Guid.NewGuid().ToString("N"),
                UserId = user.Id,
                IsActive = true,
                DateCreated = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(_apiKeySettings.ExpirationTimeInMinutes)
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
/*public class ApiKeysController : ControllerBase // The case with ApiKeyService implementation
{
    private readonly IApikeyService _apikeyService;

    public ApiKeysController(IApikeyService apikeyService)
    {
        _apikeyService = apikeyService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiKeyResponse>> Create(ApiKeyRequest request)
    {
        try
        {
            var apiKey = await _apikeyService.CreateApiKey(request.Username, request.Password);

            return new ApiKeyResponse
            {
                Id = apiKey.Id,
                ApiKey = apiKey.Key,
                UserId = apiKey.UserId,
                IsActive = apiKey.IsActive,
                DateCreated = apiKey.DateCreated,
                ExpirationDate = apiKey.ExpirationDate
            };
        }
        catch (BadHttpRequestException exception)
        {
            switch (exception.StatusCode)
            {
                case 404:
                    return NotFound(exception.Message);
                case 400:
                    return BadRequest(exception.Message);
                default: throw;
            }
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApiKeyResponse>>> GetAllKeys(string username, string password)
    {
        var apiKeys = await _apikeyService.GetAllApiKeys(username, password);

        return Ok(apiKeys.Select(apiKey => new ApiKeyResponse
        {
            Id = apiKey.Id,
            ApiKey = apiKey.Key,
            UserId = apiKey.UserId,
            IsActive = apiKey.IsActive,
            DateCreated = apiKey.DateCreated,
            ExpirationDate = apiKey.ExpirationDate
        }));
    }
*/
        //
        // [HttpPut]
        // [Route("{id}/isActive")]
        // public async Task<ActionResult<ApiKeyResponse>> UpdateKeyState(Guid id, UpdateKeyStateRequest request)
        // {
        //     var apiKey = await _apiKeysRepository.GetByApiKeyIdAsync(id);
        //
        //     if (apiKey is null)
        //     {
        //         return NotFound($"Api key with Id: '{id}' does not exists");
        //     }
        //
        //     await _apiKeysRepository.UpdateIsActive(id, request.IsActive);
        //
        //     return new ApiKeyResponse
        //     {
        //         Id = apiKey.Id,
        //         ApiKey = apiKey.ApiKey,
        //         UserId = apiKey.UserId,
        //         IsActive = request.IsActive,
        //         DateCreated = apiKey.DateCreated,
        //         ExpirationDate = apiKey.ExpirationDate
        //     };
        // }
