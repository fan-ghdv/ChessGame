using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CaptureTests
{
    [Fact]
    public void Rook_CanCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(4, 7),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(4, 7)
        );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Bishop_CanCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(2, 2),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(2, 2)
        );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Knight_CanCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(2, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(2, 5)
        );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Pawn_CanCaptureEnemyPieceDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(3, 5)
        );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Pawn_CannotCaptureStraightAhead()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(3, 4)
        );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Piece_CannotCaptureFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(4, 7),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(4, 7)
        );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }
}