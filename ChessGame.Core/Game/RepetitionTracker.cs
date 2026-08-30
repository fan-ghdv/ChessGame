using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class RepetitionTracker
{
    private readonly Dictionary<string, int> positionCounts =
        new Dictionary<string, int>();

    public void RecordPosition(
        Board board)
    {
        string key =
            PositionKeyGenerator.Generate(
                board
            );

        RecordPositionKey(key);
    }

    public void RecordPositionKey(
        string key)
    {
        if (positionCounts.ContainsKey(key))
        {
            positionCounts[key]++;
        }
        else
        {
            positionCounts[key] = 1;
        }
    }

    public int GetPositionCount(
        Board board)
    {
        string key =
            PositionKeyGenerator.Generate(
                board
            );

        return GetPositionCount(key);
    }

    public int GetPositionCount(
        string key)
    {
        if (positionCounts.TryGetValue(
                key,
                out int count))
        {
            return count;
        }

        return 0;
    }

    public bool IsThreefoldRepetition(
        Board board)
    {
        return GetPositionCount(board) >= 3;
    }

    public bool IsThreefoldRepetition(
        string key)
    {
        return GetPositionCount(key) >= 3;
    }

    public void Clear()
    {
        positionCounts.Clear();
    }
}