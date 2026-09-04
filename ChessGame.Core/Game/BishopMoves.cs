using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class BishopMoves
{
    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        // Up-Left
        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            -1,
            moves
        );

        // Up-Right
        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            1,
            moves
        );

        // Down-Left
        AddMovesInDirection(
            board,
            position,
            color,
            1,
            -1,
            moves
        );

        // Down-Right
        AddMovesInDirection(
            board,
            position,
            color,
            1,
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
                if (targetPiece.Color != color)
                {
                    moves.Add(targetPosition);
                }

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