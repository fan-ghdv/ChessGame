using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class UndoCastlingTests
{
    [Fact]
    public void UndoMove_ShouldRestoreKingSideCastling()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // White king
        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        // White rook
        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        // Black king
        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        game.Board.SetPiece(
            new Position(7, 4),
            king
        );

        game.Board.SetPiece(
            new Position(7, 7),
            rook
        );

        game.SetSideToMove(
            PieceColor.White
        );

        // White king-side castle:
        // e1 -> g1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 4),
                    new Position(7, 6)
                ),
                PieceColor.White
            )
        );

        // Verify castle happened.
        Assert.Same(
            king,
            game.Board.GetPiece(
                new Position(7, 6)
            )
        );

        Assert.Same(
            rook,
            game.Board.GetPiece(
                new Position(7, 5)
            )
        );

        Assert.True(
            king.HasMoved
        );

        Assert.True(
            rook.HasMoved
        );

        // Undo.
        Assert.True(
            game.UndoMove()
        );

        // King returns to e1.
        Assert.Same(
            king,
            game.Board.GetPiece(
                new Position(7, 4)
            )
        );

        // Rook returns to h1.
        Assert.Same(
            rook,
            game.Board.GetPiece(
                new Position(7, 7)
            )
        );

        // Castle squares become empty.
        Assert.Null(
            game.Board.GetPiece(
                new Position(7, 6)
            )
        );

        Assert.Null(
            game.Board.GetPiece(
                new Position(7, 5)
            )
        );

        // HasMoved must be restored.
        Assert.False(
            king.HasMoved
        );

        Assert.False(
            rook.HasMoved
        );

        // Game state restored.
        Assert.Equal(
            PieceColor.White,
            game.SideToMove
        );

        Assert.Null(
            game.LastMove
        );

        Assert.Equal(
            0,
            game.MoveHistory.Count
        );
    }

    [Fact]
    public void UndoMove_ShouldRestoreQueenSideCastling()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // White king
        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        // White rook
        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        // Black king
        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        game.Board.SetPiece(
            new Position(7, 4),
            king
        );

        game.Board.SetPiece(
            new Position(7, 0),
            rook
        );

        game.SetSideToMove(
            PieceColor.White
        );

        // White queen-side castle:
        // e1 -> c1
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(7, 4),
                    new Position(7, 2)
                ),
                PieceColor.White
            )
        );

        // Verify castle happened.
        Assert.Same(
            king,
            game.Board.GetPiece(
                new Position(7, 2)
            )
        );

        Assert.Same(
            rook,
            game.Board.GetPiece(
                new Position(7, 3)
            )
        );

        Assert.True(
            king.HasMoved
        );

        Assert.True(
            rook.HasMoved
        );

        // Undo.
        Assert.True(
            game.UndoMove()
        );

        // King returns to e1.
        Assert.Same(
            king,
            game.Board.GetPiece(
                new Position(7, 4)
            )
        );

        // Rook returns to a1.
        Assert.Same(
            rook,
            game.Board.GetPiece(
                new Position(7, 0)
            )
        );

        // Castle squares become empty.
        Assert.Null(
            game.Board.GetPiece(
                new Position(7, 2)
            )
        );

        Assert.Null(
            game.Board.GetPiece(
                new Position(7, 3)
            )
        );

        // HasMoved must be restored.
        Assert.False(
            king.HasMoved
        );

        Assert.False(
            rook.HasMoved
        );

        // Game state restored.
        Assert.Equal(
            PieceColor.White,
            game.SideToMove
        );

        Assert.Null(
            game.LastMove
        );

        Assert.Equal(
            0,
            game.MoveHistory.Count
        );
    }
}