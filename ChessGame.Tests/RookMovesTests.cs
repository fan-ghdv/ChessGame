using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class RookMovesTests
{
    [Fact]
    public void Rook_ShouldMoveVertically()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 4), moves);
        Assert.Contains(new Position(2, 4), moves);
        Assert.Contains(new Position(5, 4), moves);
        Assert.Contains(new Position(6, 4), moves);
    }

    [Fact]
    public void Rook_ShouldMoveHorizontally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 3), moves);
        Assert.Contains(new Position(4, 2), moves);
        Assert.Contains(new Position(4, 5), moves);
        Assert.Contains(new Position(4, 6), moves);
    }

    [Fact]
    public void Rook_ShouldNotMoveDiagonally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(new Position(3, 3), moves);
        Assert.DoesNotContain(new Position(3, 5), moves);
        Assert.DoesNotContain(new Position(5, 3), moves);
        Assert.DoesNotContain(new Position(5, 5), moves);
    }

    [Fact]
    public void Rook_ShouldStopBeforeFriendlyPiece()
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

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 5), moves);
        Assert.DoesNotContain(new Position(4, 6), moves);
        Assert.DoesNotContain(new Position(4, 7), moves);
    }

    [Fact]
    public void Rook_ShouldCaptureEnemyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 6),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(4, 5), moves);
        Assert.Contains(new Position(4, 6), moves);
        Assert.DoesNotContain(new Position(4, 7), moves);
    }

    [Fact]
    public void Rook_ShouldNotJumpOverPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var position = new Position(4, 4);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 4), moves);
        Assert.Contains(new Position(2, 4), moves);
        Assert.DoesNotContain(new Position(1, 4), moves);
        Assert.DoesNotContain(new Position(0, 4), moves);
    }

    [Fact]
    public void Rook_ShouldReachBoardEdges()
    {
        var board = new Board();

        var position = new Position(0, 0);

        var moves = RookMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(0, 7), moves);
        Assert.Contains(new Position(7, 0), moves);
    }
}