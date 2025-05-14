using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koridor
{
    [Serializable]
    public class GameStats
    {
        public DateTime GameDate { get; set; }
        public string Winner { get; set; } // "Red" или "Blue"
        public int TotalMoves { get; set; }
        public TimeSpan Duration { get; set; }
        public int RedWallsUsed { get; set; }
        public int BlueWallsUsed { get; set; }
    }
}
