using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class MoveExecutorTests
{
    [Fact]
    public void KingSideCastling_ShouldMoveKingAndRook()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 7),
            rook
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 6)
            );

        Assert.True(
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            )
        );

        // King should be on (7,6)
        Assert.Same(
            king,
            board.GetPiece(
                new Position(7, 6)
            )
        );

        // Rook should be on (7,5)
        Assert.Same(
            rook,
            board.GetPiece(
                new Position(7, 5)
            )
        );

        // Original squares should be empty.
        Assert.Null(
            board.GetPiece(
                new Position(7, 4)
            )
        );

        Assert.Null(
            board.GetPiece(
                new Position(7, 7)
            )
        );

        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
    }

    [Fact]
    public void QueenSideCastling_ShouldMoveKingAndRook()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 0),
            rook
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 2)
            );

        Assert.True(
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            )
        );

        // King should be on (7,2)
        Assert.Same(
            king,
            board.GetPiece(
                new Position(7, 2)
            )
        );

        // Rook should be on (7,3)
        Assert.Same(
            rook,
            board.GetPiece(
                new Position(7, 3)
            )
        );

        // Original squares should be empty.
        Assert.Null(
            board.GetPiece(
                new Position(7, 4)
            )
        );

        Assert.Null(
            board.GetPiece(
                new Position(7, 0)
            )
        );

        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
    }

    [Fact]
    public void KingSideCastling_WhenIllegal_ShouldNotMovePieces()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 7),
            rook
        );

        // Black rook attacks the king's
        // destination square (7,6).
        board.SetPiece(
            new Position(5, 6),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 6)
            );

        Assert.False(
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            )
        );

        // Nothing should have moved.
        Assert.Same(
            king,
            board.GetPiece(
                new Position(7, 4)
            )
        );

        Assert.Same(
            rook,
            board.GetPiece(
                new Position(7, 7)
            )
        );

        Assert.Null(
            board.GetPiece(
                new Position(7, 6)
            )
        );

        Assert.False(king.HasMoved);
        Assert.False(rook.HasMoved);
    }

    [Fact]
    public void QueenSideCastling_WhenIllegal_ShouldNotMovePieces()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 0),
            rook
        );

        // Black rook attacks the king's
        // destination square (7,2).
        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 2)
            );

        Assert.False(
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            )
        );

        Assert.Same(
            king,
            board.GetPiece(
                new Position(7, 4)
            )
        );

        Assert.Same(
            rook,
            board.GetPiece(
                new Position(7, 0)
            )
        );

        Assert.Null(
            board.GetPiece(
                new Position(7, 2)
            )
        );

        Assert.False(king.HasMoved);
        Assert.False(rook.HasMoved);
    }
}