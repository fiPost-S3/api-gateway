using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace api_gateway.Controllers
{
    [ApiController]
    public class AuthenticationController
    {
        [HttpGet]
        [Route("/[controller]/auth")]
        [Authorize]
        public string authorize()
        {
            return "You are Authorized!";
        }
    }
}
