//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Koridor
//{
//    class GameState
//    {
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;

namespace Koridor
{
    public class GameState
    {
        public Chess BlueChess { get; set; }
        public Chess RedChess { get; set; }
        public bool CurrentPlayer { get; set; } // true = красный, false = синий
        public List<(int x, int y, bool horizontal)> Walls { get; set; }
        public AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>> Field { get; set; }
        public static int Rows { get; } = 9;
        public static int Cols { get; } = 9;

        int EdgeWeights = 10;

        public GameState(Chess blueChess, Chess redChess, bool currentPlayer,
                        List<(int x, int y, bool horizontal)> walls,
                        AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>> field)
        {
            BlueChess = new Chess(blueChess.posX, blueChess.posY, blueChess.red);
            BlueChess.WallsCount = blueChess.WallsCount;

            RedChess = new Chess(redChess.posX, redChess.posY, redChess.red);
            RedChess.WallsCount = redChess.WallsCount;

            CurrentPlayer = currentPlayer;
            Walls = new List<(int x, int y, bool horizontal)>(walls);
            Field = new AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>>();


            //Field = field;
            // Копируем граф
            foreach (var vertex in field.Vertices)
            {
                Field.AddVertex(vertex);
            }
            foreach (var edge in field.Edges)
            {
                Field.AddEdge(new TaggedEdge<(int x, int y), int>(edge.Source, edge.Target, edge.Tag));
            }
        }

        public bool IsEndState()
        {
            return BlueChess.posY == 8 || RedChess.posY == 0;
        }

        public string GetWinner()
        {
            if (BlueChess.posY == 8) return "Blue";
            if (RedChess.posY == 0) return "Red";
            return null;
        }

        public int GetShortestDistance((int x, int y) start, bool isRed)
        {
            var dijkstra = new DijkstraShortestPathAlgorithm<(int x, int y), TaggedEdge<(int x, int y), int>>(
                Field,
                edge => edge.Tag
            );
            dijkstra.Compute(start);

            int minDist = int.MaxValue;
            int targetY = isRed ? 0 : 8;

            for (int i = 0; i < Cols; i++)
            {
                if (dijkstra.Distances.TryGetValue((i, targetY), out double distance) && distance < minDist)
                    minDist = (int)distance;
            }

            return minDist;
        }

        public List<(bool isMove, int x, int y, bool horizontal)> GetAllPossibleActions(bool playerOneMaximizer)
        {
            var actions = new List<(bool isMove, int x, int y, bool horizontal)>();

            // Добавляем возможные ходы фишкой
            var chess = playerOneMaximizer ? BlueChess : RedChess;
            var moves = GetAvailableMoves(chess.posX, chess.posY);
            foreach (var move in moves)
            {
                actions.Add((true, move.x, move.y, false));
            }

            // Добавляем возможные установки стен
            if ((playerOneMaximizer && BlueChess.WallsCount > 0) || (!playerOneMaximizer && RedChess.WallsCount > 0))
            {
                var wallPlacements = GetAllAvailableWallPlacements();
                actions.AddRange(wallPlacements);
            }

            return actions;
        }

        public List<(int x, int y)> GetAvailableMoves(int startX, int startY)
        {
            var possibleMoves = new List<(int x, int y)>();
            var directions = new List<(int dx, int dy)> { (0, 1), (1, 0), (-1, 0), (0, -1) };

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

        private bool IsMoveValid(int startX, int startY, int endX, int endY)
        {
            // Проверка выхода за границы поля
            if (endX < 0 || endY < 0 || endX >= Cols || endY >= Rows)
                return false;

            // Проверка расстояния (должен быть ход на 1 клетку)
            if (Math.Abs(startX - endX) + Math.Abs(startY - endY) != 1)
                return false;

            // Проверка на наличие стенки между клетками
            if (HasWallBetween(startX, startY, endX, endY))
                return false;

            return true;
        }

        private bool HasWallBetween(int x1, int y1, int x2, int y2)
        {
            // Горизонтальное перемещение
            if (y1 == y2)
            {
                int leftX = Math.Min(x1, x2);
                return Walls.Any(w =>
                    (w.x == leftX && w.y == y1 && !w.horizontal) ||
                    (w.x == leftX && w.y == y1 - 1 && !w.horizontal));
            }
            // Вертикальное перемещение
            else if (x1 == x2)
            {
                int topY = Math.Min(y1, y2);
                return Walls.Any(w =>
                    (w.x == x1 && w.y == topY && w.horizontal) ||
                    (w.x == x1 - 1 && w.y == topY && w.horizontal));
            }

            return false;
        }

        private List<(bool isMove, int x, int y, bool horizontal)> GetAllAvailableWallPlacements()
        {
            var placements = new List<(bool isMove, int x, int y, bool horizontal)>();

            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    // Проверяем горизонтальную стенку
                    if (CanPlaceWall(i, j, true))
                    {
                        placements.Add((false, i, j, true));
                    }

                    // Проверяем вертикальную стенку
                    if (CanPlaceWall(i, j, false))
                    {
                        placements.Add((false, i, j, false));
                    }
                }
            }
            return placements;
        }

        public bool CanPlaceWall(int x, int y, bool isHorizontal)
        {
            // Проверка границ поля
            if (x >= Cols - 1 || y >= Rows - 1) return false;

            // Проверка пересечений с другими стенками
            foreach (var wall in Walls)
            {
                if (isHorizontal)
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
            UpdateGraphWithWall(x, y, isHorizontal, true);
            Walls.Add((x, y, isHorizontal));

            // Проверка путей для обеих фишек
            bool blueCanReach = GetShortestDistance((BlueChess.posX, BlueChess.posY), false) < 1000;
            bool redCanReach = GetShortestDistance((RedChess.posX, RedChess.posY), true) < 1000;

            // Удаление временной стенки
            Walls.RemoveAt(Walls.Count - 1);
            UpdateGraphWithWall(x, y, isHorizontal, false);

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
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y), (x, y + 1), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y + 1), (x, y), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y), (x + 1, y + 1), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y + 1), (x + 1, y), EdgeWeights));
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
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y), (x + 1, y), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y), (x, y), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x, y + 1), (x + 1, y + 1), EdgeWeights));
                    Field.AddEdge(new TaggedEdge<(int x, int y), int>((x + 1, y + 1), (x, y + 1), EdgeWeights));
                }
            }
        }

        private void RemoveEdgeBetween((int x, int y) source, (int x, int y) target)
        {
            // Находим ребро между вершинами
            if (Field.TryGetEdge(source, target, out var edge))
            {
                Field.RemoveEdge(edge);
            }

            // Удаляем обратное ребро (для неориентированного графа)
            if (Field.TryGetEdge(target, source, out var reverseEdge))
            {
                Field.RemoveEdge(reverseEdge);
            }
        }

        public bool Equal(GameState check_state)
        {
            var l1 = check_state.GetAllPossibleActions(true);
            var l2 = this.GetAllPossibleActions(true);
            if (l1.Count != l2.Count) return false;
            for (int i = 0; i < l1.Count; i++)
            {
                if ((l1[i].isMove != l2[i].isMove) || (l1[i].x != l2[i].x) || (l1[i].y != l2[i].y) || (l1[i].horizontal != l2[i].horizontal)) return false;
            }
            return true;
        }

        //public Chess BlueChess { get; set; }
        //public Chess RedChess { get; set; }
        //public bool CurrentPlayer { get; set; } // true = красный, false = синий
        //public List<(int x, int y, bool horizontal)> Walls { get; set; }
    }
}
