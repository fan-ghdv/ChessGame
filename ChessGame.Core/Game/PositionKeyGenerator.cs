using System.Text;
using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class PositionKeyGenerator
{
    public static string Generate(
        Board board)
    {
        return Generate(
            board,
            null,
            null
        );
    }

    public static string Generate(
        Board board,
        PieceColor? sideToMove,
        Move? lastMove)
    {
        var result =
            new StringBuilder();

        // =====================================================
        // BOARD POSITION
        // =====================================================

        for (
            int row = 0;
            row < Board.Size;
            row++)
        {
            for (
                int column = 0;
                column < Board.Size;
                column++)
            {
                Position position =
                    new Position(
                        row,
                        column
                    );

                Piece? piece =
                    board.GetPiece(position);

                if (piece == null)
                {
                    result.Append('.');
                    continue;
                }

                result.Append(
                    GetPieceSymbol(piece)
                );
            }
        }

        // =====================================================
        // SIDE TO MOVE
        // =====================================================

        result.Append('|');

        if (sideToMove == null)
        {
            result.Append('-');
        }
        else
        {
            result.Append(
                sideToMove == PieceColor.White
                    ? 'w'
                    : 'b'
            );
        }

        // =====================================================
        // CASTLING RIGHTS
        // =====================================================

        result.Append('|');

        result.Append(
            GetCastlingRights(board)
        );

        // =====================================================
        // EN PASSANT
        // =====================================================

        result.Append('|');

        result.Append(
            GetEnPassantTarget(
                board,
                lastMove
            )
        );

        return result.ToString();
    }

    // =========================================================
    // PIECE SYMBOL
    // =========================================================

    private static char GetPieceSymbol(
        Piece piece)
    {
        char symbol =
            piece.Type switch
            {
                PieceType.King => 'K',
                PieceType.Queen => 'Q',
                PieceType.Rook => 'R',
                PieceType.Bishop => 'B',
                PieceType.Knight => 'N',
                PieceType.Pawn => 'P',
                _ => '?'
            };

        if (piece.Color ==
            PieceColor.Black)
        {
            symbol =
                char.ToLowerInvariant(
                    symbol
                );
        }

        return symbol;
    }

    // =========================================================
    // CASTLING RIGHTS
    // =========================================================

    private static string GetCastlingRights(
        Board board)
    {
        var result =
            new StringBuilder();

        Piece? whiteKing =
            board.GetPiece(
                new Position(7, 4)
            );

        Piece? whiteKingRook =
            board.GetPiece(
                new Position(7, 7)
            );

        Piece? whiteQueenRook =
            board.GetPiece(
                new Position(7, 0)
            );

        Piece? blackKing =
            board.GetPiece(
                new Position(0, 4)
            );

        Piece? blackKingRook =
            board.GetPiece(
                new Position(0, 7)
            );

        Piece? blackQueenRook =
            board.GetPiece(
                new Position(0, 0)
            );

        // White king-side
        if (CanCastle(
                whiteKing,
                whiteKingRook,
                PieceColor.White))
        {
            result.Append('K');
        }

        // White queen-side
        if (CanCastle(
                whiteKing,
                whiteQueenRook,
                PieceColor.White))
        {
            result.Append('Q');
        }

        // Black king-side
        if (CanCastle(
                blackKing,
                blackKingRook,
                PieceColor.Black))
        {
            result.Append('k');
        }

        // Black queen-side
        if (CanCastle(
                blackKing,
                blackQueenRook,
                PieceColor.Black))
        {
            result.Append('q');
        }

        if (result.Length == 0)
        {
            return "-";
        }

        return result.ToString();
    }

    private static bool CanCastle(
        Piece? king,
        Piece? rook,
        PieceColor color)
    {
        if (king == null ||
            rook == null)
        {
            return false;
        }

        if (king.Type != PieceType.King ||
            rook.Type != PieceType.Rook)
        {
            return false;
        }

        if (king.Color != color ||
            rook.Color != color)
        {
            return false;
        }

        return
            !king.HasMoved &&
            !rook.HasMoved;
    }

    // =========================================================
    // EN PASSANT
    // =========================================================

    private static string GetEnPassantTarget(
        Board board,
        Move? lastMove)
    {
        if (lastMove == null)
        {
            return "-";
        }

        Piece? movedPiece =
            board.GetPiece(
                lastMove.To
            );

        if (movedPiece == null ||
            movedPiece.Type != PieceType.Pawn)
        {
            return "-";
        }

        int rowDifference =
            Math.Abs(
                lastMove.To.Row -
                lastMove.From.Row
            );

        if (rowDifference != 2)
        {
            return "-";
        }

        int targetRow =
            (
                lastMove.From.Row +
                lastMove.To.Row
            ) / 2;

        Position target =
            new Position(
                targetRow,
                lastMove.To.Column
            );

        return
            $"{target.Row},{target.Column}";
    }
}