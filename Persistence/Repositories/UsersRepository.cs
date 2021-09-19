using Persistence.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
   public class UsersRepository : IUsersRepository
    {
        private readonly ISqlClient _sqlClient;
        private const string TableName = "users";
        public Task<UserReadModel> GetAsync(string username, string password)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Username = @Username AND Password = @Password";
            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                Username = username,
                Password = password
            });
        }

        public Task<UserReadModel> GetAsync(string username)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Username = @Username";
            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                Username = username
            });
                
        }

        public Task<int> SaveAsync(UserReadModel model)
        {
            var sql = $"INSERT INTO {TableName} (Id, Username, Password, DateCreated) VALUES (@Id, @Username, @Password, @DateCreated)";
            return _sqlClient.ExecuteAsync(sql, model);
        }
    }
}
