using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Models
{
    public class Starship
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public double MaxAtmospheringSpeed { get; set; }
        public string StarshipClass { get; set; }
    }
}