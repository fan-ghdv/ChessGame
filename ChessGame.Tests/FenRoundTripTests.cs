using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class FenRoundTripTests
{
    [Fact]
    public void InitialPosition_ShouldRoundTrip()
    {
        var originalGame =
            new ChessGameGame();

        string originalFen =
            FenGenerator.Generate(
                originalGame.Board,
                originalGame.SideToMove,
                originalGame.LastMove
            );

        var loadedGame =
            ChessGameGame.FromFen(originalFen);

        string generatedFen =
            FenGenerator.Generate(
                loadedGame.Board,
                loadedGame.SideToMove,
                loadedGame.LastMove
            );

        Assert.Equal(
            originalFen,
            generatedFen
        );
    }

    [Fact]
    public void CustomPosition_ShouldRoundTrip()
    {
        string originalFen =
            "r3k2r/pppq1ppp/2np1n2/8/8/2NP1N2/PPPQ1PPP/R3K2R w KQkq - 12 8";

        var game =
            ChessGameGame.FromFen(originalFen);

        string generatedFen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.Equal(
            originalFen,
            generatedFen
        );
    }

    [Fact]
    public void BlackToMove_ShouldRoundTrip()
    {
        string originalFen =
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1";

        var game =
            ChessGameGame.FromFen(originalFen);

        string generatedFen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.Equal(
            originalFen,
            generatedFen
        );
    }

    [Fact]
    public void NoCastlingRights_ShouldRoundTrip()
    {
        string originalFen =
            "4k3/8/8/8/8/8/8/4K3 w - - 25 42";

        var game =
            ChessGameGame.FromFen(originalFen);

        string generatedFen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove,
                game.LastMove
            );

        Assert.Equal(
            originalFen,
            generatedFen
        );
    }
}