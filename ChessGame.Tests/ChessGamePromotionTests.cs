using ChessGame.Core.Game;
using ChessGame.Core.Models;
using ChessGameModel = ChessGame.Core.Game.ChessGame;

namespace ChessGame.Tests;

public class ChessGamePromotionTests
{
    [Fact]
    public void TryMove_ShouldPromotePawnToQueen()
    {
        // White pawn on e7.
        // Black king is safely away from e8.
        string fen =
            "7k/4P3/8/8/8/8/8/4K3 w - - 0 1";

        ChessGameModel game =
            ChessGameModel.FromFen(fen);

        Move move =
            new Move(
                new Position(1, 4),
                new Position(0, 4)
            );

        bool result =
            game.TryMove(
                move,
                PieceColor.White,
                PieceType.Queen
            );

        Assert.True(result);

        Piece? piece =
            game.Board.GetPiece(
                new Position(0, 4)
            );

        Assert.NotNull(piece);

        Assert.Equal(
            PieceType.Queen,
            piece!.Type
        );

        Assert.Equal(
            PieceColor.White,
            piece.Color
        );
    }
}