using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CastlingExecutionTests
{
    private static Board CreateBoard()
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
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
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
                PieceType.King,
                PieceColor.Black
            )
        );

        return board;
    }

    [Fact]
    public void KingSideCastling_ShouldMoveKingAndRook()
    {
        var board = CreateBoard();

        var move = new Move(
            new Position(7, 4),
            new Position(7, 6)
        );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.True(result);

        // King e1 -> g1
        Piece? king =
            board.GetPiece(
                new Position(7, 6)
            );

        Assert.NotNull(king);
        Assert.Equal(
            PieceType.King,
            king!.Type
        );

        Assert.Equal(
            PieceColor.White,
            king.Color
        );

        // Rook h1 -> f1
        Piece? rook =
            board.GetPiece(
                new Position(7, 5)
            );

        Assert.NotNull(rook);
        Assert.Equal(
            PieceType.Rook,
            rook!.Type
        );

        // Original squares are empty
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
    }

    [Fact]
    public void QueenSideCastling_ShouldMoveKingAndRook()
    {
        var board = CreateBoard();

        var move = new Move(
            new Position(7, 4),
            new Position(7, 2)
        );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.True(result);

        // King e1 -> c1
        Piece? king =
            board.GetPiece(
                new Position(7, 2)
            );

        Assert.NotNull(king);

        Assert.Equal(
            PieceType.King,
            king!.Type
        );

        // Rook a1 -> d1
        Piece? rook =
            board.GetPiece(
                new Position(7, 3)
            );

        Assert.NotNull(rook);

        Assert.Equal(
            PieceType.Rook,
            rook!.Type
        );

        // Original squares are empty
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
    }

    [Fact]
    public void Castling_ShouldMarkKingAndRookAsMoved()
    {
        var board = CreateBoard();

        var move = new Move(
            new Position(7, 4),
            new Position(7, 6)
        );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.True(result);

        Piece king =
            board.GetPiece(
                new Position(7, 6)
            )!;

        Piece rook =
            board.GetPiece(
                new Position(7, 5)
            )!;

        Assert.True(king.HasMoved);
        Assert.True(rook.HasMoved);
    }

    [Fact]
    public void Castling_ShouldNotExecuteWhenPathIsBlocked()
    {
        var board = CreateBoard();

        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        var move = new Move(
            new Position(7, 4),
            new Position(7, 6)
        );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.False(result);

        // King stays on e1
        Assert.NotNull(
            board.GetPiece(
                new Position(7, 4)
            )
        );

        // Rook stays on h1
        Assert.NotNull(
            board.GetPiece(
                new Position(7, 7)
            )
        );
    }
}