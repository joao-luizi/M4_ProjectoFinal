using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Horses.DTOs
{
    public class HorseOverviewDto
    {
        public int TotalHorses { get; set; }
        public int SchoolHorses { get; set; }
        public int PrivateHorses { get; set; }

        public List<HorseListItemDto> Horses { get; set; } = new();
    }
}
