using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameModel = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class StockfishRealPromotionTests
{
    [Fact]
    public void Stockfish_ShouldFindPromotionMove()
    {
        string enginePath =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "Engine",
                    "stockfish-windows-x86-64-avx2.exe"
                )
            );

        enginePath =
            Path.GetFullPath(
                enginePath
            );

        Assert.True(
            File.Exists(enginePath),
            $"Stockfish not found: {enginePath}"
        );

        string fen =
            "7k/4P3/8/8/8/8/8/4K3 w - - 0 1";

        ChessGameModel game =
            ChessGameModel.FromFen(fen);

        using StockfishEngine engine =
            new StockfishEngine(
                enginePath
            );

        engine.Initialize();
        engine.WaitUntilReady();

        Move? move =
            engine.GetBestMove(
                game,
                1000
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
            PieceType.Queen,
            engine.LastPromotion
        );
    }
}