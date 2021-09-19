using Persistence.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IUsersRepository
    {
        Task<UserReadModel> GetAsync(string username);
        Task<UserReadModel> GetAsync(string username, string password);
        Task<int> SaveAsync(UserReadModel model);
    }
}
