using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Koridor
{
    public partial class MainWindow : Window
    {
        int cols = 9;
        int rows = 9;
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

            // Вертикальные линии
            for (int i = 0; i <= cols; i++)
            {
                Line line = new Line()
                {
                    X1 = cellWidth * i,
                    Y1 = 0,
                    X2 = cellWidth * i,
                    Y2 = canvasHeight,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                };
                canvas.Children.Add(line);
                vertLines.Add(line);
            }

            // Горизонтальные линии
            for (int i = 0; i <= rows; i++)
            {
                Line line = new Line()
                {
                    X1 = 0,
                    Y1 = cellHeight * i,
                    X2 = canvasWidth,
                    Y2 = cellHeight * i,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                };
                canvas.Children.Add(line);
                horizontLines.Add(line);
            }

            //BlueChessElp.Width = cellWidth;
            //BlueChessElp.Height = cellHeight;
            //BlueChessElp.Fill = new SolidColorBrush(Colors.Blue);
            //BlueChessElp.Margin = new Thickness(cellWidth * 4.5,0,0,0);

            //RedChessElp.Width = cellWidth;
            //RedChessElp.Height = cellHeight;
            //RedChessElp.Fill = new SolidColorBrush(Colors.Blue);
            //RedChessElp.Margin = new Thickness(cellWidth * 4.5, canvasHeight - 0.5 * cellHeight, 0, 0);


            BlueChess.red = false;
            BlueChess.posX = 5;
            BlueChess.posY = 0;

            RedChess.red = true;
            RedChess.posX = 5;
            RedChess.posY = 9;

        }

        private void UpdateGrid()
        {
            canvasWidth = canvas.ActualWidth;
            canvasHeight = canvas.ActualHeight;
            cellWidth = canvasWidth / cols;
            cellHeight = canvasHeight / rows;


            // Обновление вертикальных линий
            for (int i = 0; i < vertLines.Count; i++)
            {
                vertLines[i].X1 = cellWidth * i;
                vertLines[i].X2 = cellWidth * i;
                vertLines[i].Y2 = canvasHeight;
            }

            // Обновление горизонтальных линий
            for (int i = 0; i < horizontLines.Count; i++)
            {
                horizontLines[i].Y1 = cellHeight * i;
                horizontLines[i].Y2 = cellHeight * i;
                horizontLines[i].X2 = canvasWidth;
            }


            BlueChessElp.Width = cellWidth;
            BlueChessElp.Height = cellHeight;
            BlueChessElp.Fill = new SolidColorBrush(Colors.Blue);
            BlueChessElp.Margin = new Thickness(cellWidth * 4, 0, 0, 0);

            RedChessElp.Width = cellWidth;
            RedChessElp.Height = cellHeight;
            RedChessElp.Fill = new SolidColorBrush(Colors.Red);
            RedChessElp.Margin = new Thickness(cellWidth * 4, canvasHeight - cellHeight, 0, 0);

            if(canvas.Children.Contains(BlueChessElp)) canvas.Children.Remove(BlueChessElp); canvas.Children.Remove(RedChessElp);
            canvas.Children.Add(BlueChessElp);
            canvas.Children.Add(RedChessElp);
            //BlueChess.red = false;
            //BlueChess.posX = 5;
            //BlueChess.posY = 0;

            //RedChess.red = true;
            //RedChess.posX = 5;
            //RedChess.posY = 9;
        }
    }
}