using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class PieceMovement
{
    public static bool IsValidMove(
        Board board,
        Position from,
        Position to)
    {
        Piece? piece = board.GetPiece(from);

        if (piece == null)
        {
            return false;
        }

        if (from == to)
        {
            return false;
        }

        Piece? target = board.GetPiece(to);

        if (target != null &&
            target.Color == piece.Color)
        {
            return false;
        }

        return piece.Type switch
        {
            PieceType.Pawn =>
                IsPawnMove(
                    board,
                    from,
                    to,
                    piece.Color
                ),

            PieceType.Knight =>
                IsKnightMove(from, to),

            PieceType.Bishop =>
                IsBishopMove(
                    board,
                    from,
                    to
                ),

            PieceType.Rook =>
                IsRookMove(
                    board,
                    from,
                    to
                ),

            PieceType.Queen =>
                IsQueenMove(
                    board,
                    from,
                    to
                ),

            PieceType.King =>
                IsKingMove(from, to),

            _ => false
        };
    }

    private static bool IsPawnMove(
        Board board,
        Position from,
        Position to,
        PieceColor color)
    {
        int direction =
            color == PieceColor.White
                ? -1
                : 1;

        int startRow =
            color == PieceColor.White
                ? 6
                : 1;

        int rowDifference =
            to.Row - from.Row;

        int columnDifference =
            Math.Abs(to.Column - from.Column);

        Piece? target =
            board.GetPiece(to);

        // =========================
        // ONE SQUARE FORWARD
        // =========================

        if (columnDifference == 0 &&
            rowDifference == direction &&
            target == null)
        {
            return true;
        }

        // =========================
        // TWO SQUARES FROM START
        // =========================

        if (columnDifference == 0 &&
            rowDifference == direction * 2 &&
            from.Row == startRow &&
            target == null)
        {
            Position middlePosition =
                new Position(
                    from.Row + direction,
                    from.Column
                );

            // The square between the pawn
            // and destination must be empty.
            if (board.GetPiece(middlePosition) != null)
            {
                return false;
            }

            return true;
        }

        // =========================
        // DIAGONAL CAPTURE
        // =========================

        if (columnDifference == 1 &&
            rowDifference == direction &&
            target != null &&
            target.Color != color)
        {
            return true;
        }

        return false;
    }

    private static bool IsKnightMove(
        Position from,
        Position to)
    {
        int rowDifference =
            Math.Abs(to.Row - from.Row);

        int columnDifference =
            Math.Abs(to.Column - from.Column);

        return
            (rowDifference == 2 &&
             columnDifference == 1) ||
            (rowDifference == 1 &&
             columnDifference == 2);
    }

    private static bool IsBishopMove(
        Board board,
        Position from,
        Position to)
    {
        int rowDifference =
            Math.Abs(to.Row - from.Row);

        int columnDifference =
            Math.Abs(to.Column - from.Column);

        if (rowDifference != columnDifference)
        {
            return false;
        }

        return IsPathClear(
            board,
            from,
            to
        );
    }

    private static bool IsRookMove(
        Board board,
        Position from,
        Position to)
    {
        if (from.Row != to.Row &&
            from.Column != to.Column)
        {
            return false;
        }

        return IsPathClear(
            board,
            from,
            to
        );
    }

    private static bool IsQueenMove(
        Board board,
        Position from,
        Position to)
    {
        int rowDifference =
            Math.Abs(to.Row - from.Row);

        int columnDifference =
            Math.Abs(to.Column - from.Column);

        bool straight =
            from.Row == to.Row ||
            from.Column == to.Column;

        bool diagonal =
            rowDifference == columnDifference;

        if (!straight && !diagonal)
        {
            return false;
        }

        return IsPathClear(
            board,
            from,
            to
        );
    }

    private static bool IsKingMove(
        Position from,
        Position to)
    {
        int rowDifference =
            Math.Abs(to.Row - from.Row);

        int columnDifference =
            Math.Abs(to.Column - from.Column);

        return rowDifference <= 1 &&
               columnDifference <= 1 &&
               !(rowDifference == 0 &&
                 columnDifference == 0);
    }

    private static bool IsPathClear(
        Board board,
        Position from,
        Position to)
    {
        int rowDirection =
            Math.Sign(to.Row - from.Row);

        int columnDirection =
            Math.Sign(to.Column - from.Column);

        int row =
            from.Row + rowDirection;

        int column =
            from.Column + columnDirection;

        while (row != to.Row ||
               column != to.Column)
        {
            if (board.GetPiece(
                    new Position(row, column)
                ) != null)
            {
                return false;
            }

            row += rowDirection;
            column += columnDirection;
        }

        return true;
    }
}