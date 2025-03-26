using ChessLogic;
using ChessLogic.Moves;

namespace ChessServer.Models;

//TODO: refactor this
public class MoveDto
{
    public required string From { get; set; }
    public required string To { get; set; }
    public string Promotion { get; set; } = string.Empty;
    
    public NormalMove GetNormalMove()
    {
        return new NormalMove(ConvertToPosition(From),
            ConvertToPosition(To));
    }


    public static Position? ConvertToPosition(string positionAsString)
    {
        try
        {
            char row = positionAsString[1];
            char column = positionAsString[0];
            
            int rowValue = int.Parse(row.ToString());
            // rowValue -= 1;
            rowValue = 8 - rowValue;
            int columnValue = int.Parse((column - 'a').ToString());
            return new Position(rowValue, columnValue);
        }
        catch (Exception e)
        {
            //TODO
            Logger<MoveDto> logger = new Logger<MoveDto>(new LoggerFactory());
            logger.LogError(e, "Error parsing position. ErrMsg: " + e.Message);
            return null;
        }
    }
}