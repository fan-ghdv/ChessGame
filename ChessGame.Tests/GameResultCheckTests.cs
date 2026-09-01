using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class GameResultCheckTests
{
    // =========================================================
    // CHECKMATE
    // =========================================================

    [Fact]
    public void Checkmate_ShouldReturnWhiteWins()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // -----------------------------------------------------
        // Black King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // -----------------------------------------------------
        // White King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(2, 5),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // -----------------------------------------------------
        // White Queen
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(1, 6),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                game.Board,
                PieceColor.Black
            )
        );

        Assert.Equal(
            GameResult.WhiteWins,
            game.GetGameResult(
                PieceColor.Black
            )
        );
    }

    // =========================================================
    // CHECKMATE
    // =========================================================

    [Fact]
    public void Checkmate_ShouldReturnBlackWins()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // -----------------------------------------------------
        // White King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // -----------------------------------------------------
        // Black King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // -----------------------------------------------------
        // Black Queen
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(6, 1),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                game.Board,
                PieceColor.White
            )
        );

        Assert.Equal(
            GameResult.BlackWins,
            game.GetGameResult(
                PieceColor.White
            )
        );
    }

    // =========================================================
    // STALEMATE
    // =========================================================

    [Fact]
    public void Stalemate_ShouldReturnStalemate()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // Black king: h8
        game.Board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // White king: f7
        game.Board.SetPiece(
            new Position(1, 5),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // White queen: g6
        game.Board.SetPiece(
            new Position(2, 6),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        Assert.False(
            CheckDetector.IsInCheck(
                game.Board,
                PieceColor.Black
            )
        );

        Assert.False(
            LegalMoveFinder.HasAnyLegalMove(
                game.Board,
                PieceColor.Black
            )
        );

        Assert.Equal(
            GameResult.Stalemate,
            game.GetGameResult(
                PieceColor.Black
            )
        );
    }

    // =========================================================
    // NORMAL POSITION
    // =========================================================

    [Fact]
    public void NonCheckPosition_ShouldNotReturnCheckmate()
    {
        var game = new ChessGameGame();

        game.Board.Clear();

        // -----------------------------------------------------
        // White King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // -----------------------------------------------------
        // Black King
        // -----------------------------------------------------

        game.Board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // -----------------------------------------------------
        // White should not be considered the winner.
        // -----------------------------------------------------

        Assert.NotEqual(
            GameResult.WhiteWins,
            game.GetGameResult(
                PieceColor.White
            )
        );

        // -----------------------------------------------------
        // Black should not be considered the winner.
        // -----------------------------------------------------

        Assert.NotEqual(
            GameResult.BlackWins,
            game.GetGameResult(
                PieceColor.Black
            )
        );
    }
}