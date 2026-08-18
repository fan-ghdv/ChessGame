using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class CheckmateDetector
{
    public static bool IsCheckmate(
        Board board,
        PieceColor color)
    {
        if (!CheckDetector.IsInCheck(
                board,
                color))
        {
            return false;
        }

        if (LegalMoveFinder.HasAnyLegalMove(
                board,
                color))
        {
            return false;
        }

        return true;
    }
}