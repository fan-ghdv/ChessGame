using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class MoveExecutor
{
    public static bool TryExecuteMove(
        Board board,
        Move move,
        PieceColor color,
        Move? lastMove = null,
        PieceType? promotionType = null)
    {
        Piece? piece =
            board.GetPiece(move.From);

        if (piece == null)
        {
            return false;
        }

        if (piece.Color != color)
        {
            return false;
        }

        // =========================
        // CASTLING
        // =========================

        if (piece.Type == PieceType.King)
        {
            int row =
                color == PieceColor.White
                    ? 7
                    : 0;

            // King-side castling
            if (move.From == new Position(row, 4) &&
                move.To == new Position(row, 6))
            {
                if (!CastlingValidator.CanCastleKingSide(
                        board,
                        color))
                {
                    return false;
                }

                return ExecuteKingSideCastling(
                    board,
                    color
                );
            }

            // Queen-side castling
            if (move.From == new Position(row, 4) &&
                move.To == new Position(row, 2))
            {
                if (!CastlingValidator.CanCastleQueenSide(
                        board,
                        color))
                {
                    return false;
                }

                return ExecuteQueenSideCastling(
                    board,
                    color
                );
            }
        }

        // =========================
        // EN PASSANT
        // =========================

        if (piece.Type == PieceType.Pawn)
        {
            if (EnPassantValidator.CanCapture(
                    board,
                    move,
                    lastMove,
                    color))
            {
                return ExecuteEnPassant(
                    board,
                    move,
                    color
                );
            }
        }

        // =========================
        // NORMAL MOVE
        // =========================

        if (!MoveValidator.IsLegalMove(
                board,
                move,
                color))
        {
            return false;
        }

        // =========================
        // CHECK PROMOTION
        // =========================

        if (piece.Type == PieceType.Pawn &&
            PawnPromotion.CanPromote(
                move.To,
                color))
        {
            if (promotionType == null)
            {
                return false;
            }
        }

        board.SetPiece(
            move.To,
            piece
        );

        board.SetPiece(
            move.From,
            null
        );

        // =========================
        // PROMOTION
        // =========================

        if (piece.Type == PieceType.Pawn &&
            PawnPromotion.CanPromote(
                move.To,
                color))
        {
            if (promotionType == null)
            {
                return false;
            }

            Piece promotedPiece =
                PawnPromotion.Promote(
                    move.To,
                    color,
                    promotionType.Value
                );

            board.SetPiece(
                move.To,
                promotedPiece
            );
        }

        piece.MarkAsMoved();

        return true;
    }

    private static bool ExecuteEnPassant(
        Board board,
        Move move,
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

        Position capturedPawnPosition =
            new Position(
                move.From.Row,
                move.To.Column
            );

        Piece? capturedPawn =
            board.GetPiece(
                capturedPawnPosition
            );

        if (capturedPawn == null ||
            capturedPawn.Type != PieceType.Pawn ||
            capturedPawn.Color == color)
        {
            return false;
        }

        // Move our pawn.
        board.SetPiece(
            move.To,
            pawn
        );

        board.SetPiece(
            move.From,
            null
        );

        // Remove the captured pawn.
        board.SetPiece(
            capturedPawnPosition,
            null
        );

        pawn.MarkAsMoved();

        return true;
    }

    private static bool ExecuteKingSideCastling(
        Board board,
        PieceColor color)
    {
        int row =
            color == PieceColor.White
                ? 7
                : 0;

        Position kingFrom =
            new Position(row, 4);

        Position kingTo =
            new Position(row, 6);

        Position rookFrom =
            new Position(row, 7);

        Position rookTo =
            new Position(row, 5);

        Piece? king =
            board.GetPiece(kingFrom);

        Piece? rook =
            board.GetPiece(rookFrom);

        if (king == null ||
            rook == null)
        {
            return false;
        }

        board.SetPiece(kingTo, king);
        board.SetPiece(kingFrom, null);

        board.SetPiece(rookTo, rook);
        board.SetPiece(rookFrom, null);

        king.MarkAsMoved();
        rook.MarkAsMoved();

        return true;
    }

    private static bool ExecuteQueenSideCastling(
        Board board,
        PieceColor color)
    {
        int row =
            color == PieceColor.White
                ? 7
                : 0;

        Position kingFrom =
            new Position(row, 4);

        Position kingTo =
            new Position(row, 2);

        Position rookFrom =
            new Position(row, 0);

        Position rookTo =
            new Position(row, 3);

        Piece? king =
            board.GetPiece(kingFrom);

        Piece? rook =
            board.GetPiece(rookFrom);

        if (king == null ||
            rook == null)
        {
            return false;
        }

        board.SetPiece(kingTo, king);
        board.SetPiece(kingFrom, null);

        board.SetPiece(rookTo, rook);
        board.SetPiece(rookFrom, null);

        king.MarkAsMoved();
        rook.MarkAsMoved();

        return true;
    }
}