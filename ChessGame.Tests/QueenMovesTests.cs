using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class QueenMovesTests
{
    [Fact]
    public void Queen_ShouldMoveHorizontally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 0), moves);
        Assert.Contains(new Position(4, 1), moves);
        Assert.Contains(new Position(4, 2), moves);
        Assert.Contains(new Position(4, 3), moves);

        Assert.Contains(new Position(4, 5), moves);
        Assert.Contains(new Position(4, 6), moves);
        Assert.Contains(new Position(4, 7), moves);
    }

    [Fact]
    public void Queen_ShouldMoveVertically()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(0, 4), moves);
        Assert.Contains(new Position(1, 4), moves);
        Assert.Contains(new Position(2, 4), moves);
        Assert.Contains(new Position(3, 4), moves);

        Assert.Contains(new Position(5, 4), moves);
        Assert.Contains(new Position(6, 4), moves);
        Assert.Contains(new Position(7, 4), moves);
    }

    [Fact]
    public void Queen_ShouldMoveDiagonally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);
        Assert.Contains(new Position(2, 2), moves);
        Assert.Contains(new Position(1, 1), moves);
        Assert.Contains(new Position(0, 0), moves);

        Assert.Contains(new Position(3, 5), moves);
        Assert.Contains(new Position(2, 6), moves);
        Assert.Contains(new Position(1, 7), moves);

        Assert.Contains(new Position(5, 3), moves);
        Assert.Contains(new Position(6, 2), moves);
        Assert.Contains(new Position(7, 1), moves);

        Assert.Contains(new Position(5, 5), moves);
        Assert.Contains(new Position(6, 6), moves);
        Assert.Contains(new Position(7, 7), moves);
    }

    [Fact]
    public void Queen_ShouldCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 7),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 5), moves);
        Assert.Contains(new Position(4, 6), moves);
        Assert.Contains(new Position(4, 7), moves);

        // Cannot move beyond captured piece.
        Assert.DoesNotContain(new Position(4, 8), moves);
    }

    [Fact]
    public void Queen_ShouldNotCaptureFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 6),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 5), moves);

        Assert.DoesNotContain(new Position(4, 6), moves);
        Assert.DoesNotContain(new Position(4, 7), moves);
    }

    [Fact]
    public void Queen_ShouldNotJumpOverPieces()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 2),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);
        Assert.Contains(new Position(2, 2), moves);

        Assert.DoesNotContain(new Position(1, 1), moves);
        Assert.DoesNotContain(new Position(0, 0), moves);
    }

    [Fact]
    public void Queen_ShouldHaveEightDirections()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = QueenMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        // Up
        Assert.Contains(new Position(3, 4), moves);

        // Down
        Assert.Contains(new Position(5, 4), moves);

        // Left
        Assert.Contains(new Position(4, 3), moves);

        // Right
        Assert.Contains(new Position(4, 5), moves);

        // Diagonals
        Assert.Contains(new Position(3, 3), moves);
        Assert.Contains(new Position(3, 5), moves);
        Assert.Contains(new Position(5, 3), moves);
        Assert.Contains(new Position(5, 5), moves);
    }
}