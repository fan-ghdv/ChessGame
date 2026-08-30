using ChessGame.Core.Game;
using ChessGame.Core.Models;

namespace ChessGame.Tests;

public class PositionKeyGeneratorTests
{
    [Fact]
    public void EmptyBoards_ShouldHaveSameKey()
    {
        var board1 = new Board();
        var board2 = new Board();

        string key1 =
            PositionKeyGenerator.Generate(
                board1
            );

        string key2 =
            PositionKeyGenerator.Generate(
                board2
            );

        Assert.Equal(
            key1,
            key2
        );
    }

    [Fact]
    public void SamePosition_ShouldHaveSameKey()
    {
        var board1 = new Board();
        var board2 = new Board();

        board1.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board2.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        string key1 =
            PositionKeyGenerator.Generate(
                board1
            );

        string key2 =
            PositionKeyGenerator.Generate(
                board2
            );

        Assert.Equal(
            key1,
            key2
        );
    }

    [Fact]
    public void DifferentPosition_ShouldHaveDifferentKey()
    {
        var board1 = new Board();
        var board2 = new Board();

        board1.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board2.SetPiece(
            new Position(7, 3),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        string key1 =
            PositionKeyGenerator.Generate(
                board1
            );

        string key2 =
            PositionKeyGenerator.Generate(
                board2
            );

        Assert.NotEqual(
            key1,
            key2
        );
    }

    [Fact]
    public void DifferentPiece_ShouldHaveDifferentKey()
    {
        var board1 = new Board();
        var board2 = new Board();

        board1.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board2.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.Queen,
                PieceColor.White
            )
        );

        string key1 =
            PositionKeyGenerator.Generate(
                board1
            );

        string key2 =
            PositionKeyGenerator.Generate(
                board2
            );

        Assert.NotEqual(
            key1,
            key2
        );
    }

    [Fact]
    public void DifferentColor_ShouldHaveDifferentKey()
    {
        var board1 = new Board();
        var board2 = new Board();

        board1.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.White
            )
        );

        board2.SetPiece(
            new Position(7, 4),
            new Piece(
                PieceType.King,
                PieceColor.Black
            )
        );

        string key1 =
            PositionKeyGenerator.Generate(
                board1
            );

        string key2 =
            PositionKeyGenerator.Generate(
                board2
            );

        Assert.NotEqual(
            key1,
            key2
        );
    }
}