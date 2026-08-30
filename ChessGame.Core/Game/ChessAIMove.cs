using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class ChessAIMove
{
    public Move Move { get; }

    public PieceType? PromotionType { get; }

    public ChessAIMove(
        Move move,
        PieceType? promotionType = null)
    {
        Move = move;
        PromotionType = promotionType;
    }
}