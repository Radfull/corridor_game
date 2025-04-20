using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
        double canvasWidth ;
        double canvasHeight;
        double cellWidth;
        double cellHeight;

        Chess BlueChess = new Chess();
        Chess RedChess = new Chess();
        Ellipse BlueChessElp = new Ellipse();
        Ellipse RedChessElp = new Ellipse();

        bool SelectedChess;

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();
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

                    cellCanvas.MouseDown += (sender, e) =>
                    {
                        cellCanvas.Background = Brushes.Green;
                    };

                    canvas.Children.Add(cellCanvas);
                }
            }
            BlueChess.red = false;
            BlueChess.posX = 4;
            BlueChess.posY = 0;

            RedChess.red = true;
            RedChess.posX = 4;
            RedChess.posY = 8;
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

    }
}