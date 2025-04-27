using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Koridor.Chess;
using Point = System.Windows.Point;

namespace Koridor
{
    public partial class MainWindow : Window
    {
        static int cols = 9;
        static int rows = 9;
        int ind = cols * rows;
        List<Line> vertLines = new List<Line>();
        List<Line> horizontLines = new List<Line>();
        double canvasWidth;
        double canvasHeight;
        double cellWidth;
        double cellHeight;

        Chess BlueChess = new Chess(4, 0, false);
        Chess RedChess = new Chess(4, 8, true);
        Ellipse BlueChessElp = new Ellipse();
        Ellipse RedChessElp = new Ellipse();

        private bool currentPlayer = true; // true = красный, false = синий
        bool SelectedChess = false;
        Chess? SelectedChessObject;

        private int? selectedCellIndex = null; // Индекс текущей выделенной клетки

        private Line? temporaryWall = null;
        private bool isTemporaryWallPlaced = false; // Флаг для отслеживания временной стены
        private bool TemporaryWallPlaced = false; //вторйо флаг для того же
        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        }

        private void MainSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void DrawGrid()
        {
            canvas.Children.Clear();
            vertLines.Clear();
            horizontLines.Clear();

            canvasWidth = canvas.ActualWidth;
            canvasHeight = canvas.ActualHeight;
            cellWidth = canvasWidth / cols;
            cellHeight = canvasHeight / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Canvas cellCanvas = new Canvas()
                    {
                        Width = cellWidth,
                        Height = cellHeight,
                        Background = Brushes.Transparent,
                        Margin = new Thickness(col * cellWidth, row * cellHeight, 0, 0)
                    };

                    // вертикальная линия (левая)
                    Line leftLine = new Line()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = cellHeight * 0.25,
                        Y2 = cellHeight * 0.75,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                        Tag = "LeftLine"
                    };
                    leftLine.MouseDown += Border_MouseDown;
                    cellCanvas.Children.Add(leftLine);

                    //горизонтальная линия (верхняя)
                    Line topLine = new Line()
                    {
                        X1 = cellWidth * 0.25,
                        X2 = cellWidth * 0.75,
                        Y1 = 0,
                        Y2 = 0,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2,
                        Tag = "TopLine" // Добавляем тег
                    };
                    topLine.MouseDown += Border_MouseDown;
                    cellCanvas.Children.Add(topLine);

                    // правая линия (только для последнего столбца)
                    if (col == cols - 1)
                    {
                        Line rightLine = new Line()
                        {
                            X1 = cellWidth,
                            X2 = cellWidth,
                            Y1 = cellHeight * 0.25,
                            Y2 = cellHeight * 0.75,
                            Stroke = Brushes.Red,
                            StrokeThickness = 2,
                            Tag = "RightLine" // Добавляем тег
                        };
                        rightLine.MouseDown += Border_MouseDown;
                        cellCanvas.Children.Add(rightLine);
                    }

                    // нижняя линия (только для последней строки)
                    if (row == rows - 1)
                    {
                        Line bottomLine = new Line()
                        {
                            X1 = cellWidth * 0.25,
                            X2 = cellWidth * 0.75,
                            Y1 = cellHeight,
                            Y2 = cellHeight,
                            Stroke = Brushes.Red,
                            StrokeThickness = 2,
                            Tag = "BottomLine" // Добавляем тег
                        };
                        bottomLine.MouseDown += Border_MouseDown;
                        cellCanvas.Children.Add(bottomLine);
                    }

                    //cellCanvas.MouseDown += (sender, e) =>
                    //{
                    //    cellCanvas.Background = Brushes.Green;
                    //};

                    canvas.Children.Add(cellCanvas);
                }
            }
        }
        private void Log(string message)
        {
            DebugTextBox.Text += message + "\n";
            DebugTextBox.ScrollToEnd();
        }
        private void UpdateGrid()
        {
            canvasWidth = canvas.ActualWidth;
            canvasHeight = canvas.ActualHeight;
            cellWidth = canvasWidth / cols;
            cellHeight = canvasHeight / rows;

            for (int i = 0; i < ind; i++)
            {
                int row = i / cols;
                int col = i % cols;

                if (canvas.Children[i] is Canvas cellCanvas)
                {
                    cellCanvas.Width = cellWidth;
                    cellCanvas.Height = cellHeight;
                    cellCanvas.Margin = new Thickness(col * cellWidth, row * cellHeight, 0, 0);

                    // Обновление левой линии
                    foreach (var child in cellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "LeftLine")
                        {
                            line.X1 = 1;
                            line.X2 = 1;
                            line.Y1 = cellHeight * 0.25;
                            line.Y2 = cellHeight * 0.75;
                    }
                    }

                    // Обновление верхней линии
                    foreach (var child in cellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "TopLine")
                        {
                            line.X1 = cellWidth * 0.25;
                            line.X2 = cellWidth * 0.75;
                            line.Y1 = 1;
                            line.Y2 = 1;
                    }
                    }

                    // Обновление правой линии
                    foreach (var child in cellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "RightLine")
                        {
                            line.X1 = cellWidth - 1;
                            line.X2 = cellWidth - 1;
                            line.Y1 = cellHeight * 0.25;
                            line.Y2 = cellHeight * 0.75;
                    }
                    }

                    // Обновление нижней линии
                    foreach (var child in cellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "BottomLine")
                        {
                            line.X1 = cellWidth * 0.25;
                            line.X2 = cellWidth * 0.75;
                            line.Y1 = cellHeight - 1;
                            line.Y2 = cellHeight - 1;
                        }
                    }
                }
            }

            // обновление шашек
            BlueChessElp.Width = cellWidth * 0.8;
            BlueChessElp.Height = cellHeight * 0.8;
            BlueChessElp.Fill = Brushes.Blue;
            BlueChessElp.Stroke = Brushes.DarkBlue;
            BlueChessElp.StrokeThickness = 2;

            RedChessElp.Width = cellWidth * 0.8;
            RedChessElp.Height = cellHeight * 0.8;
            RedChessElp.Fill = Brushes.Red;
            RedChessElp.Stroke = Brushes.DarkRed;
            RedChessElp.StrokeThickness = 2;



            // удаляем фишки из старых позиций
            foreach (var child in canvas.Children)
            {
                if (child is Canvas cellCanvas)
                {
                    if (cellCanvas.Children.Contains(BlueChessElp))
                        cellCanvas.Children.Remove(BlueChessElp);
                    if (cellCanvas.Children.Contains(RedChessElp))
                        cellCanvas.Children.Remove(RedChessElp);
                }
            }

            // добавляем фишки в новые позиции
            if (BlueChess.posX >= 0 && BlueChess.posX < cols && BlueChess.posY >= 0 && BlueChess.posY < rows)
            {
                int blueIndex = BlueChess.posY * cols + BlueChess.posX;
                if (blueIndex < canvas.Children.Count && canvas.Children[blueIndex] is Canvas blueCell)
                {
                    Canvas.SetLeft(BlueChessElp, (cellWidth - BlueChessElp.Width) / 2);
                    Canvas.SetTop(BlueChessElp, (cellHeight - BlueChessElp.Height) / 2);
                    blueCell.Children.Add(BlueChessElp);
                }
            }

            if (RedChess.posX >= 0 && RedChess.posX < cols && RedChess.posY >= 0 && RedChess.posY < rows)
            {
                int redIndex = RedChess.posY * cols + RedChess.posX;
                if (redIndex < canvas.Children.Count && canvas.Children[redIndex] is Canvas redCell)
                {
                    Canvas.SetLeft(RedChessElp, (cellWidth - RedChessElp.Width) / 2);
                    Canvas.SetTop(RedChessElp, (cellHeight - RedChessElp.Height) / 2);
                    redCell.Children.Add(RedChessElp);
                }
            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(canvas);

            int clickedX = (int)(clickPoint.X / cellWidth);
            int clickedY = (int)(clickPoint.Y / cellHeight);
            int clickedIndex = clickedY * cols + clickedX;

            // Если уже есть выделенная клетка, снимаем выделение
            if (!TemporaryWallPlaced)
            {
                ResetBorderColors();
            }
            // Выделяем границы новой клетки
            if (clickedIndex >= 0 && clickedIndex < canvas.Children.Count)
            {
                HighlightBorders(clickedIndex); // Выделяем границы
                selectedCellIndex = clickedIndex; // Обновляем индекс текущей клетки
            }
            Log($"Clicked at ({clickedX}, {clickedY})");
            if (TemporaryWallPlaced && isWallEndValid())
            {
                RemoveTemporaryWall();
            }
            if (isTemporaryWallPlaced)
            {
                TemporaryWallPlaced = true;
            }
            if (!SelectedChess)
            {
                // выбор игрока
                if (clickedX == BlueChess.posX && clickedY == BlueChess.posY && !currentPlayer)
                {
                    SelectedChess = true;
                    SelectedChessObject = BlueChess;
                    HighlightAvailableMoves(GetAvailableMoves(clickedX, clickedY));
                }
                else if (clickedX == RedChess.posX && clickedY == RedChess.posY && currentPlayer)
                {
                    SelectedChess = true;
                    SelectedChessObject = RedChess;
                    HighlightAvailableMoves(GetAvailableMoves(clickedX, clickedY));
                }
            }
            else
            {
                ResetBorderColors();
                var possibleMoves = GetAvailableMoves(SelectedChessObject.posX, SelectedChessObject.posY);
                if (possibleMoves.Contains((clickedX, clickedY)))
                {
                        MoveChess(SelectedChessObject, clickedX, clickedY);
                        Log($"Moved to ({clickedX}, {clickedY})");
                    currentPlayer = !currentPlayer;
                    Log($"Current player: {(currentPlayer ? "red" : "blue")}");
                }
                ClearHighlights();
                SelectedChess = false;
                SelectedChessObject = null;
            }
        }
        private bool CheckForPlacedWall(int currentIndex, int targetIndex)
        {
            if (canvas.Children[currentIndex] is Canvas currentCellCanvas && canvas.Children[targetIndex] is Canvas targetCellCanvas)
            {
                // Проверяем наличие горизонтальной стены (движение вверх или вниз)
                if (currentIndex == targetIndex + cols)
                {
                    foreach (var child in currentCellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "TopPlacedWall") return true;
                        }
                    }
                if (currentIndex == targetIndex - cols)
                {
                    foreach (var child in targetCellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "TopPlacedWall") return true;
                    }
                }
                // Проверяем наличие вертикальной стены (движение влево или вправо)
                if (currentIndex == targetIndex + 1)
                {
                    foreach (var child in currentCellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "LeftPlacedWall") return true;
                    }
                }
                if (currentIndex == targetIndex - 1)
            {
                    foreach (var child in targetCellCanvas.Children)
                    {
                        if (child is Line line && line.Tag?.ToString() == "LeftPlacedWall") return true;
            }
        }
            }
            return false;
        }
        private void MoveChess(Chess chess, int endX, int endY)
        {
            // Удаляем фишку из текущего Canvas
            RemoveTemporaryWall();
            foreach (var child in canvas.Children)
            {
                if (child is Canvas cellCanvas)
                {
                    if (cellCanvas.Children.Contains(BlueChessElp) || cellCanvas.Children.Contains(RedChessElp))
                    {
                        cellCanvas.Children.Remove(chess.red ? RedChessElp : BlueChessElp);
                    }
                }
            }

            // Обновляем позицию фишки
            chess.posX = endX;
            chess.posY = endY;

            // Добавляем фишку в новый Canvas
            int newIndex = endY * cols + endX;
            if (newIndex < canvas.Children.Count && canvas.Children[newIndex] is Canvas newCell)
            {
                Canvas.SetLeft(chess.red ? RedChessElp : BlueChessElp, (cellWidth - (chess.red ? RedChessElp.Width : BlueChessElp.Width)) / 2);
                Canvas.SetTop(chess.red ? RedChessElp : BlueChessElp, (cellHeight - (chess.red ? RedChessElp.Height : BlueChessElp.Height)) / 2);
                newCell.Children.Add(chess.red ? RedChessElp : BlueChessElp);
            }
        }
        private List<(int x, int y)> GetAvailableMoves(int startX, int startY)
        {
            var possibleMoves = new List<(int x, int y)>();

            var directions = new List<(int dx, int dy)> // Возможные направления
            {
                (0, -1), (0, 1), (-1, 0), (1, 0)
            };
            
            foreach (var (dx, dy) in directions)
            {
                int targetX = startX + dx;
                int targetY = startY + dy;

                if (targetX >= 0 && targetX < cols && targetY >= 0 && targetY < rows) // Проверяем, что целевая клетка находится в пределах поля
                {
                    bool hasWallOnTheWay = false;
                    int currentIndex = startY * cols + startX;
                    int targetIndex = targetY * cols + targetX;

                    hasWallOnTheWay = CheckForPlacedWall(currentIndex, targetIndex);
                    if (!hasWallOnTheWay && !(targetX == BlueChess.posX && targetY == BlueChess.posY) && !(targetX == RedChess.posX && targetY == RedChess.posY))
                    {
                        possibleMoves.Add((targetX, targetY));
                    }
                    else if (!hasWallOnTheWay)// Если на клетке есть фишка, проверяем возможность перепрыгивания
                    {
                        int endX = targetX + dx;
                        int endY = targetY + dy;
                        int endIndex = endY * cols + endX;
                        if (endX >= 0 && endX < cols && endY >= 0 && endY < rows && !CheckForPlacedWall(targetIndex, endIndex))
                        {
                            possibleMoves.Add((endX, endY));
                        }
                    }
                }
            }
            return possibleMoves;
        }
        private void HighlightAvailableMoves(List<(int x, int y)> moves)
        {
            foreach (var (x, y) in moves)
            {
                int index = y * cols + x;
                if (index < canvas.Children.Count && canvas.Children[index] is Canvas cellCanvas)
                {
                    cellCanvas.Background = Brushes.Green;
                }
            }
        }
        private void ClearHighlights()
        {
            for (int i = 0; i < ind; i++)
            {
                if (canvas.Children[i] is Canvas cellCanvas)
                {
                    cellCanvas.Background = Brushes.Transparent;
                }
            }
        }
        private void HighlightBorders(int clickedIndex)
        {
            if (isTemporaryWallPlaced) return;
            if (clickedIndex >= 0 && clickedIndex < canvas.Children.Count)
            {
                if (canvas.Children[clickedIndex] is Canvas currentCellCanvas)
                {
                    foreach (var child in currentCellCanvas.Children)
                    {
                        if (child is Line line)
                        {
                            if (line.Stroke != Brushes.Black)
                            {
                                switch (line.Tag?.ToString())
                                {
                                    case "LeftLine":
                                        if (clickedIndex % cols != 0) line.Stroke = Brushes.LightGreen;
                                        break;
                                    case "TopLine":
                                        if (clickedIndex >= cols) line.Stroke = Brushes.LightGreen;
                                        break;
                                    case "TopTemporaryWall":
                                        line.Stroke = Brushes.LightGreen;
                                        line.Tag = "TopLine";
                                        break;
                                    case "LeftTemporaryWall":
                                        line.Stroke = Brushes.LightGreen;
                                        line.Tag = "LeftLine";
                                        break;
                                }
                            }
                        }
                    }
                }
                if (((clickedIndex + 1) % cols != 0) && canvas.Children[clickedIndex + 1] is Canvas rightCellCanvas)
                {
                    foreach (var child in rightCellCanvas.Children)
                    {
                        if (child is Line line)
                        {
                            if (line.Stroke != Brushes.Black)
                            {
                                switch (line.Tag?.ToString())
                                {
                                    case "LeftLine":
                                        line.Stroke = Brushes.LightGreen; // Перекраска всех линий
                                        break;
                                    case "LeftTemporaryWall":
                                        line.Stroke = Brushes.LightGreen;
                                        line.Tag = "LeftLine";
                                        break;
                                }
                            }
                        }
                    }
                }
                if ((clickedIndex + rows) < canvas.Children.Count && canvas.Children[clickedIndex + rows] is Canvas downCellCanvas)
                {
                    foreach (var child in downCellCanvas.Children)
                    {
                        if (child is Line line)
                        {
                            if (line.Stroke != Brushes.Black)
                            {
                                switch (line.Tag?.ToString())
                                {
                                    case "TopLine":
                                        line.Stroke = Brushes.LightGreen; // Перекраска всех линий
                                        break;
                                    case "TopTemporaryWall":
                                        line.Stroke = Brushes.LightGreen;
                                        line.Tag = "TopLine";
                                        break;
                                }
                            }
                        }
                    }
                }
    }
}
        private void HighlightBorder()
        {
            for (int cellIndex = 1; cellIndex < canvas.Children.Count ; cellIndex++)
            {
                if (canvas.Children[cellIndex] is Canvas cellCanvas)
                {
                    foreach (var child in cellCanvas.Children)
                    {
                        if (child is Line line)
                        {
                            if (line.Tag.ToString() == "LeftWall")
                            {
                                if (cellIndex > rows)
                                {
                                    if (canvas.Children[cellIndex - rows] is Canvas UpCellCanvas)
                                    {
                                        if (!CheckForPlacedWall(cellIndex, cellIndex - rows)
                                            || !CheckForPlacedWall(cellIndex - 1, cellIndex - 1 - rows))
                                        {
                                        foreach (var child1 in UpCellCanvas.Children)
                                        {
                                            if (child1 is Line line1)
                                            {
                                                if (line1.Tag.ToString() == "LeftLine")
                                                {
                                                    line1.Tag = "LeftTemporaryWall";
                                                    line1.Stroke = Brushes.LightGreen;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                }
                                if (cellIndex + rows < canvas.Children.Count)
                                {
                                    if (canvas.Children[cellIndex + rows] is Canvas DownCellCanvas)
                                    {
                                        if (!CheckForPlacedWall(cellIndex, cellIndex + rows)
                                            || !CheckForPlacedWall(cellIndex - 1, cellIndex - 1 + rows))
                                        {
                                        foreach (var child1 in DownCellCanvas.Children)
                                        {
                                            if (child1 is Line line1)
                                            {
                                                if (line1.Tag.ToString() == "LeftLine")
                                                {
                                                    line1.Tag = "LeftTemporaryWall";
                                                    line1.Stroke = Brushes.LightGreen;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            }
                            if (line.Tag.ToString() == "TopWall")
                            {
                                if ((cellIndex - 1) % cols != cols - 1)
                                {
                                    if (canvas.Children[cellIndex - 1] is Canvas LeftCellCanvas)
                                    {
                                        if (!CheckForPlacedWall(cellIndex, cellIndex - 1)
                                            || !CheckForPlacedWall(cellIndex - rows, cellIndex - rows - 1))
                                        {
                                        foreach (var child1 in LeftCellCanvas.Children)
                                        {
                                            if (child1 is Line line1)
                                            {
                                                if (line1.Tag.ToString() == "TopLine")
                                                {
                                                    line1.Tag = "TopTemporaryWall";
                                                    line1.Stroke = Brushes.LightGreen;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                }
                                if ((cellIndex + 1) % cols != 0)
                                {
                                    if (canvas.Children[cellIndex + 1] is Canvas RightCellCanvas)
                                    {
                                        if (!CheckForPlacedWall(cellIndex, cellIndex + 1)
                                            || !CheckForPlacedWall(cellIndex - rows, cellIndex - rows + 1))
                                        {
                                        foreach (var child1 in RightCellCanvas.Children)
                                        {
                                            if (child1 is Line line1)
                                            {
                                                if (line1.Tag.ToString() == "TopLine")
                                                {
                                                    line1.Tag = "TopTemporaryWall";
                                                    line1.Stroke = Brushes.LightGreen;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        }
        private void ResetBorderColors()
        {
            foreach (Canvas Cell in canvas.Children)
            {
                foreach (var child in Cell.Children)
                {
                    if (child is Line line)
                    {
                        if (line.Stroke == Brushes.LightGreen && (line.Tag?.ToString() != "LeftTemporaryWall" && line.Tag?.ToString() != "TopTemporaryWall"))
                        {
                            line.StrokeThickness = 2;
                            line.Stroke = Brushes.Red; // Возвращаем исходный цвет границ
                        }
                    }
                }
            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Line border)
            {
                ClearHighlights();
                ResetBorderColors();
                string? tag = border.Tag?.ToString();
                Point clickPoint = e.GetPosition(canvas);
                int clickedX = (int)(clickPoint.X / cellWidth);
                int clickedY = (int)(clickPoint.Y / cellHeight);

                // Игнорируем некорректные границы
                if (tag == "RightLine" || tag == "BottomLine" || (tag == "LeftLine" && clickedX % cols == 0) || (tag == "TopLine" && clickedY == 0)) return;

                // Определяем ориентацию стены
                bool isVertical = (tag == "LeftLine");
                ResetBorderColors();
                // Если временная стена ещё не существует
                if (temporaryWall == null)
                {
                    temporaryWall = border;
                    ResetBorderColors();
                    isTemporaryWallPlaced = true;
                    border.Stroke = Brushes.Black;
                    border.StrokeThickness = 5;
                    border.Tag = $"{(isVertical ? "Left" : "Top")}Wall";
                    if (isVertical)
                    {
                        //border.Y1 = 0;
                        //border.Y2 = cellHeight;
                        HighlightBorder();
                    }
                    else
                    {
                        //border.X1 = 0;
                        //border.X2 = cellHeight;
                        HighlightBorder();
                    }
                }
                else
                {
                    if (border.Tag == "LeftTemporaryWall")
                    {
                        border.Tag = "LeftPlacedWall";
                        border.Stroke = Brushes.Black;
                        border.StrokeThickness = 5;
                        //border.Y1 = 0;
                        //border.Y2 = cellHeight;
                        FromTemporaryToPlaced();
                        RemoveTemporaryWall();
                        ClearHighlights();
                        isTemporaryWallPlaced = false;
                        TemporaryWallPlaced = false;
                        temporaryWall = null;
                        Log($"Player {(currentPlayer ? "red" : "blue")} placed wall!");
                        currentPlayer = !currentPlayer;
                        Log($"Current player: {(currentPlayer ? "red" : "blue")}");

                    }
                    else if (border.Tag == "TopTemporaryWall")
                    {
                        border.Tag = "TopPlacedWall";
                        border.Stroke = Brushes.Black;
                        border.StrokeThickness = 5;
                        //border.Y1 = 0;
                        //border.Y2 = cellHeight;
                        FromTemporaryToPlaced();
                        RemoveTemporaryWall();
                        ClearHighlights();
                        isTemporaryWallPlaced = false;
                        TemporaryWallPlaced = false;
                        temporaryWall = null;
                        Log($"Player {(currentPlayer ? "red" : "blue")} placed wall!");
                        currentPlayer = !currentPlayer;
                        Log($"Current player: {(currentPlayer ? "red" : "blue")}");
                    }
                }
            }
        }
        private void RemoveTemporaryWall()
        {
            foreach (Canvas Cell in canvas.Children)
            {
                foreach (var child in Cell.Children)
                {
                    if (child is Line line)
                    {
                        if (line.Tag?.ToString() == "LeftTemporaryWall" || line.Tag?.ToString() == "LeftWall")
                        {
                            line.StrokeThickness = 2;
                            line.Stroke = Brushes.Red;
                            line.Tag = "LeftLine";
                            line.X1 = 0;
                            line.X2 = 0;
                            line.Y1 = cellHeight * 0.25;
                            line.Y2 = cellHeight * 0.75;
                        }
                        if (line.Tag?.ToString() == "TopTemporaryWall" || line.Tag?.ToString() == "TopWall")
                        {
                            line.StrokeThickness = 2;
                            line.Stroke = Brushes.Red;
                            line.Tag = "TopLine";
                            line.X1 = cellWidth * 0.25;
                            line.X2 = cellWidth * 0.75;
                            line.Y1 = 0;
                            line.Y2 = 0;
                        }
                    }
                }
            }
            isTemporaryWallPlaced = false;
            TemporaryWallPlaced = false;
            temporaryWall = null;
        }
        private void FromTemporaryToPlaced()
        {
            foreach (Canvas Cell in canvas.Children)
            {
                foreach (var child in Cell.Children)
                {
                    if (child is Line line)
                    {
                        if (line.Tag?.ToString() == "LeftWall") line.Tag = "LeftPlacedWall";
                        if (line.Tag?.ToString() == "TopWall") line.Tag = "TopPlacedWall";
                    }
                }
            }
        }
        private bool isWallEndValid()
        {
            return true;
        }
    }
}