using System;
using System.Collections.Generic;
using ProyectoSoftware_Ticketing.DTOs.Sector;

namespace ProyectoSoftware_Ticketing.DTOs.Event
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }

        public List<SectorDto> Sectors { get; set; }
    }
}