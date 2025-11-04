using CommunityFunctions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace CommunityFunctions;

public class CustomerBookingsFunction
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CustomerBookingsFunction> _logger;

    public CustomerBookingsFunction(AppDbContext dbContext, ILogger<CustomerBookingsFunction> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Function("GetCustomerBookings")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bookings/customer")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            // Extract the token from the Authorization header
            if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                await response.WriteStringAsync("Missing Authorization header");
                return response;
            }

            var token = authHeaders.First().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assuming the email is stored in the 'sub' or 'email' claim
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(emailClaim))
            {
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                await response.WriteStringAsync("Invalid token: email claim missing");
                return response;
            }

            // Query bookings for this customer, including event details
            var now = DateTimeOffset.Now;
            var bookings = await (from b in _dbContext.Bookings
                                           join e in _dbContext.Events
                                           on b.EventId equals e.Id
                                           where b.Email == emailClaim && e.Start > now
                                           orderby e.Start
                                           select new
                                           {
                                               b.Id,
                                               b.Name,
                                               b.Email,
                                               b.Option,
                                               b.Notes,
                                               b.CreatedAt,
                                               e.Title,
                                               e.Start,
                                               e.Category,
                                               e.ImageUrl,
                                               e.Description,
                                               e.MoreInfoUrl,
                                               e.BookingEnabled,
                                               e.Capacity,
                                               e.FunRunDistanceKm
                                           }).ToListAsync();          

            response.StatusCode = System.Net.HttpStatusCode.OK;
            await response.WriteAsJsonAsync(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer bookings");
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Error retrieving bookings");
        }

        return response;
    }
}