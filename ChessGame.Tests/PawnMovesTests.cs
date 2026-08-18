using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class PawnMovesTests
{
    [Fact]
    public void WhitePawn_ShouldMoveOneSquareForward()
    {
        var board = new Board();

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(
            new Position(5, 4),
            moves
        );
    }

    [Fact]
    public void BlackPawn_ShouldMoveOneSquareForward()
    {
        var board = new Board();

        var position = new Position(1, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.Black
        );

        Assert.Contains(
            new Position(2, 4),
            moves
        );
    }

    [Fact]
    public void WhitePawn_ShouldMoveTwoSquaresFromStartingPosition()
    {
        var board = new Board();

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(
            new Position(4, 4),
            moves
        );
    }

    [Fact]
    public void BlackPawn_ShouldMoveTwoSquaresFromStartingPosition()
    {
        var board = new Board();

        var position = new Position(1, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.Black
        );

        Assert.Contains(
            new Position(3, 4),
            moves
        );
    }

    [Fact]
    public void Pawn_ShouldNotMoveForwardIntoOccupiedSquare()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 4),
            new Piece(PieceType.Knight, PieceColor.Black)
        );

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(
            new Position(5, 4),
            moves
        );
    }

    [Fact]
    public void Pawn_ShouldCaptureEnemyPieceDiagonally()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 5),
            new Piece(PieceType.Knight, PieceColor.Black)
        );

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.Contains(
            new Position(5, 5),
            moves
        );
    }

    [Fact]
    public void Pawn_ShouldNotCaptureFriendlyPiece()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 5),
            new Piece(PieceType.Knight, PieceColor.White)
        );

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(
            new Position(5, 5),
            moves
        );
    }

    [Fact]
    public void Pawn_ShouldNotMoveTwoSquaresIfBlocked()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 4),
            new Piece(PieceType.Knight, PieceColor.Black)
        );

        var position = new Position(6, 4);

        var moves = PawnMoves.GetPossibleMoves(
            board,
            position,
            PieceColor.White
        );

        Assert.DoesNotContain(
            new Position(4, 4),
            moves
        );
    }
}