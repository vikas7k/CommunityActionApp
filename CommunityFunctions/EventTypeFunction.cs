using CommunityFunctions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CommunityFunctions;

public class EventTypeFunction
{
    private readonly ILogger<EventTypeFunction> _logger;
    private readonly AppDbContext _db;

    public EventTypeFunction(AppDbContext db,ILogger<EventTypeFunction> logger)
    {
        _db = db;
        _logger = logger;
    }

    [Function("GetEventTypes")]
    public async Task<HttpResponseData> GetEventTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "event-types")] HttpRequestData req)
    {
            _logger.LogInformation("Fetching all event types...");

            var eventTypes = await _db.EventTypes
                .Select(et => new
                {
                    et.Name,
                    et.Category
                })
                .OrderBy(et => et.Name)
                .ThenBy(et => et.Category)
                .ToListAsync();

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(eventTypes);
            return res;       
    }
}