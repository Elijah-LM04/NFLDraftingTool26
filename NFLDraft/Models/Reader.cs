using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;


namespace NFLDraft.Models
{
    public class Reader
    {
        private readonly string filepath = "../../../Data/projection-set-preseason-all-2026.csv";

        public List<Player> ReadPlayers()
        {
            using var reader = new StreamReader(filepath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<Player>().ToList();
        }
    }
}
