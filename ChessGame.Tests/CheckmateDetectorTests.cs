using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CheckmateDetectorTests
{
    [Fact]
    public void NotInCheck_ShouldNotBeCheckmate()
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

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_HasEscapeSquare_ShouldNotBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives check along the row.
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
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CannotEscapeIntoAttackedSquare()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives check.
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Black Bishop controls an escape square.
        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Bishop,
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
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureAttacker_ShouldNotBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives check directly next to White King.
        board.SetPiece(
            new Position(7, 5),
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
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureUnprotectedRook()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook is next to the White King.
        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureUnprotectedBishop()
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
                PieceType.King,
                PieceColor.Black
            )
        );

        // Bishop gives check.
        board.SetPiece(
            new Position(6, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(6, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureUnprotectedQueen()
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
                PieceType.King,
                PieceColor.Black
            )
        );

        // Queen gives check.
        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureUnprotectedKnight()
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
                PieceType.King,
                PieceColor.Black
            )
        );

        // Knight is next to the King.
        board.SetPiece(
            new Position(6, 5),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(6, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureUnprotectedPawn()
    {
        var board = new Board();

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Pawn attacks the King.
        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        var move =
            new Move(
                new Position(5, 4),
                new Position(4, 3)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CannotCaptureProtectedAttacker()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives check.
        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Black Bishop protects the Rook.
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void Piece_CanBlockRookCheck_ShouldNotBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives vertical check.
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // White Bishop can block the check.
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void RookCheck_CanBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // White Bishop can block the Rook.
        board.SetPiece(
            new Position(7, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void BishopCheck_CanBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Bishop gives diagonal check.
        board.SetPiece(
            new Position(4, 1),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // At this point the Bishop should be giving check.
        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // White Bishop blocks the diagonal.
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        // The check is now blocked,
        // so it should not be checkmate.
        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenCheck_CanBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Queen
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        // White Bishop blocks the Queen.
        board.SetPiece(
            new Position(7, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KnightCheck_CannotBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Knight attacks White King.
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // A piece cannot block a Knight attack.
        board.SetPiece(
            new Position(6, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
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
    public void KnightCheck_KingCannotCaptureKnight()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Knight gives check from an L-shaped position.
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // The Knight is outside the King's one-square movement range.
        var move =
            new Move(
                new Position(7, 4),
                new Position(5, 3)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KnightCheck_WithEscapeSquare_ShouldNotBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Knight gives check.
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Knight,
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
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void PawnCheck_CannotBeBlocked()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(5, 4),
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

        // Black Pawn attacks White King.
        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // A piece cannot block a Pawn attack.
        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Pawn,
                PieceColor.White
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
    public void PawnCheck_WithEscapeSquare_ShouldNotBeCheckmate()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(5, 4),
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

        // Black Pawn attacks White King.
        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Pawn,
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
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KingCheck_CannotBeBlocked()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        // Black King
        board.SetPiece(
            new Position(4, 5),
            new Piece(
                PieceType.King,
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
    public void QueenDiagonalCheck_CanBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Queen gives diagonal check.
        board.SetPiece(
            new Position(4, 1),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        // Queen attacks:
        // (4,1) -> (5,2) -> (6,3) -> (7,4)

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // White Bishop blocks the diagonal.
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenStraightCheck_CanBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Queen gives vertical check.
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // White Bishop blocks the queen.
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void DoubleCheck_CannotBeBlocked()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives vertical check.
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Black Bishop gives diagonal check.
        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // A white piece cannot block both checks.
        board.SetPiece(
            new Position(6, 3),
            new Piece(
                PieceType.Bishop,
                PieceColor.White
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void DoubleCheck_WithNoEscape_ShouldBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives vertical check.
        board.SetPiece(
            new Position(0, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Black Bishop gives diagonal check.
        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control King's escape squares.

        // (6,3)
        board.SetPiece(
            new Position(5, 1),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // (6,4)
        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // (6,5)
        board.SetPiece(
            new Position(5, 6),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // (7,3)
        board.SetPiece(
            new Position(7, 2),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // (7,5)
        board.SetPiece(
            new Position(7, 7),
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

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureRookAndEscapeCheck()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Rook gives check.
        board.SetPiece(
            new Position(7, 5),
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

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );

        // Execute the capture.
        board.SetPiece(
            new Position(7, 5),
            board.GetPiece(
                new Position(7, 4)
            )
        );

        board.SetPiece(
            new Position(7, 4),
            null
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureBishopAndEscapeCheck()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Bishop gives diagonal check.
        board.SetPiece(
            new Position(6, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(6, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );

        // Execute the capture.
        board.SetPiece(
            new Position(6, 5),
            board.GetPiece(
                new Position(7, 4)
            )
        );

        board.SetPiece(
            new Position(7, 4),
            null
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCaptureQueenAndEscapeCheck()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Queen gives horizontal check.
        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );

        // Execute the capture.
        board.SetPiece(
            new Position(7, 5),
            board.GetPiece(
                new Position(7, 4)
            )
        );

        board.SetPiece(
            new Position(7, 4),
            null
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KnightCheck_KingCanEscape()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Black Knight gives check.
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // King cannot capture the Knight because
        // the Knight is two rows away.
        var captureKnight =
            new Move(
                new Position(7, 4),
                new Position(5, 3)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                captureKnight,
                PieceColor.White
            )
        );

        // King can escape to (7,5).
        var escapeMove =
            new Move(
                new Position(7, 4),
                new Position(7, 5)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                escapeMove,
                PieceColor.White
            )
        );

        Assert.False(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KnightCheck_WithAllEscapeSquaresControlled_ShouldBeCheckmate()
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

        // Black King
        board.SetPiece(
            new Position(0, 0),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // =====================================
        // Knight gives check.
        //
        // Knight (5,3) attacks White King (7,4).
        // =====================================

        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        // =====================================
        // Control escape square (6,3)
        // =====================================

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        // =====================================
        // Control escape square (6,4)
        // =====================================

        board.SetPiece(
            new Position(4, 5),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        // =====================================
        // Control escape square (6,5)
        // =====================================

        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        // =====================================
        // Control escape squares (7,3) and (7,5)
        // =====================================

        board.SetPiece(
            new Position(5, 4),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void King_CanCapturePawnAndEscapeCheck()
    {
        var board = new Board();

        // White King
        board.SetPiece(
            new Position(5, 4),
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

        // Black Pawn gives check.
        board.SetPiece(
            new Position(4, 3),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        var move =
            new Move(
                new Position(5, 4),
                new Position(4, 3)
            );

        Assert.True(
            MoveValidator.IsLegalMove(
                board,
                move,
                PieceColor.White
            )
        );

        // Execute the capture.
        board.SetPiece(
            new Position(4, 3),
            board.GetPiece(
                new Position(5, 4)
            )
        );

        board.SetPiece(
            new Position(5, 4),
            null
        );

        Assert.False(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void PawnCheck_WithNoLegalEscape_ShouldBeCheckmate()
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

        // Black Pawn gives check.
        // Black Pawn at (6,6) attacks (7,7).
        board.SetPiece(
            new Position(6, 6),
            new Piece(
                PieceType.Pawn,
                PieceColor.Black
            )
        );

        // Protect the Pawn.
        // Bishop at (5,5) protects (6,6).
        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control King's escape square (6,7).
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control King's escape square (7,6).
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

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void BishopCheck_WithNoLegalEscape_ShouldBeCheckmate()
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

        // Black Bishop gives diagonal check.
        // Bishop (5,5) attacks White King (7,7).
        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Protect the Bishop.
        board.SetPiece(
            new Position(4, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        // Control escape square (6,7).
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control escape square (7,6).
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control escape square (6,6).
        board.SetPiece(
            new Position(4, 6),
            new Piece(
                PieceType.Knight,
                PieceColor.Black
            )
        );

        Assert.True(
            CheckDetector.IsInCheck(
                board,
                PieceColor.White
            )
        );

        // White King cannot capture the Bishop.
        var captureBishop =
            new Move(
                new Position(7, 7),
                new Position(5, 5)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                captureBishop,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void RookCheck_WithNoLegalEscape_ShouldBeCheckmate()
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

        // Black Rook gives horizontal check.
        // Rook (7,0) attacks White King (7,7).
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Protect the Rook.
        board.SetPiece(
            new Position(5, 0),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control escape square (6,7).
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control escape square (6,6).
        board.SetPiece(
            new Position(4, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control escape square (7,6).
        board.SetPiece(
            new Position(0, 6),
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

        // King cannot capture the protected Rook.
        var captureRook =
            new Move(
                new Position(7, 7),
                new Position(7, 0)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                captureRook,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenCheck_WithNoLegalEscape_ShouldBeCheckmate()
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

        // Black Queen gives horizontal check.
        // Queen (7,0) attacks White King (7,7).
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Queen,
                PieceColor.Black
            )
        );

        // Protect the Queen.
        board.SetPiece(
            new Position(5, 0),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control escape square (6,7).
        board.SetPiece(
            new Position(0, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control escape square (6,6).
        board.SetPiece(
            new Position(4, 5),
            new Piece(
                PieceType.Bishop,
                PieceColor.Black
            )
        );

        // Control escape square (7,6).
        board.SetPiece(
            new Position(0, 6),
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

        // King cannot capture the protected Queen.
        var captureQueen =
            new Move(
                new Position(7, 7),
                new Position(7, 0)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                captureQueen,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KingCheck_WithNoLegalEscape_ShouldBeCheckmate()
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

        // Black King directly attacks White King.
        board.SetPiece(
            new Position(6, 6),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        // Control (6,7)
        board.SetPiece(
            new Position(4, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        // Control (7,6)
        board.SetPiece(
            new Position(7, 4),
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

        // White King cannot capture Black King.
        var captureKing =
            new Move(
                new Position(7, 7),
                new Position(6, 6)
            );

        Assert.False(
            MoveValidator.IsLegalMove(
                board,
                captureKing,
                PieceColor.White
            )
        );

        Assert.True(
            CheckmateDetector.IsCheckmate(
                board,
                PieceColor.White
            )
        );
    }
}