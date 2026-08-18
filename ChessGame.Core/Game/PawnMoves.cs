using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class PawnMoves
{
    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        int direction = color == PieceColor.White ? -1 : 1;

        int oneStepRow = position.Row + direction;

        // One square forward
        if (IsInsideBoard(oneStepRow, position.Column) &&
            board.GetPiece(
                new Position(oneStepRow, position.Column)
            ) == null)
        {
            moves.Add(
                new Position(oneStepRow, position.Column)
            );

            // Two squares forward from starting position
            int startingRow = color == PieceColor.White ? 6 : 1;
            int twoStepRow = position.Row + direction * 2;

            if (position.Row == startingRow &&
                board.GetPiece(
                    new Position(twoStepRow, position.Column)
                ) == null)
            {
                moves.Add(
                    new Position(twoStepRow, position.Column)
                );
            }
        }

        // Capture diagonally
        int[] columns = { -1, 1 };

        foreach (int columnOffset in columns)
        {
            int targetColumn = position.Column + columnOffset;

            if (!IsInsideBoard(oneStepRow, targetColumn))
            {
                continue;
            }

            var target = board.GetPiece(
                new Position(oneStepRow, targetColumn)
            );

            if (target != null &&
                target.Color != color)
            {
                moves.Add(
                    new Position(oneStepRow, targetColumn)
                );
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