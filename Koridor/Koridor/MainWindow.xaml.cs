using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;
using System.Collections.Generic;


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

        public Chess BlueChess = new Chess(4, 0, false);
        public Chess RedChess = new Chess(4, 8, true);
        Ellipse BlueChessElp = new Ellipse();
        Ellipse RedChessElp = new Ellipse();

        public bool currentPlayer = true; // true = красный, false = синий
        bool SelectedChess = false;
        Chess SelectedChessObject;

        public AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>> field = new AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>>();

        private bool isPlacingWall = false;
        private bool isHorizontalWall = true;
        public List<(int x, int y, bool horizontal)> walls = new List<(int x, int y, bool horizontal)>();

        bool isBotSelected = true;
        private Bot gameBot = new Bot();

        private DateTime gameStartTime;
        private int totalMoves = 0;
        private List<GameStats> gameHistory = new List<GameStats>();

        public MainWindow()
        {
            InitializeComponent();
            gameStartTime = DateTime.Now;
            totalMoves = 0;
            UpdateCurrentPlayerDisplay();
            DrawGrid();
            FillField();
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            UpdateWallButtons();

        }

        private void UpdateWallButtons()
        {
            if (!isPlacingWall)
            {
                HorizontalWallButton.Background = Brushes.White;
                VerticalWallButton.Background = Brushes.White;
            }
            else
            {
                HorizontalWallButton.Background = isHorizontalWall ? Brushes.LightGreen : Brushes.White;
                VerticalWallButton.Background = !isHorizontalWall ? Brushes.LightGreen : Brushes.White;
            }
        }

        private void ToggleWallPlacementMode(object sender, RoutedEventArgs e)
        {
            if ((currentPlayer && RedChess.WallsCount == 0) ||
                (!currentPlayer && BlueChess.WallsCount == 0))
            {
                isPlacingWall = false;
                WallModeButton.Background = Brushes.White;
                return;
            }

            if (!SelectedChess)
            {
                isPlacingWall = !isPlacingWall;
                if (!isPlacingWall)
                {
                    ClearWallHighlights();
                }
            }
            WallModeButton.Background = isPlacingWall ? Brushes.LightGreen : Brushes.White;
            UpdateWallButtons();
        }
        private void SwitchPlayer()
        {
            currentPlayer = !currentPlayer;
            UpdateCurrentPlayerDisplay();
            ResetWallButtons(); // Сбрасываем кнопки стенок при смене хода
        }

        private void ResetWallButtons()
        {
            isPlacingWall = false;
            isHorizontalWall = true;
            WallModeButton.Background = Brushes.White;
            HorizontalWallButton.Background = Brushes.White;
            VerticalWallButton.Background = Brushes.White;
            ClearWallHighlights();
        }
        private void SetHorizontalWall(object sender, RoutedEventArgs e)
        {
            isHorizontalWall = true;
            UpdateWallButtons();
            ClearWallHighlights();
        }

        private void SetVerticalWall(object sender, RoutedEventArgs e)
        {
            isHorizontalWall = false;
            UpdateWallButtons();
            ClearWallHighlights();
        }

        private void AddWall(int x, int y, bool horizontal)
        {
            totalMoves++;
            Rectangle wall = new Rectangle
            {
                Fill = Brushes.Black,
                Stroke = Brushes.DarkGray,

            };
            if (currentPlayer)
            {
                RedChess.WallsCount -= 1;
                RedWallsBox.Text = (RedChess.WallsCount).ToString();
            }
            else
            {
                BlueChess.WallsCount -= 1;
                BlueWallsBox.Text = (BlueChess.WallsCount).ToString();
            }

            if (horizontal)
            {
                wall.Width = cellWidth * 2;
                wall.Height = 8;
                Canvas.SetLeft(wall, x * cellWidth);
                Canvas.SetTop(wall, (y + 1) * cellHeight - 2);
            }
            else
            {
                wall.Width = 8;
                wall.Height = cellHeight * 2;
                Canvas.SetLeft(wall, (x + 1) * cellWidth - 2);
                Canvas.SetTop(wall, y * cellHeight);
            }

            canvas.Children.Add(wall);
            walls.Add((x, y, horizontal));
            UpdateGraphWithWall(x, y, horizontal, true);
            //SwitchPlayer();
        }
        private void UpdateCurrentPlayerDisplay()
        {
            if (currentPlayer) // Красный игрок
            {
                CurrentPlayerText.Text = "Ход: Красный игрок";
                CurrentPlayerText.Foreground = Brushes.Red;
                RedWallsBox.BorderThickness = new Thickness(3);
                RedWallsBox.BorderBrush = Brushes.Red;
                BlueWallsBox.BorderThickness = new Thickness(0);
            }
            else // Синий игрок
            {
                CurrentPlayerText.Text = "Ход: Синий игрок";
                CurrentPlayerText.Foreground = Brushes.Blue;
                BlueWallsBox.BorderThickness = new Thickness(3);
                BlueWallsBox.BorderBrush = Brushes.Blue;
                RedWallsBox.BorderThickness = new Thickness(0);
            }
        }
        private bool CanPlaceWall(int x, int y)
        {
            // Проверка границ поля
            if (x >= cols - 1 || y >= rows - 1) return false;

            // Проверка пересечений с другими стенками
            foreach (var wall in walls)
            {
                if (isHorizontalWall)
                {
                    // Для горизонтальной стенки
                    if (wall.horizontal && wall.y == y &&
                        (wall.x == x || wall.x == x - 1 || wall.x == x + 1))
                        return false;

                    // Проверка пересечения с вертикальными стенками
                    if (!wall.horizontal &&
                         wall.x == x && wall.y == y)
                        return false;
                }
                else
                {
                    // Для вертикальной стенки
                    if (!wall.horizontal && wall.x == x &&
                        (wall.y == y || wall.y == y - 1 || wall.y == y + 1))
                        return false;

                    // Проверка пересечения с горизонтальными стенками
                    if (wall.horizontal &&
                         wall.y == y && wall.x == x)
                        return false;
                }
            }

            // Временная установка стенки для проверки
            UpdateGraphWithWall(x, y, isHorizontalWall, true);
            walls.Add((x, y, isHorizontalWall));

            // Проверка путей для обеих фишек
            bool blueCanReach = GetShortesDist((BlueChess.posX, BlueChess.posY), false) < 1000;
            bool redCanReach = GetShortesDist((RedChess.posX, RedChess.posY), true) < 1000;

            // Удаление временной стенки
            walls.RemoveAt(walls.Count - 1);
            UpdateGraphWithWall(x, y, isHorizontalWall, false);

            return blueCanReach && redCanReach;
        }


        private void UpdateGraphWithWall(int x, int y, bool horizontal, bool remove)
        {

            if (horizontal)
            {
                if (remove)
                {
                    // Удаляем вертикальные связи между клетками
                    RemoveEdgeBetween((x, y), (x, y + 1));
                    RemoveEdgeBetween((x + 1, y), (x + 1, y + 1));
                }
                else
                {
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y), (x, y + 1), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y + 1), (x, y), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y), (x + 1, y + 1), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y + 1), (x + 1, y), 1));
                }
            }
            else
            {
                if (remove)
                {
                    // Удаляем горизонтальные связи между клетками
                    RemoveEdgeBetween((x, y), (x + 1, y));
                    RemoveEdgeBetween((x, y + 1), (x + 1, y + 1));

                }
                else
                {
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y), (x + 1, y), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y), (x, y), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y + 1), (x + 1, y + 1), 1));
                    field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y + 1), (x, y + 1), 1));
                }
            }
        }

        private void RemoveEdgeBetween((int x, int y) source, (int x, int y) target)
        {
            // Находим ребро между вершинами
            if (field.TryGetEdge(source, target, out var edge))
            {
                field.RemoveEdge(edge);
            }

            // Удаляем обратное ребро (для неориентированного графа)
            if (field.TryGetEdge(target, source, out var reverseEdge))
            {
                field.RemoveEdge(reverseEdge);
            }
        }



        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            var menuWindow = new MenuWindow();
            menuWindow.Show();
            this.Close();
        }



        private void FillField()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    field.AddVertex((i, j));

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

        public int GetShortesDist((int x, int y) start, bool red)
        {
            var dijkstra = new DijkstraShortestPathAlgorithm<(int x, int y), TaggedEdge<(int x, int y), int>>(
                field,
                edge => edge.Tag
            );
            dijkstra.Compute(start);

            int MinDist = int.MaxValue;

            for (int i = 0; i < cols; i++)
            {
                if (dijkstra.Distances.TryGetValue((i, (red) ? 0 : 8), out double distance) && distance < MinDist) MinDist = (int)distance;

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
                    cellCanvas.MouseMove += CellCanvas_MouseMove;

                    // вертикальная линия (левая)
                    Line leftLine = new Line()
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = cellHeight * 0.25,
                        Y2 = cellHeight * 0.75,
                        Stroke = Brushes.DarkGray,
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
                        Stroke = Brushes.DarkGray,
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
                            Stroke = Brushes.DarkGray,
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
                            Stroke = Brushes.DarkGray,
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
        private void CellCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isPlacingWall || (!currentPlayer && isBotSelected)) return;

            // Очищаем предыдущие подсветки
            ClearWallHighlights();

            // Получаем текущую позицию мыши
            Point mousePos = e.GetPosition(canvas);
            int cellX = (int)(mousePos.X / cellWidth);
            int cellY = (int)(mousePos.Y / cellHeight);

            // Проверяем границы поля для стенок
            if (cellX >= cols - 1 || cellY >= rows - 1) return;

            // Создаем подсветку для потенциальной стенки
            Rectangle highlight = new Rectangle
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                Opacity = 0.7
            };

            if (isHorizontalWall)
            {
                // Горизонтальная стенка
                highlight.Width = cellWidth * 2;
                highlight.Height = 4;
                Canvas.SetLeft(highlight, cellX * cellWidth);
                Canvas.SetTop(highlight, (cellY + 1) * cellHeight - 2);
            }
            else
            {
                // Вертикальная стенка
                highlight.Width = 4;
                highlight.Height = cellHeight * 2;
                Canvas.SetLeft(highlight, (cellX + 1) * cellWidth - 2);
                Canvas.SetTop(highlight, cellY * cellHeight);
            }

            // Проверяем, можно ли здесь поставить стенку
            if (!CanPlaceWall(cellX, cellY) ||
                (currentPlayer && int.Parse(RedWallsBox.Text) == 0) ||
                 (!currentPlayer && int.Parse(BlueWallsBox.Text) == 0))
            {
                highlight.Stroke = Brushes.Red;
            }

            // Добавляем подсветку на canvas и запоминаем ее
            canvas.Children.Add(highlight);
            currentWallHighlight = highlight;
        }

        private Rectangle currentWallHighlight = null;

        private void ClearWallHighlights()
        {
            if (currentWallHighlight != null)
            {
                canvas.Children.Remove(currentWallHighlight);
                currentWallHighlight = null;
            }
        }

        //public void Log(string message)
        //{
        //    DebugTextBox.Text += message + "\n";
        //    DebugTextBox.ScrollToEnd();
        //}


        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!currentPlayer && isBotSelected)
            {
                //Dispatcher.BeginInvoke(new Action(MakeBotMove),
                //    System.Windows.Threading.DispatcherPriority.Background);
                MakeBotMove();
                //Log($"Current player: {(currentPlayer ? "red" : "blue")}");
                UpdateCurrentPlayerDisplay();

            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ClearWallHighlights();
            Point clickPoint = e.GetPosition(canvas);

            int clickedX = (int)(clickPoint.X / cellWidth);
            int clickedY = (int)(clickPoint.Y / cellHeight);


            if (isPlacingWall)
            {
                if (CanPlaceWall(clickedX, clickedY) && ((currentPlayer && RedChess.WallsCount > 0) ||
                    (!isBotSelected && !currentPlayer && BlueChess.WallsCount > 0)))
                {

                    AddWall(clickedX, clickedY, isHorizontalWall);
                    currentPlayer = !currentPlayer; // Передаем ход
                    //Log($"Wall placed at ({clickedX}, {clickedY},{isHorizontalWall}),{(currentPlayer ? "red" : "blue")}");
                }
            }
            else
            {

                //Log($"Clicked at ({clickedX}, {clickedY})");
                if (!SelectedChess)
                {
                    if (clickedX == BlueChess.posX && clickedY == BlueChess.posY && !currentPlayer)
                    {
                        int MinDist = GetShortesDist((BlueChess.posX, BlueChess.posY), false);
                        //Log($"Min Distance: {MinDist}");
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
                        //Log($"Moved to ({clickedX}, {clickedY})");
                        currentPlayer = !currentPlayer;
                        //Log($"Current player: {(currentPlayer ? "red" : "blue")}");
                    }

                    ClearHighlights();
                    SelectedChess = false;
                    SelectedChessObject = null;
                }
            }
            //if (!currentPlayer && isBotSelected) MakeBotMove();
            if (BlueChess.posY == 8) MessageBox.Show("Синия фишка победила!", "Поздравляю");
            if (RedChess.posY == 0) MessageBox.Show("Красная фишка победила!", "Поздравляю");
            UpdateCurrentPlayerDisplay();

        }


        private void SaveGameStats(GameStats stats)
        {
            try
            {
                // Загружаем текущую историю
                var fullHistory = LoadAllGameStats();

                // Добавляем новую запись
                fullHistory.Add(stats);

                // Сохраняем обновленную историю
                string json = JsonConvert.SerializeObject(fullHistory, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("game_stats.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении статистики: {ex.Message}");
            }
        }

        private List<GameStats> LoadAllGameStats()
        {
            string filePath = "game_stats.json";
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<GameStats>>(json)
                           ?? new List<GameStats>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}");
            }
            return new List<GameStats>();
        }

        private bool IsMoveValid(int startX, int startY, int endX, int endY)
        {
            // Проверка выхода за границы поля
            if (endX < 0 || endY < 0 || endX >= cols || endY >= rows)
                return false;

            // Проверка расстояния (должен быть ход на 1 клетку)
            if (Math.Abs(startX - endX) + Math.Abs(startY - endY) != 1)
                return false;

            // Проверка на наличие стенки между клетками
            if (HasWallBetween(startX, startY, endX, endY))
                return false;

            // Проверка на занятость клетки другой фишкой
            if ((endX == BlueChess.posX && endY == BlueChess.posY) ||
                (endX == RedChess.posX && endY == RedChess.posY))
            {
                // Проверка возможности перепрыгивания
                int jumpX = endX + (endX - startX);
                int jumpY = endY + (endY - startY);

                // Проверка границ поля после прыжка
                if (jumpX < 0 || jumpY < 0 || jumpX >= cols || jumpY >= rows)
                    return false;

                // Полностью запрещаем прыжки, если между клетками есть стенка
                if (HasWallBetween(endX, endY, jumpX, jumpY))
                    return false;

                // Проверка занятости клетки после прыжка
                if ((jumpX == BlueChess.posX && jumpY == BlueChess.posY) ||
                    (jumpX == RedChess.posX && jumpY == RedChess.posY))
                    return false;

                return true;
            }

            return true;
        }

        private bool HasWallBetween(int x1, int y1, int x2, int y2)
        {
            // Горизонтальное перемещение
            if (y1 == y2)
            {
                int leftX = Math.Min(x1, x2);
                return walls.Any(w =>
                    (w.x == leftX && w.y == y1 && !w.horizontal) ||
                    (w.x == leftX && w.y == y1 - 1 && !w.horizontal)); // Вертикальная стенка
            }
            // Вертикальное перемещение
            else if (x1 == x2)
            {
                int topY = Math.Min(y1, y2);
                return walls.Any(w =>
                    (w.x == x1 && w.y == topY && w.horizontal) ||
                    (w.x == x1 - 1 && w.y == topY && w.horizontal)); // Горизонтальная стенка
            }

            return false;
        }

        private void MoveChess(Chess chess, int endX, int endY)
        {
            totalMoves++;
            // Проверка на прыжок через фишку
            if ((endX == BlueChess.posX && endY == BlueChess.posY) ||
                (endX == RedChess.posX && endY == RedChess.posY))
            {
                // Вычисляем клетку после прыжка
                int jumpX = endX + (endX - chess.posX);
                int jumpY = endY + (endY - chess.posY);
                endX = jumpX;
                endY = jumpY;
            }

            // Удаляем фишку из текущей позиции
            foreach (var child in canvas.Children)
            {
                if (child is Canvas cellCanvas)
                {
                    if (cellCanvas.Children.Contains(chess.red ? RedChessElp : BlueChessElp))
                    {
                        cellCanvas.Children.Remove(chess.red ? RedChessElp : BlueChessElp);
                    }
                }
            }

            // Обновляем позицию фишки
            chess.posX = endX;
            chess.posY = endY;

            // Добавляем фишку в новую позицию
            int newIndex = endY * cols + endX;
            if (newIndex < canvas.Children.Count && canvas.Children[newIndex] is Canvas newCell)
            {
                Canvas.SetLeft(chess.red ? RedChessElp : BlueChessElp,
                              (cellWidth - (chess.red ? RedChessElp.Width : BlueChessElp.Width)) / 2);
                Canvas.SetTop(chess.red ? RedChessElp : BlueChessElp,
                             (cellHeight - (chess.red ? RedChessElp.Height : BlueChessElp.Height)) / 2);
                newCell.Children.Add(chess.red ? RedChessElp : BlueChessElp);
            }
        }
        public List<(int x, int y)> GetAvailableMoves(int startX, int startY, bool include_state = true)
        {
            var possibleMoves = new List<(int x, int y)>();
            var directions = new List<(int dx, int dy)> { (0, -1), (0, 1), (-1, 0), (1, 0) };

            foreach (var (dx, dy) in directions)
            {
                int targetX = startX + dx;
                int targetY = startY + dy;

                if (IsMoveValid(startX, startY, targetX, targetY))
                {
                    // Если клетка занята фишкой - проверяем возможность прыжка
                    if ((targetX == BlueChess.posX && targetY == BlueChess.posY) ||
                        (targetX == RedChess.posX && targetY == RedChess.posY))
                    {
                        int jumpX = targetX + dx;
                        int jumpY = targetY + dy;

                        // Добавляем клетку после прыжка, если прыжок возможен
                        if (IsMoveValid(targetX, targetY, jumpX, jumpY))
                        {
                            possibleMoves.Add((jumpX, jumpY));
                        }
                    }
                    else
                    {
                        possibleMoves.Add((targetX, targetY));
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
        // Методы для бота

        // Дописать

        //public List<(bool isMove, int x, int y, bool horizontal)> GetAllPossibleActions(bool playerOneMaximizer)
        //{
        //    var actions = new List<(bool isMove, int x, int y, bool horizontal)>();

        //    // Добавляем возможные ходы фишкой
        //    var chess = playerOneMaximizer ? BlueChess : RedChess;
        //    var moves = GetAvailableMoves(chess.posX, chess.posY);
        //    foreach (var move in moves)
        //    {
        //        actions.Add((true, move.x, move.y, false));
        //    }

        //    // Добавляем возможные установки стен
        //    if ((playerOneMaximizer && BlueChess.WallsCount > 0) || (!playerOneMaximizer && RedChess.WallsCount > 0))
        //    {
        //        var wallPlacements = GetAllAvailableWallPlacements();
        //        actions.AddRange(wallPlacements);
        //    }

        //    return actions;
        //}

        //private bool CanPlaceWallAnother(int x, int y, bool isHorizontal)
        //{
        //    // Проверка границ поля
        //    if (x >= cols - 1 || y >= rows - 1) return false;

        //    // Проверка пересечений с другими стенками
        //    foreach (var wall in walls)
        //    {
        //        if (isHorizontal)
        //        {
        //            // Для горизонтальной стенки
        //            if (wall.horizontal && wall.y == y &&
        //                (wall.x == x || wall.x == x - 1 || wall.x == x + 1))
        //                return false;

        //            // Проверка пересечения с вертикальными стенками
        //            if (!wall.horizontal &&
        //                 wall.x == x && wall.y == y)
        //                return false;
        //        }
        //        else
        //        {
        //            // Для вертикальной стенки
        //            if (!wall.horizontal && wall.x == x &&
        //                (wall.y == y || wall.y == y - 1 || wall.y == y + 1))
        //                return false;

        //            // Проверка пересечения с горизонтальными стенками
        //            if (wall.horizontal &&
        //                 wall.y == y && wall.x == x)
        //                return false;
        //        }
        //    }

        //    // Временная установка стенки для проверки
        //    var tempWalls = new List<(int x, int y, bool horizontal)>(walls);
        //    tempWalls.Add((x, y, isHorizontal));

        //    // Проверка путей для обеих фишек
        //    bool blueCanReach = GetShortesDist((BlueChess.posX, BlueChess.posY), false) < 1000;
        //    bool redCanReach = GetShortesDist((RedChess.posX, RedChess.posY), true) < 1000;

        //    return blueCanReach && redCanReach;
        //}

        //private List<(bool isMove, int x, int y, bool horizontal)> GetAllAvailableWallPlacements()
        //{
        //    var placements = new List<(bool isMove, int x, int y, bool horizontal)>();

        //    for (int i = 0; i < cols - 1; i++)
        //    {
        //        for (int j = 0; j < rows - 1; j++)
        //        {
        //            // Проверяем горизонтальную стенку
        //            if (CanPlaceWallAnother(i, j, true))
        //            {
        //                placements.Add((false, i, j, true));
        //            }

        //            // Проверяем вертикальную стенку
        //            if (CanPlaceWallAnother(i, j, false))
        //            {
        //                placements.Add((false, i, j, false));
        //            }
        //        }
        //    }

        //    return placements;
        //}


        //private void MakeBotMove()
        //{
        //    if (!currentPlayer && isBotSelected && !isPlacingWall)
        //    {
        //        var bestMove = gameBot.FindBestMove(this);

        //        if (bestMove.isMove)
        //        {
        //            MoveChess(BlueChess, bestMove.x, bestMove.y);
        //        }
        //        else
        //        {
        //            AddWall(bestMove.x, bestMove.y, bestMove.horizontal);
        //        }

        //        currentPlayer = !currentPlayer;
        //    }
        //}

        private void MakeBotMove()
        {
            if (!currentPlayer && isBotSelected && !isPlacingWall)
            {
                var bestMove = gameBot.FindBestMove(this);

                if (bestMove.isMove)
                {
                    MoveChess(BlueChess, bestMove.x, bestMove.y);
                }
                else
                {
                    AddWall(bestMove.x, bestMove.y, bestMove.horizontal);
                }

                currentPlayer = !currentPlayer;
            }
        }

        private List<(int x, int y, bool horizontal)> GetAllAvailableWallPlacemets(bool include_state)
        {
            bool temp = isHorizontalWall;
            List<(int x, int y, bool horizontal)> moves = new List<(int x, int y, bool horizontal)>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    isHorizontalWall = true;
                    if (CanPlaceWall(i, j)) moves.Add((i, j, true));
                    isHorizontalWall = false;
                    if (CanPlaceWall(i, j)) moves.Add((i, j, false));

                }
            }
            isHorizontalWall = temp;
            return moves;
        }
        public List<(bool player_move, int x, int y, bool horizontal)> GetAllChildStates(bool player_one_maximizer, bool include_state = true)
        {
            var children = new List<(bool player_move, int x, int y, bool horizontal)>();

            foreach ((int x, int y) in GetAvailableMoves((player_one_maximizer) ? BlueChess.posX : RedChess.posX, (player_one_maximizer) ? BlueChess.posY : RedChess.posY, include_state))
            {
                children.Add((true, x, y, false));
            }

            foreach ((int x, int y, bool h) in GetAllAvailableWallPlacemets(include_state))
            {
                children.Add((false, x, y, h));
            }

            return children;
        }
    }


}