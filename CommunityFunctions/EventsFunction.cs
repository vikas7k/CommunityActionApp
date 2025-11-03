using CommunityFunctions.Data;
using CommunityFunctions.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CommunityFunctions
{
    public class EventsFunction
    {
        private readonly AppDbContext _db;
        public EventsFunction(AppDbContext db) { _db = db; }

        [Function("GetEvents")]
        public async Task<HttpResponseData> GetEvents([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events")] HttpRequestData req)
        {
            var q = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var search = q["search"];
            var category = q["category"];
            var fromDate = q["fromDate"];
            var toDate = q["toDate"];

            var events = _db.Events.AsQueryable();
            var today = DateTimeOffset.Now; 
            events = events.Where(e => e.Start >= today);

            if (!string.IsNullOrEmpty(search))
                events = events.Where(e => e.Title.Contains(search));

            if (DateTimeOffset.TryParse(fromDate, out var fd))
                events = events.Where(e => e.Start >= fd);
            if (DateTimeOffset.TryParse(toDate, out var td))
                events = events.Where(e => e.Start <= td);

            var list = await events
                .OrderBy(e => e.Start)
                .Select(e => new EventListItemDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    Start = e.Start,
                    Category = e.Category.ToString(),
                    Description = e.Description,
                    MoreInfoUrl = e.MoreInfoUrl,
                    BookingEnabled = e.BookingEnabled
                })
                .ToListAsync();

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(list);
            return res;
        }

        [Function("GetEventById")]
        public async Task<HttpResponseData> GetEventById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/{id:guid}")] HttpRequestData req, Guid id)
        {
            var e = await _db.Events.Include(ev => ev.EventOrganisers).ThenInclude(x => x.Organiser)
                .FirstOrDefaultAsync(ev => ev.Id == id);

            if (e == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Event not found");
                return notFound;
            }

            var dto = new
            {
                e.Id,
                e.Title,
                e.ImageUrl,
                e.Start,
                Category = e.Category.ToString(),
                e.Description,
                e.MoreInfoUrl,
                e.BookingEnabled,
                e.Capacity,
                e.FunRunDistanceKm,
                Organisers = e.EventOrganisers.Select(o => new { o.OrganiserId, o.Organiser.Name })
            };

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(dto);
            return res;
        }
    }
}
