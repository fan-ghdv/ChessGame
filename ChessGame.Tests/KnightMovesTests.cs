using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class KnightMovesTests
{
    [Fact]
    public void Knight_ShouldHaveEightMovesFromCenter()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(8, moves.Count);

        Assert.Contains(new Position(2, 3), moves);
        Assert.Contains(new Position(2, 5), moves);
        Assert.Contains(new Position(3, 2), moves);
        Assert.Contains(new Position(3, 6), moves);
        Assert.Contains(new Position(5, 2), moves);
        Assert.Contains(new Position(5, 6), moves);
        Assert.Contains(new Position(6, 3), moves);
        Assert.Contains(new Position(6, 5), moves);
    }

    [Fact]
    public void Knight_ShouldHaveTwoMovesFromCorner()
    {
        var board = new Board();

        var position = new Position(0, 0);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(2, moves.Count);

        Assert.Contains(new Position(1, 2), moves);
        Assert.Contains(new Position(2, 1), moves);
    }

    [Fact]
    public void Knight_ShouldCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(
            new Position(2, 3),
            moves
        );
    }

    [Fact]
    public void Knight_ShouldNotCaptureFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var position = new Position(4, 4);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(
            new Position(2, 3),
            moves
        );
    }

    [Fact]
    public void Knight_ShouldJumpOverPieces()
    {
        var board = new Board();

        // Surround the knight with pieces.
        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Pawn,
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

        var position = new Position(4, 4);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(8, moves.Count);
    }

    [Fact]
    public void Knight_ShouldNotMoveOutsideBoard()
    {
        var board = new Board();

        var position = new Position(0, 1);

        var moves = KnightMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(3, moves.Count);

        Assert.Contains(new Position(2, 0), moves);
        Assert.Contains(new Position(2, 2), moves);
        Assert.Contains(new Position(1, 3), moves);
    }
}