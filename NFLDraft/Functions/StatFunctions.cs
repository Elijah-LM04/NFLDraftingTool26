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

        public Player GameSense(Player player, List<Player> playerList, List<Player> playerListFull)
        {
            //sort playerList
            playerList = playerList.OrderByDescending(p => p.ProjectedPointsPerGame).ToList();

            //get count
            int fullListCount = playerListFull.Count;

            List<double> ppg = GetPPG(playerList);
            List<double> ppgFull = GetPPG(playerListFull);

            double quality = 0;
            double tierGap = 0;
            double pool = 0;
            double valueSpread = 0;

            //QUALITY
            quality = 100 * ((player.ProjectedPointsPerGame - ppgFull.Min()) / (ppgFull.Max() - ppgFull.Min()));

            //tierGap
            List<double> bucket = ppg.Where(p => p < player.ProjectedPointsPerGame).OrderByDescending(p => p).Take(13).ToList();

            double difference = bucket.Count > 0 ? bucket.Average() : ppg.Min();

            double rawtierGap = player.ProjectedPointsPerGame - difference;

            double maxtierGap = ppg.Max() - ppg.Min();

            tierGap = maxtierGap > 0 ? 100.0 * (rawtierGap / maxtierGap) : 0;

            //POSITION POOL
            pool = fullListCount > 0 ? 100.0 * (1.0 - ((double)playerList.Count / fullListCount)) : 0;

            //POSITION VALUE SPREAD
            bucket = ppg.Where(p => p < player.ProjectedPointsPerGame).OrderByDescending(p => p).Take(13).ToList();

            if (bucket.Count < 1)
            {
                bucket.Add(ppg.Min());
            }
            

            valueSpread = ppg.Max() > 0 ? 100.0 * ((ppg.Max() - bucket.Min()) / ppg.Max()) : 0;

            //storing info
            player.QualityScore = quality;
            player.TierGapScore = tierGap;
            player.ValueSpreadScore = valueSpread;
            player.PoolScore = pool;


            player.GameSenseScore = quality * 0.60 + tierGap * 0.15 + valueSpread * 0.10 + pool * 0.15;

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
