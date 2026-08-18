using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class KnightMoves
{
    private static readonly (int Row, int Column)[] Offsets =
    {
        (-2, -1),
        (-2, 1),
        (-1, -2),
        (-1, 2),
        (1, -2),
        (1, 2),
        (2, -1),
        (2, 1)
    };

    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        foreach (var offset in Offsets)
        {
            int row = position.Row + offset.Row;
            int column = position.Column + offset.Column;

            if (!IsInsideBoard(row, column))
            {
                continue;
            }

            var targetPosition = new Position(row, column);
            var targetPiece = board.GetPiece(targetPosition);

            // Empty square
            if (targetPiece == null)
            {
                moves.Add(targetPosition);
                continue;
            }

            // Enemy piece can be captured
            if (targetPiece.Color != color)
            {
                moves.Add(targetPosition);
            }

            // Friendly piece is not a valid destination
        }

        return moves;
    }

    private static bool IsInsideBoard(int row, int column)
    {
        return row >= 0 &&
               row < Board.Size &&
               column >= 0 &&
               column < Board.Size;
    }
}