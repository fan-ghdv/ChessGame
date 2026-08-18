using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class PawnPromotionTests
{
    [Fact]
    public void WhitePawn_ShouldPromoteOnRowZero()
    {
        var position = new Position(0, 4);

        bool result = PawnPromotion.CanPromote(
            position,
            PieceColor.White
        );

        Assert.True(result);
    }

    [Fact]
    public void BlackPawn_ShouldPromoteOnRowSeven()
    {
        var position = new Position(7, 4);

        bool result = PawnPromotion.CanPromote(
            position,
            PieceColor.Black
        );

        Assert.True(result);
    }

    [Fact]
    public void WhitePawn_ShouldNotPromoteBeforeLastRank()
    {
        var position = new Position(1, 4);

        bool result = PawnPromotion.CanPromote(
            position,
            PieceColor.White
        );

        Assert.False(result);
    }

    [Fact]
    public void BlackPawn_ShouldNotPromoteBeforeLastRank()
    {
        var position = new Position(6, 4);

        bool result = PawnPromotion.CanPromote(
            position,
            PieceColor.Black
        );

        Assert.False(result);
    }

    [Fact]
    public void WhitePawn_ShouldPromoteToQueen()
    {
        var piece = PawnPromotion.Promote(
            new Position(0, 4),
            PieceColor.White,
            PieceType.Queen
        );

        Assert.Equal(PieceType.Queen, piece.Type);
        Assert.Equal(PieceColor.White, piece.Color);
    }

    [Fact]
    public void WhitePawn_ShouldPromoteToKnight()
    {
        var piece = PawnPromotion.Promote(
            new Position(0, 4),
            PieceColor.White,
            PieceType.Knight
        );

        Assert.Equal(PieceType.Knight, piece.Type);
        Assert.Equal(PieceColor.White, piece.Color);
    }

    [Fact]
    public void BlackPawn_ShouldPromoteToRook()
    {
        var piece = PawnPromotion.Promote(
            new Position(7, 4),
            PieceColor.Black,
            PieceType.Rook
        );

        Assert.Equal(PieceType.Rook, piece.Type);
        Assert.Equal(PieceColor.Black, piece.Color);
    }

    [Fact]
    public void BlackPawn_ShouldPromoteToBishop()
    {
        var piece = PawnPromotion.Promote(
            new Position(7, 4),
            PieceColor.Black,
            PieceType.Bishop
        );

        Assert.Equal(PieceType.Bishop, piece.Type);
        Assert.Equal(PieceColor.Black, piece.Color);
    }

    [Fact]
    public void Pawn_ShouldNotPromoteToKing()
    {
        Assert.Throws<ArgumentException>(() =>
            PawnPromotion.Promote(
                new Position(0, 4),
                PieceColor.White,
                PieceType.King
            )
        );
    }

    [Fact]
    public void Pawn_ShouldNotPromoteToPawn()
    {
        Assert.Throws<ArgumentException>(() =>
            PawnPromotion.Promote(
                new Position(0, 4),
                PieceColor.White,
                PieceType.Pawn
            )
        );
    }

    [Fact]
    public void Pawn_ShouldNotPromoteBeforeReachingLastRank()
    {
        Assert.Throws<InvalidOperationException>(() =>
            PawnPromotion.Promote(
                new Position(1, 4),
                PieceColor.White,
                PieceType.Queen
            )
        );
    }
}