using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class LegalMoveGeneratorTests
{
    [Fact]
    public void Rook_ShouldGenerateLegalMoves()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var moves =
            LegalMoveGenerator.GetLegalMoves(
                board,
                PieceColor.White
            );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(4, 4) &&
                move.To == new Position(4, 7)
        );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(4, 4) &&
                move.To == new Position(0, 4)
        );
    }

    [Fact]
    public void Knight_ShouldGenerateLShapeMoves()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var moves =
            LegalMoveGenerator.GetLegalMoves(
                board,
                PieceColor.White
            );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(4, 4) &&
                move.To == new Position(2, 3)
        );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(4, 4) &&
                move.To == new Position(3, 2)
        );
    }

    [Fact]
    public void Pawn_ShouldGenerateOneAndTwoSquareMoves()
    {
        var board = new Board();

        board.SetPiece(
            new Position(6, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var moves =
            LegalMoveGenerator.GetLegalMoves(
                board,
                PieceColor.White
            );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(6, 4) &&
                move.To == new Position(5, 4)
        );

        Assert.Contains(
            moves,
            move =>
                move.From == new Position(6, 4) &&
                move.To == new Position(4, 4)
        );
    }

    [Fact]
    public void FriendlyPiece_ShouldNotBeIncluded()
    {
        var board = new Board();

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(4, 5),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        var moves =
            LegalMoveGenerator.GetLegalMoves(
                board,
                PieceColor.White
            );

        Assert.DoesNotContain(
            moves,
            move =>
                move.From == new Position(4, 4) &&
                move.To == new Position(4, 5)
        );
    }
}