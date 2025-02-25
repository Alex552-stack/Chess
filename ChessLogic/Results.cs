namespace ChessLogic;

public class Results
{
    public Results(Player winner, EndReason reason)
    {
        Winner = winner;
        Reason = reason;
    }

    public Player Winner { get; set; }
    public EndReason Reason { get; set; }

    public static Results Win(Player winner)
    {
        return new Results(winner, EndReason.Checkmate);
    }

    public static Results Draw(EndReason reason)
    {
        return new Results(Player.None, reason);
    }
}