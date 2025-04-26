using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
        Chess SelectedChessObject;

        AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>> field = new AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>>();

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
            FillField();
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;

        }

        private void FillField()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    field.AddVertex((i,j));

                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i > 0) field.AddEdge(new TaggedEdge<(int x, int y), int>((i, j), (i - 1, j), 1));
                    if (i < rows - 1) field.AddEdge(new TaggedEdge<(int x, int y), int>((i, j), (i + 1, j), 1));
                    if (j > 0) field.AddEdge(new TaggedEdge<(int x, int y), int>((i, j), (i, j - 1), 1));
                    if (j < rows - 1) field.AddEdge(new TaggedEdge<(int x, int y), int>((i, j), (i, j + 1), 1));

                }
            }

        }

        public int GetShortesDist((int x, int y) start)
        {
            var dijkstra = new DijkstraShortestPathAlgorithm<(int x, int y), TaggedEdge<(int x, int y), int>>(
                field,
                edge => edge.Tag
            );
            dijkstra.Compute(start);

            int MinDist = int.MaxValue;
            for (int i = 0; i < cols; i++)
            {
                if (dijkstra.Distances.TryGetValue((i,8), out double distance) && distance < MinDist) MinDist = (int)distance;

            }
            
            return MinDist;
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
                        StrokeThickness = 2
                    };
                    cellCanvas.Children.Add(leftLine);

                    //горизонтальная линия (верхняя)
                    Line topLine = new Line()
                    {
                        X1 = cellWidth * 0.25,
                        X2 = cellWidth * 0.75,
                        Y1 = 0,
                        Y2 = 0,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2
                    };
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
                            StrokeThickness = 2
                        };
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
                            StrokeThickness = 2
                        };
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

                    // обновление левой линии
                    if (cellCanvas.Children[0] is Line leftLine)
                    {
                        leftLine.Y1 = cellHeight * 0.25;
                        leftLine.Y2 = cellHeight * 0.75;
                    }

                    // обновление верхней линии
                    if (cellCanvas.Children[1] is Line topLine)
                    {
                        topLine.X1 = cellWidth * 0.25;
                        topLine.X2 = cellWidth * 0.75;
                    }

                    // обновление правой линии
                    if (col == cols - 1 && cellCanvas.Children.Count > 2 && cellCanvas.Children[2] is Line rightLine)
                    {
                        rightLine.X1 = cellWidth;
                        rightLine.X2 = cellWidth;
                        rightLine.Y1 = cellHeight * 0.25;
                        rightLine.Y2 = cellHeight * 0.75;
                    }

                    // обновление нижней линии
                    if (row == rows - 1)
                    {
                        int bottomLineIndex = (col == cols - 1) ? 3 : 2;
                        if (cellCanvas.Children.Count > bottomLineIndex && cellCanvas.Children[bottomLineIndex] is Line bottomLine)
                        {
                            bottomLine.X1 = cellWidth * 0.25;
                            bottomLine.X2 = cellWidth * 0.75;
                            bottomLine.Y1 = cellHeight;
                            bottomLine.Y2 = cellHeight;
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


            Log($"Clicked at ({clickedX}, {clickedY})");
            if (!SelectedChess)
            {
                if (clickedX == BlueChess.posX && clickedY == BlueChess.posY && !currentPlayer)
                {
                    int MinDist = GetShortesDist((BlueChess.posX, BlueChess.posY));
                    Log($"Min Distance: {MinDist}");
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
        private bool IsMoveValid(int startX, int startY, int endX, int endY)
        {
            if (startX < 0 || startY < 0 || startX >= cols || startY >= rows) return false; //ход в поле p.1
            if (endX < 0 || endY < 0 || endX >= cols || endY >= rows) return false; //ход на поле p.2
            if (Math.Abs(startX - endX) + Math.Abs(startY - endY) != 1) return false; //ход на одну клетку
            if ((endX == BlueChess.posX && endY == BlueChess.posY) || (endX == RedChess.posX && endY == RedChess.posY)) //занято ли поле
            {
                if (IsMoveValid(endX, endY, endX + (endX - startX), endY + (endY - startY))) return true; //можно ли перепрыгнуть
                else return false;
            }
            //ещё нужна проверка на стенку
            return true;
        }
        private void MoveChess(Chess chess, int endX, int endY)
        {
            // Удаляем фишку из текущего Canvas
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
                    if (!(targetX == BlueChess.posX && targetY == BlueChess.posY) && !(targetX == RedChess.posX && targetY == RedChess.posY))
                    {
                        possibleMoves.Add((targetX, targetY));
                    }
                    else // Если на клетке есть фишка, проверяем возможность перепрыгивания
                    {
                        int endX = targetX + dx;
                        int endY = targetY + dy;
                        if (endX >= 0 && endX < cols && endY >= 0 && endY < rows && IsMoveValid(targetX, targetY, endX, endY))
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
    }
}