using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class PawnPromotion
{
    public static bool CanPromote(Position position, PieceColor color)
    {
        if (color == PieceColor.White)
        {
            return position.Row == 0;
        }

        return position.Row == Board.Size - 1;
    }

    public static Piece Promote(
        Position position,
        PieceColor color,
        PieceType promotionType)
    {
        if (!CanPromote(position, color))
        {
            throw new InvalidOperationException(
                "Pawn has not reached the promotion rank."
            );
        }

        if (!IsValidPromotionType(promotionType))
        {
            throw new ArgumentException(
                "A pawn can only be promoted to a Queen, Rook, Bishop, or Knight."
            );
        }

        return new Piece(
            promotionType,
            color
        );
    }

    private static bool IsValidPromotionType(PieceType type)
    {
        return type == PieceType.Queen ||
               type == PieceType.Rook ||
               type == PieceType.Bishop ||
               type == PieceType.Knight;
    }
}