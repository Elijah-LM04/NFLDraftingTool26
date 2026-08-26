using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace NFLDraft.Models
{ 
    public class Player
    {
        
        [Ignore]
        public double QualityScore { get; set; }
        [Ignore]
        public double ScarcityScore { get; set; }
        [Ignore]
        public double ValueSpreadScore { get; set; }
        [Ignore]
        public double PoolScore { get; set; }


        [Ignore]
        public int Rank { get; set; }
        [Ignore]
        public double ProjectedPointsTotal { get; set; }
        [Ignore]
        public double ProjectedPointsPerGame { get; set; }
        [Ignore]

        public double GameSenseScore { get; set; }


        [Name("id")]
        public string Id { get; set; } = "";

        [Name("name")]
        public string Name { get; set; } = "";

        [Name("pos")]
        public string Pos { get; set; } = "";

        [Name("team")]
        public string Team { get; set; } = "";

        [Name("set-id")]
        public int SetId { get; set; }

        [Name("set-userid")]
        public int SetUserid { get; set; }

        [Name("set-name")]
        public string SetName { get; set; } = "";

        [Name("ssn-gms")]
        public double SsnGms { get; set; }

        [Name("ssn-ssn")]
        public double SsnSsn { get; set; }

        [Name("pass-2pt")]
        public double Pass2Pt { get; set; }

        [Name("pass-att")]
        public double PassAtt { get; set; }

        [Name("pass-cmp")]
        public double PassCmp { get; set; }

        [Name("pass-1d")]
        public double Pass1D { get; set; }

        [Name("pass-int")]
        public double PassInt { get; set; }

        [Name("pass-sck")]
        public double PassSck { get; set; }

        [Name("pass-td")]
        public double PassTd { get; set; }

        [Name("pass-yds")]
        public double PassYds { get; set; }

        [Name("rush-2pt")]
        public double Rush2Pt { get; set; }

        [Name("rush-car")]
        public double RushCar { get; set; }

        [Name("rush-1d")]
        public double Rush1D { get; set; }

        [Name("rush-td")]
        public double RushTd { get; set; }

        [Name("rush-yds")]
        public double RushYds { get; set; }

        [Name("rec-2pt")]
        public double Rec2Pt { get; set; }

        [Name("rec-1d")]
        public double Rec1D { get; set; }

        [Name("rec-rec")]
        public double RecRec { get; set; }

        [Name("rec-tgt")]
        public double RecTgt { get; set; }

        [Name("rec-td")]
        public double RecTd { get; set; }

        [Name("rec-yds")]
        public double RecYds { get; set; }

        [Name("fum-lost")]
        public double FumLost { get; set; }

        [Name("kck-xpa")]
        public double KckXpa { get; set; }

        [Name("kck-xpc")]
        public double KckXpc { get; set; }

        [Name("kck-xpm")]
        public double KckXpm { get; set; }

        [Name("kck-fga")]
        public double KckFga { get; set; }

        [Name("kck-fgc")]
        public double KckFgc { get; set; }

        [Name("kck-fgm")]
        public double KckFgm { get; set; }

        [Name("idp-2pr")]
        public double Idp2Pr { get; set; }

        [Name("idp-ast")]
        public double IdpAst { get; set; }

        [Name("idp-blk")]
        public double IdpBlk { get; set; }

        [Name("idp-fmr")]
        public double IdpFmr { get; set; }

        [Name("idp-fmf")]
        public double IdpFmf { get; set; }

        [Name("idp-int")]
        public double IdpInt { get; set; }

        [Name("idp-pd")]
        public double IdpPd { get; set; }

        [Name("idp-sck")]
        public double IdpSck { get; set; }

        [Name("idp-saf")]
        public double IdpSaf { get; set; }

        [Name("idp-tac")]
        public double IdpTac { get; set; }

        [Name("idp-tfl")]
        public double IdpTfl { get; set; }

        [Name("idp-td")]
        public double IdpTd { get; set; }

        [Name("tmd-2pr")]
        public double Tmd2Pr { get; set; }

        [Name("tmd-blk")]
        public double TmdBlk { get; set; }

        [Name("tmd-fmf")]
        public double TmdFmf { get; set; }

        [Name("tmd-fmr")]
        public double TmdFmr { get; set; }

        [Name("tmd-int")]
        public double TmdInt { get; set; }

        [Name("tmd-pa")]
        public double TmdPa { get; set; }

        [Name("tmd-sck")]
        public double TmdSck { get; set; }

        [Name("tmd-saf")]
        public double TmdSaf { get; set; }

        [Name("tmd-td")]
        public double TmdTd { get; set; }

        [Name("tmd-ya")]
        public double TmdYa { get; set; }

        [Name("pr-td")]
        public double PrTd { get; set; }

        [Name("pr-yds")]
        public double PrYds { get; set; }

        [Name("kr-td")]
        public double KrTd { get; set; }

        [Name("kr-yds")]
        public double KrYds { get; set; }
    }
}
