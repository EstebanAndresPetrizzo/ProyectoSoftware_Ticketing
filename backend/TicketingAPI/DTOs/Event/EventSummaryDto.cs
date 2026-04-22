using System;

namespace ProyectoSoftware_Ticketing.DTOs.Event
{
    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
    }
}