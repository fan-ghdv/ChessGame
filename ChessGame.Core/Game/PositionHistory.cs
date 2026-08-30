using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class PositionHistory
{
    private readonly Dictionary<string, int> positionCounts =
        new Dictionary<string, int>();

    public void Clear()
    {
        positionCounts.Clear();
    }

    // =========================================================
    // RECORD POSITION
    // =========================================================

    public void Record(
        Board board,
        PieceColor sideToMove,
        Move? lastMove = null)
    {
        string key =
            PositionKeyGenerator.Generate(
                board,
                sideToMove,
                lastMove
            );

        if (positionCounts.ContainsKey(key))
        {
            positionCounts[key]++;
        }
        else
        {
            positionCounts[key] = 1;
        }
    }

    // =========================================================
    // GET POSITION COUNT
    // =========================================================

    public int GetCount(
        Board board,
        PieceColor sideToMove,
        Move? lastMove = null)
    {
        string key =
            PositionKeyGenerator.Generate(
                board,
                sideToMove,
                lastMove
            );

        if (positionCounts.TryGetValue(
                key,
                out int count))
        {
            return count;
        }

        return 0;
    }

    // =========================================================
    // THREEFOLD REPETITION
    // =========================================================

    public bool IsThreefoldRepetition(
        Board board,
        PieceColor sideToMove,
        Move? lastMove = null)
    {
        return
            GetCount(
                board,
                sideToMove,
                lastMove
            ) >= 3;
    }
}