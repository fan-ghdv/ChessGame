using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class AttackDetector
{
    public static bool IsSquareAttacked(
        Board board,
        Position target,
        PieceColor attackingColor)
    {
        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                var position = new Position(row, column);
                var piece = board.GetPiece(position);

                if (piece == null || piece.Color != attackingColor)
                {
                    continue;
                }

                if (DoesPieceAttackSquare(
                    board,
                    position,
                    piece,
                    target))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool DoesPieceAttackSquare(
        Board board,
        Position position,
        Piece piece,
        Position target)
    {
        if (position == target)
        {
            return false;
        }

        return piece.Type switch
        {
            PieceType.Pawn =>
                DoesPawnAttack(position, piece.Color, target),

            PieceType.Knight =>
                DoesKnightAttack(position, target),

            PieceType.Bishop =>
                DoesBishopAttack(board, position, target),

            PieceType.Rook =>
                DoesRookAttack(board, position, target),

            PieceType.Queen =>
                DoesQueenAttack(board, position, target),

            PieceType.King =>
                DoesKingAttack(position, target),

            _ => false
        };
    }

    private static bool DoesPawnAttack(
        Position position,
        PieceColor color,
        Position target)
    {
        int direction = color == PieceColor.White ? -1 : 1;

        int rowDifference = target.Row - position.Row;
        int columnDifference = Math.Abs(
            target.Column - position.Column
        );

        return rowDifference == direction &&
               columnDifference == 1;
    }

    private static bool DoesKnightAttack(
        Position position,
        Position target)
    {
        int rowDifference = Math.Abs(
            target.Row - position.Row
        );

        int columnDifference = Math.Abs(
            target.Column - position.Column
        );

        return
            (rowDifference == 2 && columnDifference == 1) ||
            (rowDifference == 1 && columnDifference == 2);
    }

    private static bool DoesBishopAttack(
        Board board,
        Position position,
        Position target)
    {
        int rowDifference = Math.Abs(
            target.Row - position.Row
        );

        int columnDifference = Math.Abs(
            target.Column - position.Column
        );

        if (rowDifference != columnDifference)
        {
            return false;
        }

        return IsPathClear(
            board,
            position,
            target
        );
    }

    private static bool DoesRookAttack(
        Board board,
        Position position,
        Position target)
    {
        if (position.Row != target.Row &&
            position.Column != target.Column)
        {
            return false;
        }

        return IsPathClear(
            board,
            position,
            target
        );
    }

    private static bool DoesQueenAttack(
        Board board,
        Position position,
        Position target)
    {
        int rowDifference = Math.Abs(
            target.Row - position.Row
        );

        int columnDifference = Math.Abs(
            target.Column - position.Column
        );

        bool straight =
            position.Row == target.Row ||
            position.Column == target.Column;

        bool diagonal =
            rowDifference == columnDifference;

        if (!straight && !diagonal)
        {
            return false;
        }

        return IsPathClear(
            board,
            position,
            target
        );
    }

    private static bool DoesKingAttack(
        Position position,
        Position target)
    {
        int rowDifference = Math.Abs(
            target.Row - position.Row
        );

        int columnDifference = Math.Abs(
            target.Column - position.Column
        );

        return rowDifference <= 1 &&
               columnDifference <= 1 &&
               !(rowDifference == 0 &&
                 columnDifference == 0);
    }

    private static bool IsPathClear(
        Board board,
        Position start,
        Position target)
    {
        int rowDirection = Math.Sign(
            target.Row - start.Row
        );

        int columnDirection = Math.Sign(
            target.Column - start.Column
        );

        int row = start.Row + rowDirection;
        int column = start.Column + columnDirection;

        while (row != target.Row ||
               column != target.Column)
        {
            if (board.GetPiece(
                    new Position(row, column)) != null)
            {
                return false;
            }

            row += rowDirection;
            column += columnDirection;
        }

        return true;
    }
}