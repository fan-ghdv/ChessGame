using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class CastlingValidatorTests
{
    // =========================================
    // KING-SIDE CASTLING
    // =========================================

    [Fact]
    public void KingSideCastling_ShouldBeAllowed()
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

        // White Rook
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        Assert.True(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_ShouldBeAllowed()
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

        // White Rook
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        Assert.True(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // PIECES MUST EXIST
    // =========================================

    [Fact]
    public void KingSideCastling_WithoutRook_ShouldBeRejected()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_WithoutRook_ShouldBeRejected()
    {
        var board = new Board();

        board.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // KING / ROOK MUST NOT HAVE MOVED
    // =========================================

    [Fact]
    public void KingSideCastling_AfterKingMoved_ShouldBeRejected()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        king.MarkAsMoved();

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 7),
            rook
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void KingSideCastling_AfterRookMoved_ShouldBeRejected()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        rook.MarkAsMoved();

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 7),
            rook
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_AfterKingMoved_ShouldBeRejected()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        king.MarkAsMoved();

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 0),
            rook
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_AfterRookMoved_ShouldBeRejected()
    {
        var board = new Board();

        var king =
            new Piece(
                PieceType.King,
                PieceColor.White
            );

        var rook =
            new Piece(
                PieceType.Rook,
                PieceColor.White
            );

        rook.MarkAsMoved();

        board.SetPiece(
            new Position(7, 4),
            king
        );

        board.SetPiece(
            new Position(7, 0),
            rook
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // BLOCKED SQUARES
    // =========================================

    [Fact]
    public void KingSideCastling_WithPieceBetween_ShouldBeRejected()
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
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 5),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_WithPieceBetween_ShouldBeRejected()
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
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        board.SetPiece(
            new Position(7, 2),
            new Piece(
                PieceType.Knight,
                PieceColor.White
            )
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // KING CANNOT CASTLE OUT OF CHECK
    // =========================================

    [Fact]
    public void KingSideCastling_WhileInCheck_ShouldBeRejected()
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
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks White King.
        board.SetPiece(
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_WhileInCheck_ShouldBeRejected()
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
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks White King.
        board.SetPiece(
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // KING CANNOT PASS THROUGH CHECK
    // =========================================

    [Fact]
    public void KingSideCastling_ThroughAttackedSquare_ShouldBeRejected()
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
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks (7,5).
        board.SetPiece(
            new Position(5, 5),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_ThroughAttackedSquare_ShouldBeRejected()
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
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks (7,3).
        board.SetPiece(
            new Position(5, 3),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }

    // =========================================
    // KING CANNOT FINISH ON ATTACKED SQUARE
    // =========================================

    [Fact]
    public void KingSideCastling_OnAttackedTargetSquare_ShouldBeRejected()
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
            new Position(7, 7),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks (7,6).
        board.SetPiece(
            new Position(5, 6),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleKingSide(
                board,
                PieceColor.White
            )
        );
    }

    [Fact]
    public void QueenSideCastling_OnAttackedTargetSquare_ShouldBeRejected()
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
            new Position(7, 0),
            new Piece(
                PieceType.Rook,
                PieceColor.White
            )
        );

        // Black rook attacks (7,2).
        board.SetPiece(
            new Position(5, 2),
            new Piece(
                PieceType.Rook,
                PieceColor.Black
            )
        );

        Assert.False(
            CastlingValidator.CanCastleQueenSide(
                board,
                PieceColor.White
            )
        );
    }
}