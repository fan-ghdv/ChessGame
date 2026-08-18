using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class MoveValidator
{
    public static bool IsLegalMove(
        Board board,
        Move move,
        PieceColor color)
    {
        Piece? piece = board.GetPiece(move.From);

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

        if (!PieceMovement.IsValidMove(
                board,
                move.From,
                move.To))
        {
            return false;
        }

        Piece? capturedPiece = targetPiece;

        board.SetPiece(move.To, piece);
        board.SetPiece(move.From, null);

        bool kingInCheck =
            CheckDetector.IsInCheck(
                board,
                color
            );

        board.SetPiece(move.From, piece);
        board.SetPiece(move.To, capturedPiece);

        return !kingInCheck;
    }
}