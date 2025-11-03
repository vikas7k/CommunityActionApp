using CommunityFunctions.Data;
using CommunityFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CommunityFunctions;

public class GenerateCustomerToken
{
    private readonly AppDbContext _dbContext;
    private readonly JwtService _jwtService;

    public GenerateCustomerToken(AppDbContext dbContext, JwtService jwtService)
    {        
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    [Function("GenerateCustomerToken")]
    public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/customer-token")] HttpRequest req)
    {
            // Read body
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic? data = JsonConvert.DeserializeObject(body);

            string? name = data?.name;
            string? email = data?.email;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
            {
                return new BadRequestObjectResult(new { error = "Name and Email are required." });
            }

            // Check if the customer exists in the database
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Name == name);

            if (customer == null)
            {               
                return new UnauthorizedObjectResult(new { error = "Customer not found or credentials invalid." });
            }

            // Generate token only for existing customers
            var token = _jwtService.GenerateCustomerToken(customer.Name, customer.Email);            

            return new OkObjectResult(new
            {
                message = "Login successful",
                token,
                customer = new { customer.Name, customer.Email }
            });
    }
}