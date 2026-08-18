using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class AttackDetectorTests
{
    [Fact]
    public void Pawn_ShouldAttackDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 3),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 5),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Pawn_ShouldNotAttackStraightAhead()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        Assert.False(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 4),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Knight_ShouldAttackInLShape()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(2, 3),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 2),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Bishop_ShouldAttackDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(2, 2),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Bishop_ShouldBeBlockedByPiece()
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
            new Position(3, 3),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        Assert.False(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(2, 2),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Rook_ShouldAttackStraight()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(4, 7),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(0, 4),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Rook_ShouldBeBlockedByPiece()
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

        Assert.False(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(4, 7),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Queen_ShouldAttackStraightAndDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(4, 7),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(1, 1),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_ShouldAttackAdjacentSquares()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 3),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 4),
                PieceColor.White
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(5, 5),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_ShouldNotAttackFarAwaySquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        Assert.False(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(2, 4),
                PieceColor.White
            )
        );
    }

    [Fact]
    public void EnemyKing_ShouldAttackAdjacentSquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        Assert.True(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(3, 4),
                PieceColor.Black
            )
        );
    }

    [Fact]
    public void EmptyBoard_ShouldHaveNoAttacks()
    {
        var board = new Board();

        Assert.False(
            AttackDetector.IsSquareAttacked(
                board,
                new Position(4, 4),
                PieceColor.White
            )
        );
    }
}