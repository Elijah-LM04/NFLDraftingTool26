using NFLDraft.Functions;
using NFLDraft.Models;
using System.Data;

class Program
{
    static void Main(string[] args)
    {
        //load players
        Reader reader = new Reader();
        List<Player> players = reader.ReadPlayers().Where(p => p.SetUserid == -1).ToList(); //CONSENSUS

        StatFunctions updater = new StatFunctions();
        players = updater.updateKeyStats(players);

        //RB
        List<Player> rb = players
            .Where(p => p.Pos.ToLower() == "rb")
            .ToList();

        //WR
        List<Player> wr = players
            .Where(p => p.Pos.ToLower() == "wr")
            .ToList();


        //TE
        List<Player> te = players
            .Where(p => p.Pos.ToLower() == "te")
            .ToList();


        int fullWR = wr.Count();
        int fullRB = rb.Count();
        int fullTE = te.Count();

        //calculate game sense for each, then combine
        List<Player> pNew = new List<Player>();

        foreach (Player p in wr)
        {
            updater.GameSense(p, wr, fullWR);
            pNew.Add(p);
        }

        foreach (Player p in rb)
        {
            updater.GameSense(p, rb, fullRB);
            pNew.Add(p);
        }

        foreach (Player p in te)
        {
            updater.GameSense(p, te, fullTE);
            pNew.Add(p);
        }

        
        
        // Start interface
        RunInterface(pNew);
    }


    static void RunInterface(List<Player> players)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  NFL DRAFT ENGINE                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine($"  Players:  {players.Count}");
            Console.WriteLine();

            Console.WriteLine("  ┌──────────────────────────────────────────────┐");
            Console.WriteLine("  │                  MAIN MENU                   │");
            Console.WriteLine("  ├──────────────────────────────────────────────┤");
            Console.WriteLine("  │  1. View GameSense Rankings                  │");
            Console.WriteLine("  │  2. View Top 10                              │");
            Console.WriteLine("  │  3. Search Player                            │");
            Console.WriteLine("  │  4. View PPG Rankings                        │");
            Console.WriteLine("  │  5. Compare GameSense vs PPG                 │");
            Console.WriteLine("  │                                              │");
            Console.WriteLine("  │  Q. Quit                                     │");
            Console.WriteLine("  └──────────────────────────────────────────────┘");
            Console.WriteLine();

            Console.Write("  Select an option: ");
            string input = Console.ReadLine()?.Trim().ToLower();

            switch (input)
            {
                case "1":
                    ShowGameSenseRankings(players);
                    break;

                case "2":
                    ShowTop10(players);
                    break;

                case "3":
                    SearchPlayer(players);
                    break;

                case "4":
                    ShowPPGRankings(players);
                    break;

                case "5":
                    CompareRankings(players);
                    break;

                case "q":
                    return;

                default:
                    Pause("Invalid selection.");
                    break;
            }
        }
    }


    static void ShowGameSenseRankings(List<Player> players)
    {
        Console.Clear();

        List<Player> ranked = players
            .OrderByDescending(p => p.GameSenseScore)
            .ToList();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     GAMESENSE RANKINGS                             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine(
            $"{"Rank",-5}" +
            $"{"Score",-8}" +
            $"{"Position",-8}" +
            $"{"Player",-27}" +
            $"{"PPG",-10}" +
            $"{"Total",-10}"
        );

        Console.WriteLine(new string('─', 65));

        for (int i = 0; i < ranked.Count; i++)
        {
            Player p = ranked[i];

            Console.WriteLine(
                $"{i + 1,-5}" +
                $"{p.GameSenseScore,-8:F2}" +
                $"{p.Pos.ToUpper(),-8:F2}" +
                $"{p.Name,-27}" +
                $"{p.ProjectedPointsPerGame,-10:F2}" +
                $"{p.ProjectedPointsTotal,-10:F1}"
            );
        }

        Pause();
    }


    static void ShowTop10(List<Player> players)
    {
        Console.Clear();

        List<Player> ranked = players
            .OrderByDescending(p => p.GameSenseScore)
            .Take(10)
            .ToList();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                         TOP 10                                     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        for (int i = 0; i < ranked.Count; i++)
        {
            Player p = ranked[i];

            Console.WriteLine(
                $"  {i + 1,2}. " +
                $"{p.Name,-25} " +
                $"GameSense: {p.GameSenseScore,6:F2}   " +
                $"PPG: {p.ProjectedPointsPerGame,5:F2}"
            );
        }

        Pause();
    }


    static void SearchPlayer(List<Player> players)
    {
        Console.Clear();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                       PLAYER SEARCH                                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("  Search: ");
        string search = Console.ReadLine()?.Trim().ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(search))
            return;

        List<Player> results = players
            .Where(p => p.Name.ToLower().Contains(search))
            .OrderByDescending(p => p.GameSenseScore)
            .ToList();

        Console.WriteLine();

        if (results.Count == 0)
        {
            Pause("No players found.");
            return;
        }

        foreach (Player p in results)
        {
            Console.WriteLine($"  {p.Name}");
            Console.WriteLine($"    GameSense : {p.GameSenseScore:F2}");
            Console.WriteLine($"    PPG       : {p.ProjectedPointsPerGame:F2}");
            Console.WriteLine($"    Total     : {p.ProjectedPointsTotal:F1}");
            Console.WriteLine();
        }

        Pause();
    }


    static void ShowPPGRankings(List<Player> players)
    {
        Console.Clear();

        List<Player> ranked = players
            .OrderByDescending(p => p.ProjectedPointsPerGame)
            .ToList();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                        PPG RANKINGS                                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine(
            $"{"Rank",-5}" +
            $"{"Player",-27}" +
            $"{"PPG",-10}" +
            $"{"GameSense",-12}"
        );

        Console.WriteLine(new string('─', 55));

        for (int i = 0; i < ranked.Count; i++)
        {
            Player p = ranked[i];

            Console.WriteLine(
                $"{i + 1,-5}" +
                $"{p.Name,-27}" +
                $"{p.ProjectedPointsPerGame,-10:F2}" +
                $"{p.GameSenseScore,-12:F2}"
            );
        }

        Pause();
    }


    static void CompareRankings(List<Player> players)
    {
        Console.Clear();

        List<Player> gameSense = players
            .OrderByDescending(p => p.GameSenseScore)
            .ToList();

        List<Player> ppg = players
            .OrderByDescending(p => p.ProjectedPointsPerGame)
            .ToList();

        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  GAMESENSE vs PPG                                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine(
            $"{"Player",-27}" +
            $"{"PPG Rank",-10}" +
            $"{"GS Rank",-10}" +
            $"{"Difference",-12}"
        );

        Console.WriteLine(new string('─', 60));

        for (int i = 0; i < gameSense.Count; i++)
        {
            Player p = gameSense[i];

            int gsRank = i + 1;
            int ppgRank = ppg.IndexOf(p) + 1;

            int difference = ppgRank - gsRank;

            string direction;

            if (difference > 0)
                direction = $"↑ {difference}";
            else if (difference < 0)
                direction = $"↓ {Math.Abs(difference)}";
            else
                direction = "—";

            Console.WriteLine(
                $"{p.Name,-27}" +
                $"{ppgRank,-10}" +
                $"{gsRank,-10}" +
                $"{direction,-12}"
            );
        }

        Pause();
    }


    static void Pause(string message = "")
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine();
            Console.WriteLine($"  {message}");
        }

        Console.WriteLine();
        Console.Write("  Press any key to continue...");
        Console.ReadKey();
    }
}