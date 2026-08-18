using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class QueenMoves
{
    public static List<Position> GetPossibleMoves(
        Board board,
        Position position,
        PieceColor color)
    {
        var moves = new List<Position>();

        // Rook directions
        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            0,
            moves
        );

        AddMovesInDirection(
            board,
            position,
            color,
            1,
            0,
            moves
        );

        AddMovesInDirection(
            board,
            position,
            color,
            0,
            -1,
            moves
        );

        AddMovesInDirection(
            board,
            position,
            color,
            0,
            1,
            moves
        );

        // Bishop directions
        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            -1,
            moves
        );

        AddMovesInDirection(
            board,
            position,
            color,
            -1,
            1,
            moves
        );

        AddMovesInDirection(
            board,
            position,
            color,
            1,
            -1,
            moves
        );

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

            if (targetPiece == null)
            {
                moves.Add(targetPosition);
            }
            else
            {
                // Queen can capture enemy pieces.
                if (targetPiece.Color != color)
                {
                    moves.Add(targetPosition);
                }

                // Queen cannot move through pieces.
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