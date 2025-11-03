using System.ComponentModel.DataAnnotations;

namespace CommunityFunctions.DTOs
{
    public class EventListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset Start { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string MoreInfoUrl { get; set; }
        public bool BookingEnabled { get; set; }
    }

    public class CreateEventDto
    {
        [Required] public string Title { get; set; }
        public string ImageUrl { get; set; }
        [Required] public DateTimeOffset Start { get; set; }
        [Required] public string Category { get; set; } // "Fun-run" or "Bake-off"
        public string Description { get; set; }
        public string MoreInfoUrl { get; set; }
        public int Capacity { get; set; } = 0;
        public int? FunRunDistanceKm { get; set; } // 5 or 10 for fun-run
        public Guid OrganiserId { get; set; }
    }

    public class BookingRequestDto
    {
        [Required] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public string EntryType { get; set; } // Walk/Run or Cakes/Pastries/Desserts
        public string Notes { get; set; }
    }
}
