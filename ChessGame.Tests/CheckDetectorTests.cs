using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CheckDetectorTests
{
    [Fact]
    public void BlackKing_ShouldBeInCheck_FromWhiteQueen()
    {
        var board = new Board();

        board.Clear();

        // White king
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black king
        board.SetPiece(
            new Position(5, 6),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White queen
        board.SetPiece(
            new Position(6, 6),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        bool result =
            CheckDetector.IsInCheck(
                board,
                PieceColor.Black
            );

        Assert.True(result);
    }

    [Fact]
    public void WhiteKing_ShouldBeInCheck_FromBlackQueen()
    {
        var board = new Board();

        board.Clear();

        // Black king
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White king
        board.SetPiece(
            new Position(2, 6),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black queen
        board.SetPiece(
            new Position(1, 6),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        bool result =
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            );

        Assert.True(result);
    }

    [Fact]
    public void BlackKing_ShouldNotBeInCheck_InStalematePosition()
    {
        var board = new Board();

        board.Clear();

        // -----------------------------------------------------
        // Known stalemate position:
        //
        // Black King: h8
        // White King: f7
        // White Queen: g6
        // -----------------------------------------------------

        // Black king: h8
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White king: f7
        board.SetPiece(
            new Position(1, 5),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // White queen: g6
        board.SetPiece(
            new Position(2, 6),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        bool result =
            CheckDetector.IsInCheck(
                board,
                PieceColor.Black
            );

        Assert.False(result);
    }
}