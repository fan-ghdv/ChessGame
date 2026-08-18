using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class EnPassantValidator
{
    public static bool CanCapture(
        Board board,
        Move move,
        Move? lastMove,
        PieceColor color)
    {
        Piece? pawn =
            board.GetPiece(move.From);

        if (pawn == null ||
            pawn.Type != PieceType.Pawn ||
            pawn.Color != color)
        {
            return false;
        }

        int direction =
            color == PieceColor.White
                ? -1
                : 1;

        int rowDifference =
            move.To.Row - move.From.Row;

        int columnDifference =
            Math.Abs(
                move.To.Column -
                move.From.Column
            );

        // En passant must be one square forward
        // and one column sideways.
        if (rowDifference != direction ||
            columnDifference != 1)
        {
            return false;
        }

        // Destination square must be empty.
        if (board.GetPiece(move.To) != null)
        {
            return false;
        }

        // There must have been a previous move.
        if (lastMove == null)
        {
            return false;
        }

        Piece? enemyPawn =
            board.GetPiece(
                new Position(
                    move.From.Row,
                    move.To.Column
                )
            );

        // The adjacent piece must be an enemy pawn.
        if (enemyPawn == null ||
            enemyPawn.Type != PieceType.Pawn ||
            enemyPawn.Color == color)
        {
            return false;
        }

        // The enemy pawn must have just moved two squares.
        int lastRowDifference =
            lastMove.To.Row -
            lastMove.From.Row;

        if (Math.Abs(lastRowDifference) != 2)
        {
            return false;
        }

        // The last move must belong to the pawn
        // next to our pawn.
        if (lastMove.To !=
            new Position(
                move.From.Row,
                move.To.Column
            ))
        {
            return false;
        }

        if (lastMove.From.Column !=
            lastMove.To.Column)
        {
            return false;
        }

        return true;
    }
}