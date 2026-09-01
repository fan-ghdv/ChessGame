using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using ChessGame.Core.Game;
using ChessGame.Core.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChessGame.Avalonia;

public partial class MainWindow : Window
{
    private ChessGame.Core.Game.ChessGame _game;

    private enum GameMode
    {
        LocalPlayer,
        AI
    }

    private enum AIDifficulty
    {
        Easy,
        Normal,
        Hard,
        Bot
    }

    private enum PlayerColorChoice
    {
        White,
        Black,
        Random
    }

    private GameMode selectedGameMode =
        GameMode.LocalPlayer;

    private AIDifficulty? selectedDifficulty = null;

    private PlayerColorChoice? selectedPlayerColor = null;

    private PlayerColorChoice? actualPlayerColor;

    private void ShowMainMenu()
    {
        MainMenuPanel.IsVisible = true;
        AISetupPanel.IsVisible = false;
        GamePanel.IsVisible = false;

        selectedDifficulty = null;
        selectedPlayerColor = null;

        UpdateAIStartButton();
    }

    private void LocalPlayerButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        selectedGameMode =
            GameMode.LocalPlayer;

        StartGame();
    }

    private void AIButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        selectedGameMode =
            GameMode.AI;

        selectedDifficulty = null;
        selectedPlayerColor = null;
        actualPlayerColor = null;

        DifficultyComboBox.SelectedIndex = -1;
        ColorComboBox.SelectedIndex = -1;

        MainMenuPanel.IsVisible = false;
        AISetupPanel.IsVisible = true;
        GamePanel.IsVisible = false;

        AIStartButton.IsEnabled = false;
    }

    private void DifficultyComboBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (DifficultyComboBox.SelectedItem is ComboBoxItem item)
        {
            selectedDifficulty =
                item.Tag?.ToString() switch
                {
                    "Easy" =>
                        AIDifficulty.Easy,

                    "Normal" =>
                        AIDifficulty.Normal,

                    "Hard" =>
                        AIDifficulty.Hard,

                    "Bot" =>
                        AIDifficulty.Bot,

                    _ =>
                        null
                };
        }

        UpdateAIStartButton();
    }

    private void ColorComboBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (ColorComboBox.SelectedItem is ComboBoxItem item)
        {
            selectedPlayerColor =
                item.Tag?.ToString() switch
                {
                    "White" =>
                        PlayerColorChoice.White,

                    "Black" =>
                        PlayerColorChoice.Black,

                    "Random" =>
                        PlayerColorChoice.Random,

                    _ =>
                        null
                };
        }

        UpdateAIStartButton();
    }

    private void UpdateAIStartButton()
    {
        AIStartButton.IsEnabled =
            selectedDifficulty.HasValue &&
            selectedPlayerColor.HasValue;
    }

    private void AIStartButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (!selectedDifficulty.HasValue ||
            !selectedPlayerColor.HasValue)
        {
            return;
        }

        StartGame();
    }

    private void StartGame()
    {
        // =========================================================
        // CREATE NEW GAME
        // =========================================================

        _game =
            new ChessGame.Core.Game.ChessGame();

        selectedPosition = null;

        gameOver = false;

        viewingHistory = false;

        historyViewGame = null;


        // =========================================================
        // LOCAL PLAYER
        // =========================================================

        if (selectedGameMode ==
            GameMode.LocalPlayer)
        {
            // White starts from White perspective
            blackPerspective = false;
        }


        // =========================================================
        // AI
        // =========================================================

        if (selectedGameMode ==
            GameMode.AI)
        {
            PlayerColorChoice selectedColor =
                selectedPlayerColor!.Value;

            // -----------------------------------------------------
            // RANDOM COLOR
            // -----------------------------------------------------

            if (selectedColor ==
                PlayerColorChoice.Random)
            {
                actualPlayerColor =
                    Random.Shared.Next(2) == 0
                        ? PlayerColorChoice.White
                        : PlayerColorChoice.Black;
            }
            // =====================================================
            // WHITE / BLACK
            // =====================================================

            else
            {
                actualPlayerColor =
                    selectedColor;
            }

            // -----------------------------------------------------
            // BOARD PERSPECTIVE
            // -----------------------------------------------------

            blackPerspective =
                actualPlayerColor ==
                PlayerColorChoice.Black;
        }


        // =========================================================
        // SHOW GAME
        // =========================================================

        MainMenuPanel.IsVisible = false;

        AISetupPanel.IsVisible = false;

        GamePanel.IsVisible = true;


        // =========================================================
        // HISTORY / ROTATE BUTTONS
        // =========================================================

        RotateBoardButton.IsVisible = false;

        ExitHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsVisible = false;
        NextHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsEnabled = false;
        NextHistoryButton.IsEnabled = false;


        // =========================================================
        // CREATE BOARD
        // =========================================================

        CreateBoard();

        UpdateMoveHistory();

        UpdateCapturedPieces();

        UpdateGameState();

        // =========================================================
        // AI FIRST MOVE
        // =========================================================

        if (selectedGameMode == GameMode.AI &&
            actualPlayerColor == PlayerColorChoice.Black)
        {
            MakeAIMove();
        }
    }

    private Position? selectedPosition;

    private readonly HashSet<Position> legalMovePositions = new();

    private bool showLegalMoves = true;

    private bool blackPerspective = false;

    private bool gameOver = false;

    // =========================================================
    // AI
    // =========================================================

    private bool aiThinking = false;

    // =========================================================
    // MOVE HISTORY VIEW MODE
    // =========================================================

    private bool viewingHistory = false;

    private ChessGame.Core.Game.ChessGame? historyViewGame = null;

    // =========================================================
    // MOVE HISTORY SCROLL STATE
    // =========================================================

    private bool moveHistoryUserScrolled = false;

    private bool moveHistoryAutoScrolling = false;

    // =========================================================
    // MOVE HISTORY SCROLL CHANGED
    // =========================================================

    private void MoveHistoryScrollViewer_ScrollChanged(
        object? sender,
        ScrollChangedEventArgs e)
    {
        if (moveHistoryAutoScrolling)
        {
            return;
        }

        double maxOffset =
            Math.Max(
                0,
                MoveHistoryScrollViewer.Extent.Height -
                MoveHistoryScrollViewer.Viewport.Height
            );

        double currentOffset =
            MoveHistoryScrollViewer.Offset.Y;

        const double tolerance = 5.0;

        bool isAtBottom =
            currentOffset >= maxOffset - tolerance;

        if (isAtBottom)
        {
            moveHistoryUserScrolled = false;
        }
        else
        {
            moveHistoryUserScrolled = true;
        }
    }

    // =========================================================
    // SCROLL MOVE HISTORY TO BOTTOM
    // =========================================================

    private void ScrollMoveHistoryToBottom()
    {
        // User manually scrolled up.
        // Do not force the history back down.
        if (moveHistoryUserScrolled)
        {
            return;
        }

        global::Avalonia.Threading.Dispatcher.UIThread.Post(
            () =>
            {
                moveHistoryAutoScrolling = true;

                MoveHistoryScrollViewer.ScrollToEnd();

                moveHistoryAutoScrolling = false;
            },
            global::Avalonia.Threading.DispatcherPriority.Background
        );
    }

    // =========================================================
    // MOVE HISTORY VIEW MODE
    // =========================================================

    private int viewingMoveCount = 0;

    // =========================================================
    // SVG CACHE
    // =========================================================

    private static readonly Dictionary<
        string,
        global::Avalonia.Svg.Skia.SvgSource>
        SvgCache = new();

    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public MainWindow()
    {
        InitializeComponent();

        LegalMovesToggle.IsCheckedChanged +=
            LegalMovesToggle_IsCheckedChanged;

        MoveHistoryScrollViewer.ScrollChanged +=
            MoveHistoryScrollViewer_ScrollChanged;

        _game =
            new ChessGame.Core.Game.ChessGame();

        selectedPosition = null;

        blackPerspective = false;

        gameOver = false;

        viewingHistory = false;

        historyViewGame = null;

        // =====================================================
        // START AT MAIN MENU
        // =====================================================

        MainMenuPanel.IsVisible = true;
        AISetupPanel.IsVisible = false;
        GamePanel.IsVisible = false;

        RotateBoardButton.IsVisible = false;
        ExitHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsVisible = false;
        NextHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsEnabled = false;
        NextHistoryButton.IsEnabled = false;

    }

    // =========================================================
    // CREATE BOARD
    // =========================================================

    private void CreateBoard(
        ChessGame.Core.Game.ChessGame? displayGame = null)
    {
        // =========================================================
        // DETERMINE WHICH GAME TO DISPLAY
        // =========================================================

        ChessGame.Core.Game.ChessGame gameToDisplay =
            displayGame ?? _game;

        // =========================================================
        // CLEAR BOARD
        // =========================================================

        ChessBoard.Children.Clear();

        ChessBoard.RowDefinitions.Clear();

        ChessBoard.ColumnDefinitions.Clear();

        // =========================================================
        // CREATE 8 x 8 GRID
        // =========================================================

        for (int i = 0; i < 8; i++)
        {
            ChessBoard.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Star
                )
            );

            ChessBoard.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Star
                )
            );
        }

        // =========================================================
        // CREATE BOARD SQUARES
        // =========================================================

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Position position =
                    new Position(
                        row,
                        col
                    );

                // =================================================
                // SQUARE
                // =================================================

                // =========================================================
                // BASE SQUARE COLOR
                // =========================================================

                Color baseColor =
                    (row + col) % 2 == 0
                        ? Color.Parse("#F0D9B5")
                        : Color.Parse("#B58863");

                // =========================================================
                // GAME OVER KING COLOR
                // =========================================================

                Color? gameOverKingColor =
                    GetGameOverKingColor(
                        gameToDisplay,
                        position
                    );

                var square =
                    new Border
                    {
                        Background =
                            new SolidColorBrush(
                                gameOverKingColor ??
                                baseColor
                            ),

                        BorderBrush =
                            new SolidColorBrush(
                                Color.Parse("#555555")
                            ),

                        BorderThickness =
                            new Thickness(0.7),

                        Padding =
                            new Thickness(0)
                    };

                // =================================================
                // SELECTED SQUARE
                // =================================================

                if (!viewingHistory &&
                    selectedPosition.HasValue &&
                    selectedPosition.Value == position)
                {
                    square.Background =
                        new SolidColorBrush(
                            Color.Parse(
                                "#F7EC59"
                            )
                        );
                }

                // =================================================
                // CASTLING MOVE HIGHLIGHT
                // =================================================

                if (!viewingHistory &&
                    showLegalMoves &&
                    castlingMovePositions.Contains(position))
                {
                    square.Background =
                        new SolidColorBrush(
                            Color.Parse("#5B8DEF")
                        );
                }

                // =================================================
                // NORMAL LEGAL MOVE HIGHLIGHT
                // =================================================

                if (!viewingHistory &&
                    showLegalMoves &&
                    legalMovePositions.Contains(position) &&
                    !castlingMovePositions.Contains(position) &&
                    !(selectedPosition.HasValue &&
                    selectedPosition.Value == position))
                {
                    Color legalMoveColor =
                        (row + col) % 2 == 0
                            ? Color.Parse("#D8C7F2")
                            : Color.Parse("#A98CCF");

                    square.Background =
                        new SolidColorBrush(
                            legalMoveColor
                        );
                }

                // =================================================
                // CLICK
                // =================================================

                square.PointerPressed +=
                    (_, _) =>
                        HandleSquareClick(
                            position
                        );

                // =================================================
                // PIECE
                // =================================================

                Piece? piece =
                    gameToDisplay.Board.GetPiece(
                        position
                    );

                if (piece != null)
                {
                    AddPiece(
                        square,
                        piece
                    );
                }

                // =================================================
                // BOARD PERSPECTIVE
                // =================================================

                int displayRow =
                    blackPerspective
                        ? 7 - row
                        : row;

                int displayColumn =
                    blackPerspective
                        ? 7 - col
                        : col;

                Grid.SetRow(
                    square,
                    displayRow
                );

                Grid.SetColumn(
                    square,
                    displayColumn
                );

                ChessBoard.Children.Add(
                    square
                );
            }
        }

        // =========================================================
        // UPDATE COORDINATES
        // =========================================================

        UpdateBoardCoordinates();

    }

    private void LegalMovesToggle_IsCheckedChanged(
        object? sender,
        RoutedEventArgs e)
    {
        showLegalMoves =
            LegalMovesToggle.IsChecked == true;

        if (!showLegalMoves)
        {
            legalMovePositions.Clear();
            castlingMovePositions.Clear();
        }
        else if (selectedPosition.HasValue)
        {
            UpdateLegalMovePositions(
                selectedPosition.Value
            );
        }

        CreateBoard();
        UpdateCapturedPieces();
    }

    private void AddCastlingMovePositions(
        Position kingPosition,
        PieceColor color)
    {
        // =========================================================
        // KING MUST BE ON ORIGINAL SQUARE
        // =========================================================

        int kingRow =
            color == PieceColor.White
                ? 7
                : 0;

        if (kingPosition !=
            new Position(kingRow, 4))
        {
            return;
        }

        Piece? king =
            _game.Board.GetPiece(
                kingPosition
            );

        if (king == null ||
            king.Type != PieceType.King ||
            king.Color != color ||
            king.HasMoved)
        {
            return;
        }

        // =========================================================
        // KING-SIDE CASTLING
        // =========================================================

        if (CastlingValidator.CanCastleKingSide(
                _game.Board,
                color))
        {
            castlingMovePositions.Add(
                new Position(kingRow, 6)
            );
        }

        // =========================================================
        // QUEEN-SIDE CASTLING
        // =========================================================

        if (CastlingValidator.CanCastleQueenSide(
                _game.Board,
                color))
        {
            castlingMovePositions.Add(
                new Position(kingRow, 2)
            );
        }
    }

    private readonly HashSet<Position> castlingMovePositions = new();

    private void UpdateLegalMovePositions(
        Position from)
    {
        legalMovePositions.Clear();
        castlingMovePositions.Clear();

        if (!showLegalMoves)
        {
            return;
        }

        Piece? piece =
            _game.Board.GetPiece(from);

        if (piece == null)
        {
            return;
        }

        if (piece.Color != _game.SideToMove)
        {
            return;
        }

        // =========================================================
        // NORMAL LEGAL MOVES
        // =========================================================

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                Position to =
                    new Position(
                        row,
                        column
                    );

                if (to == from)
                {
                    continue;
                }

                Move move =
                    new Move(
                        from,
                        to
                    );

                if (MoveValidator.IsLegalMove(
                        _game.Board,
                        move,
                        piece.Color))
                {
                    legalMovePositions.Add(to);
                }
            }
        }

        // =========================================================
        // CASTLING
        // =========================================================

        if (piece.Type == PieceType.King)
        {
            AddCastlingMovePositions(
                from,
                piece.Color
            );
        }
    }

    // =========================================================
    // GET GAME OVER KING SQUARE COLOR
    // =========================================================

    private Color? GetGameOverKingColor(
        ChessGame.Core.Game.ChessGame gameToDisplay,
        Position position)
    {
        // =====================================================
        // HISTORY MODE
        // =====================================================

        if (viewingHistory)
        {
            return null;
        }

        // =====================================================
        // GAME MUST BE OVER
        // =====================================================

        GameResult result =
            gameToDisplay.GetGameResult(
                gameToDisplay.SideToMove
            );

        if (result == GameResult.Ongoing)
        {
            return null;
        }

        // =====================================================
        // GET PIECE
        // =====================================================

        Piece? piece =
            gameToDisplay.Board.GetPiece(
                position
            );

        if (piece == null ||
            piece.Type != PieceType.King)
        {
            return null;
        }

        // =====================================================
        // CHECKMATE
        // =====================================================

        if (result == GameResult.WhiteWins)
        {
            if (piece.Color == PieceColor.White)
            {
                // Winning king
                return Color.Parse("#4CAF50");
            }

            // Losing king
            return Color.Parse("#E74C3C");
        }

        if (result == GameResult.BlackWins)
        {
            if (piece.Color == PieceColor.Black)
            {
                // Winning king
                return Color.Parse("#4CAF50");
            }

            // Losing king
            return Color.Parse("#E74C3C");
        }

        // =====================================================
        // DRAW
        // =====================================================

        if (result == GameResult.Stalemate ||
            result == GameResult.ThreefoldRepetition ||
            result == GameResult.FiftyMoveDraw ||
            result == GameResult.InsufficientMaterial)
        {
            return Color.Parse("#777777");
        }

        return null;
    }

    // =========================================================
    // UPDATE BOARD COORDINATES
    // =========================================================

    private void UpdateBoardCoordinates()
    {
        TopCoordinates.Children.Clear();

        BottomCoordinates.Children.Clear();

        LeftCoordinates.Children.Clear();

        RightCoordinates.Children.Clear();

        // =====================================================
        // FILES
        // =====================================================

        string[] files =
            blackPerspective
                ? new[]
                {
                    "h", "g", "f", "e",
                    "d", "c", "b", "a"
                }
                : new[]
                {
                    "a", "b", "c", "d",
                    "e", "f", "g", "h"
                };

        // =====================================================
        // RANKS
        // =====================================================

        string[] ranks =
            blackPerspective
                ? new[]
                {
                    "1", "2", "3", "4",
                    "5", "6", "7", "8"
                }
                : new[]
                {
                    "8", "7", "6", "5",
                    "4", "3", "2", "1"
                };

        // =====================================================
        // FILE COORDINATES
        // =====================================================

        for (int i = 0; i < 8; i++)
        {
            // -------------------------------------------------
            // TOP
            // -------------------------------------------------

            var topText =
                new TextBlock
                {
                    Text =
                        files[i],

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse(
                                "#DDDDDD"
                            )
                        )
                };

            Grid.SetColumn(
                topText,
                i
            );

            TopCoordinates.Children.Add(
                topText
            );

            // -------------------------------------------------
            // BOTTOM
            // -------------------------------------------------

            var bottomText =
                new TextBlock
                {
                    Text =
                        files[i],

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse(
                                "#DDDDDD"
                            )
                        )
                };

            Grid.SetColumn(
                bottomText,
                i
            );

            BottomCoordinates.Children.Add(
                bottomText
            );
        }

        // =====================================================
        // RANK COORDINATES
        // =====================================================

        for (int i = 0; i < 8; i++)
        {
            // -------------------------------------------------
            // LEFT
            // -------------------------------------------------

            var leftText =
                new TextBlock
                {
                    Text =
                        ranks[i],

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse(
                                "#DDDDDD"
                            )
                        )
                };

            Grid.SetRow(
                leftText,
                i
            );

            LeftCoordinates.Children.Add(
                leftText
            );

            // -------------------------------------------------
            // RIGHT
            // -------------------------------------------------

            var rightText =
                new TextBlock
                {
                    Text =
                        ranks[i],

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse(
                                "#DDDDDD"
                            )
                        )
                };

            Grid.SetRow(
                rightText,
                i
            );

            RightCoordinates.Children.Add(
                rightText
            );
        }
    }

    // =========================================================
    // HANDLE SQUARE CLICK
    // =========================================================

    private void HandleSquareClick(
        Position position)
    {
        // =====================================================
        // HISTORY VIEW
        // =====================================================

        if (viewingHistory)
        {
            return;
        }

        // =====================================================
        // GAME OVER
        // =====================================================

        if (gameOver)
        {
            return;
        }

        // =========================================================
        // AI MODE - PLAYER CAN ONLY CONTROL THEIR OWN COLOR
        // =========================================================

        if (selectedGameMode == GameMode.AI)
        {
            if (!actualPlayerColor.HasValue)
            {
                return;
            }

            PieceColor playerColor =
                actualPlayerColor.Value ==
                    PlayerColorChoice.White
                    ? PieceColor.White
                    : PieceColor.Black;

            if (_game.SideToMove != playerColor)
            {
                return;
            }
        }

        // =====================================================
        // HISTORY VIEW MODE
        // =====================================================

        Piece? clickedPiece =
            _game.Board.GetPiece(
                position
            );

        // =====================================================
        // NOTHING SELECTED
        // =====================================================

        if (!selectedPosition.HasValue)
        {
            if (clickedPiece == null)
            {
                return;
            }

            if (clickedPiece.Color !=
                _game.SideToMove)
            {
                return;
            }

            selectedPosition =
                position;

            UpdateLegalMovePositions(
                position
            );

            CreateBoard();
            UpdateCapturedPieces();

            return;
        }

        // =====================================================
        // SOMETHING SELECTED
        // =====================================================

        Position from =
            selectedPosition.Value;

        // =====================================================
        // SAME SQUARE
        // =====================================================

        if (from == position)
        {
            selectedPosition = null;

            legalMovePositions.Clear();

            castlingMovePositions.Clear();

            CreateBoard();
            UpdateCapturedPieces();

            return;
        }

        // =====================================================
        // FRIENDLY PIECE
        // =====================================================

        if (clickedPiece != null &&
            clickedPiece.Color ==
                _game.SideToMove)
        {
            selectedPosition =
                position;

            UpdateLegalMovePositions(
                position
            );

            CreateBoard();
            UpdateCapturedPieces();

            return;
        }

        // =====================================================
        // CREATE MOVE
        // =====================================================

        Move move =
            new Move(
                from,
                position
            );

        PieceColor movingColor =
            _game.SideToMove;

        Piece? movingPiece =
            _game.Board.GetPiece(
                from
            );

        // =====================================================
        // CHECK PROMOTION
        // =====================================================

        bool isPromotion =
            movingPiece != null &&
            movingPiece.Type ==
                PieceType.Pawn &&
            PawnPromotion.CanPromote(
                position,
                movingColor
            );

        // =====================================================
        // PROMOTION
        // =====================================================

        if (isPromotion)
        {
            ShowPromotionChoice(
                move,
                movingColor
            );

            return;
        }

        // =====================================================
        // NORMAL MOVE
        // =====================================================

        bool success =
            _game.TryMove(
                move,
                movingColor
            );

        if (success &&
            movingPiece != null &&
            SoundEffectsToggle.IsChecked == true)
        {
            ChessSoundPlayer.Play(
                movingPiece.Type
            );
        }

        // =====================================================
        // SUCCESS
        // =====================================================

    if (success)
    {
        Console.WriteLine(
            $"Move successful: " +
            $"{PositionToChessNotation(from)}" +
            $" -> " +
            $"{PositionToChessNotation(position)}"
        );

        selectedPosition = null;

        legalMovePositions.Clear();

        castlingMovePositions.Clear();

        // =====================================================
        // BOARD PERSPECTIVE
        // =====================================================

        if (selectedGameMode ==
            GameMode.LocalPlayer)
        {
            blackPerspective =
                _game.SideToMove ==
                PieceColor.Black;
        }
        else if (selectedGameMode ==
                GameMode.AI)
        {
            blackPerspective =
                actualPlayerColor ==
                PlayerColorChoice.Black;
        }

        CreateBoard();

        UpdateMoveHistory();

        UpdateCapturedPieces();

        UpdateGameState();

        // =====================================================
        // AI TURN
        // =====================================================

        if (selectedGameMode ==
            GameMode.AI)
        {
            MakeAIMove();
        }

        return;
    }

        // =====================================================
        // ILLEGAL MOVE
        // =====================================================

        Console.WriteLine(
            $"Illegal move: " +
            $"{PositionToChessNotation(from)}" +
            $" -> " +
            $"{PositionToChessNotation(position)}"
        );

        CreateBoard();
        UpdateCapturedPieces();
    }

    // =========================================================
    // SHOW PROMOTION CHOICE
    // =========================================================

    private async void ShowPromotionChoice(
        Move move,
        PieceColor color)
    {
        var dialog =
            new Window
            {
                Title =
                    "Pawn Promotion",

                Width =
                    320,

                Height =
                    300,

                MinWidth =
                    320,

                MinHeight =
                    300,

                MaxWidth =
                    320,

                MaxHeight =
                    300,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                CanResize =
                    false,

                Background =
                    new SolidColorBrush(
                        Color.Parse(
                            "#302E2B"
                        )
                    )
            };

        // =====================================================
        // TITLE
        // =====================================================

        var title =
            new TextBlock
            {
                Text =
                    "Choose Promotion",

                FontSize =
                    22,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse(
                            "#F0F0F0"
                        )
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Margin =
                    new Thickness(
                        0,
                        15,
                        0,
                        15
                    )
            };

        // =====================================================
        // BUTTON PANEL
        // =====================================================

        var buttonPanel =
            new StackPanel
            {
                Spacing =
                    10,

                Margin =
                    new Thickness(
                        25,
                        0,
                        25,
                        20
                    )
            };

        // =====================================================
        // QUEEN
        // =====================================================

        var queenButton =
            CreatePromotionButton(
                "♕   Queen"
            );

        queenButton.Click +=
            (_, _) =>
            {
                dialog.Close(
                    PieceType.Queen
                );
            };

        // =====================================================
        // ROOK
        // =====================================================

        var rookButton =
            CreatePromotionButton(
                "♖   Rook"
            );

        rookButton.Click +=
            (_, _) =>
            {
                dialog.Close(
                    PieceType.Rook
                );
            };

        // =====================================================
        // BISHOP
        // =====================================================

        var bishopButton =
            CreatePromotionButton(
                "♗   Bishop"
            );

        bishopButton.Click +=
            (_, _) =>
            {
                dialog.Close(
                    PieceType.Bishop
                );
            };

        // =====================================================
        // KNIGHT
        // =====================================================

        var knightButton =
            CreatePromotionButton(
                "♘   Knight"
            );

        knightButton.Click +=
            (_, _) =>
            {
                dialog.Close(
                    PieceType.Knight
                );
            };

        // =====================================================
        // ADD BUTTONS
        // =====================================================

        buttonPanel.Children.Add(
            queenButton
        );

        buttonPanel.Children.Add(
            rookButton
        );

        buttonPanel.Children.Add(
            bishopButton
        );

        buttonPanel.Children.Add(
            knightButton
        );

        // =====================================================
        // MAIN PANEL
        // =====================================================

        var mainPanel =
            new StackPanel
            {
                Spacing =
                    5
            };

        mainPanel.Children.Add(
            title
        );

        mainPanel.Children.Add(
            buttonPanel
        );

        dialog.Content =
            mainPanel;

        // =====================================================
        // SHOW DIALOG
        // =====================================================

        PieceType? result =
            await dialog.ShowDialog<PieceType?>(
                this
            );

        // =====================================================
        // USER CLOSED WINDOW
        // =====================================================

        if (result == null)
        {
            return;
        }

        // =====================================================
        // EXECUTE PROMOTION
        // =====================================================

        bool success =
            _game.TryMove(
                move,
                color,
                result.Value
            );

        // =====================================================
        // PROMOTION SUCCESS
        // =====================================================

        if (success)
        {
            Console.WriteLine(
                $"Promotion successful: " +
                $"{result.Value}"
            );

            selectedPosition = null;

            legalMovePositions.Clear();

            castlingMovePositions.Clear();


            // =========================================================
            // KEEP PLAYER PERSPECTIVE
            // =========================================================

            if (selectedGameMode == GameMode.AI)
            {
                blackPerspective =
                    actualPlayerColor ==
                    PlayerColorChoice.Black;
            }
            else
            {
                RestoreCurrentGamePerspective();
            }


            // =========================================================
            // REDRAW
            // =========================================================

            CreateBoard();

            UpdateMoveHistory();

            UpdateCapturedPieces();

            UpdateGameState();


            // =========================================================
            // AI TURN AFTER PROMOTION
            // =========================================================

            if (selectedGameMode == GameMode.AI &&
                actualPlayerColor.HasValue &&
                !gameOver &&
                !viewingHistory &&
                !aiThinking)
            {
                PieceColor playerColor =
                    actualPlayerColor.Value ==
                    PlayerColorChoice.White
                        ? PieceColor.White
                        : PieceColor.Black;

                PieceColor aiColor =
                    playerColor == PieceColor.White
                        ? PieceColor.Black
                        : PieceColor.White;

                if (_game.SideToMove == aiColor)
                {
                    Console.WriteLine(
                        "Player promotion completed."
                    );

                    Console.WriteLine(
                        $"AI turn after promotion: {aiColor}"
                    );

                    MakeAIMove();
                }
            }

            return;
        }

        // =====================================================
        // PROMOTION FAILED
        // =====================================================

        Console.WriteLine(
            "Promotion move failed."
        );

        CreateBoard();
        UpdateCapturedPieces();
    }

    // =========================================================
    // CREATE PROMOTION BUTTON
    // =========================================================

    private static Button CreatePromotionButton(
        string text)
    {
        return new Button
        {
            Content =
                text,

            Height =
                42,

            FontSize =
                17,

            HorizontalContentAlignment =
                HorizontalAlignment.Left,

            Padding =
                new Thickness(
                    15,
                    0
                ),

            Background =
                new SolidColorBrush(
                    Color.Parse(
                        "#3A3937"
                    )
                ),

            Foreground =
                new SolidColorBrush(
                    Color.Parse(
                        "#F0F0F0"
                    )
                ),

            BorderThickness =
                new Thickness(0)
        };
    }

    // =========================================================
    // UPDATE MOVE HISTORY
    // =========================================================

    private void UpdateMoveHistory()
    {
        MoveHistoryPanel.Children.Clear();

        IReadOnlyList<MoveRecord> history =
            _game.MoveHistory.GetAll();

        // =========================================================
        // NO MOVES
        // =========================================================

        if (history.Count == 0)
        {
            var emptyText =
                new TextBlock
                {
                    Text =
                        "No moves yet",

                    FontSize =
                        14,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse("#777777")
                        ),

                    HorizontalAlignment =
                        HorizontalAlignment.Center
                };

            MoveHistoryPanel.Children.Add(
                emptyText
            );

            return;
        }

        // =========================================================
        // CREATE MOVE ROWS
        // =========================================================

        for (int i = 0; i < history.Count; i += 2)
        {
            int whiteMoveNumber =
                i + 1;

            int blackMoveNumber =
                i + 2;

            // =====================================================
            // ROW
            // =====================================================

            var row =
                new Grid
                {
                    ColumnDefinitions =
                        new ColumnDefinitions(
                            "35,*,*"
                        ),

                    Margin =
                        new Thickness(
                            0,
                            2
                        )
                };

            // =====================================================
            // MOVE NUMBER
            // =====================================================

            var numberText =
                new TextBlock
                {
                    Text =
                        $"{(i / 2) + 1}.",

                    FontSize =
                        14,

                    Foreground =
                        new SolidColorBrush(
                            Color.Parse("#888888")
                        ),

                    VerticalAlignment =
                        VerticalAlignment.Center
                };

            Grid.SetColumn(
                numberText,
                0
            );

            row.Children.Add(
                numberText
            );

            // =====================================================
            // WHITE MOVE
            // =====================================================

            MoveRecord whiteMove =
                history[i];

            var whiteButton =
                CreateHistoryMoveButton(
                    FormatMove(whiteMove)
                );

            whiteButton.Click +=
                (_, _) =>
                {
                    Console.WriteLine(
                        $"History clicked: move {whiteMoveNumber}"
                    );

                    if (aiThinking)
                    {
                        FooterStatusText.Text = "Waiting for AI...";
                        return;
                    }

                    ShowHistoryPosition(
                        whiteMoveNumber
                    );

                };

            Grid.SetColumn(
                whiteButton,
                1
            );

            row.Children.Add(
                whiteButton
            );

            // =====================================================
            // BLACK MOVE
            // =====================================================

            if (i + 1 < history.Count)
            {
                MoveRecord blackMove =
                    history[i + 1];

                var blackButton =
                    CreateHistoryMoveButton(
                        FormatMove(blackMove)
                    );

                blackButton.Click +=
                    (_, _) =>
                    {
                        Console.WriteLine(
                            $"History clicked: move {blackMoveNumber}"
                        );

                        if (aiThinking)
                        {
                            FooterStatusText.Text = "Waiting for AI...";
                            return;
                        }

                        ShowHistoryPosition(
                            blackMoveNumber
                        );
                    };

                Grid.SetColumn(
                    blackButton,
                    2
                );

                row.Children.Add(
                    blackButton
                );
            }

            // =====================================================
            // ADD ROW
            // =====================================================

            MoveHistoryPanel.Children.Add(
                row
            );
        }

        // =========================================================
        // AUTO SCROLL TO BOTTOM
        // =========================================================

        ScrollMoveHistoryToBottom();
    }

    private static Button CreateHistoryMoveButton(
        string text)
    {
        return new Button
        {
            Content =
                text,

            FontSize =
                14,

            HorizontalAlignment =
                HorizontalAlignment.Stretch,

            HorizontalContentAlignment =
                HorizontalAlignment.Left,

            Padding =
                new Thickness(
                    8,
                    5
                ),

            Background =
                new SolidColorBrush(
                    Color.Parse("#262522")
                ),

            Foreground =
                new SolidColorBrush(
                    Color.Parse("#DDDDDD")
                ),

            BorderThickness =
                new Thickness(0),

            Cursor =
                new Cursor(
                    StandardCursorType.Hand
                )
        };
    }

    private void UpdateHistoryNavigationButtons()
    {
        if (!viewingHistory)
        {
            PreviousHistoryButton.IsVisible = false;
            NextHistoryButton.IsVisible = false;
            return;
        }

        PreviousHistoryButton.IsVisible = true;
        NextHistoryButton.IsVisible = true;

        PreviousHistoryButton.IsEnabled =
            viewingMoveCount > 0;

        NextHistoryButton.IsEnabled =
            viewingMoveCount < _game.MoveHistory.Count;
    }

    private void PreviousHistoryButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (!viewingHistory)
        {
            return;
        }

        if (viewingMoveCount <= 0)
        {
            return;
        }

        ShowHistoryPosition(
            viewingMoveCount - 1
        );

        UpdateHistoryNavigationButtons();
    }

    private void NextHistoryButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (!viewingHistory)
        {
            return;
        }

        int maxMoveCount =
            _game.MoveHistory.Count;

        if (viewingMoveCount >= maxMoveCount)
        {
            return;
        }

        ShowHistoryPosition(
            viewingMoveCount + 1
        );

        UpdateHistoryNavigationButtons();
    }

    // =========================================================
    // CREATE GAME AT HISTORY POSITION
    // =========================================================

    private ChessGame.Core.Game.ChessGame CreateGameAtMove(
        int moveCount)
    {
        var historyGame =
            new ChessGame.Core.Game.ChessGame();

        IReadOnlyList<MoveRecord> history =
            _game.MoveHistory.GetAll();

        for (
            int i = 0;
            i < moveCount && i < history.Count;
            i++)
        {
            MoveRecord record =
                history[i];

            bool success =
                historyGame.TryMove(
                    record.Move,
                    record.Color,
                    record.WasPromotion
                        ? record.PromotionType
                        : null
                );

            if (!success)
            {
                Console.WriteLine(
                    $"Failed to replay move {i + 1}: " +
                    $"{record.Move.From} -> " +
                    $"{record.Move.To}"
                );

                break;
            }
        }

        return historyGame;
    }

    // =========================================================
    // FORMAT MOVE
    // =========================================================

    private static string FormatMove(
        MoveRecord record)
    {
        // =====================================================
        // USE SAN
        // =====================================================

        if (!string.IsNullOrWhiteSpace(
                record.SanNotation))
        {
            return record.SanNotation;
        }

        // =====================================================
        // FALLBACK
        // =====================================================

        string from =
            PositionToChessNotation(
                record.Move.From
            );

        string to =
            PositionToChessNotation(
                record.Move.To
            );

        return $"{from} → {to}";
    }

    // =========================================================
    // ADD SVG PIECE
    // =========================================================

    private static void AddPiece(
        Border square,
        Piece piece)
    {
        string imagePath =
            GetPieceImagePath(
                piece
            );

        // =====================================================
        // CHECK FILE
        // =====================================================

        if (!File.Exists(imagePath))
        {
            Console.WriteLine(
                $"SVG NOT FOUND: {imagePath}"
            );

            return;
        }

        try
        {
            // =================================================
            // BASE URI
            // =================================================

            var baseUri =
                new Uri(
                    AppContext.BaseDirectory,
                    UriKind.Absolute
                );

            // =================================================
            // CREATE SVG
            // =================================================

            var svg =
                new global::Avalonia.Svg.Skia.Svg(
                    baseUri
                );

            // =================================================
            // SVG PATH
            // =================================================

            svg.Path =
                imagePath;

            // =================================================
            // PIECE SIZE
            // =================================================

            double pieceSize =
                piece.Type ==
                    PieceType.Pawn
                        ? 55
                        : 65;

            svg.Width =
                pieceSize;

            svg.Height =
                pieceSize;

            // =================================================
            // SVG DISPLAY
            // =================================================

            svg.Stretch =
                Stretch.Uniform;

            svg.HorizontalAlignment =
                HorizontalAlignment.Center;

            svg.VerticalAlignment =
                VerticalAlignment.Center;

            svg.Margin =
                new Thickness(0);

            svg.EnableCache =
                true;

            svg.Wireframe =
                false;

            svg.DisableFilters =
                false;

            // =================================================
            // ADD TO SQUARE
            // =================================================

            square.Child =
                svg;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"SVG ERROR: {imagePath}"
            );

            Console.WriteLine(
                ex.ToString()
            );
        }
    }

    // =========================================================
    // GET PIECE IMAGE PATH
    // =========================================================

    private static string GetPieceImagePath(
        Piece piece)
    {
        string color =
            piece.Color ==
                PieceColor.White
                ? "w"
                : "b";

        string type =
            piece.Type switch
            {
                PieceType.King =>
                    "king",

                PieceType.Queen =>
                    "queen",

                PieceType.Rook =>
                    "rook",

                PieceType.Bishop =>
                    "bishop",

                PieceType.Knight =>
                    "knight",

                PieceType.Pawn =>
                    "pawn",

                _ =>
                    ""
            };

        string fileName =
            $"{color}_{type}_svg_NoShadow.svg";

        return Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Pieces",
            fileName
        );
    }

    private static bool CaptureMessageIsStillActive(
        IReadOnlyList<MoveRecord> history,
        MoveRecord captureRecord)
    {
        // =====================================================
        // FIND CAPTURE RECORD INDEX
        // =====================================================

        int captureIndex =
            -1;

        for (int i = 0; i < history.Count; i++)
        {
            if (ReferenceEquals(
                    history[i],
                    captureRecord))
            {
                captureIndex = i;
                break;
            }
        }

        if (captureIndex < 0)
        {
            return false;
        }

        // =====================================================
        // CHECK MOVES AFTER THE CAPTURE
        // =====================================================

        for (
            int i = captureIndex + 1;
            i < history.Count;
            i++)
        {
            MoveRecord laterMove =
                history[i];

            // =================================================
            // SAME SIDE MOVED AGAIN
            // =================================================

            if (laterMove.Color ==
                captureRecord.Color)
            {
                return false;
            }
        }

        // =====================================================
        // SAME SIDE HAS NOT MOVED AGAIN
        // =====================================================

        return true;
    }

    // =========================================================
    // UPDATE CAPTURED PIECES
    // =========================================================

    private void UpdateCapturedPieces(
        IReadOnlyList<MoveRecord>? history = null)
    {
        // =====================================================
        // CLEAR OLD PIECES
        // =====================================================

        PlayerCapturedPieces.Children.Clear();
        OpponentCapturedPieces.Children.Clear();

        // =====================================================
        // CLEAR OLD CAPTURE MESSAGES
        // =====================================================

        PlayerCaptureMessageText.Text = "";
        OpponentCaptureMessageText.Text = "";

        // =====================================================
        // SCORE
        // =====================================================

        int playerScore = 0;
        int opponentScore = 0;

        // =====================================================
        // GET HISTORY
        // =====================================================

        if (history == null)
        {
            if (viewingHistory &&
                historyViewGame != null)
            {
                history =
                    historyViewGame.MoveHistory.GetAll();
            }
            else
            {
                history =
                    _game.MoveHistory.GetAll();
            }
        }

        // =====================================================
        // DETERMINE CAPTURED PIECE COLOR BY BOARD PERSPECTIVE
        // =====================================================

        PieceColor topCapturedColor =
            blackPerspective
                ? PieceColor.Black
                : PieceColor.White;

        PieceColor bottomCapturedColor =
            blackPerspective
                ? PieceColor.White
                : PieceColor.Black;

        // =====================================================
        // CREATE CAPTURED PIECE IMAGES
        // =====================================================

        foreach (MoveRecord record in history)
        {
            Piece? capturedPiece =
                record.CapturedPiece;

            if (capturedPiece == null)
            {
                continue;
            }

            string imagePath =
                GetPieceImagePath(
                    capturedPiece
                );

            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    $"CAPTURED SVG NOT FOUND: {imagePath}"
                );

                continue;
            }

            try
            {
                // =================================================
                // BASE URI
                // =================================================

                var baseUri =
                    new Uri(
                        AppContext.BaseDirectory,
                        UriKind.Absolute
                    );

                // =================================================
                // CREATE SVG
                // =================================================

                var svg =
                    new global::Avalonia.Svg.Skia.Svg(
                        baseUri
                    );

                svg.Path =
                    imagePath;

                // =================================================
                // CAPTURED PIECE SIZE
                // =================================================

                double pieceSize =
                    capturedPiece.Type ==
                        PieceType.Pawn
                            ? 30
                            : 36;

                svg.Width =
                    pieceSize;

                svg.Height =
                    pieceSize;

                // =================================================
                // SVG DISPLAY
                // =================================================

                svg.Stretch =
                    Stretch.Uniform;

                svg.HorizontalAlignment =
                    HorizontalAlignment.Center;

                svg.VerticalAlignment =
                    VerticalAlignment.Center;

                svg.Margin =
                    new Thickness(0);

                svg.EnableCache =
                    true;

                svg.Wireframe =
                    false;

                svg.DisableFilters =
                    false;

                // =====================================================
                // TOP / BOTTOM BY CAPTURED PIECE COLOR
                // =====================================================

                if (capturedPiece.Color == topCapturedColor)
                {
                    PlayerCapturedPieces.Children.Add(
                        svg
                    );

                    playerScore +=
                        GetCapturedPieceScore(
                            capturedPiece.Type
                        );
                }
                else if (capturedPiece.Color == bottomCapturedColor)
                {
                    OpponentCapturedPieces.Children.Add(
                        svg
                    );

                    opponentScore +=
                        GetCapturedPieceScore(
                            capturedPiece.Type
                        );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"CAPTURED SVG ERROR: {imagePath}"
                );

                Console.WriteLine(
                    ex.ToString()
                );
            }
        }

        // =====================================================
        // UPDATE SCORES
        // =====================================================

        PlayerScoreText.Text =
            $"SCORE: {playerScore}";

        OpponentScoreText.Text =
            $"SCORE: {opponentScore}";

        // =====================================================
        // CAPTURE MESSAGE
        // =====================================================

        MoveRecord? topLastCapture = null;
        MoveRecord? bottomLastCapture = null;

        // =====================================================
        // FIND LAST CAPTURE FOR TOP / BOTTOM COLOR
        // =====================================================

        for (int i = 0; i < history.Count; i++)
        {
            MoveRecord record =
                history[i];

            Piece? capturedPiece =
                record.CapturedPiece;

            if (capturedPiece == null)
            {
                continue;
            }

            // -------------------------------------------------
            // TOP
            // -------------------------------------------------

            if (capturedPiece.Color == topCapturedColor)
            {
                topLastCapture = record;
            }

            // -------------------------------------------------
            // BOTTOM
            // -------------------------------------------------

            else if (capturedPiece.Color == bottomCapturedColor)
            {
                bottomLastCapture = record;
            }
        }

        // =====================================================
        // CHECK TOP MESSAGE
        // =====================================================

        if (topLastCapture != null &&
            CaptureMessageIsStillActive(
                history,
                topLastCapture
            ))
        {
            Piece capturedPiece =
                topLastCapture.CapturedPiece!;

            int captureScore =
                GetCapturedPieceScore(
                    capturedPiece.Type
                );

            string pieceName =
                GetCapturedPieceName(
                    capturedPiece.Type
                );

            PlayerCaptureMessageText.Text =
                $"{pieceName} Captured!   +{captureScore}";

            PlayerCaptureMessageText.IsVisible =
                true;
        }

        // =====================================================
        // CHECK BOTTOM MESSAGE
        // =====================================================

        if (bottomLastCapture != null &&
            CaptureMessageIsStillActive(
                history,
                bottomLastCapture
            ))
        {
            Piece capturedPiece =
                bottomLastCapture.CapturedPiece!;

            int captureScore =
                GetCapturedPieceScore(
                    capturedPiece.Type
                );

            string pieceName =
                GetCapturedPieceName(
                    capturedPiece.Type
                );

            OpponentCaptureMessageText.Text =
                $"{pieceName} Captured!   +{captureScore}";

            OpponentCaptureMessageText.IsVisible =
                true;
        }
    }

    // =========================================================
    // GET CAPTURED PIECE SCORE
    // =========================================================

    private static int GetCapturedPieceScore(
        PieceType type)
    {
        return type switch
        {
            PieceType.Pawn   => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook   => 5,
            PieceType.Queen  => 9,
            _ => 0
        };
    }

    // =========================================================
    // SHOW HISTORY POSITION
    // =========================================================

    private void ShowHistoryPosition(
        int moveCount)
    {
        if (aiThinking)
        {
            FooterStatusText.Text = "Waiting for AI...";
            return;
        }
        Console.WriteLine(
            $"ShowHistoryPosition called: {moveCount}"
        );

        // =========================================================
        // VALIDATE
        // =========================================================

        IReadOnlyList<MoveRecord> history =
            _game.MoveHistory.GetAll();

        if (moveCount < 1 ||
            moveCount > history.Count)
        {
            Console.WriteLine(
                $"Invalid history move count: {moveCount}"
            );

            return;
        }

        // =========================================================
        // CREATE HISTORY GAME
        // =========================================================

        ChessGame.Core.Game.ChessGame historyGame =
            CreateGameAtMove(
                moveCount
            );

        // =========================================================
        // ENTER HISTORY VIEW
        // =========================================================

        viewingHistory = true;

        viewingMoveCount =
            moveCount;

        historyViewGame =
            historyGame;

        ExitHistoryButton.IsVisible = true;
        RotateBoardButton.IsVisible = true;

        UpdateHistoryNavigationButtons();

        selectedPosition = null;

        legalMovePositions.Clear();
        castlingMovePositions.Clear();

        // =========================================================
        // DISPLAY HISTORY BOARD
        // =========================================================

        CreateBoard(
            historyGame
        );

        // =========================================================
        // UPDATE CAPTURED PIECES
        // =========================================================

        List<MoveRecord> visibleHistory =
            history
                .Take(moveCount)
                .ToList();

        UpdateCapturedPieces(
            visibleHistory
        );

        // =========================================================
        // UPDATE STATUS
        // =========================================================

        StatusText.Text =
            $"Viewing move {moveCount}";

        FooterStatusText.Text =
            $"Viewing move {moveCount}. Board is read-only";

        TurnText.Text =
            historyGame.SideToMove ==
                PieceColor.White
                    ? "White"
                    : "Black";

        // =========================================================
        // DEBUG
        // =========================================================

        Console.WriteLine(
            $"Viewing history at move {moveCount}"
        );

        Console.WriteLine(
            $"Captured records shown: {visibleHistory.Count}"
        );
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

    // =========================================================
    // UNDO
    // =========================================================

    private void UndoMove()
    {
        // =========================================================
        // EXIT HISTORY VIEW
        // =========================================================

        viewingHistory = false;

        historyViewGame = null;

        viewingMoveCount = 0;

        if (ExitHistoryButton != null)
        {
            ExitHistoryButton.IsVisible = false;
            PreviousHistoryButton.IsVisible = false;
            NextHistoryButton.IsVisible = false;

            PreviousHistoryButton.IsEnabled = false;
            NextHistoryButton.IsEnabled = false;
        }

        if (RotateBoardButton != null)
        {
            RotateBoardButton.IsVisible = false;
        }

        selectedPosition = null;

        legalMovePositions.Clear();

        castlingMovePositions.Clear();


        // =========================================================
        // AI THINKING
        // =========================================================

        if (aiThinking)
        {
            Console.WriteLine(
                "Cannot undo while AI is thinking."
            );

            return;
        }


        // =========================================================
        // AI MODE
        // =========================================================

        if (selectedGameMode == GameMode.AI)
        {
            // -----------------------------------------------------
            // SAFETY CHECK
            // -----------------------------------------------------

            if (!actualPlayerColor.HasValue)
            {
                Console.WriteLine(
                    "Player color is not set."
                );

                return;
            }


            // -----------------------------------------------------
            // PLAYER COLOR
            // -----------------------------------------------------

            PieceColor playerColor =
                actualPlayerColor.Value ==
                PlayerColorChoice.White
                    ? PieceColor.White
                    : PieceColor.Black;


            // -----------------------------------------------------
            // AI COLOR
            // -----------------------------------------------------

            PieceColor aiColor =
                playerColor == PieceColor.White
                    ? PieceColor.Black
                    : PieceColor.White;


            // =====================================================
            // CHECK MOVE HISTORY
            // =====================================================

            int moveCount =
                _game.MoveHistory.Count;


            // =====================================================
            // NOTHING TO UNDO
            // =====================================================

            if (moveCount == 0)
            {
                Console.WriteLine(
                    "Nothing to undo."
                );

                blackPerspective =
                    actualPlayerColor.Value ==
                    PlayerColorChoice.Black;

                CreateBoard();

                UpdateMoveHistory();

                UpdateCapturedPieces();

                UpdateGameState();

                return;
            }


            // =====================================================
            // BLACK PLAYER SPECIAL CASE
            // =====================================================

            if (playerColor == PieceColor.Black &&
                moveCount == 1)
            {
                Console.WriteLine(
                    "Black player has not made the first move. "
                    + "Undo is disabled."
                );

                // Keep Black perspective.
                blackPerspective = true;

                return;
            }


            // =====================================================
            // NORMAL AI MODE UNDO
            // =====================================================

            if (moveCount >= 2)
            {
                // -------------------------------------------------
                // UNDO AI MOVE
                // -------------------------------------------------

                bool aiUndoSuccess =
                    _game.UndoMove();

                Console.WriteLine(
                    $"AI move undo: {aiUndoSuccess}"
                );


                // -------------------------------------------------
                // UNDO PLAYER MOVE
                // -------------------------------------------------

                bool playerUndoSuccess =
                    _game.UndoMove();

                Console.WriteLine(
                    $"Player move undo: {playerUndoSuccess}"
                );


                // -------------------------------------------------
                // VERIFY
                // -------------------------------------------------

                if (aiUndoSuccess &&
                    playerUndoSuccess)
                {
                    Console.WriteLine(
                        "Player + AI moves undone."
                    );
                }
                else
                {
                    Console.WriteLine(
                        "Undo was incomplete."
                    );
                }
            }


            // =====================================================
            // KEEP PLAYER'S PERSPECTIVE
            // =====================================================

            blackPerspective =
                actualPlayerColor.Value ==
                PlayerColorChoice.Black;


            // =====================================================
            // RESET GAME OVER
            // =====================================================

            gameOver = false;

            // =====================================================
            // REDRAW
            // =====================================================

            CreateBoard();

            UpdateMoveHistory();

            UpdateCapturedPieces();

            UpdateGameState();


            // =====================================================
            // AI TURN AFTER UNDO
            // =====================================================

            if (_game.SideToMove == aiColor)
            {
                Console.WriteLine(
                    "After undo, it is AI's turn."
                );

                MakeAIMove();

                return;
            }


            // =====================================================
            // DEBUG
            // =====================================================

            Console.WriteLine(
                "AI Undo completed."
            );

            Console.WriteLine(
                $"Move count: {_game.MoveHistory.Count}"
            );

            Console.WriteLine(
                $"Side to move: {_game.SideToMove}"
            );

            Console.WriteLine(
                $"Player color: {actualPlayerColor}"
            );

            Console.WriteLine(
                $"AI color: {aiColor}"
            );

            return;
        }


        // =========================================================
        // LOCAL PLAYER MODE
        // =========================================================

        bool success =
            _game.UndoMove();


        // =========================================================
        // UNDO FAILED
        // =========================================================

        if (!success)
        {
            Console.WriteLine(
                "Nothing to undo."
            );

            RestoreCurrentGamePerspective();

            CreateBoard();

            UpdateMoveHistory();

            UpdateCapturedPieces();

            UpdateGameState();

            return;
        }


        // =========================================================
        // LOCAL PLAYER PERSPECTIVE
        // =========================================================

        RestoreCurrentGamePerspective();


        // =========================================================
        // RESET GAME OVER
        // =========================================================

        gameOver = false;

        // =========================================================
        // REDRAW
        // =========================================================

        CreateBoard();

        UpdateMoveHistory();

        UpdateCapturedPieces();

        UpdateGameState();


        // =========================================================
        // DEBUG
        // =========================================================

        Console.WriteLine(
            "Move undone."
        );

        Console.WriteLine(
            $"Side to move: {_game.SideToMove}"
        );
    }

    // =========================================================
    // NEW GAME
    // =========================================================

    private void NewGame()
    {
        // =========================================================
        // EXIT HISTORY / READ-ONLY MODE
        // =========================================================

        viewingHistory = false;

        historyViewGame = null;

        viewingMoveCount = 0;

        selectedPosition = null;

        legalMovePositions.Clear();

        castlingMovePositions.Clear();


        // =========================================================
        // HIDE HISTORY BUTTONS
        // =========================================================

        if (ExitHistoryButton != null)
        {
            ExitHistoryButton.IsVisible = false;
            PreviousHistoryButton.IsVisible = false;
            NextHistoryButton.IsVisible = false;

            PreviousHistoryButton.IsEnabled = false;
            NextHistoryButton.IsEnabled = false;
        }

        if (RotateBoardButton != null)
        {
            RotateBoardButton.IsVisible = false;
        }


        // =========================================================
        // RESET GAME OVER
        // =========================================================

        gameOver = false;

        // =========================================================
        // CREATE COMPLETELY NEW CHESS GAME
        // =========================================================

        _game =
            new ChessGame.Core.Game.ChessGame();

        aiThinking = false;

        UpdateAIThinkingUI();

        // =========================================================
        // DETERMINE PLAYER COLOR
        // =========================================================

        if (selectedGameMode == GameMode.AI)
        {
            // -----------------------------------------------------
            // RANDOM
            // -----------------------------------------------------

            if (selectedPlayerColor ==
                PlayerColorChoice.Random)
            {
                actualPlayerColor =
                    Random.Shared.Next(2) == 0
                        ? PlayerColorChoice.White
                        : PlayerColorChoice.Black;
            }

            // -----------------------------------------------------
            // WHITE / BLACK
            // -----------------------------------------------------

            else if (selectedPlayerColor ==
                    PlayerColorChoice.White)
            {
                actualPlayerColor =
                    PlayerColorChoice.White;
            }

            else if (selectedPlayerColor ==
                    PlayerColorChoice.Black)
            {
                actualPlayerColor =
                    PlayerColorChoice.Black;
            }

            // -----------------------------------------------------
            // SAFETY
            // -----------------------------------------------------

            else
            {
                actualPlayerColor =
                    PlayerColorChoice.White;
            }


            // -----------------------------------------------------
            // SET BOARD VIEW
            // -----------------------------------------------------

            blackPerspective =
                actualPlayerColor ==
                PlayerColorChoice.Black;
        }
        else
        {
            // =====================================================
            // LOCAL PLAYER
            // =====================================================

            actualPlayerColor =
                PlayerColorChoice.White;

            blackPerspective = false;
        }

    // =========================================================
    // SHOW GAME
    // =========================================================

    MainMenuPanel.IsVisible =
        false;

    AISetupPanel.IsVisible =
        false;

    GamePanel.IsVisible =
        true;


    // =========================================================
    // REBUILD BOARD
    // =========================================================

    CreateBoard();


    // =========================================================
    // RESET MOVE HISTORY
    // =========================================================

    UpdateMoveHistory();

    UpdateCapturedPieces();


    // =========================================================
    // UPDATE GAME STATE
    // =========================================================

    UpdateGameState();

    // =========================================================
    // AI FIRST MOVE
    // =========================================================

    if (selectedGameMode ==
        GameMode.AI &&
        actualPlayerColor ==
        PlayerColorChoice.Black)
    {
        MakeAIMove();
    }

    // =========================================================
    // RESET MOVE HISTORY SCROLL
    // =========================================================

    if (MoveHistoryScrollViewer != null)
    {
        MoveHistoryScrollViewer.Offset =
            new Vector(
                0,
                0
            );
    }


    // =========================================================
    // DEBUG
    // =========================================================

    Console.WriteLine(
        "========================================"
    );

    Console.WriteLine(
        "NEW GAME"
    );

    Console.WriteLine(
        $"Game Mode       : {selectedGameMode}"
    );

    Console.WriteLine(
        $"Selected Color  : {selectedPlayerColor}"
    );

    Console.WriteLine(
        $"Actual Color    : {actualPlayerColor}"
    );

    Console.WriteLine(
        $"Black Perspective: {blackPerspective}"
    );

    Console.WriteLine(
        $"Difficulty      : {selectedDifficulty}"
    );

    Console.WriteLine(
        "========================================"
    );
}

    // =========================================================
    // RESTORE CURRENT GAME PERSPECTIVE
    // =========================================================

    private void RestoreCurrentGamePerspective()
    {
        // =========================================================
        // AI MODE
        // =========================================================

        if (selectedGameMode == GameMode.AI)
        {
            blackPerspective =
                actualPlayerColor ==
                PlayerColorChoice.Black;

            Console.WriteLine(
                $"AI player color: {actualPlayerColor}"
            );

            Console.WriteLine(
                $"Restored AI perspective: " +
                $"{(blackPerspective ? "Black" : "White")}"
            );

            return;
        }


        // =========================================================
        // LOCAL PLAYER MODE
        // =========================================================

        if (_game.SideToMove == PieceColor.White)
        {
            blackPerspective = false;
        }
        else
        {
            blackPerspective = true;
        }

        Console.WriteLine(
            $"Current side to move: {_game.SideToMove}"
        );

        Console.WriteLine(
            $"Restored perspective: " +
            $"{(blackPerspective ? "Black" : "White")}"
        );
    }

    // =========================================================
    // EXIT HISTORY MODE
    // =========================================================

    private void ExitHistoryButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Console.WriteLine(
            "Exit History button clicked."
        );

        // =========================================================
        // EXIT READ-ONLY MODE
        // =========================================================

        viewingHistory = false;

        historyViewGame = null;

        viewingMoveCount = 0;

        selectedPosition = null;

        // =========================================================
        // HIDE HISTORY CONTROLS
        // =========================================================

        ExitHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsVisible = false;
        NextHistoryButton.IsVisible = false;

        PreviousHistoryButton.IsEnabled = false;
        NextHistoryButton.IsEnabled = false;

        RotateBoardButton.IsVisible = false;

        // =========================================================
        // RESTORE REAL CURRENT GAME PERSPECTIVE
        // =========================================================

        RestoreCurrentGamePerspective();

        // =========================================================
        // RESTORE REAL CURRENT GAME
        // =========================================================

        gameOver = false;

        CreateBoard(_game);

        // =========================================================
        // RESTORE MOVE HISTORY
        // =========================================================

        UpdateMoveHistory();

        // =========================================================
        // RESTORE CAPTURED PIECES + SCORE + MESSAGE
        // =========================================================

        UpdateCapturedPieces();

        // =========================================================
        // RESTORE GAME STATE
        // =========================================================

        UpdateGameState();

        Console.WriteLine(
            "Exited History Mode."
        );
    }

    // =========================================================
    // UNDO BUTTON
    // =========================================================

    private void UndoButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (aiThinking)
        {
            FooterStatusText.Text = "Waiting for AI...";
            return;
        }

        UndoMove();
    }

    // =========================================================
    // NEW GAME BUTTON
    // =========================================================

    private async void NewGameButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        bool confirmed =
            await ShowNewGameConfirmation();

        if (!confirmed)
        {
            return;
        }

        NewGame();
    }

    private async Task<bool> ShowNewGameConfirmation()
    {
        var dialog =
            new Window
            {
                Title = "Start New Game",

                Width = 400,
                Height = 230,

                MinWidth = 400,
                MinHeight = 230,

                MaxWidth = 400,
                MaxHeight = 230,

                CanResize = false,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#302E2B")
                    )
            };


        // =========================================================
        // TITLE
        // =========================================================

        var title =
            new TextBlock
            {
                Text = "Start New Game?",

                FontSize = 22,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#F0F0F0")
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };


        // =========================================================
        // MESSAGE
        // =========================================================

        var message =
            new TextBlock
            {
                Text =
                    "Your current game will be reset.\nAre you sure?",

                FontSize = 15,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#BBBBBB")
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };


        // =========================================================
        // CANCEL BUTTON
        // =========================================================

        var cancelButton =
            new Button
            {
                Content = "Cancel",

                Width = 120,
                Height = 40,

                FontSize = 15,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#3A3937")
                    ),

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#F0F0F0")
                    ),

                BorderThickness =
                    new Thickness(0),

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                Padding =
                    new Thickness(0)
            };


        // =========================================================
        // YES BUTTON
        // =========================================================

        var yesButton =
            new Button
            {
                Content = "Yes",

                Width = 120,
                Height = 40,

                FontSize = 15,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#629924")
                    ),

                Foreground =
                    Brushes.White,

                BorderThickness =
                    new Thickness(0),

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                Padding =
                    new Thickness(0)
            };


        // =========================================================
        // BUTTON PANEL
        // =========================================================

        var buttonPanel =
            new StackPanel
            {
                Orientation =
                    Orientation.Horizontal,

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Spacing = 15
            };

        buttonPanel.Children.Add(
            cancelButton
        );

        buttonPanel.Children.Add(
            yesButton
        );


        // =========================================================
        // MAIN PANEL
        // =========================================================

        var panel =
            new StackPanel
            {
                Spacing = 15,

                HorizontalAlignment =
                    HorizontalAlignment.Stretch,

                VerticalAlignment =
                    VerticalAlignment.Center,

                Margin =
                    new Thickness(25)
            };

        panel.Children.Add(title);

        panel.Children.Add(message);

        panel.Children.Add(buttonPanel);


        dialog.Content = panel;


        // =========================================================
        // RESULT
        // =========================================================

        bool confirmed = false;


        cancelButton.Click +=
            (_, _) =>
            {
                confirmed = false;

                dialog.Close();
            };


        yesButton.Click +=
            (_, _) =>
            {
                confirmed = true;

                dialog.Close();
            };


        // =========================================================
        // SHOW
        // =========================================================

        await dialog.ShowDialog(this);

        return confirmed;
    }
    private async void MainMenuButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        // =========================================================
        // CONFIRM
        // =========================================================

        bool confirmed =
            await ShowMainMenuConfirmation();

        if (!confirmed)
        {
            return;
        }

        // =========================================================
        // RETURN TO MAIN MENU
        // =========================================================

        selectedPosition = null;

        legalMovePositions.Clear();

        castlingMovePositions.Clear();

        viewingHistory = false;

        historyViewGame = null;

        viewingMoveCount = 0;

        ChessBoard.Children.Clear();

        ShowMainMenu();
    }

    private void UpdateTurnPieceIcon()
    {
        if (_game.SideToMove == PieceColor.White)
        {
            TurnPieceIcon.Text = "♔";

            TurnPieceIcon.Foreground =
                new SolidColorBrush(
                    Color.Parse("#000000")
                );

            TurnPieceIconBackground.Background =
                new SolidColorBrush(
                    Color.Parse("#FFFFFF")
                );
        }
        else
        {
            TurnPieceIcon.Text = "♚";

            TurnPieceIcon.Foreground =
                new SolidColorBrush(
                    Color.Parse("#FFFFFF")
                );

            TurnPieceIconBackground.Background =
                new SolidColorBrush(
                    Color.Parse("#000000")
                );
        }
    }

    // =========================================================
    // UPDATE GAME STATE
    // =========================================================

    private void UpdateGameState()
    {
        UpdateTurnPieceIcon();
        GameResult result =
            _game.GetGameResult(
                _game.SideToMove
            );

        // =====================================================
        // GAME CONTINUES
        // =====================================================

        if (result == GameResult.Ongoing)
        {
            gameOver = false;

            string color =
                _game.SideToMove == PieceColor.White
                    ? "White"
                    : "Black";

            StatusText.Text =
                $"{color} to move";

            FooterStatusText.Text =
                $"{color} to move";

            TurnText.Text =
                color;

            return;
        }

        // =====================================================
        // GAME OVER
        // =====================================================

        bool wasAlreadyGameOver = gameOver;

        gameOver = true;

        string resultText;

        string reasonText;

        switch (result)
        {
            case GameResult.WhiteWins:

                resultText = "White Wins!";
                reasonText = "Checkmate";

                break;

            case GameResult.BlackWins:

                resultText = "Black Wins!";
                reasonText = "Checkmate";

                break;

            case GameResult.Stalemate:

                resultText = "Draw";
                reasonText = "Stalemate";

                break;

            case GameResult.ThreefoldRepetition:

                resultText = "Draw";
                reasonText = "Threefold Repetition";

                break;

            case GameResult.FiftyMoveDraw:

                resultText = "Draw";
                reasonText = "Fifty-Move Rule";

                break;

            case GameResult.InsufficientMaterial:

                resultText = "Draw";
                reasonText = "Insufficient Material";

                break;

            default:

                resultText = "Game Over";
                reasonText = "";

                break;
        }

        // =====================================================
        // UPDATE STATUS
        // =====================================================

        StatusText.Text =
            "Game Over";

        FooterStatusText.Text =
            "Game Over";

        TurnText.Text =
            "END";

        if (!wasAlreadyGameOver)
        {
            ShowGameOverDialog(
                resultText,
                reasonText
            );
        }
    }

    // =========================================================
    // SHOW GAME OVER DIALOG
    // =========================================================

    private async void ShowGameOverDialog(
        string resultText,
        string reasonText)
    {
        var dialog =
            new Window
            {
                Title =
                    "Game Over",

                Width =
                    380,

                Height =
                    260,

                MinWidth =
                    380,

                MinHeight =
                    260,

                MaxWidth =
                    380,

                MaxHeight =
                    260,

                CanResize =
                    false,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                Background =
                    new SolidColorBrush(
                        Color.Parse(
                            "#302E2B"
                        )
                    )
            };

        // =====================================================
        // TITLE
        // =====================================================

        var title =
            new TextBlock
            {
                Text =
                    "GAME OVER",

                FontSize =
                    16,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse(
                            "#AAAAAA"
                        )
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center
            };

        // =====================================================
        // RESULT
        // =====================================================

        var result =
            new TextBlock
            {
                Text =
                    resultText,

                FontSize =
                    30,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse(
                            "#FFFFFF"
                        )
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };

        // =====================================================
        // REASON
        // =====================================================

        var reason =
            new TextBlock
            {
                Text =
                    reasonText,

                FontSize =
                    17,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse(
                            "#BBBBBB"
                        )
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };

        // =====================================================
        // OK BUTTON
        // =====================================================

        var okButton =
            new Button
            {
                Content =
                    "OK",

                Width =
                    120,

                Height =
                    40,

                FontSize =
                    16,

                Background =
                    new SolidColorBrush(
                        Color.Parse(
                            "#629924"
                        )
                    ),

                Foreground =
                    Brushes.White,

                BorderThickness =
                    new Thickness(0),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                VerticalAlignment =
                    VerticalAlignment.Center,

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                Padding =
                    new Thickness(0)
            };

        okButton.Click +=
            (_, _) =>
            {
                dialog.Close();
            };

        // =====================================================
        // CONTENT
        // =====================================================

        var panel =
            new StackPanel
            {
                Spacing =
                    15,

                HorizontalAlignment =
                    HorizontalAlignment.Stretch,

                VerticalAlignment =
                    VerticalAlignment.Center,

                Margin =
                    new Thickness(25)
            };

        panel.Children.Add(
            title
        );

        panel.Children.Add(
            result
        );

        panel.Children.Add(
            reason
        );

        panel.Children.Add(
            okButton
        );

        dialog.Content =
            panel;

        // =====================================================
        // SHOW
        // =====================================================

        await dialog.ShowDialog(
            this
        );
    }

    private void RotateBoardButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (!viewingHistory ||
            historyViewGame == null)
        {
            return;
        }

        blackPerspective = !blackPerspective;

        selectedPosition = null;

        CreateBoard(historyViewGame);

        // =========================================================
        // KEEP HISTORY STATUS
        // =========================================================

        StatusText.Text =
            $"Viewing move {viewingMoveCount}";

        FooterStatusText.Text =
            $"Viewing move {viewingMoveCount}. Board is read-only";

        TurnText.Text =
            historyViewGame.SideToMove ==
                PieceColor.White
                    ? "White"
                    : "Black";
    }

    private async Task<bool> ShowMainMenuConfirmation()
    {
        var dialog =
            new Window
            {
                Title = "Return to Main Menu",

                Width = 400,
                Height = 230,

                MinWidth = 400,
                MinHeight = 230,

                MaxWidth = 400,
                MaxHeight = 230,

                CanResize = false,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#302E2B")
                    )
            };


        // =========================================================
        // TITLE
        // =========================================================

        var title =
            new TextBlock
            {
                Text = "Return to Main Menu?",

                FontSize = 22,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#F0F0F0")
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };


        // =========================================================
        // MESSAGE
        // =========================================================

        var message =
            new TextBlock
            {
                Text =
                    "Your current game will be left.\nAre you sure?",

                FontSize = 15,

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#BBBBBB")
                    ),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };


        // =========================================================
        // CANCEL BUTTON
        // =========================================================

        var cancelButton =
            new Button
            {
                Content = "Cancel",

                Width = 120,
                Height = 40,

                FontSize = 15,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#3A3937")
                    ),

                Foreground =
                    new SolidColorBrush(
                        Color.Parse("#F0F0F0")
                    ),

                BorderThickness =
                    new Thickness(0),

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                Padding =
                    new Thickness(0)
            };


        // =========================================================
        // YES BUTTON
        // =========================================================

        var yesButton =
            new Button
            {
                Content = "Yes",

                Width = 120,
                Height = 40,

                FontSize = 15,

                Background =
                    new SolidColorBrush(
                        Color.Parse("#629924")
                    ),

                Foreground =
                    Brushes.White,

                BorderThickness =
                    new Thickness(0),

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                Padding =
                    new Thickness(0)
            };


        // =========================================================
        // BUTTON PANEL
        // =========================================================

        var buttonPanel =
            new StackPanel
            {
                Orientation =
                    Orientation.Horizontal,

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Spacing = 15
            };


        buttonPanel.Children.Add(
            cancelButton
        );

        buttonPanel.Children.Add(
            yesButton
        );


        // =========================================================
        // MAIN PANEL
        // =========================================================

        var panel =
            new StackPanel
            {
                Spacing = 15,

                HorizontalAlignment =
                    HorizontalAlignment.Stretch,

                VerticalAlignment =
                    VerticalAlignment.Center,

                Margin =
                    new Thickness(25)
            };


        panel.Children.Add(title);

        panel.Children.Add(message);

        panel.Children.Add(buttonPanel);


        dialog.Content = panel;


        // =========================================================
        // RESULT
        // =========================================================

        bool confirmed = false;


        cancelButton.Click +=
            (_, _) =>
            {
                confirmed = false;

                dialog.Close();
            };


        yesButton.Click +=
            (_, _) =>
            {
                confirmed = true;

                dialog.Close();
            };


        // =========================================================
        // SHOW
        // =========================================================

        await dialog.ShowDialog(this);

        return confirmed;
    }

    // =========================================================
    // AI MOVE
    // =========================================================

    private async void MakeAIMove()
    {
        // =========================================================
        // SAFETY CHECK
        // =========================================================

        if (selectedGameMode != GameMode.AI)
        {
            return;
        }

        if (aiThinking)
        {
            return;
        }

        if (viewingHistory)
        {
            return;
        }

        if (gameOver)
        {
            return;
        }


        // =========================================================
        // CHECK AI COLOR
        // =========================================================

        PieceColor aiColor =
            actualPlayerColor == PlayerColorChoice.White
                ? PieceColor.Black
                : PieceColor.White;


        // =========================================================
        // CHECK TURN
        // =========================================================

        if (_game.SideToMove != aiColor)
        {
            return;
        }


        // =========================================================
        // AI THINKING
        // =========================================================

        aiThinking = true;

        UpdateAIThinkingUI();

        try
        {
            await Task.Delay(1500);


            // =====================================================
            // RE-CHECK GAME STATE
            // =====================================================

            if (viewingHistory ||
                gameOver ||
                selectedGameMode != GameMode.AI)
            {
                return;
            }

            if (_game.SideToMove != aiColor)
            {
                return;
            }

            // =====================================================
            // GET AI MOVE
            // =====================================================

            Move? aiMove = null;

            PieceType? stockfishPromotion = null;

            if (selectedDifficulty == AIDifficulty.Easy)
            {
                aiMove =
                    ChessAI.GetRandomMove(
                        _game,
                        aiColor
                    );
            }
            else if (selectedDifficulty == AIDifficulty.Normal)
            {
                aiMove =
                    ChessAINormal.GetMove(
                        _game,
                        aiColor
                    );
            }
            else if (selectedDifficulty == AIDifficulty.Hard)
            {
                aiMove =
                    await Task.Run(
                        () =>
                            ChessAIHard.GetBestMove(
                                _game,
                                aiColor
                            )
                    );
            }
            else if (selectedDifficulty == AIDifficulty.Bot)
            {
                string enginePath =
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Engine",
                        "stockfish-windows-x86-64-avx2.exe"
                    );

                if (!File.Exists(enginePath))
                {

                    return;
                }

                StockfishResult? stockfishResult =
                    await Task.Run(
                        () =>
                        {
                            using StockfishEngine engine =
                                new StockfishEngine(
                                    enginePath
                                );

                            engine.Initialize();

                            engine.WaitUntilReady();

                            Move? move =
                                engine.GetBestMove(
                                    _game,
                                    2000
                                );

                            return new StockfishResult(
                                move,
                                engine.LastPromotion
                            );
                        }
                    );

                aiMove =
                    stockfishResult.Move;

                stockfishPromotion =
                    stockfishResult.Promotion;
            }

            // =====================================================
            // NO LEGAL MOVE
            // =====================================================

            if (aiMove == null)
            {
                UpdateGameState();

                return;
            }


            // =====================================================
            // CHECK AI PROMOTION
            // =====================================================

            Piece? movingPiece =
                _game.Board.GetPiece(
                    aiMove.From
                );

            bool isPromotion =
                movingPiece != null &&
                movingPiece.Type == PieceType.Pawn &&
                PawnPromotion.CanPromote(
                    aiMove.To,
                    aiColor
                );


            // =====================================================
            // EXECUTE AI MOVE
            // =====================================================

            bool success;


            if (isPromotion)
            {
                PieceType promotionType =
                    PieceType.Queen;

                // =====================================================
                // STOCKFISH PROMOTION
                // =====================================================

                if (selectedDifficulty == AIDifficulty.Bot &&
                    stockfishPromotion.HasValue)
                {
                    promotionType =
                        stockfishPromotion.Value;
                }

                success =
                    _game.TryMove(
                        aiMove,
                        aiColor,
                        promotionType
                    );

                Console.WriteLine(
                    $"AI promotion: {promotionType}"
                );
            }
            else
            {
                success =
                    _game.TryMove(
                        aiMove,
                        aiColor
                    );
            }


            // =====================================================
            // AI MOVE FAILED
            // =====================================================

            if (!success)
            {
                Console.WriteLine(
                    "AI move failed."
                );

                return;
            }

            if (movingPiece != null &&
                SoundEffectsToggle.IsChecked == true)
            {
                ChessSoundPlayer.Play(
                    movingPiece.Type
                );
            }


            // =====================================================
            // UPDATE BOARD
            // =====================================================

            selectedPosition = null;


            // =====================================================
            // KEEP PLAYER PERSPECTIVE
            // =====================================================

            blackPerspective =
                actualPlayerColor ==
                PlayerColorChoice.Black;


            // =====================================================
            // REDRAW
            // =====================================================

            CreateBoard();

            UpdateMoveHistory();

            UpdateCapturedPieces();

            UpdateGameState();


            // =====================================================
            // DEBUG
            // =====================================================

            Console.WriteLine(
                $"AI move: " +
                $"{PositionToChessNotation(aiMove.From)}" +
                $" -> " +
                $"{PositionToChessNotation(aiMove.To)}"
            );

            if (isPromotion)
            {
                Console.WriteLine(
                    $"AI promotion: {stockfishPromotion ?? PieceType.Queen}"
                );
            }
        }
        finally
        {
            // =====================================================
            // AI FINISHED THINKING
            // =====================================================

            aiThinking = false;

            UpdateAIThinkingUI();
        }
    }

    private async void TestStockfishButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        string enginePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Engine",
                "stockfish-windows-x86-64-avx2.exe"
            );

        if (!File.Exists(enginePath))
        {

            return;
        }

        try
        {
            string fen =
                FenGenerator.Generate(
                    _game.Board,
                    _game.SideToMove,
                    _game.LastMove
                );

            Move? result =
                await Task.Run(() =>
                {
                    using StockfishEngine engine =
                        new StockfishEngine(
                            enginePath
                        );

                    engine.Initialize();

                    engine.WaitUntilReady();

                    return engine.GetBestMove(
                        _game,
                        1000
                    );
                });
        }
        catch
        {
        }
    }

    private void UpdateAIThinkingUI()
    {
        if (aiThinking)
        {
            UndoButton.IsEnabled = false;

            FooterStatusText.Text =
                "Waiting for AI...";

            return;
        }

        UndoButton.IsEnabled = true;

        FooterStatusText.Text =
            _game.SideToMove == PieceColor.White
                ? "White to move"
                : "Black to move";
    }

    private static string GetCapturedPieceName(
        PieceType type)
    {
        return type switch
        {
            PieceType.Pawn   => "Pawn",
            PieceType.Knight => "Knight",
            PieceType.Bishop => "Bishop",
            PieceType.Rook   => "Rook",
            PieceType.Queen  => "Queen",
            _ => "Piece"
        };
    }
}

internal sealed record StockfishResult(
    Move? Move,
    PieceType? Promotion
);