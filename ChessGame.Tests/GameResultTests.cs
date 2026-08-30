using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class GameResultTests
{
    [Fact]
    public void NewGame_ShouldBeOngoing()
    {
        var game = new ChessGameGame();

        Assert.Equal(
            GameResult.Ongoing,
            game.GetGameResult(
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KingVsKing_ShouldBeInsufficientMaterial()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        game.Board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        Assert.Equal(
            GameResult.InsufficientMaterial,
            game.GetGameResult(
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KingAndRookVsKing_ShouldBeOngoing()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        game.Board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        game.Board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        Assert.Equal(
            GameResult.Ongoing,
            game.GetGameResult(
                PieceColor.White
            )
        );
    }

    [Fact]
    public void ThreefoldRepetition_ShouldReturnThreefoldRepetition()
    {
        var game = new ChessGameGame();

        // =====================================================
        // FIRST CYCLE
        // =====================================================

        // White: b1 -> c3
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 1),
                    new Position(5, 2)
                ),
                PieceColor.White
            )
        );

        // Black: b8 -> c6
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(0, 1),
                    new Position(2, 2)
                ),
                PieceColor.Black
            )
        );

        // White: c3 -> b1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(5, 2),
                    new Position(7, 1)
                ),
                PieceColor.White
            )
        );

        // Black: c6 -> b8
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(2, 2),
                    new Position(0, 1)
                ),
                PieceColor.Black
            )
        );

        // =====================================================
        // SECOND CYCLE
        // =====================================================

        // White: b1 -> c3
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 1),
                    new Position(5, 2)
                ),
                PieceColor.White
            )
        );

        // Black: b8 -> c6
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(0, 1),
                    new Position(2, 2)
                ),
                PieceColor.Black
            )
        );

        // White: c3 -> b1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(5, 2),
                    new Position(7, 1)
                ),
                PieceColor.White
            )
        );

        // Black: c6 -> b8
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(2, 2),
                    new Position(0, 1)
                ),
                PieceColor.Black
            )
        );

        Assert.Equal(
            3,
            game.GetCurrentPositionCount()
        );

        Assert.True(
            game.IsThreefoldRepetition()
        );

        Assert.Equal(
            GameResult.ThreefoldRepetition,
            game.GetGameResult(
                PieceColor.White
            )
        );
    }
}