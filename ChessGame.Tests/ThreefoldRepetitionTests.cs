using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class ThreefoldRepetitionTests
{
    [Fact]
    public void InitialPosition_ShouldBeRecordedOnce()
    {
        var game = new ChessGameGame();

        Assert.Equal(
            1,
            game.GetCurrentPositionCount()
        );

        Assert.False(
            game.IsThreefoldRepetition()
        );
    }

    [Fact]
    public void SamePositionRecordedThreeTimes_ShouldBeThreefoldRepetition()
    {
        var game = new ChessGameGame();

        // First cycle.

        // White Knight: b1 -> c3
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 1),
                    new Position(5, 2)
                ),
                PieceColor.White
            )
        );

        // Black Knight: b8 -> c6
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(0, 1),
                    new Position(2, 2)
                ),
                PieceColor.Black
            )
        );

        // White Knight: c3 -> b1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(5, 2),
                    new Position(7, 1)
                ),
                PieceColor.White
            )
        );

        // Black Knight: c6 -> b8
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(2, 2),
                    new Position(0, 1)
                ),
                PieceColor.Black
            )
        );

        // Initial position has occurred twice.
        Assert.Equal(
            2,
            game.GetCurrentPositionCount()
        );

        Assert.False(
            game.IsThreefoldRepetition()
        );

        // Second cycle.

        // White Knight: b1 -> c3
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 1),
                    new Position(5, 2)
                ),
                PieceColor.White
            )
        );

        // Black Knight: b8 -> c6
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(0, 1),
                    new Position(2, 2)
                ),
                PieceColor.Black
            )
        );

        // White Knight: c3 -> b1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(5, 2),
                    new Position(7, 1)
                ),
                PieceColor.White
            )
        );

        // Black Knight: c6 -> b8
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(2, 2),
                    new Position(0, 1)
                ),
                PieceColor.Black
            )
        );

        // Initial position has occurred three times.
        Assert.Equal(
            3,
            game.GetCurrentPositionCount()
        );

        Assert.True(
            game.IsThreefoldRepetition()
        );
    }

    [Fact]
    public void DifferentPosition_ShouldNotBeThreefoldRepetition()
    {
        var game = new ChessGameGame();

        Assert.False(
            game.IsThreefoldRepetition()
        );

        // White Knight: b1 -> c3
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 1),
                    new Position(5, 2)
                ),
                PieceColor.White
            )
        );

        Assert.False(
            game.IsThreefoldRepetition()
        );

        Assert.Equal(
            1,
            game.GetCurrentPositionCount()
        );
    }

    [Fact]
    public void IllegalMove_ShouldNotRecordNewPosition()
    {
        var game = new ChessGameGame();

        Assert.Equal(
            1,
            game.GetCurrentPositionCount()
        );

        // White pawn e2 -> e5 is illegal.
        Assert.False(
            game.TryMove(
                new Move(
                    new Position(6, 4),
                    new Position(3, 4)
                ),
                PieceColor.White
            )
        );

        // Illegal moves must not create
        // a new position-history entry.
        Assert.Equal(
            1,
            game.GetCurrentPositionCount()
        );

        Assert.False(
            game.IsThreefoldRepetition()
        );
    }
}