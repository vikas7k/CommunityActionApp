using CommunityFunctions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityFunctions.Models
{
    public enum EventCategory { FunRun, BakeOff }

    public class Event
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset Start { get; set; }   // date & time event starts (timezone stored as offset)
        public string Category { get; set; }
        public string Description { get; set; }
        public string MoreInfoUrl { get; set; }
        public bool BookingEnabled { get; set; } = true;
        public int Capacity { get; set; } = 0; // 0 = unlimited
      

        // For fun run: distance (5 or 10) stored as int kms
        public int? FunRunDistanceKm { get; set; }

        // navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // link to organisers (many-to-many)
        public ICollection<EventOrganiser> EventOrganisers { get; set; } = new List<EventOrganiser>();

    }
}

   
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; }
        [Required, MaxLength(200)] public string Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // For fun run: Walk or Run; for bake-off: Category choice
        public string Option { get; set; } // e.g. "Walk", "Run", "Cakes", "Pastries", "Desserts"

        // optionally store organiser-visible notes etc
        public string Notes { get; set; }

}

    public class Organiser
    {
        public Guid Id { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; }
        [MaxLength(200)] public string Email { get; set; }
        public ICollection<EventOrganiser> EventOrganisers { get; set; } = new List<EventOrganiser>();
    }

    public class EventOrganiser
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public Guid OrganiserId { get; set; }
        public Organiser Organiser { get; set; }
    }

public class Customer
{
    public string Email { get; set; }  // Primary key
    public string Name { get; set; }
}

public class EventType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; } 
}