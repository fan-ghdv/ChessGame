using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class StalemateDetector
{
    public static bool IsStalemate(
        Board board,
        PieceColor color)
    {
        // A player who is in check
        // cannot be in stalemate.
        if (CheckDetector.IsInCheck(
                board,
                color))
        {
            return false;
        }

        // If the player has any legal move,
        // it is not stalemate.
        if (LegalMoveFinder.HasAnyLegalMove(
                board,
                color))
        {
            return false;
        }

        return true;
    }
}