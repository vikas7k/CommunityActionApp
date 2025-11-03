using CommunityFunctions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CommunityFunctions;

public class CreateCustomerFunction
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateCustomerFunction> _logger;

    public CreateCustomerFunction(AppDbContext dbContext, ILogger<CreateCustomerFunction> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Function("CreateCustomer")]
    public async Task<HttpResponseData> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            // Read request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonSerializer.Deserialize<Customer>(requestBody);

            if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Name))
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid input: Email and Name are required.");
                return response;
            }

            // Check if customer already exists
            var existing = await _dbContext.Customers.FindAsync(customer.Email);
            if (existing != null)
            {
                response.StatusCode = System.Net.HttpStatusCode.Conflict;
                await response.WriteStringAsync("Customer with this email already exists.");
                return response;
            }

            // Add customer
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            response.StatusCode = System.Net.HttpStatusCode.Created;
            await response.WriteAsJsonAsync(customer);
            return response;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed");
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Failed to create customer.");
            return response;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("An unexpected error occurred.");
            return response;
        }
    }
}