using System.Text;
using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class FenGenerator
{
    public static string Generate(
        Board board,
        PieceColor sideToMove,
        Move? lastMove = null)
    {
        StringBuilder boardPart =
            new StringBuilder();

        for (int row = 0; row < Board.Size; row++)
        {
            int emptySquares = 0;

            for (int column = 0; column < Board.Size; column++)
            {
                Piece? piece =
                    board.GetPiece(
                        new Position(row, column)
                    );

                if (piece == null)
                {
                    emptySquares++;
                    continue;
                }

                if (emptySquares > 0)
                {
                    boardPart.Append(emptySquares);
                    emptySquares = 0;
                }

                boardPart.Append(
                    GetPieceCharacter(piece)
                );
            }

            if (emptySquares > 0)
            {
                boardPart.Append(emptySquares);
            }

            if (row < Board.Size - 1)
            {
                boardPart.Append('/');
            }
        }

        string activeColor =
            sideToMove == PieceColor.White
                ? "w"
                : "b";

        string castlingRights =
            GetCastlingRights(board);

        string enPassantTarget =
            GetEnPassantTargetSquare(
                board,
                lastMove
            );

        return
            $"{boardPart} {activeColor} {castlingRights} {enPassantTarget} {board.HalfmoveClock} {board.FullmoveNumber}";
    }

    // =========================================================
    // PIECE CHARACTER
    // =========================================================

    private static char GetPieceCharacter(
        Piece piece)
    {
        char character =
            piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Rook => 'r',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                PieceType.King => 'k',

                _ => throw new ArgumentOutOfRangeException()
            };

        if (piece.Color == PieceColor.White)
        {
            character =
                char.ToUpperInvariant(
                    character
                );
        }

        return character;
    }

    // =========================================================
    // CASTLING RIGHTS
    // =========================================================

    private static string GetCastlingRights(
        Board board)
    {
        StringBuilder rights =
            new StringBuilder();

        // -----------------------------------------------------
        // WHITE KING-SIDE: K
        // -----------------------------------------------------

        Piece? whiteKing =
            board.GetPiece(
                new Position(7, 4)
            );

        Piece? whiteKingSideRook =
            board.GetPiece(
                new Position(7, 7)
            );

        if (CanCastleWithPieces(
                whiteKing,
                whiteKingSideRook,
                PieceColor.White))
        {
            rights.Append('K');
        }

        // -----------------------------------------------------
        // WHITE QUEEN-SIDE: Q
        // -----------------------------------------------------

        Piece? whiteQueenSideRook =
            board.GetPiece(
                new Position(7, 0)
            );

        if (CanCastleWithPieces(
                whiteKing,
                whiteQueenSideRook,
                PieceColor.White))
        {
            rights.Append('Q');
        }

        // -----------------------------------------------------
        // BLACK KING-SIDE: k
        // -----------------------------------------------------

        Piece? blackKing =
            board.GetPiece(
                new Position(0, 4)
            );

        Piece? blackKingSideRook =
            board.GetPiece(
                new Position(0, 7)
            );

        if (CanCastleWithPieces(
                blackKing,
                blackKingSideRook,
                PieceColor.Black))
        {
            rights.Append('k');
        }

        // -----------------------------------------------------
        // BLACK QUEEN-SIDE: q
        // -----------------------------------------------------

        Piece? blackQueenSideRook =
            board.GetPiece(
                new Position(0, 0)
            );

        if (CanCastleWithPieces(
                blackKing,
                blackQueenSideRook,
                PieceColor.Black))
        {
            rights.Append('q');
        }

        if (rights.Length == 0)
        {
            return "-";
        }

        return rights.ToString();
    }

    private static string GetEnPassantTargetSquare(
        Board board,
        Move? lastMove)
    {
        if (lastMove == null)
        {
            return "-";
        }

        Piece? movedPiece =
            board.GetPiece(lastMove.To);

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
            (lastMove.From.Row +
            lastMove.To.Row) / 2;

        Position targetPosition =
            new Position(
                targetRow,
                lastMove.From.Column
            );

        return ToAlgebraicSquare(
            targetPosition
        );
    }

    private static string ToAlgebraicSquare(
        Position position)
    {
        char file =
            (char)('a' + position.Column);

        char rank =
            (char)('8' - position.Row);

        return $"{file}{rank}";
    }

    // =========================================================
    // CHECK KING + ROOK CASTLING STATE
    // =========================================================

    private static bool CanCastleWithPieces(
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

        if (king.HasMoved ||
            rook.HasMoved)
        {
            return false;
        }

        return true;
    }
}