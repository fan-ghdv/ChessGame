using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class PawnMovementTests
{
    [Fact]
    public void Pawn_CannotMoveForwardIntoPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(PieceType.Pawn, PieceColor.Black)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(5, 4)
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
    public void Pawn_CannotMoveDiagonallyToEmptySquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(5, 5)
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
    public void Pawn_CanCaptureEnemyPieceDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(5, 5),
            new Piece(PieceType.Pawn, PieceColor.Black)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(5, 5)
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
    public void Pawn_CannotCaptureOwnPieceDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(5, 5),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(5, 5)
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
    public void Pawn_CanMoveTwoSquaresFromStartingPosition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(4, 4)
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
    public void Pawn_CannotMoveTwoSquaresIfBlockedInMiddle()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(PieceType.Pawn, PieceColor.Black)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(4, 4)
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
    public void Pawn_CannotMoveTwoSquaresIfDestinationBlocked()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(PieceType.Pawn, PieceColor.White)
        );

        board.SetPiece(
            new Position(4, 4),
            new Piece(PieceType.Pawn, PieceColor.Black)
        );

        var move = new Move(
            new Position(6, 4),
            new Position(4, 4)
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
    public void WhitePawn_CanMoveTwoSquaresFromStartingPosition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        Assert.True(
            PieceMovement.IsValidMove(
                board,
                new Position(6, 4),
                new Position(4, 4)
            )
        );
    }

    [Fact]
    public void BlackPawn_CanMoveTwoSquaresFromStartingPosition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(1, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        Assert.True(
            PieceMovement.IsValidMove(
                board,
                new Position(1, 4),
                new Position(3, 4)
            )
        );
    }

    [Fact]
    public void WhitePawn_CannotMoveTwoSquaresAfterLeavingStartingPosition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        Assert.False(
            PieceMovement.IsValidMove(
                board,
                new Position(5, 4),
                new Position(3, 4)
            )
        );
    }

    [Fact]
    public void BlackPawn_CannotMoveTwoSquaresAfterLeavingStartingPosition()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        Assert.False(
            PieceMovement.IsValidMove(
                board,
                new Position(2, 4),
                new Position(4, 4)
            )
        );
    }

    [Fact]
    public void Pawn_CannotJumpOverPieceWhenMovingTwoSquares()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.False(
            PieceMovement.IsValidMove(
                board,
                new Position(6, 4),
                new Position(4, 4)
            )
        );
    }

    [Fact]
    public void Pawn_CannotMoveTwoSquaresIntoOccupiedSquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.False(
            PieceMovement.IsValidMove(
                board,
                new Position(6, 4),
                new Position(4, 4)
            )
        );
    }
}