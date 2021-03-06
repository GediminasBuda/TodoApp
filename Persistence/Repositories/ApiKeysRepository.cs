using Persistence.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ApiKeysRepository : IApiKeysRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TableName = "apikeys";

        public ApiKeysRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public Task<IEnumerable<ApiKeyReadModel>> GetByUserIdAsync(Guid userId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserId = @UserId";
            return _sqlClient.QueryAsync<ApiKeyReadModel>(sql, new
            {
                UserId = userId
            });

        }

        public Task<ApiKeyReadModel> GetByApiKeyAsync(string apiKey)
        {
            var sql = $"SELECT * FROM {TableName} WHERE ApiKey = @ApiKey";
            return _sqlClient.QuerySingleOrDefaultAsync<ApiKeyReadModel>(sql, new
            {
                ApiKey = apiKey
            });
        }

        public Task<int> SaveAsync(ApiKeyReadModel model)
        {
            var sql = $"INSERT INTO {TableName} (Id, ApiKey, UserId, IsActive, DateCreated, ExpirationDate) VALUES (@Id, @ApiKey, @UserId, @IsActive, @DateCreated, @ExpirationDate)";
            return _sqlClient.ExecuteAsync(sql, model);
        }

        public Task<int> UpdateIsActive(Guid id, bool isActive)
        {
            var sql = $"UPDATE {TableName} SET IsActive = @IsActive WHERE Id = @Id";
            return _sqlClient.ExecuteAsync(sql, new 
            { 
                Id =id,
                IsActive = isActive
            });
        }

        public Task<ApiKeyReadModel> GetByApiKeyIdAsync(Guid id)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Id = @Id";

            return _sqlClient.QuerySingleOrDefaultAsync<ApiKeyReadModel>(sql, new
            {
                Id = id
            });
        }
    }
}
