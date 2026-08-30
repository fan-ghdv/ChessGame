using ChessGame.Core.Game;
using ChessGame.Core.Models;
using Xunit;

namespace ChessGame.Tests;

public class ChessAITests
{
    [Fact]
    public void EasyAI_ShouldReturnLegalMove()
    {
        ChessGame.Core.Game.ChessGame game =
            new ChessGame.Core.Game.ChessGame();

        Move? move =
            ChessAI.GetRandomMove(
                game,
                PieceColor.White
            );

        Assert.NotNull(move);
    }
}