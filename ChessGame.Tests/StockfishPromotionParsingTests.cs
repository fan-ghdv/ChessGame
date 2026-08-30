using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class StockfishPromotionParsingTests
{
    [Theory]
    [InlineData("e7e8q", PieceType.Queen)]
    [InlineData("e7e8r", PieceType.Rook)]
    [InlineData("e7e8b", PieceType.Bishop)]
    [InlineData("e7e8n", PieceType.Knight)]
    public void ParseBestMove_ShouldDetectPromotion(
        string notation,
        PieceType expectedPromotion)
    {
        Move? move =
            StockfishEngine.ParseBestMove(
                notation,
                out PieceType? promotion
            );

        Assert.NotNull(move);

        Assert.Equal(
            new Position(1, 4),
            move!.From
        );

        Assert.Equal(
            new Position(0, 4),
            move.To
        );

        Assert.Equal(
            expectedPromotion,
            promotion
        );
    }
}