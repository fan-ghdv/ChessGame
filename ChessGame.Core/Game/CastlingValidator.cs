using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class CastlingValidator
{
    public static bool CanCastleKingSide(
        Board board,
        PieceColor color)
    {
        int row =
            color == PieceColor.White
                ? 7
                : 0;

        Position kingPosition =
            new Position(row, 4);

        Position rookPosition =
            new Position(row, 7);

        Position kingPassPosition =
            new Position(row, 5);

        Position kingTargetPosition =
            new Position(row, 6);

        Piece? king =
            board.GetPiece(kingPosition);

        Piece? rook =
            board.GetPiece(rookPosition);

        // King and rook must exist.
        if (king == null ||
            rook == null)
        {
            return false;
        }

        // They must be the correct pieces.
        if (king.Type != PieceType.King ||
            rook.Type != PieceType.Rook)
        {
            return false;
        }

        // Both pieces must belong to the same player.
        if (king.Color != color ||
            rook.Color != color)
        {
            return false;
        }

        // Neither piece may have moved before.
        if (king.HasMoved ||
            rook.HasMoved)
        {
            return false;
        }

        // Squares between King and Rook must be empty.
        if (board.GetPiece(
                kingPassPosition) != null ||
            board.GetPiece(
                kingTargetPosition) != null)
        {
            return false;
        }

        PieceColor enemyColor =
            color == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        // King cannot castle while in check.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingPosition,
                enemyColor))
        {
            return false;
        }

        // King cannot pass through an attacked square.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingPassPosition,
                enemyColor))
        {
            return false;
        }

        // King cannot finish on an attacked square.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingTargetPosition,
                enemyColor))
        {
            return false;
        }

        return true;
    }

    public static bool CanCastleQueenSide(
        Board board,
        PieceColor color)
    {
        int row =
            color == PieceColor.White
                ? 7
                : 0;

        Position kingPosition =
            new Position(row, 4);

        Position rookPosition =
            new Position(row, 0);

        Position kingPassPosition =
            new Position(row, 3);

        Position kingTargetPosition =
            new Position(row, 2);

        Position betweenPosition =
            new Position(row, 1);

        Piece? king =
            board.GetPiece(kingPosition);

        Piece? rook =
            board.GetPiece(rookPosition);

        // King and rook must exist.
        if (king == null ||
            rook == null)
        {
            return false;
        }

        // They must be the correct pieces.
        if (king.Type != PieceType.King ||
            rook.Type != PieceType.Rook)
        {
            return false;
        }

        // They must belong to the same player.
        if (king.Color != color ||
            rook.Color != color)
        {
            return false;
        }

        // Neither piece may have moved before.
        if (king.HasMoved ||
            rook.HasMoved)
        {
            return false;
        }

        // All squares between King and Rook must be empty.
        if (board.GetPiece(
                betweenPosition) != null ||
            board.GetPiece(
                kingPassPosition) != null ||
            board.GetPiece(
                kingTargetPosition) != null)
        {
            return false;
        }

        PieceColor enemyColor =
            color == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        // King cannot castle while in check.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingPosition,
                enemyColor))
        {
            return false;
        }

        // King cannot pass through an attacked square.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingPassPosition,
                enemyColor))
        {
            return false;
        }

        // King cannot finish on an attacked square.
        if (AttackDetector.IsSquareAttacked(
                board,
                kingTargetPosition,
                enemyColor))
        {
            return false;
        }

        return true;
    }
}