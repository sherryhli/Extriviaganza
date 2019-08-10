using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Extensions;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.Controllers
{
    [Route("/api/[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IMapper _mapper;

        public AuthenticateController(IAuthenticateService authenticateService, IMapper mapper)
        {
            _authenticateService = authenticateService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetAuthToken([FromBody] AuthenticateUserResource userResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            User user = _mapper.Map<AuthenticateUserResource, User>(userResource);

            string token;
            if (_authenticateService.IsAuthenticated(user, out token))
            {
                return Ok(token); 
            }

            return BadRequest("Invalid authentication request.");
        }
    }
}
