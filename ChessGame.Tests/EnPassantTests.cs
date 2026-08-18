using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class EnPassantTests
{
    [Fact]
    public void WhitePawn_ShouldCaptureEnPassant()
    {
        var board = new Board();

        // White pawn on f5
        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        // Black pawn just moved e7 -> e5
        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var lastMove =
            new Move(
                new Position(1, 4),
                new Position(3, 4)
            );

        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        Assert.True(
            EnPassantValidator.CanCapture(
                board,
                move,
                lastMove,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void EnPassant_ShouldFail_WhenPawnDidNotMoveTwoSquares()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var lastMove =
            new Move(
                new Position(2, 4),
                new Position(3, 4)
            );

        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        Assert.False(
            EnPassantValidator.CanCapture(
                board,
                move,
                lastMove,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void EnPassant_ShouldFail_WhenLastMoveWasNotAdjacentPawn()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var lastMove =
            new Move(
                new Position(1, 3),
                new Position(3, 3)
            );

        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        Assert.False(
            EnPassantValidator.CanCapture(
                board,
                move,
                lastMove,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void EnPassant_ShouldFail_WhenDestinationIsOccupied()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(2, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        var lastMove =
            new Move(
                new Position(1, 4),
                new Position(3, 4)
            );

        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        Assert.False(
            EnPassantValidator.CanCapture(
                board,
                move,
                lastMove,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void ExecuteEnPassant_ShouldMovePawnAndRemoveEnemyPawn()
    {
        var board = new Board();

        // White pawn on f5
        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        // Black pawn on e5
        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        // Black just moved e7 -> e5
        var lastMove =
            new Move(
                new Position(1, 4),
                new Position(3, 4)
            );

        // White f5 -> e6
        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White,
                lastMove
            );

        Assert.True(result);

        // White pawn is now on e6.
        Piece? whitePawn =
            board.GetPiece(
                new Position(2, 4)
            );

        Assert.NotNull(whitePawn);

        Assert.Equal(
            PieceType.Pawn,
            whitePawn!.Type
        );

        Assert.Equal(
            PieceColor.White,
            whitePawn.Color
        );

        // Original f5 is empty.
        Assert.Null(
            board.GetPiece(
                new Position(3, 5)
            )
        );

        // Black pawn on e5 has been captured.
        Assert.Null(
            board.GetPiece(
                new Position(3, 4)
            )
        );
    }

    [Fact]
    public void EnPassant_ShouldNotExecuteWithoutLastMove()
    {
        var board = new Board();

        board.SetPiece(
            new Position(3, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(3, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(3, 5),
                new Position(2, 4)
            );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.False(result);

        // Both pawns must remain.
        Assert.NotNull(
            board.GetPiece(
                new Position(3, 5)
            )
        );

        Assert.NotNull(
            board.GetPiece(
                new Position(3, 4)
            )
        );
    }
}