using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public class MoveRecord
{
    // =========================================================
    // MOVE
    // =========================================================

    public Move Move { get; }

    // =========================================================
    // SAN NOTATION
    // =========================================================

    public string SanNotation { get; }

    // =========================================================
    // COLOR
    // =========================================================

    public PieceColor Color { get; }

    // =========================================================
    // MOVED PIECE
    // =========================================================

    public Piece? MovedPiece { get; }

    public bool PreviousMovedState { get; }

    // =========================================================
    // CAPTURE
    // =========================================================

    public Piece? CapturedPiece { get; }

    public Position? CapturedPosition { get; }

    // =========================================================
    // CASTLING
    // =========================================================

    public bool WasCastling { get; }

    public Position? RookFrom { get; }

    public Position? RookTo { get; }

    public Piece? Rook { get; }

    public bool RookPreviousMovedState { get; }

    // =========================================================
    // PROMOTION
    // =========================================================

    public bool WasPromotion { get; }

    public PieceType? PromotionType { get; }

    // =========================================================
    // BOARD STATE
    // =========================================================

    public int PreviousHalfmoveClock { get; }

    public int PreviousFullmoveNumber { get; }

    // =========================================================
    // GAME STATE
    // =========================================================

    public PieceColor PreviousSideToMove { get; }

    public Move? PreviousLastMove { get; }

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public MoveRecord(
        Move move,
        string sanNotation,
        PieceColor color,
        Piece? movedPiece,
        bool previousMovedState,
        Piece? capturedPiece,
        Position? capturedPosition,
        bool wasCastling,
        Position? rookFrom,
        Position? rookTo,
        Piece? rook,
        bool rookPreviousMovedState,
        bool wasPromotion,
        PieceType? promotionType,
        int previousHalfmoveClock,
        int previousFullmoveNumber,
        PieceColor previousSideToMove,
        Move? previousLastMove)
    {
        // =====================================================
        // MOVE
        // =====================================================

        Move = move;

        // =====================================================
        // SAN
        // =====================================================

        SanNotation =
            sanNotation;

        // =====================================================
        // COLOR
        // =====================================================

        Color = color;

        // =====================================================
        // MOVED PIECE
        // =====================================================

        MovedPiece =
            movedPiece;

        PreviousMovedState =
            previousMovedState;

        // =====================================================
        // CAPTURE
        // =====================================================

        CapturedPiece =
            capturedPiece;

        CapturedPosition =
            capturedPosition;

        // =====================================================
        // CASTLING
        // =====================================================

        WasCastling =
            wasCastling;

        RookFrom =
            rookFrom;

        RookTo =
            rookTo;

        Rook =
            rook;

        RookPreviousMovedState =
            rookPreviousMovedState;

        // =====================================================
        // PROMOTION
        // =====================================================

        WasPromotion =
            wasPromotion;

        PromotionType =
            promotionType;

        // =====================================================
        // BOARD STATE
        // =====================================================

        PreviousHalfmoveClock =
            previousHalfmoveClock;

        PreviousFullmoveNumber =
            previousFullmoveNumber;

        // =====================================================
        // GAME STATE
        // =====================================================

        PreviousSideToMove =
            previousSideToMove;

        PreviousLastMove =
            previousLastMove;
    }
}