using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class StockfishMoveParsingTests
{
    [Theory]
    [InlineData("e7e8q", PieceType.Queen)]
    [InlineData("e7e8r", PieceType.Rook)]
    [InlineData("e7e8b", PieceType.Bishop)]
    [InlineData("e7e8n", PieceType.Knight)]
    public void ParseBestMove_ShouldReadPromotion(
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

    [Fact]
    public void ParseBestMove_ShouldReadNormalMove()
    {
        Move? move =
            StockfishEngine.ParseBestMove(
                "e2e4",
                out PieceType? promotion
            );

        Assert.NotNull(move);

        Assert.Equal(
            new Position(6, 4),
            move!.From
        );

        Assert.Equal(
            new Position(4, 4),
            move.To
        );

        Assert.Null(
            promotion
        );
    }

    [Fact]
    public void ParseBestMove_ShouldRejectInvalidMove()
    {
        Move? move =
            StockfishEngine.ParseBestMove(
                "xxxx",
                out PieceType? promotion
            );

        Assert.Null(move);

        Assert.Null(
            promotion
        );
    }
}