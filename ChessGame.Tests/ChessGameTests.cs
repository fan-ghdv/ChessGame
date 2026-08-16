using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameModel = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class ChessGameTests
{
    [Fact]
    public void NewGame_ShouldHave32Pieces()
    {
        var game = new ChessGameModel();

        Assert.Equal(32, game.CountPieces());
    }

    [Fact]
    public void NewGame_ShouldHave16WhitePieces()
    {
        var game = new ChessGameModel();

        int whitePieces = 0;

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                var piece = game.Board.GetPiece(
                    new Position(row, column)
                );

                if (piece?.Color == PieceColor.White)
                {
                    whitePieces++;
                }
            }
        }

        Assert.Equal(16, whitePieces);
    }

    [Fact]
    public void NewGame_ShouldHave16BlackPieces()
    {
        var game = new ChessGameModel();

        int blackPieces = 0;

        for (int row = 0; row < Board.Size; row++)
        {
            for (int column = 0; column < Board.Size; column++)
            {
                var piece = game.Board.GetPiece(
                    new Position(row, column)
                );

                if (piece?.Color == PieceColor.Black)
                {
                    blackPieces++;
                }
            }
        }

        Assert.Equal(16, blackPieces);
    }

    [Fact]
    public void WhiteKing_ShouldStartAtCorrectPosition()
    {
        var game = new ChessGameModel();

        var piece = game.Board.GetPiece(
            new Position(7, 4)
        );

        Assert.NotNull(piece);
        Assert.Equal(PieceType.King, piece.Type);
        Assert.Equal(PieceColor.White, piece.Color);
    }

    [Fact]
    public void BlackKing_ShouldStartAtCorrectPosition()
    {
        var game = new ChessGameModel();

        var piece = game.Board.GetPiece(
            new Position(0, 4)
        );

        Assert.NotNull(piece);
        Assert.Equal(PieceType.King, piece.Type);
        Assert.Equal(PieceColor.Black, piece.Color);
    }

    [Fact]
    public void WhiteQueen_ShouldStartAtCorrectPosition()
    {
        var game = new ChessGameModel();

        var piece = game.Board.GetPiece(
            new Position(7, 3)
        );

        Assert.NotNull(piece);
        Assert.Equal(PieceType.Queen, piece.Type);
        Assert.Equal(PieceColor.White, piece.Color);
    }

    [Fact]
    public void BlackQueen_ShouldStartAtCorrectPosition()
    {
        var game = new ChessGameModel();

        var piece = game.Board.GetPiece(
            new Position(0, 3)
        );

        Assert.NotNull(piece);
        Assert.Equal(PieceType.Queen, piece.Type);
        Assert.Equal(PieceColor.Black, piece.Color);
    }

    [Fact]
    public void Pawns_ShouldStartOnCorrectRows()
    {
        var game = new ChessGameModel();

        for (int column = 0; column < Board.Size; column++)
        {
            var whitePawn = game.Board.GetPiece(
                new Position(6, column)
            );

            var blackPawn = game.Board.GetPiece(
                new Position(1, column)
            );

            Assert.NotNull(whitePawn);
            Assert.Equal(PieceType.Pawn, whitePawn.Type);
            Assert.Equal(PieceColor.White, whitePawn.Color);

            Assert.NotNull(blackPawn);
            Assert.Equal(PieceType.Pawn, blackPawn.Type);
            Assert.Equal(PieceColor.Black, blackPawn.Color);
        }
    }
}