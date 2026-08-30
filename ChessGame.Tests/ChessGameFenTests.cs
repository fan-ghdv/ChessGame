using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class ChessGameFenTests
{
    [Fact]
    public void FromFen_ShouldLoadBoard()
    {
        string fen =
            "4k3/8/8/8/8/8/8/4K3 w - - 0 1";

        ChessGameGame game =
            ChessGameGame.FromFen(fen);

        Piece? whiteKing =
            game.Board.GetPiece(
                new Position(7, 4)
            );

        Piece? blackKing =
            game.Board.GetPiece(
                new Position(0, 4)
            );

        Assert.NotNull(whiteKing);
        Assert.NotNull(blackKing);

        Assert.Equal(
            PieceType.King,
            whiteKing!.Type
        );

        Assert.Equal(
            PieceColor.White,
            whiteKing.Color
        );

        Assert.Equal(
            PieceColor.Black,
            blackKing!.Color
        );
    }

    [Fact]
    public void FromFen_ShouldSetSideToMove()
    {
        string fen =
            "4k3/8/8/8/8/8/8/4K3 b - - 0 1";

        ChessGameGame game =
            ChessGameGame.FromFen(fen);

        Assert.Equal(
            PieceColor.Black,
            game.SideToMove
        );
    }

    [Fact]
    public void FromFen_ShouldRestoreHalfmoveClock()
    {
        string fen =
            "4k3/8/8/8/8/8/8/4K3 w - - 37 12";

        ChessGameGame game =
            ChessGameGame.FromFen(fen);

        Assert.Equal(
            37,
            game.Board.HalfmoveClock
        );
    }

    [Fact]
    public void FromFen_ShouldStartWithEmptyMoveHistory()
    {
        string fen =
            "4k3/8/8/8/8/8/8/4K3 w - - 0 1";

        ChessGameGame game =
            ChessGameGame.FromFen(fen);

        Assert.Equal(
            0,
            game.MoveHistory.Count
        );

        Assert.Null(
            game.LastMove
        );
    }

    [Fact]
    public void FromFen_ShouldRestoreCastlingPieceStates()
    {
        string fen =
            "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";

        ChessGameGame game =
            ChessGameGame.FromFen(fen);

        Piece whiteKing =
            game.Board.GetPiece(
                new Position(7, 4)
            )!;

        Piece whiteRookA =
            game.Board.GetPiece(
                new Position(7, 0)
            )!;

        Piece whiteRookH =
            game.Board.GetPiece(
                new Position(7, 7)
            )!;

        Piece blackKing =
            game.Board.GetPiece(
                new Position(0, 4)
            )!;

        Assert.False(
            whiteKing.HasMoved
        );

        Assert.False(
            whiteRookA.HasMoved
        );

        Assert.False(
            whiteRookH.HasMoved
        );

        Assert.False(
            blackKing.HasMoved
        );
    }
}