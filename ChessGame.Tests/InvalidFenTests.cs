using ChessGame.Core.Game;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class InvalidFenTests
{
    [Fact]
    public void EmptyFen_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen("")
        );
    }

    [Fact]
    public void MissingFenFields_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/8 w - -"
            )
        );
    }

    [Fact]
    public void InvalidActiveColor_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/8 x - - 0 1"
            )
        );
    }

    [Fact]
    public void InvalidPieceCharacter_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "4x3/8/8/8/8/8/8/4K3 w - - 0 1"
            )
        );
    }

    [Fact]
    public void SevenRanks_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8 w - - 0 1"
            )
        );
    }

    [Fact]
    public void InvalidRankLength_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "9/8/8/8/8/8/8/4K3 w - - 0 1"
            )
        );
    }

    [Fact]
    public void NegativeHalfmoveClock_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/4K3 w - - -1 1"
            )
        );
    }

    [Fact]
    public void ZeroFullmoveNumber_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/4K3 w - - 0 0"
            )
        );
    }

    [Fact]
    public void InvalidHalfmoveClock_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/4K3 w - - abc 1"
            )
        );
    }

    [Fact]
    public void InvalidFullmoveNumber_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => ChessGameGame.FromFen(
                "8/8/8/8/8/8/8/4K3 w - - 0 abc"
            )
        );
    }
}