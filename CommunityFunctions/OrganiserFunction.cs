using CommunityFunctions.Data;
using CommunityFunctions.Helpers;
using CommunityFunctions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CommunityFunctions
{
    public class OrganiserFunction
    {
        private readonly AppDbContext _db;
        private readonly IJwtValidator _jwt;
        public OrganiserFunction(AppDbContext db, IJwtValidator jwt) { _db = db; _jwt = jwt; }

        [Function("GetAttendees")]
        public async Task<HttpResponseData> GetAttendees([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organiser/events/{eventId:guid}/attendees")] HttpRequestData req, Guid eventId)
        {
            // Validate token and role
            var auth = req.Headers.TryGetValues("Authorization", out var vals) ? vals.FirstOrDefault() : null;
            var result = await _jwt.ValidateTokenAsync(auth);
            if (!result.IsValid) { var r = req.CreateResponse(HttpStatusCode.Unauthorized); await r.WriteStringAsync(result.FailureMessage ?? "Unauthorized"); return r; }

            var principal = result.Principal;
            // Check role claim
            if (!principal.IsInRole("organiser"))
            {
                var r = req.CreateResponse(HttpStatusCode.Forbidden);
                await r.WriteStringAsync("Requires organiser role");
                return r;
            }

            // optional extra check: ensure organiserId in token matches an organiser on this event
            var organiserIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "organiserId")?.Value;
            if (!Guid.TryParse(organiserIdClaim, out var organiserId))
            {
                var r = req.CreateResponse(HttpStatusCode.Forbidden);
                await r.WriteStringAsync("organiserId claim missing");
                return r;
            }

            var hasAccess = await _db.EventOrganisers.AnyAsync(eo => eo.EventId == eventId && eo.OrganiserId == organiserId);
            if (!hasAccess)
            {
                var r = req.CreateResponse(HttpStatusCode.Forbidden);
                await r.WriteStringAsync("You are not an organiser for this event");
                return r;
            }

            var bookings = await _db.Bookings.Where(b => b.EventId == eventId).ToListAsync();
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(bookings.Select(b => new {
                b.Id,
                b.Name,
                b.Email,
                b.Option,
                b.Notes,
                b.CreatedAt
            }));
            return res;
        }

        [Function("GetAttendeesCsv")]
        public async Task<HttpResponseData> GetAttendeesCsv([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "organiser/events/{eventId:guid}/attendees/csv")] HttpRequestData req, Guid eventId)
        {
            // repeat validation
            var auth = req.Headers.TryGetValues("Authorization", out var vals) ? vals.FirstOrDefault() : null;
            var result = await _jwt.ValidateTokenAsync(auth);
            if (!result.IsValid) { var r = req.CreateResponse(HttpStatusCode.Unauthorized); await r.WriteStringAsync(result.FailureMessage ?? "Unauthorized"); return r; }
            var principal = result.Principal;
            if (!principal.IsInRole("organiser")) { var r = req.CreateResponse(HttpStatusCode.Forbidden); await r.WriteStringAsync("Requires organiser role"); return r; }
            var organiserIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "organiserId")?.Value;
            if (!Guid.TryParse(organiserIdClaim, out var organiserId)) { var r = req.CreateResponse(HttpStatusCode.Forbidden); await r.WriteStringAsync("organiserId claim missing"); return r; }
            var hasAccess = await _db.EventOrganisers.AnyAsync(eo => eo.EventId == eventId && eo.OrganiserId == organiserId);
            if (!hasAccess) { var r = req.CreateResponse(HttpStatusCode.Forbidden); await r.WriteStringAsync("You are not an organiser for this event"); return r; }

            var bookings = await _db.Bookings.Where(b => b.EventId == eventId).ToListAsync();
            var csv = CsvHelper.BookingsToCsv(bookings);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/csv; charset=utf-8");
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"event-{eventId}-attendees.csv\"");
            await response.WriteStringAsync(csv);
            return response;
        }
    }
}
