using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class GameStateDetectorTests
{
    [Fact]
    public void Checkmate_ShouldBeDetected()
    {
        var board = new Board();

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White Queen
        board.SetPiece(
            new Position(1, 1),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        // White King protects the Queen
        board.SetPiece(
            new Position(2, 2),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        Assert.True(
            GameStateDetector.IsCheckmate(
                board,
                PieceColor.Black
            )
        );
    }

    [Fact]
    public void Stalemate_ShouldBeDetected()
    {
        var board = new Board();

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White King
        board.SetPiece(
            new Position(2, 2),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // White Queen controls the escape squares
        board.SetPiece(
            new Position(1, 2),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        Assert.True(
            GameStateDetector.IsStalemate(
                board,
                PieceColor.Black
            )
        );
    }
}