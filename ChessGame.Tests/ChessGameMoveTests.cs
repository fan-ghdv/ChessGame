using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameModel = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class ChessGameMoveTests
{
    [Fact]
    public void TryMove_ShouldUpdateLastMove()
    {
        var game = new ChessGameModel();

        var move =
            new Move(
                new Position(6, 4),
                new Position(5, 4)
            );

        bool result =
            game.TryMove(
                move,
                PieceColor.White
            );

        Assert.True(result);

        Assert.NotNull(game.LastMove);

        Assert.Equal(
            move.From,
            game.LastMove!.From
        );

        Assert.Equal(
            move.To,
            game.LastMove.To
        );
    }

    [Fact]
    public void TryMove_ShouldExecuteEnPassantUsingLastMove()
    {
        var game = new ChessGameModel();

        // Clear the normal starting position.
        game.Board.Clear();

        // White pawn on f5.
        game.Board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        // Black pawn on e7.
        game.Board.SetPiece(
            new Position(1, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        // Black: e7 -> e5.
        var blackMove =
            new Move(
                new Position(1, 4),
                new Position(3, 4)
            );

        bool blackResult =
            game.TryMove(
                blackMove,
                PieceColor.Black
            );

        Assert.True(blackResult);

        // White: f5 -> e6.
        var whiteMove =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        bool whiteResult =
            game.TryMove(
                whiteMove,
                PieceColor.White
            );

        Assert.True(whiteResult);

        // White pawn should now be on e6.
        Piece? pawn =
            game.Board.GetPiece(
                new Position(2, 4)
            );

        Assert.NotNull(pawn);

        Assert.Equal(
            PieceType.Pawn,
            pawn!.Type
        );

        Assert.Equal(
            PieceColor.White,
            pawn.Color
        );

        // e5 must now be empty.
        Assert.Null(
            game.Board.GetPiece(
                new Position(3, 4)
            )
        );

        // LastMove should now be White's move.
        Assert.NotNull(game.LastMove);

        Assert.Equal(
            whiteMove.From,
            game.LastMove!.From
        );

        Assert.Equal(
            whiteMove.To,
            game.LastMove.To
        );
    }

    [Fact]
    public void InvalidMove_ShouldNotUpdateLastMove()
    {
        var game = new ChessGameModel();

        var invalidMove =
            new Move(
                new Position(6, 0),
                new Position(4, 1)
            );

        bool result =
            game.TryMove(
                invalidMove,
                PieceColor.White
            );

        Assert.False(result);

        Assert.Null(
            game.LastMove
        );
    }
}