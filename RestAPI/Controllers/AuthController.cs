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
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("signUp")]
        public async Task<ActionResult<SignUpResponse>> SignUp(SignUpRequest request)
        {
            var user = await _userRepository.GetAsync(request.Username);
            if (user is not null)
            {
                return Conflict($"User with Username: '{request.Username}' already exists!");
            }
            var userReadModel = new UserReadModel
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password = request.Password,
                DateCreated = DateTime.Now
            };
            await _userRepository.SaveAsync(userReadModel);

            return new SignUpResponse
            {
                Id = userReadModel.Id,
                Username = userReadModel.Username
            };
        }


    }
}
