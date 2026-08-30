using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class UndoMoveTests
{
    [Fact]
    public void UndoMove_ShouldRestoreSimpleMove()
    {
        var game = new ChessGameGame();

        var move =
            new Move(
                new Position(6, 4),
                new Position(5, 4)
            );

        Assert.True(
            game.TryMove(
                move,
                PieceColor.White
            )
        );

        Assert.Null(
            game.Board.GetPiece(
                new Position(6, 4)
            )
        );

        Assert.NotNull(
            game.Board.GetPiece(
                new Position(5, 4)
            )
        );

        Assert.True(
            game.UndoMove()
        );

        Piece? pawn =
            game.Board.GetPiece(
                new Position(6, 4)
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

        Assert.Null(
            game.Board.GetPiece(
                new Position(5, 4)
            )
        );

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
    public void UndoMove_ShouldRestoreCapturedPiece()
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

        game.Board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        game.Board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        game.SetSideToMove(
            PieceColor.White
        );

        var move =
            new Move(
                new Position(5, 4),
                new Position(3, 4)
            );

        Assert.True(
            game.TryMove(
                move,
                PieceColor.White
            )
        );

        Assert.True(
            game.UndoMove()
        );

        Piece? rook =
            game.Board.GetPiece(
                new Position(5, 4)
            );

        Piece? pawn =
            game.Board.GetPiece(
                new Position(3, 4)
            );

        Assert.NotNull(rook);
        Assert.Equal(
            PieceType.Rook,
            rook!.Type
        );

        Assert.NotNull(pawn);
        Assert.Equal(
            PieceType.Pawn,
            pawn!.Type
        );

        Assert.Equal(
            PieceColor.Black,
            pawn.Color
        );
    }

    [Fact]
    public void UndoMove_WithNoHistory_ShouldReturnFalse()
    {
        var game = new ChessGameGame();

        Assert.False(
            game.UndoMove()
        );
    }

    [Fact]
    public void UndoMove_ShouldRestoreEnPassant()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // White king.
        game.Board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black king.
        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

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

        game.SetSideToMove(
            PieceColor.Black
        );

        // e7 -> e5.
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(1, 4),
                    new Position(3, 4)
                ),
                PieceColor.Black
            )
        );

        // f5 -> e6 en passant.
        Assert.True(
            game.TryMove(
                new Move(
                    new Position(3, 5),
                    new Position(2, 4)
                ),
                PieceColor.White
            )
        );

        // Undo en passant.
        Assert.True(
            game.UndoMove()
        );

        // White pawn returns to f5.
        Piece? whitePawn =
            game.Board.GetPiece(
                new Position(3, 5)
            );

        Assert.NotNull(whitePawn);

        Assert.Equal(
            PieceType.Pawn,
            whitePawn!.Type
        );

        Assert.Equal(
            PieceColor.White,
            whitePawn.Color
        );

        // Captured black pawn returns to e5.
        Piece? blackPawn =
            game.Board.GetPiece(
                new Position(3, 4)
            );

        Assert.NotNull(blackPawn);

        Assert.Equal(
            PieceType.Pawn,
            blackPawn!.Type
        );

        Assert.Equal(
            PieceColor.Black,
            blackPawn.Color
        );

        // e6 must be empty.
        Assert.Null(
            game.Board.GetPiece(
                new Position(2, 4)
            )
        );

        // It should be White's turn again.
        Assert.Equal(
            PieceColor.White,
            game.SideToMove
        );
    }
}