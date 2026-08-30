namespace ChessGame.Core.Models;

public class FenState
{
    public Board Board { get; }

    public PieceColor SideToMove { get; }

    public bool WhiteKingSideCastling { get; }

    public bool WhiteQueenSideCastling { get; }

    public bool BlackKingSideCastling { get; }

    public bool BlackQueenSideCastling { get; }

    public Position? EnPassantTarget { get; }

    public int HalfmoveClock { get; }

    public int FullmoveNumber { get; }

    public FenState(
        Board board,
        PieceColor sideToMove,
        bool whiteKingSideCastling,
        bool whiteQueenSideCastling,
        bool blackKingSideCastling,
        bool blackQueenSideCastling,
        Position? enPassantTarget,
        int halfmoveClock,
        int fullmoveNumber)
    {
        Board = board;

        SideToMove = sideToMove;

        WhiteKingSideCastling =
            whiteKingSideCastling;

        WhiteQueenSideCastling =
            whiteQueenSideCastling;

        BlackKingSideCastling =
            blackKingSideCastling;

        BlackQueenSideCastling =
            blackQueenSideCastling;

        EnPassantTarget =
            enPassantTarget;

        HalfmoveClock =
            halfmoveClock;

        FullmoveNumber =
            fullmoveNumber;
    }
}