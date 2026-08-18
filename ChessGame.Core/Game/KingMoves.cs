using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class KingMoves
{
    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        // The King can move one square in any of the 8 directions.
        for (int rowDirection = -1; rowDirection <= 1; rowDirection++)
        {
            for (int columnDirection = -1; columnDirection <= 1; columnDirection++)
            {
                // Do not stay on the same square.
                if (rowDirection == 0 && columnDirection == 0)
                {
                    continue;
                }

                int row = position.Row + rowDirection;
                int column = position.Column + columnDirection;

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
                }
                // Enemy piece
                else if (targetPiece.Color != color)
                {
                    moves.Add(targetPosition);
                }

                // Friendly pieces are not added.
            }
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