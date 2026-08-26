using NFLDraft.Functions;
using NFLDraft.Models;
using System.Data;

class Program
{
    static void Main(string[] args)
    {
        //make new manager
        Manager user = new Manager();
        user.Name = "User";



        //load players
        Reader reader = new Reader();
        List<Player> players = reader.ReadPlayers().Where(p => p.SetUserid == -1 && (p.Pos.ToLower() == "rb" || p.Pos.ToLower() == "wr" || p.Pos.ToLower() == "te"|| p.Pos.ToLower() == "qb")).ToList(); //CONSENSUS

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

        //QB
        List<Player> qb = players
            .Where(p => p.Pos.ToLower() == "qb")
            .ToList();


        int fullListCount = wr.Count + rb.Count + te.Count + qb.Count;

        //calculate game sense for each, then combine
        List<Player> pNew = new List<Player>();

        foreach (Player p in wr)
        {
            updater.GameSense(p, wr, players);
            pNew.Add(p);
        }

        foreach (Player p in rb)
        {
            updater.GameSense(p, rb, players);
            pNew.Add(p);
        }

        foreach (Player p in te)
        {
            updater.GameSense(p, te, players);
            pNew.Add(p);
        }

        foreach (Player p in qb)
        {
            updater.GameSense(p, qb, players);
            pNew.Add(p);
        }


        RecalculateGameSense(pNew, user);

        //List<Player> zero = pNew
        //    .Where(p => p.GameSenseScore <= 0.01)
        //    .ToList();

        //foreach (Player p in zero)
        //{
        //    Console.WriteLine(
        //        $"{p.Name,-25} " +
        //        $"GS={p.GameSenseScore:F4} " +
        //        $"Q={p.QualityScore:F2} " +
        //        $"S={p.ScarcityScore:F2} " +
        //        $"V={p.ValueSpreadScore:F2} " +
        //        $"P={p.PoolScore:F2}"
        //    );
        //}

        //Start interface
        RunInterface(pNew, user);
    }


    static void RunInterface(List<Player> players, Manager user)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  NFL DRAFT ENGINE                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine($"  Manager: {user.Name}");
            Console.WriteLine($"  Available Players: {players.Count}");
            Console.WriteLine();

            Console.WriteLine("  ┌──────────────────────────────────────────────┐");
            Console.WriteLine("  │                  MAIN MENU                   │");
            Console.WriteLine("  ├──────────────────────────────────────────────┤");
            Console.WriteLine("  │  1. View GameSense Rankings                  │");
            Console.WriteLine("  │  2. View Top 10                              │");
            Console.WriteLine("  │  3. Search Player                            │");
            Console.WriteLine("  │  4. Compare GameSense vs PPG                 │");
            Console.WriteLine("  │                                              │");
            Console.WriteLine("  │  5. Remove Player                            │");
            Console.WriteLine("  │  6. Draft Player To My Team                  │");
            Console.WriteLine("  │  7. View My Team                             │");
            Console.WriteLine("  │                                              │");
            Console.WriteLine("  │  Q. Quit                                     │");
            Console.WriteLine("  └──────────────────────────────────────────────┘");
            Console.WriteLine();

            Console.Write("  Select an option: ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

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
                    CompareRankings(players);
                    break;

                case "5":
                    RemovePlayer(players);
                    RecalculateGameSense(players, user);
                    break;

                case "6":
                    DraftPlayer(players, user);
                    RecalculateGameSense(players, user);
                    break;

                case "7":
                    ShowMyTeam(user);
                    break;

                case "q":
                    return;

                default:
                    Pause("Invalid selection.");
                    break;
            }
        }
    }

    static void ShowMyTeam(Manager user)
    {
        Console.Clear();

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine($"               {user.Name.ToUpper()}'S TEAM");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        ShowPosition("QB", user.QB);
        ShowPosition("RB", user.RB);
        ShowPosition("WR", user.WR);
        ShowPosition("TE", user.TE);

        int totalPlayers =
            user.QB.Count +
            user.RB.Count +
            user.WR.Count +
            user.TE.Count;

        double totalPPG =
            user.QB.Sum(p => p.ProjectedPointsPerGame) +
            user.RB.Sum(p => p.ProjectedPointsPerGame) +
            user.WR.Sum(p => p.ProjectedPointsPerGame) +
            user.TE.Sum(p => p.ProjectedPointsPerGame);

        double totalPoints =
            user.QB.Sum(p => p.ProjectedPointsTotal) +
            user.RB.Sum(p => p.ProjectedPointsTotal) +
            user.WR.Sum(p => p.ProjectedPointsTotal) +
            user.TE.Sum(p => p.ProjectedPointsTotal);

        Console.WriteLine("  ─────────────────────────────────────────");
        Console.WriteLine($"  Players:           {totalPlayers}");
        Console.WriteLine($"  Projected PPG:     {totalPPG:F2}");
        Console.WriteLine($"  Projected Total:   {totalPoints:F1}");

        Pause();
    }

    static void DraftPlayer(List<Player> players, Manager user)
    {
        Console.Clear();

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  DRAFT PLAYER                        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("  Search player: ");
        string search = Console.ReadLine()?.Trim().ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(search))
            return;

        List<Player> results = players
            .Where(p => p.Name.ToLower().Contains(search))
            .OrderByDescending(p => p.GameSenseScore)
            .ToList();

        if (results.Count == 0)
        {
            Pause("No players found.");
            return;
        }

        Console.WriteLine();

        for (int i = 0; i < results.Count; i++)
        {
            Player p = results[i];

            Console.WriteLine(
                $"  {i + 1}. {p.Name,-25} " +
                $"{p.Pos,-3} " +
                $"{p.ProjectedPointsPerGame,5:F2} PPG " +
                $"GS: {p.GameSenseScore,6:F2}"
            );
        }

        Console.WriteLine();
        Console.Write("  Select player (number): ");

        if (!int.TryParse(Console.ReadLine(), out int selection))
        {
            Pause("Invalid selection.");
            return;
        }

        if (selection < 1 || selection > results.Count)
        {
            Pause("Invalid selection.");
            return;
        }

        Player selected = results[selection - 1];

        // Remove from available pool
        players.Remove(selected);

        // Add to manager's position list
        switch (selected.Pos.ToUpper())
        {
            case "QB":
                user.QB.Add(selected);
                break;

            case "RB":
                user.RB.Add(selected);
                break;

            case "WR":
                user.WR.Add(selected);
                break;

            case "TE":
                user.TE.Add(selected);
                break;

            default:
                Pause($"Unknown position: {selected.Pos}");
                return;
        }

        Pause($"{selected.Name} drafted to {user.Name}'s team.");
    }

    static void RecalculateGameSense(List<Player> players, Manager user)
    {
        StatFunctions updater = new StatFunctions();

        List<Player> rb = players
            .Where(p => p.Pos.ToLower() == "rb")
            .ToList();

        List<Player> wr = players
            .Where(p => p.Pos.ToLower() == "wr")
            .ToList();

        List<Player> te = players
            .Where(p => p.Pos.ToLower() == "te")
            .ToList();

        int fullList = rb.Count + wr.Count + te.Count;


        // Calculate base GameSense
        foreach (Player p in wr)
            updater.GameSense(p, wr, players);

        foreach (Player p in rb)
            updater.GameSense(p, rb, players);

        foreach (Player p in te)
            updater.GameSense(p, te, players);


        // Apply roster need
        foreach (Player p in players)
        {
            double multiplier = GetPositionMultiplier(p.Pos, user);

            p.GameSenseScore = p.GameSenseScore * multiplier;
        }
    }

    static void RemovePlayer(List<Player> players)
    {
        Console.Clear();

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   REMOVE PLAYER                      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        string search = "";

        do
        {
            Console.Write("  Search player. Type 1 to exit: ");
            search = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(search))
                break; ;

            List<Player> results = players
                .Where(p => p.Name.ToLower().Contains(search))
                .OrderByDescending(p => p.GameSenseScore)
                .ToList();

            if (results.Count == 0)
            {
                Pause("No players found.");
                return;
            }

            Console.WriteLine();

            for (int i = 0; i < results.Count; i++)
            {
                Player p = results[i];

                Console.WriteLine(
                    $"  {i + 1}. {p.Name,-25} " +
                    $"{p.Pos,-3} " +
                    $"{p.ProjectedPointsPerGame,5:F2} PPG " +
                    $"GS: {p.GameSenseScore,6:F2}"
                );
            }

            Console.WriteLine();
            Console.Write("  Select player (number): ");

            if (!int.TryParse(Console.ReadLine(), out int selection))
            {
                Pause("Invalid selection.");
                return;
            }

            if (selection < 1 || selection > results.Count)
            {
                Pause("Invalid selection.");
                return;
            }

            Player selected = results[selection - 1];

            players.Remove(selected);
        } while (search != "1");
        

        Pause($"Players removed from the available player pool.");
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
            Console.WriteLine("  ┌──────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"  │ {p.Name,-60} ");
            Console.WriteLine("  ├──────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"  │ GameSense        : {p.GameSenseScore,6:F2}  ");
            Console.WriteLine($"  │ PPG              : {p.ProjectedPointsPerGame,6:F2}");
            Console.WriteLine($"  │ Total            : {p.ProjectedPointsTotal,6:F1}");
            Console.WriteLine("  │                                                              |");
            Console.WriteLine($"  │ Quality          : {p.QualityScore,6:F2}  × 60%");
            Console.WriteLine($"  │ Scarcity         : {p.ScarcityScore,6:F2}  × 15%");
            Console.WriteLine($"  │ Value Spread     : {p.ValueSpreadScore,6:F2}  × 10%");
            Console.WriteLine($"  │ Position Pool    : {p.PoolScore,6:F2}  × 15%");
            Console.WriteLine("  └──────────────────────────────────────────────────────────────┘");
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

    static void ShowPosition(string position, List<Player> players)
    {
        if (players == null || players.Count == 0)
            return;

        Console.WriteLine($"  {position}");
        Console.WriteLine("  ─────────────────────────────────────────");

        foreach (Player p in players)
        {
            Console.WriteLine(
                $"    {p.Name,-25} " +
                $"{p.ProjectedPointsPerGame,6:F2} PPG"
            );
        }

        Console.WriteLine();
    }

    static double GetPositionMultiplier(string position, Manager user)
    {
        int openSlots = 0;

        switch (position.ToUpper())
        {
            case "RB":
                openSlots = (2 + (1 - (int)user.FLEX.Count / 2)) - user.RB.Count;
                break;

            case "WR":
                openSlots = (2 + (1 - (int)user.FLEX.Count / 2)) - user.WR.Count;
                break;

            case "TE":
                openSlots = (1 + (1 - (int)user.FLEX.Count / 2)) - user.TE.Count;
                break;

            case "QB":
                openSlots = 1 - user.QB.Count;
                break;

            default:
                return 1.0;
        }

        //No open starting slots
        if (openSlots <= 0)
            return 0.0;

        return 1.0 + ((openSlots - 1) * 0.25);
    }
}