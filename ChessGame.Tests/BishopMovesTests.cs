using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class BishopMovesTests
{
    [Fact]
    public void Bishop_ShouldMoveDiagonally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = BishopMoves.GetPossibleMoves(
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
    public void Bishop_ShouldNotMoveHorizontally()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(new Position(4, 0), moves);
        Assert.DoesNotContain(new Position(4, 1), moves);
        Assert.DoesNotContain(new Position(4, 3), moves);
        Assert.DoesNotContain(new Position(4, 5), moves);
        Assert.DoesNotContain(new Position(4, 7), moves);
    }

    [Fact]
    public void Bishop_ShouldNotMoveVertically()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(new Position(0, 4), moves);
        Assert.DoesNotContain(new Position(1, 4), moves);
        Assert.DoesNotContain(new Position(3, 4), moves);
        Assert.DoesNotContain(new Position(5, 4), moves);
        Assert.DoesNotContain(new Position(7, 4), moves);
    }

    [Fact]
    public void Bishop_ShouldCaptureEnemyPiece()
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

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);
        Assert.Contains(new Position(2, 2), moves);

        // Bishop cannot continue through the captured piece.
        Assert.DoesNotContain(new Position(1, 1), moves);
        Assert.DoesNotContain(new Position(0, 0), moves);
    }

    [Fact]
    public void Bishop_ShouldStopBeforeFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(2, 2),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var position = new Position(4, 4);

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(3, 3), moves);

        // Cannot capture or move through friendly piece.
        Assert.DoesNotContain(new Position(2, 2), moves);
        Assert.DoesNotContain(new Position(1, 1), moves);
        Assert.DoesNotContain(new Position(0, 0), moves);
    }

    [Fact]
    public void Bishop_ShouldNotJumpOverPieces()
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

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        // Can capture the first piece.
        Assert.Contains(new Position(3, 3), moves);

        // Cannot jump over it.
        Assert.DoesNotContain(new Position(2, 2), moves);
        Assert.DoesNotContain(new Position(1, 1), moves);
        Assert.DoesNotContain(new Position(0, 0), moves);
    }

    [Fact]
    public void Bishop_ShouldStayOnSameColorSquares()
    {
        var board = new Board();

        var position = new Position(4, 4);

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        foreach (var move in moves)
        {
            int startingColor = (position.Row + position.Column) % 2;
            int targetColor = (move.Row + move.Column) % 2;

            Assert.Equal(startingColor, targetColor);
        }
    }

    [Fact]
    public void Bishop_ShouldReachBoardEdges()
    {
        var board = new Board();

        var position = new Position(0, 0);

        var moves = BishopMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(new Position(7, 7), moves);

        Assert.Equal(7, moves.Count);
    }
}