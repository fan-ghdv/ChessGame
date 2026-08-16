using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class MoveTests
{
    [Fact]
    public void Move_ShouldStoreFromAndToPositions()
    {
        var from = new Position(6, 4);
        var to = new Position(5, 4);

        var move = new Move(from, to);

        Assert.Equal(from, move.From);
        Assert.Equal(to, move.To);
    }
}