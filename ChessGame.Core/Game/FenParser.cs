using ChessGame.Core.Models;

namespace ChessGame.Core.Game;

public static class FenParser
{
    public static Board ParseBoard(string fen)
    {
        if (string.IsNullOrWhiteSpace(fen))
        {
            throw new ArgumentException(
                "FEN cannot be empty.",
                nameof(fen)
            );
        }

        string[] parts =
            fen.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );

        if (parts.Length < 1)
        {
            throw new ArgumentException(
                "Invalid FEN."
            );
        }

        string boardPart = parts[0];

        string[] ranks =
            boardPart.Split('/');

        if (ranks.Length != Board.Size)
        {
            throw new ArgumentException(
                "FEN board must contain exactly 8 ranks."
            );
        }

        Board board = new Board();

        for (int row = 0; row < Board.Size; row++)
        {
            ParseRank(
                board,
                ranks[row],
                row
            );
        }

        return board;
    }

    public static FenState Parse(string fen)
    {
        if (string.IsNullOrWhiteSpace(fen))
        {
            throw new ArgumentException(
                "FEN cannot be empty.",
                nameof(fen)
            );
        }

        string[] parts =
            fen.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );

        if (parts.Length != 6)
        {
            throw new ArgumentException(
                "FEN must contain exactly 6 fields."
            );
        }

        Board board =
            ParseBoard(fen);

        string castling =
            parts[2];

        ApplyCastlingRights(
            board,
            castling
        );

        PieceColor sideToMove =
            parts[1] switch
            {
                "w" => PieceColor.White,
                "b" => PieceColor.Black,

                _ => throw new ArgumentException(
                    "Invalid FEN active color."
                )
            };

        bool whiteKingSide =
            castling.Contains('K');

        bool whiteQueenSide =
            castling.Contains('Q');

        bool blackKingSide =
            castling.Contains('k');

        bool blackQueenSide =
            castling.Contains('q');

        Position? enPassantTarget =
            ParseEnPassantTarget(
                parts[3]
            );

        if (!int.TryParse(
                parts[4],
                out int halfmoveClock) ||
            halfmoveClock < 0)
        {
            throw new ArgumentException(
                "Invalid FEN halfmove clock."
            );
        }

        if (!int.TryParse(
                parts[5],
                out int fullmoveNumber) ||
            fullmoveNumber < 1)
        {
            throw new ArgumentException(
                "Invalid FEN fullmove number."
            );
        }

        return new FenState(
            board,
            sideToMove,
            whiteKingSide,
            whiteQueenSide,
            blackKingSide,
            blackQueenSide,
            enPassantTarget,
            halfmoveClock,
            fullmoveNumber
        );
    }

    private static Position? ParseEnPassantTarget(
        string value)
    {
        if (value == "-")
        {
            return null;
        }

        if (value.Length != 2)
        {
            throw new ArgumentException(
                "Invalid FEN en passant square."
            );
        }

        char file = value[0];
        char rank = value[1];

        if (file < 'a' ||
            file > 'h' ||
            rank < '1' ||
            rank > '8')
        {
            throw new ArgumentException(
                "Invalid FEN en passant square."
            );
        }

        int column =
            file - 'a';

        int row =
            8 - (rank - '0');

        return new Position(
            row,
            column
        );
    }

    private static void ParseRank(
        Board board,
        string rank,
        int row)
    {
        int column = 0;

        foreach (char character in rank)
        {
            if (character >= '1' &&
                character <= '8')
            {
                column +=
                    character - '0';

                continue;
            }

            if (column >= Board.Size)
            {
                throw new ArgumentException(
                    "Invalid FEN rank."
                );
            }

            Piece piece =
                CreatePiece(character);

            board.SetPiece(
                new Position(row, column),
                piece
            );

            column++;
        }

        if (column != Board.Size)
        {
            throw new ArgumentException(
                "Invalid FEN rank."
            );
        }
    }

    private static Piece CreatePiece(
        char character)
    {
        PieceColor color =
            char.IsUpper(character)
                ? PieceColor.White
                : PieceColor.Black;

        PieceType type =
            char.ToLowerInvariant(character) switch
            {
                'p' => PieceType.Pawn,
                'r' => PieceType.Rook,
                'n' => PieceType.Knight,
                'b' => PieceType.Bishop,
                'q' => PieceType.Queen,
                'k' => PieceType.King,

                _ => throw new ArgumentException(
                    $"Invalid FEN piece character: {character}"
                )
            };

        return new Piece(
            type,
            color
        );
    }

    private static void ApplyCastlingRights(
        Board board,
        string castling)
    {
        // ---------------------------------------------------------
        // WHITE KING
        // ---------------------------------------------------------

        Piece? whiteKing =
            board.GetPiece(
                new Position(7, 4)
            );

        if (whiteKing != null &&
            whiteKing.Type == PieceType.King &&
            whiteKing.Color == PieceColor.White)
        {
            whiteKing.RestoreMovedState(
                !castling.Contains('K') &&
                !castling.Contains('Q')
            );
        }

        // ---------------------------------------------------------
        // WHITE KING-SIDE ROOK
        // ---------------------------------------------------------

        Piece? whiteKingSideRook =
            board.GetPiece(
                new Position(7, 7)
            );

        if (whiteKingSideRook != null &&
            whiteKingSideRook.Type == PieceType.Rook &&
            whiteKingSideRook.Color == PieceColor.White)
        {
            whiteKingSideRook.RestoreMovedState(
                !castling.Contains('K')
            );
        }

        // ---------------------------------------------------------
        // WHITE QUEEN-SIDE ROOK
        // ---------------------------------------------------------

        Piece? whiteQueenSideRook =
            board.GetPiece(
                new Position(7, 0)
            );

        if (whiteQueenSideRook != null &&
            whiteQueenSideRook.Type == PieceType.Rook &&
            whiteQueenSideRook.Color == PieceColor.White)
        {
            whiteQueenSideRook.RestoreMovedState(
                !castling.Contains('Q')
            );
        }

        // ---------------------------------------------------------
        // BLACK KING
        // ---------------------------------------------------------

        Piece? blackKing =
            board.GetPiece(
                new Position(0, 4)
            );

        if (blackKing != null &&
            blackKing.Type == PieceType.King &&
            blackKing.Color == PieceColor.Black)
        {
            blackKing.RestoreMovedState(
                !castling.Contains('k') &&
                !castling.Contains('q')
            );
        }

        // ---------------------------------------------------------
        // BLACK KING-SIDE ROOK
        // ---------------------------------------------------------

        Piece? blackKingSideRook =
            board.GetPiece(
                new Position(0, 7)
            );

        if (blackKingSideRook != null &&
            blackKingSideRook.Type == PieceType.Rook &&
            blackKingSideRook.Color == PieceColor.Black)
        {
            blackKingSideRook.RestoreMovedState(
                !castling.Contains('k')
            );
        }

        // ---------------------------------------------------------
        // BLACK QUEEN-SIDE ROOK
        // ---------------------------------------------------------

        Piece? blackQueenSideRook =
            board.GetPiece(
                new Position(0, 0)
            );

        if (blackQueenSideRook != null &&
            blackQueenSideRook.Type == PieceType.Rook &&
            blackQueenSideRook.Color == PieceColor.Black)
        {
            blackQueenSideRook.RestoreMovedState(
                !castling.Contains('q')
            );
        }
    }
}