using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class SanNotationGenerator
{
    // =========================================================
    // GENERATE SAN
    // =========================================================

    public static string Generate(
        Board board,
        Move move,
        Piece movedPiece,
        Piece? capturedPiece = null,
        PieceType? promotionType = null,
        bool isCheck = false,
        bool isCheckmate = false,
        bool isCastling = false)
    {
        // =====================================================
        // CASTLING
        // =====================================================

        if (isCastling)
        {
            if (move.To.Column > move.From.Column)
            {
                return isCheckmate
                    ? "O-O#"
                    : isCheck
                        ? "O-O+"
                        : "O-O";
            }

            return isCheckmate
                ? "O-O-O#"
                : isCheck
                    ? "O-O-O+"
                    : "O-O";
        }

        // =====================================================
        // PIECE SYMBOL
        // =====================================================

        string san = "";

        if (movedPiece.Type != PieceType.Pawn)
        {
            san += GetPieceSymbol(
                movedPiece.Type
            );
        }

        // =====================================================
        // CAPTURE
        // =====================================================

        bool isCapture =
            capturedPiece != null;

        // =====================================================
        // PAWN CAPTURE
        // =====================================================

        if (movedPiece.Type == PieceType.Pawn &&
            isCapture)
        {
            san +=
                (char)('a' + move.From.Column);
        }

        // =====================================================
        // "x"
        // =====================================================

        if (isCapture)
        {
            san += "x";
        }

        // =====================================================
        // DESTINATION
        // =====================================================

        san +=
            PositionToChessNotation(
                move.To
            );

        // =====================================================
        // PROMOTION
        // =====================================================

        if (promotionType != null)
        {
            san += "=";

            san += GetPieceSymbol(
                promotionType.Value
            );
        }

        // =====================================================
        // CHECK / CHECKMATE
        // =====================================================

        if (isCheckmate)
        {
            san += "#";
        }
        else if (isCheck)
        {
            san += "+";
        }

        return san;
    }

    // =========================================================
    // PIECE SYMBOL
    // =========================================================

    private static string GetPieceSymbol(
        PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.King =>
                "K",

            PieceType.Queen =>
                "Q",

            PieceType.Rook =>
                "R",

            PieceType.Bishop =>
                "B",

            PieceType.Knight =>
                "N",

            PieceType.Pawn =>
                "",

            _ =>
                ""
        };
    }

    // =========================================================
    // POSITION -> CHESS NOTATION
    // =========================================================

    private static string PositionToChessNotation(
        Position position)
    {
        char file =
            (char)(
                'a' +
                position.Column
            );

        char rank =
            (char)(
                '8' -
                position.Row
            );

        return $"{file}{rank}";
    }
}