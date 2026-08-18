using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CheckDetectorTests
{
    [Fact]
    public void WhiteKing_ShouldBeInCheck_ByBlackRook()
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
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void WhiteKing_ShouldNotBeInCheck_WhenPathIsBlocked()
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
                PieceType.Rook,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void BlackKing_ShouldBeInCheck_ByWhiteQueen()
    {
        var board = new Board();

        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.Black
            )
        );
    }

    [Fact]
    public void King_ShouldNotBeInCheck_OnSafeSquare()
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
            new Position(0, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }
}