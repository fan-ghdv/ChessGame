using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class PromotionExecutionTests
{
    [Fact]
    public void WhitePawn_ShouldPromoteToQueen()
    {
        var board = new Board();

        board.SetPiece(
            new Position(1, 0),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(1, 0),
                new Position(0, 0)
            );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White,
                null,
                PieceType.Queen
            );

        Assert.True(result);

        Piece? piece =
            board.GetPiece(
                new Position(0, 0)
            );

        Assert.NotNull(piece);

        Assert.Equal(
            PieceType.Queen,
            piece!.Type
        );

        Assert.Equal(
            PieceColor.White,
            piece.Color
        );
    }

    [Fact]
    public void BlackPawn_ShouldPromoteToKnight()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 0),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(6, 0),
                new Position(7, 0)
            );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.Black,
                null,
                PieceType.Knight
            );

        Assert.True(result);

        Piece? piece =
            board.GetPiece(
                new Position(7, 0)
            );

        Assert.NotNull(piece);

        Assert.Equal(
            PieceType.Knight,
            piece!.Type
        );

        Assert.Equal(
            PieceColor.Black,
            piece.Color
        );
    }

    [Fact]
    public void Promotion_ShouldFailWithoutPromotionType()
    {
        var board = new Board();

        board.SetPiece(
            new Position(1, 0),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(1, 0),
                new Position(0, 0)
            );

        bool result =
            MoveExecutor.TryExecuteMove(
                board,
                move,
                PieceColor.White
            );

        Assert.False(result);

        Piece? piece =
            board.GetPiece(
                new Position(1, 0)
            );

        Assert.NotNull(piece);

        Assert.Equal(
            PieceType.Pawn,
            piece!.Type
        );
    }

    [Fact]
    public void Promotion_ShouldNotAllowKing()
    {
        var board = new Board();

        board.SetPiece(
            new Position(1, 0),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(1, 0),
                new Position(0, 0)
            );

        Assert.Throws<ArgumentException>(
            () =>
                MoveExecutor.TryExecuteMove(
                    board,
                    move,
                    PieceColor.White,
                    null,
                    PieceType.King
                )
        );
    }
}