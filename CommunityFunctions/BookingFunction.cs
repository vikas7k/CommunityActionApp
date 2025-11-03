using CommunityFunctions.Data;
using CommunityFunctions.DTOs;
using CommunityFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CommunityFunctions
{
    public class BookingFunction
    {
        private readonly AppDbContext _db;
        public BookingFunction(AppDbContext db) { _db = db; }

        [Function("BookEvent")]
        public async Task<HttpResponseData> BookEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events/{id:guid}/book")] HttpRequestData req, Guid id)
        {
            var dto = await System.Text.Json.JsonSerializer.DeserializeAsync<BookingRequestDto>(req.Body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null || string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Email))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Invalid booking payload");
                return bad;
            }

            var ev = await _db.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null)
            {
                var nf = req.CreateResponse(HttpStatusCode.NotFound);
                await nf.WriteStringAsync("Event not found");
                return nf;
            }

            if (!ev.BookingEnabled)
            {
                var disabled = req.CreateResponse(HttpStatusCode.BadRequest);
                await disabled.WriteStringAsync("Booking is disabled for this event");
                return disabled;
            }

            if (ev.Capacity > 0 && ev.Bookings.Count >= ev.Capacity)
            {
                var full = req.CreateResponse((HttpStatusCode)429);
                await full.WriteStringAsync("Event is full");
                return full;
            }

            // Validate options depending on category          

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                EventId = ev.Id,
                Name = dto.Name,
                Email = dto.Email,
                Option = dto.EntryType,
                Notes = dto.Notes,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            var res = req.CreateResponse(HttpStatusCode.Created);
            await res.WriteAsJsonAsync(new { booking.Id });
            return res;
        }
    }
}
