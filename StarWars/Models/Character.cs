using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StarWars.Models
{
    public class Character
    {
        public string Name { get; set; }
        public double Height { get; set; }
        public double Mass { get; set; }
        public string Skin_Color { get; set; }
        public string Birth_Year { get; set; }
        public string Gender { get; set; }
        [JsonPropertyName("vehicles")] public List<string> VehiclesDataUrl { get; set; }
        [JsonPropertyName("starships")] public List<string> StarshipsDataUrl { get; set; }
        public List<Vehicle> VehiclesData { get; set; } = [];
        public List<Starship> StarshipsData { get; set; } = [];
    }
}