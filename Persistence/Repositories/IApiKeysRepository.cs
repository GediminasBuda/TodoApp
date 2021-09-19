using Persistence.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IApiKeysRepository
    {
        Task<IEnumerable<ApiKeyReadModel>> GetAsync(Guid userId);
        Task<ApiKeyReadModel> GetAsync(string apiKey);
        Task<int> SaveAsync(ApiKeyReadModel model);
        Task<int> UpdateIsActive(Guid id, bool isActive);
    }
}
