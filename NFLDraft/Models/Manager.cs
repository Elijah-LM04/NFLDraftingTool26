using System;
using System.Collections.Generic;
using System.Text;

namespace NFLDraft.Models
{
    public class Manager
    {
        public string Name { get; set; }
        public List<Player> QB { get; set; } = new List<Player>();
        public List<Player> RB { get; set; } = new List<Player>();
        public List<Player> WR { get; set; } = new List<Player>();
        public List<Player> TE { get; set; } = new List<Player>();
        public List<Player> FLEX { get; set; } = new List<Player>();
    }
}
