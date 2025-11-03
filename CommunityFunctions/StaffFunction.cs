using CommunityFunctions.Data;
using CommunityFunctions.DTOs;
using CommunityFunctions.Models;
using CommunityFunctions.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace CommunityFunctions
{
    public class StaffFunction
    {
        private readonly AppDbContext _db;
        private readonly IJwtValidator _jwt;
        public StaffFunction(AppDbContext db, IJwtValidator jwt) { _db = db; _jwt = jwt; }

        private async Task<(bool ok, HttpResponseData response)> EnsureStaff(HttpRequestData req)
        {
            var auth = req.Headers.TryGetValues("Authorization", out var vals) ? vals.FirstOrDefault() : null;
            var result = await _jwt.ValidateTokenAsync(auth);
            if (!result.IsValid) return (false, req.CreateResponse(HttpStatusCode.Unauthorized));
            var principal = result.Principal;
            if (!principal.IsInRole("staff")) return (false, req.CreateResponse(HttpStatusCode.Forbidden));
            return (true, null);
        }

        [Function("CreateEvent")]
        public async Task<HttpResponseData> CreateEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "staff/events")] HttpRequestData req)
        {
            var check = await EnsureStaff(req);
            if (!check.ok) return check.response;

            var dto = await System.Text.Json.JsonSerializer.DeserializeAsync<CreateEventDto>(req.Body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (dto == null) { var br = req.CreateResponse(HttpStatusCode.BadRequest); await br.WriteStringAsync("Invalid payload"); return br; }

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ImageUrl = dto.ImageUrl,
                Start = dto.Start,
                Category = dto.Category,
                Description = dto.Description,
                MoreInfoUrl = dto.MoreInfoUrl,
                Capacity = dto.Capacity,
                FunRunDistanceKm = dto.FunRunDistanceKm
            };

            _db.Events.Add(ev);

            if (dto.OrganiserId != Guid.Empty)
            {
                var link = new EventOrganiser
                {                  
                    EventId = ev.Id,
                    OrganiserId = dto.OrganiserId
                };
                _db.EventOrganisers.Add(link);
            }

            await _db.SaveChangesAsync();

            var res = req.CreateResponse(HttpStatusCode.Created);
            await res.WriteAsJsonAsync(new { ev.Id });
            return res;
        }

        [Function("UpdateEvent")]
        public async Task<HttpResponseData> UpdateEvent([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "staff/events/{id:guid}")] HttpRequestData req, Guid id)
        {
            var check = await EnsureStaff(req);
            if (!check.ok) return check.response;

            var dto = await System.Text.Json.JsonSerializer.DeserializeAsync<CreateEventDto>(req.Body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (dto == null) { var br = req.CreateResponse(HttpStatusCode.BadRequest); await br.WriteStringAsync("Invalid payload"); return br; }

            var ev = await _db.Events.FindAsync(id);
            if (ev == null) { var nf = req.CreateResponse(HttpStatusCode.NotFound); await nf.WriteStringAsync("Event not found"); return nf; }

            ev.Title = dto.Title;
            ev.ImageUrl = dto.ImageUrl;
            ev.Start = dto.Start;
            ev.Category = dto.Category;
            ev.Description = dto.Description;
            ev.MoreInfoUrl = dto.MoreInfoUrl;
            ev.Capacity = dto.Capacity;
            ev.FunRunDistanceKm = dto.FunRunDistanceKm;

            _db.Events.Update(ev);
            await _db.SaveChangesAsync();

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { ev.Id });
            return res;
        }

        [Function("DeleteEvent")]
        public async Task<HttpResponseData> DeleteEvent([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "staff/events/{id:guid}")] HttpRequestData req, Guid id)
        {
            var check = await EnsureStaff(req);
            if (!check.ok) return check.response;

            var ev = await _db.Events.FindAsync(id);
            if (ev == null) { var nf = req.CreateResponse(HttpStatusCode.NotFound); await nf.WriteStringAsync("Event not found"); return nf; }

            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();

            var res = req.CreateResponse(HttpStatusCode.NoContent);
            return res;
        }

        [Function("ToggleBooking")]
        public async Task<HttpResponseData> ToggleBooking([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "staff/events/{id:guid}/{action}")] HttpRequestData req, Guid id, string action)
        {
            var check = await EnsureStaff(req);
            if (!check.ok) return check.response;

            var ev = await _db.Events.FindAsync(id);
            if (ev == null) { var nf = req.CreateResponse(HttpStatusCode.NotFound); await nf.WriteStringAsync("Event not found"); return nf; }

            if (action.Equals("enable-booking", StringComparison.OrdinalIgnoreCase)) ev.BookingEnabled = true;
            else if (action.Equals("disable-booking", StringComparison.OrdinalIgnoreCase)) ev.BookingEnabled = false;
            else { var bad = req.CreateResponse(HttpStatusCode.BadRequest); await bad.WriteStringAsync("Unknown action"); return bad; }

            await _db.SaveChangesAsync();
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { ev.Id, ev.BookingEnabled });
            return res;
        }
    }
}
