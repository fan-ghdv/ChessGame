using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameGame = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class FenGeneratorTests
{
    [Fact]
    public void InitialPosition_ShouldGenerateCorrectBoard()
    {
        var game = new ChessGameGame();

        string fen =
            FenGenerator.Generate(
                game.Board,
                game.SideToMove
            );

        Assert.Equal(
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
            fen
        );
    }

    [Fact]
    public void EmptyBoard_ShouldGenerateEightEmptyRanks()
    {
        var board = new Board();

        board.Clear();

        string fen =
            FenGenerator.Generate(
                board,
                PieceColor.White
            );

        Assert.Equal(
            "8/8/8/8/8/8/8/8 w - - 0 1",
            fen
        );
    }

    [Fact]
    public void BlackToMove_ShouldGenerateBlackActiveColor()
    {
        var board = new Board();

        board.Clear();

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

        string fen =
            FenGenerator.Generate(
                board,
                PieceColor.Black
            );

        Assert.Equal(
            "4k3/8/8/8/8/8/8/4K3 b - - 0 1",
            fen
        );
    }

    [Fact]
    public void Pieces_ShouldUseCorrectFenCharacters()
    {
        var board = new Board();

        board.Clear();

        var blackRook =
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            );

        blackRook.MarkAsMoved();

        board.SetPiece(
            new Position(0, 0),
            blackRook
        );

        board.SetPiece(
            new Position(0, 1),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(0, 2),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        board.SetPiece(
            new Position(0, 3),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
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
            new Position(0, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 6),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        string fen =
            FenGenerator.Generate(
                board,
                PieceColor.White
            );

        Assert.Equal(
            "rnbqkBNR/8/8/8/8/8/8/8 w - - 0 1",
            fen
        );
    }
}