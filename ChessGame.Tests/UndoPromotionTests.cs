using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class UndoPromotionTests
{
    [Fact]
    public void UndoMove_ShouldRestorePawnAfterQueenPromotion()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // White king
        game.Board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black king
        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White pawn one step before promotion.
        var pawn =
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            );

        game.Board.SetPiece(
            new Position(1, 0),
            pawn
        );

        game.SetSideToMove(
            PieceColor.White
        );

        // Promote a8 -> Queen.
        var move =
            new Move(
                new Position(1, 0),
                new Position(0, 0)
            );

        Assert.True(
            game.TryMove(
                move,
                PieceColor.White,
                PieceType.Queen
            )
        );

        // The pawn should now be a Queen.
        Piece? promotedPiece =
            game.Board.GetPiece(
                new Position(0, 0)
            );

        Assert.NotNull(promotedPiece);

        Assert.Equal(
            PieceType.Queen,
            promotedPiece!.Type
        );

        Assert.Equal(
            PieceColor.White,
            promotedPiece.Color
        );

        // Undo promotion.
        Assert.True(
            game.UndoMove()
        );

        // Pawn should return.
        Piece? restoredPawn =
            game.Board.GetPiece(
                new Position(1, 0)
            );

        Assert.NotNull(restoredPawn);

        Assert.Same(
            pawn,
            restoredPawn
        );

        Assert.Equal(
            PieceType.Pawn,
            restoredPawn!.Type
        );

        Assert.Equal(
            PieceColor.White,
            restoredPawn.Color
        );

        Assert.False(
            restoredPawn.HasMoved
        );

        // Promotion square must be empty.
        Assert.Null(
            game.Board.GetPiece(
                new Position(0, 0)
            )
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