using RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Services
{
    interface IApiKeyService
    {
        Task<ApiKey> CreateApiKey(string username, string password);

        Task<IEnumerable<ApiKey>> GetAllApiKeys(string username, string password);

        Task<ApiKey> UpdateApiKeyState(Guid id, bool state);
    }
}
