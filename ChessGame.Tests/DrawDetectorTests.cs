using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class DrawDetectorTests
{
    [Fact]
    public void KingVsKing_ShouldBeInsufficientMaterial()
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
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        Assert.True(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndRook_ShouldNotBeInsufficientMaterial()
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
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndBishopVsKing_ShouldBeInsufficientMaterial()
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
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
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

        Assert.True(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndKnightVsKing_ShouldBeInsufficientMaterial()
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
            new Position(6, 2),
            new Piece(
                PieceType.Knight,
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

        Assert.True(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void SameColorBishops_ShouldBeInsufficientMaterial()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // White Bishop
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        // Black King
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Bishop
        // Same color square as White Bishop.
        board.SetPiece(
            new Position(1, 2),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        Assert.True(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void OppositeColorBishops_ShouldNotBeInsufficientMaterial()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // White Bishop
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        // Black King
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Bishop
        // Opposite color square.
        board.SetPiece(
            new Position(1, 1),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndQueenVsKing_ShouldNotBeInsufficientMaterial()
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
            new Position(7, 0),
            new Piece(
                PieceType.Queen,
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

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndBishopAndKnightVsKing_ShouldNotBeInsufficientMaterial()
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
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Knight,
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

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndTwoBishopsVsKing_ShouldNotBeInsufficientMaterial()
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
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.Bishop,
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

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void KingAndTwoKnightsVsKing_ShouldNotBeInsufficientMaterial()
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
            new Position(6, 2),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.Knight,
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

        Assert.False(
            DrawDetector.IsInsufficientMaterial(
                board
            )
        );
    }

    [Fact]
    public void NewBoard_ShouldHaveZeroHalfmoveClock()
    {
        var board = new Board();

        Assert.Equal(
            0,
            board.HalfmoveClock
        );
    }

    [Fact]
    public void HalfmoveClock_Below100_ShouldNotBeFiftyMoveDraw()
    {
        var board = new Board();

        for (int i = 0; i < 99; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            99,
            board.HalfmoveClock
        );

        Assert.False(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void HalfmoveClock_At100_ShouldBeFiftyMoveDraw()
    {
        var board = new Board();

        for (int i = 0; i < 100; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            100,
            board.HalfmoveClock
        );

        Assert.True(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void HalfmoveClock_Above100_ShouldBeFiftyMoveDraw()
    {
        var board = new Board();

        for (int i = 0; i < 120; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.True(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void ClearBoard_ShouldResetHalfmoveClock()
    {
        var board = new Board();

        for (int i = 0; i < 50; i++)
        {
            board.IncrementHalfmoveClock();
        }

        board.Clear();

        Assert.Equal(
            0,
            board.HalfmoveClock
        );
    }

    [Fact]
    public void FiftyMoveRule_Below100_ShouldNotBeDraw()
    {
        var board = new Board();

        board.IncrementHalfmoveClock();

        for (int i = 0; i < 98; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            99,
            board.HalfmoveClock
        );

        Assert.False(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void FiftyMoveRule_At100_ShouldBeDraw()
    {
        var board = new Board();

        for (int i = 0; i < 100; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            100,
            board.HalfmoveClock
        );

        Assert.True(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void FiftyMoveRule_Above100_ShouldBeDraw()
    {
        var board = new Board();

        for (int i = 0; i < 101; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            101,
            board.HalfmoveClock
        );

        Assert.True(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void ResetHalfmoveClock_ShouldSetClockToZero()
    {
        var board = new Board();

        for (int i = 0; i < 100; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.Equal(
            100,
            board.HalfmoveClock
        );

        board.ResetHalfmoveClock();

        Assert.Equal(
            0,
            board.HalfmoveClock
        );

        Assert.False(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void HalfmoveClock_ShouldIncrement()
    {
        var board = new Board();

        board.IncrementHalfmoveClock();

        Assert.Equal(
            1,
            board.HalfmoveClock
        );
    }

    [Fact]
    public void HalfmoveClock_ShouldReset()
    {
        var board = new Board();

        board.IncrementHalfmoveClock();
        board.IncrementHalfmoveClock();

        board.ResetHalfmoveClock();

        Assert.Equal(
            0,
            board.HalfmoveClock
        );
    }

    [Fact]
    public void FiftyMoveDraw_ShouldOccurAt100Halfmoves()
    {
        var board = new Board();

        for (int i = 0; i < 100; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.True(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }

    [Fact]
    public void FiftyMoveDraw_ShouldNotOccurBefore100Halfmoves()
    {
        var board = new Board();

        for (int i = 0; i < 99; i++)
        {
            board.IncrementHalfmoveClock();
        }

        Assert.False(
            DrawDetector.IsFiftyMoveDraw(
                board
            )
        );
    }
}