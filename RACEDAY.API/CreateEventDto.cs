csharp Dtos/CreateEventDto.cs
using System;

namespace RaceDay.API.Dtos
{
    public class CreateEventDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}