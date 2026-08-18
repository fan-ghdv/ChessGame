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
}