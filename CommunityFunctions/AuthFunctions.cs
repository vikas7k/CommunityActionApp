using CommunityFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CommunityFunctions
{
    public class AuthFunctions
    {
        private readonly JwtService _jwtService;

        public AuthFunctions(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
       
        [Function("GenerateOrganiserToken")]
        public async Task<IActionResult> GenerateOrganiserToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/organiser-token")] HttpRequest req,
            ILogger log)
        {
            
             string body = await new StreamReader(req.Body).ReadToEndAsync();
             dynamic data = JsonConvert.DeserializeObject(body);
             string name = data?.name;
             Guid organiserId = data?.organiserId ?? Guid.NewGuid();

             var token = _jwtService.GenerateOrganiserToken(organiserId, name ?? "Jane Organiser");
             return new OkObjectResult(new { token });
         
        }

        [Function("GenerateStaffToken")]
        public async Task<IActionResult> GenerateStaffToken(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/staff-token")] HttpRequest req,
           ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(body);
            string name = data?.name ?? "Community Staff";

            var token = _jwtService.GenerateStaffToken(name);
            return new OkObjectResult(new { token });
        }

    }
}
