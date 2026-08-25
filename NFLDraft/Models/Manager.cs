using System;
using System.Collections.Generic;
using System.Text;

namespace NFLDraft.Models
{
    public class Manager
    {
        public string Name { get; set; }
        public int Pos { get; set; }
        public List<Player> QB { get; set; }
        public List<Player> RB { get; set; }
        public List<Player> WR { get; set; }
        public List<Player> TE { get; set; }
        public List<Player> K { get; set; }
    }
}
