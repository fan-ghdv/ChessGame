using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class StalemateDetectorTests
{
    [Fact]
    public void NotStalemate_WhenKingIsInCheck()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook checks White King.
        board.SetPiece(
            new Position(7, 0),
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

        Assert.False(
            StalemateDetector.IsStalemate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void NotStalemate_WhenLegalMoveExists()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            StalemateDetector.IsStalemate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_WithNoLegalMoveAndNotInCheck_ShouldBeStalemate()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black King controls (6,6).
        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Queen controls:
        //
        // (6,7) horizontally
        // (7,6) diagonally
        //
        // but DOES NOT attack (7,7).
        board.SetPiece(
            new Position(6, 5),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            LegalMoveFinder.HasAnyLegalMove(
                board,
                PieceColor.White
            )
        );

        Assert.True(
            StalemateDetector.IsStalemate(
                board,
                PieceColor.White
            )
        );
    }
}