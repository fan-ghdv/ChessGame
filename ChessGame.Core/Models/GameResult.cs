namespace ChessGame.Core.Models;

public enum GameResult
{
    Ongoing,

    WhiteWins,

    BlackWins,

    Stalemate,

    ThreefoldRepetition,

    FiftyMoveDraw,

    InsufficientMaterial
}