using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class MoveValidator
{
    public static bool IsLegalMove(
        Board board,
        Move move,
        PieceColor color,
        Move? lastMove = null)
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

        Piece? targetPiece =
            board.GetPiece(move.To);

        if (targetPiece != null &&
            targetPiece.Color == color)
        {
            return false;
        }

        if (targetPiece != null &&
            targetPiece.Type == PieceType.King)
        {
            return false;
        }

        // =========================================================
        // CHECK NORMAL MOVE / EN PASSANT
        // =========================================================

        bool isNormalMove =
            PieceMovement.IsValidMove(
                board,
                move.From,
                move.To
            );

        bool isEnPassant =
            EnPassantValidator.CanCapture(
                board,
                move,
                lastMove,
                color
            );

        if (!isNormalMove &&
            !isEnPassant)
        {
            return false;
        }

        // =========================================================
        // SAVE PIECES
        // =========================================================

        Piece? capturedPiece =
            targetPiece;

        Position? enPassantCapturedPosition =
            null;

        // =========================================================
        // EN PASSANT CAPTURED PAWN
        // =========================================================

        if (isEnPassant)
        {
            enPassantCapturedPosition =
                new Position(
                    move.From.Row,
                    move.To.Column
                );

            capturedPiece =
                board.GetPiece(
                    enPassantCapturedPosition.Value
                );

            // Remove the captured pawn temporarily.
            board.SetPiece(
                enPassantCapturedPosition.Value,
                null
            );
        }

        // =========================================================
        // SIMULATE MOVE
        // =========================================================

        board.SetPiece(
            move.From,
            null
        );

        board.SetPiece(
            move.To,
            piece
        );

        // =========================================================
        // CHECK KING
        // =========================================================

        bool kingInCheck =
            CheckDetector.IsInCheck(
                board,
                color
            );

        // =========================================================
        // RESTORE MOVE
        // =========================================================

        board.SetPiece(
            move.From,
            piece
        );

        board.SetPiece(
            move.To,
            targetPiece
        );

        // =========================================================
        // RESTORE EN PASSANT PAWN
        // =========================================================

        if (isEnPassant &&
            enPassantCapturedPosition.HasValue)
        {
            board.SetPiece(
                enPassantCapturedPosition.Value,
                capturedPiece
            );
        }

        return !kingInCheck;
    }
}