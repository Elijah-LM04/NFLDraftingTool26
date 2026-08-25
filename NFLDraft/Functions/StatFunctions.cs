using NFLDraft.Models;
using System;
using System.Collections.Generic;
using System.Text;
using NFLDraft.Functions;

namespace NFLDraft.Functions
{
    public class StatFunctions()
    {
        public List<Player> updateKeyStats(List<Player> pl)
        { 
            //setting projected points
            foreach (Player p in pl)
            {
                p.ProjectedPointsTotal = GetProjectedPoints(p);
                p.ProjectedPointsPerGame = GetProjectedPointsPerGame(p);
            }
            return pl;
        }

        public double GetProjectedPoints(Player p)
        {
            return
                (p.PassYds * 0.04)
                + (p.PassTd * 4)
                - (p.PassInt * 2)
                + (p.RushYds * 0.1)
                + (p.RushTd * 6)
                + p.RecRec
                + (p.RecYds * 0.1)
                + (p.RecTd * 6)
                + ((p.Pass2Pt + p.Rush2Pt + p.Rec2Pt) * 2)
                - (p.FumLost * 2);
        }

        public double GetProjectedPointsPerGame(Player p)
        {
            return p.SsnGms > 0 ? GetProjectedPoints(p) / p.SsnGms : 0;
        }

        public Player GameSense(Player player, List<Player> playerList, int fullList)
        {
            //sort playerList
            playerList = playerList.OrderByDescending(p => p.ProjectedPointsPerGame).ToList();


            List<double> ppg = GetPPG(playerList);
            List<double> gaps = GetGaps(ppg);

            double quality = 0;
            double scarcity = 0;
            double pool = 0;
            double valueSpread = 0;

            //QUALITY
            quality = 100 * ((player.ProjectedPointsPerGame - ppg.Min()) / (ppg.Max() - ppg.Min()));

            //SCARCITY
            List<double> bucket = ppg.Where(p => p < player.ProjectedPointsPerGame).OrderByDescending(p => p).Take(3).ToList();

            double difference = bucket.Count > 0 ? bucket.Average() : ppg.Min();
            double rawScarcity = player.ProjectedPointsPerGame - difference;
            double maxScarcity = ppg.Max() - ppg.Min();

            scarcity = maxScarcity > 0 ? 100.0 * (rawScarcity / maxScarcity) : 0;

            //POSITION POOL
            pool = 100 * (1.0 - ((double)playerList.Count() / fullList));

            //POSITION VALUE SPREAD
            bucket = ppg.Where(p => p < player.ProjectedPointsPerGame).OrderByDescending(p => p).Take(3).ToList();
            if (bucket.Count() < 1)
            {
                bucket.Add(ppg.Min());
            }
            valueSpread = 100.0 * ((ppg.Max() - bucket.Min()) / ppg.Max());


            player.GameSenseScore = Math.Round(quality * 0.38 + scarcity * 0.31 + valueSpread * 0.19 + pool * 0.12, 2);

            return player;
        }

        public List<double> GetPPG(List<Player> playerList)
        {
            List<double> ppg = new List<double>();
            foreach (Player p in playerList)
            {
                ppg.Add(p.ProjectedPointsPerGame);
            }

            return ppg;
        }

        public List<double> GetGaps(List<double> ppg)
        {
            List<double> gaps = new List<double>();
            for (int i = 1; i < ppg.Count; i++)
            {
                gaps.Add(ppg[i - 1] - ppg[i]);
            }

            return gaps;
        }
    }
}
