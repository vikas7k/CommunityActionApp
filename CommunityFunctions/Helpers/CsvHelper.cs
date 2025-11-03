using CommunityFunctions.Models;
using System.Text;

namespace CommunityFunctions.Helpers
{
    public static class CsvHelper
    {
        public static string BookingsToCsv(IEnumerable<Booking> bookings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BookingId,EventId,Name,Email,Option,Notes,CreatedAt");
            foreach (var b in bookings)
            {
                // Basic escaping for commas/quotes
                string esc(string s) => s == null ? "" : $"\"{s.Replace("\"", "\"\"")}\"";
                sb.AppendLine($"{b.Id},{b.EventId},{esc(b.Name)},{esc(b.Email)},{esc(b.Option)},{esc(b.Notes)},{b.CreatedAt:O}");
            }
            return sb.ToString();
        }
    }
}
