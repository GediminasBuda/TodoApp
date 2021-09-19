using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models.RequestModels
{
    public class ApiKeyRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
