using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class KingMovesTests
{
    [Fact]
    public void King_ShouldMoveOneSquareInAllDirections()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);
        Assert.Contains(new Position(3, 4), moves);
        Assert.Contains(new Position(3, 5), moves);

        Assert.Contains(new Position(4, 3), moves);
        Assert.Contains(new Position(4, 5), moves);

        Assert.Contains(new Position(5, 3), moves);
        Assert.Contains(new Position(5, 4), moves);
        Assert.Contains(new Position(5, 5), moves);
    }

    [Fact]
    public void King_ShouldHaveAtMostEightMoves()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(8, moves.Count);
    }

    [Fact]
    public void King_ShouldNotMoveOutsideBoard()
    {
        var board = new Board();

        var position = new Position(0, 0);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(0, 1), moves);
        Assert.Contains(new Position(1, 0), moves);
        Assert.Contains(new Position(1, 1), moves);

        Assert.Equal(3, moves.Count);
    }

    [Fact]
    public void King_ShouldCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);
    }

    [Fact]
    public void King_ShouldNotCaptureFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var position = new Position(4, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(new Position(3, 3), moves);
    }

    [Fact]
    public void King_ShouldNotStayOnSameSquare()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(position, moves);
    }

    [Fact]
    public void King_ShouldHaveFiveMovesFromEdge()
    {
        var board = new Board();

        var position = new Position(0, 4);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(5, moves.Count);
    }

    [Fact]
    public void King_ShouldHaveThreeMovesFromCorner()
    {
        var board = new Board();

        var position = new Position(0, 0);

        var moves = KingMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Equal(3, moves.Count);
    }
}