using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class GameStateDetector
{
    public static bool IsCheckmate(
        Board board,
        PieceColor color)
    {
        // The king must currently be in check.
        if (!CheckDetector.IsInCheck(
                board,
                color))
        {
            return false;
        }

        // If there is no legal move,
        // the player is checkmated.
        return !HasLegalMoves(
            board,
            color
        );
    }

    public static bool IsStalemate(
        Board board,
        PieceColor color)
    {
        // Stalemate can only happen when
        // the king is NOT in check.
        if (CheckDetector.IsInCheck(
                board,
                color))
        {
            return false;
        }

        // No legal moves + not in check
        // = stalemate.
        return !HasLegalMoves(
            board,
            color
        );
    }

    private static bool HasLegalMoves(
        Board board,
        PieceColor color)
    {
        return LegalMoveGenerator
            .GetLegalMoves(
                board,
                color
            )
            .Count > 0;
    }
}