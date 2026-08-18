using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class RookMoves
{
    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        // Up
        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            0,
            moves
        );

        // Down
        AddMovesInDirection(
            board,
            position,
            color,
            1,
            0,
            moves
        );

        // Left
        AddMovesInDirection(
            board,
            position,
            color,
            0,
            -1,
            moves
        );

        // Right
        AddMovesInDirection(
            board,
            position,
            color,
            0,
            1,
            moves
        );

        return moves;
    }

    private static void AddMovesInDirection(
        Board board,
        Position position,
        PieceColor color,
        int rowDirection,
        int columnDirection,
        List<Position> moves)
    {
        int row = position.Row + rowDirection;
        int column = position.Column + columnDirection;

        while (IsInsideBoard(row, column))
        {
            var targetPosition = new Position(row, column);
            var targetPiece = board.GetPiece(targetPosition);

            // Empty square
            if (targetPiece == null)
            {
                moves.Add(targetPosition);
            }
            else
            {
                // Enemy piece can be captured
                if (targetPiece.Color != color)
                {
                    moves.Add(targetPosition);
                }

                // Rook cannot move through any piece
                break;
            }

            row += rowDirection;
            column += columnDirection;
        }
    }

    private static bool IsInsideBoard(int row, int column)
    {
        return row >= 0 &&
               row < Board.Size &&
               column >= 0 &&
               column < Board.Size;
    }
}