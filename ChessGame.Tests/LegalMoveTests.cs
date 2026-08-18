using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class LegalMoveTests
{
    [Fact]
    public void Piece_CannotMoveToSquareOccupiedByOwnPiece()
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
            new Position(4, 6),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var move = new Move(
            new Position(4, 4),
            new Position(4, 6)
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

        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
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
    public void Rook_CannotMoveThroughPiece()
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
            new Position(4, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.King,
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


    [Fact]
    public void King_CannotMoveIntoEnemyRookAttack()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 4)
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
    public void King_CannotMoveIntoEnemyPawnAttack()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 4)
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
    public void King_CannotMoveIntoEnemyKnightAttack()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(4, 2),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 3)
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
    public void King_CannotMoveIntoEnemyBishopAttack()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(4, 1),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 3)
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
    public void King_CannotMoveIntoEnemyQueenAttack()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 3)
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
    public void King_CannotMoveNextToEnemyKing()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 4)
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
    public void King_CanMoveToSafeSquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(6, 4)
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
    public void King_CannotMakeMove_ThatLeavesItInCheck()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        var move = new Move(
            new Position(7, 0),
            new Position(6, 0)
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