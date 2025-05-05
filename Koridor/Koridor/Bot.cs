using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;

namespace Koridor
{
    //internal class GameState
    //{
    //    static int cols = 9;
    //    static int rows = 9;



    //    public int GetShortesDist((int x, int y) start, bool red)
    //    {
    //        var dijkstra = new DijkstraShortestPathAlgorithm<(int x, int y), TaggedEdge<(int x, int y), int>>(
    //            field,
    //            edge => edge.Tag
    //        );
    //        dijkstra.Compute(start);

    //        int MinDist = int.MaxValue;

    //        for (int i = 0; i < cols; i++)
    //        {
    //            if (dijkstra.Distances.TryGetValue((i, (red) ? 0 : 8), out double distance) && distance < MinDist) MinDist = (int)distance;

    //        }

    //        return MinDist;
    //    }
    //    private List<(int x, int y, bool horizontal)> GetAllAvailableMoves(bool include_state)
    //    {
    //        List<(int x, int y, bool horizontal)> moves = new List<(int x, int y, bool horizontal)>();

    //        for (int i = 0; i < rows; i++)
    //        {
    //            for (int j = 0; j < cols; j++)
    //            {
    //                if (GetShortesDist((i, j), )


    //            }
    //        }
    //        return moves;
    //    }
    //    private List<(int x, int y, bool horizontal)> GetAllAvailableWallPlacemets(bool include_state)
    //    {
    //        bool temp = isHorizontalWall;
    //        List<(int x, int y, bool horizontal)> moves = new List<(int x, int y, bool horizontal)>();
    //        for (int i = 0; i < rows; i++)
    //        {
    //            for (int j = 0; j < cols; j++)
    //            {
    //                isHorizontalWall = true;
    //                if (CanPlaceWall(i, j)) moves.Add((i, j, true));
    //                isHorizontalWall = false;
    //                if (CanPlaceWall(i, j)) moves.Add((i, j, false));

    //            }
    //        }
    //        isHorizontalWall = temp;
    //        return moves;
    //    }
    //    public List<(int x, int y, bool horizontal)> GetAllChildStates(bool player_one_maximizer, bool include_state = true)
    //    {
    //        List<(int x, int y)> children = new List<(int x, int y)>();
    //        List<(int x, int y)> available_moves = new List<(int x, int y)>();

    //        foreach ((int x, int y) in available_moves)
    //        {
    //            children.Add((x, y));
    //        }
    //        List<(int x, int y, bool horizontal)> available_wall_placements = new List<(int x, int y, bool horizontal)>();
    //        available_wall_placements = GetAllAvailableWallPlacemets(include_state);

    //        foreach ((int x, int y, bool horizontal) in available_wall_placements)
    //        {

    //        }


    //        return children;
    //    }
    //}
    internal class Bot
    {
        // Статья про алгоритм: "https://www.labri.fr/perso/renault/working/teaching/projets/files/glendenning_ugrad_thesis.pdf"



        // Алгоритм A*. Подробнее:"https://ru.wikipedia.org/wiki/A*"
        private int astar(MainWindow GameBoard, bool check_blockage)
        {
            int ShortestDist;
            if (GameBoard.currentPlayer)
            {
                ShortestDist = GameBoard.GetShortesDist((GameBoard.RedChess.posX, GameBoard.RedChess.posY), true);
            }
            else
            {
                ShortestDist = GameBoard.GetShortesDist((GameBoard.BlueChess.posX, GameBoard.BlueChess.posY), false);
            }
            if (ShortestDist > 1000) return 0;
            else if (check_blockage) return 1;

            return ShortestDist * 100;
        }
        private double StateEvaluationHeuristic(MainWindow GameBoard, bool player_one_maximazer, bool is_expectimax)
        {
            Chess RedPlayer = GameBoard.RedChess;
            Chess BluePlayer = GameBoard.BlueChess;

            int red_player_dist = RedPlayer.posY; // player_one
            int blue_player_dist = 8 - BluePlayer.posY; // player_two

            double result = 0;

            if (player_one_maximazer)
            {
                int opponent_path_len = blue_player_dist;
                int player_path_len = red_player_dist;
                if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
                {
                    bool prev = GameBoard.currentPlayer;
                    GameBoard.currentPlayer = true;
                    player_path_len = astar(GameBoard, false);
                    GameBoard.currentPlayer = prev;
                }
                result += opponent_path_len;
                result -= red_player_dist;
                int num = 100;
                if (player_path_len != 0)
                {
                    num = player_path_len;
                }
                result += Math.Round((double)100 / num, 2);

                int num_1 = 50;
                if (blue_player_dist != 0)
                {
                    num_1 = blue_player_dist;
                }
                result -= Math.Round((double)50 / num_1, 2);

                result += (RedPlayer.WallsCount - BluePlayer.WallsCount);
                if (RedPlayer.posY == 8) result += 100;
                if (player_path_len == 0 && RedPlayer.posY != 8) result -= 500;
            }
            else
            {
                int opponent_path_len = red_player_dist;
                int player_path_len = blue_player_dist;
                if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
                {
                    bool prev = GameBoard.currentPlayer;
                    GameBoard.currentPlayer = false;
                    player_path_len = astar(GameBoard, false);
                    GameBoard.currentPlayer = prev;
                }
                if (!is_expectimax) result += opponent_path_len;
                else result += 17 * opponent_path_len;

                result -= blue_player_dist;
                int num = 100;
                if (player_path_len != 0)
                {
                    num = player_path_len;
                }
                result += Math.Round((double)100 / num, 2);

                int num_1 = 50;
                if (red_player_dist != 0)
                {
                    num_1 = red_player_dist;
                }
                result -= Math.Round((double)50 / num_1, 2);

                result += (BluePlayer.WallsCount - RedPlayer.WallsCount);
                if (BluePlayer.posY == 0) result += 100;
                if (player_path_len == 0 && BluePlayer.posY != 0) result -= 500;
            }
            return result;


        }
        private double MinimaxAlphaBetaPruning(List<(bool player_move, int x, int y, bool horizontal)> GameBoard, int depth, double alpha, double beta, bool maximizing_player, bool player_one_minimax)
        {
            // Нужно дописать метод state_evaluation_heuristic
            if (depth == 0) return StateEvaluationHeuristic(GameBoard, player_one_minimax, false);
            if (maximizing_player)
            {
                double max_eval = Double.NegativeInfinity;
                foreach (var child in GameBoard.GetAllChildStates(player_one_minimax))
                {
                    double ev = MinimaxAlphaBetaPruning(child, depth - 1, alpha, beta, false, player_one_minimax);
                    max_eval = (max_eval > ev) ? max_eval : ev;
                    alpha = (alpha > ev) ? alpha : ev;
                    if (beta <= alpha) break;
                }
                return max_eval;
            }
            else
            {
                double min_eval = Double.PositiveInfinity;
                foreach (var child in GetAllChildStates(player_one_minimax))
                {
                    double ev = MinimaxAlphaBetaPruning(child, depth - 1, alpha, beta, false, player_one_minimax);
                    min_eval = (min_eval > ev) ? ev : min_eval;
                    beta = (alpha > ev) ? ev : alpha;
                    if (beta <= alpha) break;
                }
                return min_eval;
            }


        }

        private bool ChooseAction(MainWindow GameBoard,Dictionary<int, (bool player_move,int x, int y, bool horizontal)> d)
        {
            //if (d.Keys.Count == 0) return false; // обработчик исключения

            var winner = d[d.Keys.Max()];
            bool action = winner.player_move;

            if (action) GameBoard.MoveChess(GameBoard.BlueChess, winner.x, winner.y);
            else GameBoard.AddWall(winner.x, winner.y, winner.horizontal);
            return action;
            
        }
        private void MinimaxAgent(MainWindow GameBoard, bool player_one_minimax)
        {
            Dictionary<double, List<(bool player_move,int x, int y, bool horizontal)>> d = new Dictionary<double, List<(bool player_move, int x, int y, bool horizontal)>();
            double value;
            foreach (var child in GameBoard.GetAllChildStates(player_one_minimax))
            {
                // child - список кортежей List<(bool player_move, int x, int y, bool horizontal)>
                value = MinimaxAlphaBetaPruning(child, 3, Double.NegativeInfinity, Double.PositiveInfinity, false, player_one_minimax);
                d.Add(value, child);
            }

            return ChooseAction(GameBoard,d);

        }

    }
}
