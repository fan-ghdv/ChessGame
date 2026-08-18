using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class MovementBlockingTests
{
    [Fact]
    public void Bishop_CannotMoveThroughPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(PieceType.Bishop, PieceColor.White)
        );

        board.SetPiece(
            new Position(3, 3),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        var move = new Move(
            new Position(4, 4),
            new Position(2, 2)
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
    public void Queen_CannotMoveThroughPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(PieceType.Queen, PieceColor.White)
        );

        board.SetPiece(
            new Position(4, 5),
            new Piece(PieceType.Pawn, PieceColor.White)
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

    [Fact]
    public void Knight_CanJumpOverPieces()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(PieceType.Knight, PieceColor.White)
        );

        // Surround the Knight with pieces.
        board.SetPiece(
            new Position(3, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(4, 3),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(4, 5),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        var move = new Move(
            new Position(4, 4),
            new Position(2, 3)
        );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }
}