//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Ink;
//using QuikGraph;
//using QuikGraph.Algorithms.ShortestPath;

//namespace Koridor
//{
//    //internal class GameState
//    //{
//    //    static int cols = 9;
//    //    static int rows = 9;



//    //    public int GetShortesDist((int x, int y) start, bool red)
//    //    {
//    //        var dijkstra = new DijkstraShortestPathAlgorithm<(int x, int y), TaggedEdge<(int x, int y), int>>(
//    //            field,
//    //            edge => edge.Tag
//    //        );
//    //        dijkstra.Compute(start);

//    //        int MinDist = int.MaxValue;

//    //        for (int i = 0; i < cols; i++)
//    //        {
//    //            if (dijkstra.Distances.TryGetValue((i, (red) ? 0 : 8), out double distance) && distance < MinDist) MinDist = (int)distance;

//    //        }

//    //        return MinDist;
//    //    }
//    //    private List<(int x, int y, bool horizontal)> GetAllAvailableMoves(bool include_state)
//    //    {
//    //        List<(int x, int y, bool horizontal)> moves = new List<(int x, int y, bool horizontal)>();

//    //        for (int i = 0; i < rows; i++)
//    //        {
//    //            for (int j = 0; j < cols; j++)
//    //            {
//    //                if (GetShortesDist((i, j), )


//    //            }
//    //        }
//    //        return moves;
//    //    }
//    //    private List<(int x, int y, bool horizontal)> GetAllAvailableWallPlacemets(bool include_state)
//    //    {
//    //        bool temp = isHorizontalWall;
//    //        List<(int x, int y, bool horizontal)> moves = new List<(int x, int y, bool horizontal)>();
//    //        for (int i = 0; i < rows; i++)
//    //        {
//    //            for (int j = 0; j < cols; j++)
//    //            {
//    //                isHorizontalWall = true;
//    //                if (CanPlaceWall(i, j)) moves.Add((i, j, true));
//    //                isHorizontalWall = false;
//    //                if (CanPlaceWall(i, j)) moves.Add((i, j, false));

//    //            }
//    //        }
//    //        isHorizontalWall = temp;
//    //        return moves;
//    //    }
//    //    public List<(int x, int y, bool horizontal)> GetAllChildStates(bool player_one_maximizer, bool include_state = true)
//    //    {
//    //        List<(int x, int y)> children = new List<(int x, int y)>();
//    //        List<(int x, int y)> available_moves = new List<(int x, int y)>();

//    //        foreach ((int x, int y) in available_moves)
//    //        {
//    //            children.Add((x, y));
//    //        }
//    //        List<(int x, int y, bool horizontal)> available_wall_placements = new List<(int x, int y, bool horizontal)>();
//    //        available_wall_placements = GetAllAvailableWallPlacemets(include_state);

//    //        foreach ((int x, int y, bool horizontal) in available_wall_placements)
//    //        {

//    //        }


//    //        return children;
//    //    }
//    //}
//    internal class Bot
//    {
//        // Статья про алгоритм: "https://www.labri.fr/perso/renault/working/teaching/projets/files/glendenning_ugrad_thesis.pdf"



//        // Алгоритм A*. Подробнее:"https://ru.wikipedia.org/wiki/A*"
//        private int astar(MainWindow GameBoard, bool check_blockage)
//        {
//            int ShortestDist;
//            if (GameBoard.currentPlayer)
//            {
//                ShortestDist = GameBoard.GetShortesDist((GameBoard.RedChess.posX, GameBoard.RedChess.posY), true);
//            }
//            else
//            {
//                ShortestDist = GameBoard.GetShortesDist((GameBoard.BlueChess.posX, GameBoard.BlueChess.posY), false);
//            }
//            if (ShortestDist > 1000) return 0;
//            else if (check_blockage) return 1;

//            return ShortestDist * 100;
//        }
//        private double StateEvaluationHeuristic(MainWindow GameBoard, bool player_one_maximazer, bool is_expectimax)
//        {
//Chess RedPlayer = GameBoard.RedChess;
//Chess BluePlayer = GameBoard.BlueChess;

//int red_player_dist = RedPlayer.posY; // player_one
//int blue_player_dist = 8 - BluePlayer.posY; // player_two

//double result = 0;

//if (player_one_maximazer)
//{
//    int opponent_path_len = blue_player_dist;
//    int player_path_len = red_player_dist;
//    if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
//    {
//        bool prev = GameBoard.currentPlayer;
//        GameBoard.currentPlayer = true;
//        player_path_len = astar(GameBoard, false);
//        GameBoard.currentPlayer = prev;
//    }
//    result += opponent_path_len;
//    result -= red_player_dist;
//    int num = 100;
//    if (player_path_len != 0)
//    {
//        num = player_path_len;
//    }
//    result += Math.Round((double)100 / num, 2);

//    int num_1 = 50;
//    if (blue_player_dist != 0)
//    {
//        num_1 = blue_player_dist;
//    }
//    result -= Math.Round((double)50 / num_1, 2);

//    result += (RedPlayer.WallsCount - BluePlayer.WallsCount);
//    if (RedPlayer.posY == 8) result += 100;
//    if (player_path_len == 0 && RedPlayer.posY != 8) result -= 500;
//}
//else
//{
//    int opponent_path_len = red_player_dist;
//    int player_path_len = blue_player_dist;
//    if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
//    {
//        bool prev = GameBoard.currentPlayer;
//        GameBoard.currentPlayer = false;
//        player_path_len = astar(GameBoard, false);
//        GameBoard.currentPlayer = prev;
//    }
//    if (!is_expectimax) result += opponent_path_len;
//    else result += 17 * opponent_path_len;

//    result -= blue_player_dist;
//    int num = 100;
//    if (player_path_len != 0)
//    {
//        num = player_path_len;
//    }
//    result += Math.Round((double)100 / num, 2);

//    int num_1 = 50;
//    if (red_player_dist != 0)
//    {
//        num_1 = red_player_dist;
//    }
//    result -= Math.Round((double)50 / num_1, 2);

//    result += (BluePlayer.WallsCount - RedPlayer.WallsCount);
//    if (BluePlayer.posY == 0) result += 100;
//    if (player_path_len == 0 && BluePlayer.posY != 0) result -= 500;
//}
//return result;


//        }
//        private double MinimaxAlphaBetaPruning(GameState GameBoard, int depth, double alpha, double beta, bool maximizing_player, bool player_one_minimax)
//        {
//            // Нужно дописать метод state_evaluation_heuristic
//            if (depth == 0) return StateEvaluationHeuristic(GameBoard, player_one_minimax, false);
//            if (maximizing_player)
//            {
//                double max_eval = Double.NegativeInfinity;
//                foreach (var child in GameBoard.GetAllPossibleActions(player_one_minimax))
//                {
//                    double ev = MinimaxAlphaBetaPruning(child, depth - 1, alpha, beta, false, player_one_minimax);
//                    max_eval = (max_eval > ev) ? max_eval : ev;
//                    alpha = (alpha > ev) ? alpha : ev;
//                    if (beta <= alpha) break;
//                }
//                return max_eval;
//            }
//            else
//            {
//                double min_eval = Double.PositiveInfinity;
//                foreach (var child in GameBoard.GetAllPossibleActions(player_one_minimax))
//                {
//                    double ev = MinimaxAlphaBetaPruning(child, depth - 1, alpha, beta, false, player_one_minimax);
//                    min_eval = (min_eval > ev) ? ev : min_eval;
//                    beta = (alpha > ev) ? ev : alpha;
//                    if (beta <= alpha) break;
//                }
//                return min_eval;
//            }


//        }

//        private bool ChooseAction(MainWindow GameBoard, Dictionary<int, (bool player_move, int x, int y, bool horizontal)> d)
//        {
//            //if (d.Keys.Count == 0) return false; // обработчик исключения

//            var winner = d[d.Keys.Max()];
//            bool action = winner.player_move;

//            if (action) GameBoard.MoveChess(GameBoard.BlueChess, winner.x, winner.y);
//            else GameBoard.AddWall(winner.x, winner.y, winner.horizontal);
//            return action;

//        }
//        private void MinimaxAgent(MainWindow GameBoard, bool player_one_minimax)
//        {
//            Dictionary<double, (bool player_move, int x, int y, bool horizontal)> d = new Dictionary<double, List<(bool player_move, int x, int y, bool horizontal)>();
//            double value;
//            foreach (var child in GameBoard.GetAllChildStates(player_one_minimax))
//            {
//                //var BoardState = new GameState(new Chess((child.player_move && !player_one_minimax) ? child.x:GameBoard.BlueChess.posX,
//                //    (child.player_move && !player_one_minimax) ? child.x : GameBoard.BlueChess.posX, false), );
//                // child - список кортежей List<(bool player_move, int x, int y, bool horizontal)>
//                value = MinimaxAlphaBetaPruning(child, 3, Double.NegativeInfinity, Double.PositiveInfinity, false, player_one_minimax);
//                d.Add(value, child);
//            }

//            return ChooseAction(GameBoard, d);

//        }

//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;

namespace Koridor
{

    public class Bot
    {
        private const int MaxDepth = 3;
        private double MinimaxAlphaBetaPruning(
            GameState state,
            int depth,
            double alpha,
            double beta,
            bool maximizingPlayer,
            bool playerOneMaximizer)
        {
            if (depth == 0 || state.IsEndState())
            {
                //double e = EvaluateState(state, playerOneMaximizer);
                //MessageBox.Show($"Score:{e}, ChessPos:{(state.BlueChess.posX, state.BlueChess.posY)}, WallCount:{state.Walls.Count}");
                //return e;
                return EvaluateState(state, playerOneMaximizer);
            }

            var possibleActions = state.GetAllPossibleActions(playerOneMaximizer);

            if (maximizingPlayer)
            {
                double maxEval = double.NegativeInfinity;
                foreach (var action in possibleActions)
                {
                    var newState = ApplyAction(state, action);
                    double eval = MinimaxAlphaBetaPruning(
                        newState,
                        depth - 1,
                        alpha,
                        beta,
                        false,
                        playerOneMaximizer
                    );
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                double minEval = double.PositiveInfinity;
                foreach (var action in possibleActions)
                {
                    var newState = ApplyAction(state, action);
                    double eval = MinimaxAlphaBetaPruning(
                        newState,
                        depth - 1,
                        alpha,
                        beta,
                        true,
                        playerOneMaximizer
                    );
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        //С этой оценкой бот ходит, но не оптимально

        private double EvaluateState(GameState state, bool playerOneMaximizer)
        {
            int redDist = state.RedChess.posY;
            int blueDist = 8 - state.BlueChess.posY;

            // Базовые расстояния
            double score = playerOneMaximizer
                ? (blueDist - redDist)
                : (redDist - blueDist);

            // Учитываем количество оставшихся стен
            score += (state.BlueChess.WallsCount - state.RedChess.WallsCount) * 10;

            // Проверяем достижение цели
            if (state.BlueChess.posY == 8) score += 500;
            if (state.RedChess.posY == 0) score -= 500;

            // Учитываем кратчайший путь
            int playerPath = playerOneMaximizer
                ? state.GetShortestDistance((state.BlueChess.posX, state.BlueChess.posY), false)
                : state.GetShortestDistance((state.RedChess.posX, state.RedChess.posY), true);

            int opponentPath = playerOneMaximizer
                ? state.GetShortestDistance((state.RedChess.posX, state.RedChess.posY), true)
                : state.GetShortestDistance((state.BlueChess.posX, state.BlueChess.posY), false);

            //MessageBox.Show(playerPath.ToString());
            score += (opponentPath - 2 * playerPath) * 10;

            return score;
        }

        // Ходит только по горизонтали

        //private double cost_func()
        //{
        //    var actions = new List<double>();
        //    double curr_cost = 0;
        //    foreach(var state in )

        //}

        //private int Astar(GameState state, bool check_blockage)
        //{
        //    var queue = new PriorityQueue<double, double>();

        //    (int, int) pos = (state.CurrentPlayer) ?  (state.RedChess.posX, state.RedChess.posY): (state.BlueChess.posX, state.BlueChess.posY);
        //    (GameState, (int, int,))



        //    if (state.CurrentPlayer) pos = (state.RedChess.posX, state.RedChess.posY);
        //    else pos = (state.BlueChess.posX, state.BlueChess.posY);
        //    queue.Put();

        //    while (!queue.empty())
        //    {

        //    }

        //    return 0;
        //}


        public (bool isMove, int x, int y, bool horizontal) FindBestMove(MainWindow gameWindow)
        {
            (bool isMove, int x, int y, bool horizontal) bestAction;
            var currentState = new GameState(
                    gameWindow.BlueChess,
                    gameWindow.RedChess,
                    gameWindow.currentPlayer,
                    gameWindow.walls,
                    gameWindow.field
                );
            var possibleActions = currentState.GetAllPossibleActions(!gameWindow.currentPlayer);

            //gameWindow.Log(possibleActions.Count.ToString());
            if (possibleActions.Count == 0)
                return (true, gameWindow.BlueChess.posX, gameWindow.BlueChess.posY, false);

            bestAction = possibleActions[0];
            double bestScore = double.NegativeInfinity;
            double alpha = double.NegativeInfinity;
            double beta = double.PositiveInfinity;

            foreach (var action in possibleActions)
            {
                var newState = ApplyAction(currentState, action);
                double score = MinimaxAlphaBetaPruning(
                    newState,
                    MaxDepth - 1,
                    alpha,
                    beta,
                    false,
                    !gameWindow.currentPlayer
                );

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                    alpha = Math.Max(alpha, bestScore);
                }

                if (beta <= alpha)
                    break;
            }

            return bestAction;
        }

        //private int astar(GameState state, bool check_blockage)
        //{
        //    int ShortestDist;
        //    if (state.CurrentPlayer)
        //    {
        //        ShortestDist = state.GetShortestDistance((state.RedChess.posX, state.RedChess.posY), true);
        //    }
        //    else
        //    {
        //        ShortestDist = state.GetShortestDistance((state.BlueChess.posX, state.BlueChess.posY), false);

        //    }
        //    if (ShortestDist > 1000) return 0;
        //    else if (check_blockage) return 1;

        //    return ShortestDist;
        //}

        //private double EvaluateState(GameState state, bool playerOneMaximizer)
        //{
        //    Chess RedPlayer = state.RedChess;
        //    Chess BluePlayer = state.BlueChess;

        //    int red_player_dist = RedPlayer.posY; // player_one
        //    int blue_player_dist = 8 - BluePlayer.posY; // player_two

        //    double result = 0;

        //    if (playerOneMaximizer)
        //    {
        //        int opponent_path_len = blue_player_dist;
        //        int player_path_len = red_player_dist;
        //        if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
        //        {
        //            bool prev = state.CurrentPlayer;
        //            state.CurrentPlayer = true;
        //            player_path_len = astar(state, false);
        //            state.CurrentPlayer = prev;
        //        }
        //        result += opponent_path_len;
        //        result -= red_player_dist;
        //        int num = 100;
        //        if (player_path_len != 0)
        //        {
        //            num = player_path_len;
        //        }
        //        result += Math.Round((double)100 / num, 2);

        //        int num_1 = 50;
        //        if (blue_player_dist != 0)
        //        {
        //            num_1 = blue_player_dist;
        //        }
        //        result -= Math.Round((double)50 / num_1, 2);

        //        result += (RedPlayer.WallsCount - BluePlayer.WallsCount);
        //        if (RedPlayer.posY == 8) result += 100;
        //        if (player_path_len == 0 && RedPlayer.posY != 8) result -= 500;
        //    }
        //    else
        //    {
        //        int opponent_path_len = red_player_dist;
        //        int player_path_len = blue_player_dist;
        //        if (RedPlayer.WallsCount != 10 && BluePlayer.WallsCount != 10)
        //        {
        //            bool prev = state.CurrentPlayer;
        //            state.CurrentPlayer = false;
        //            player_path_len = astar(state, false);
        //            state.CurrentPlayer = prev;
        //        }
        //        //if (!is_expectimax) result += opponent_path_len;.
        //        result += 17 * opponent_path_len;

        //        result -= blue_player_dist;
        //        int num = 100;
        //        if (player_path_len != 0)
        //        {
        //            num = player_path_len;
        //        }
        //        result += Math.Round((double)100 / num, 2);

        //        int num_1 = 50;
        //        if (red_player_dist != 0)
        //        {
        //            num_1 = red_player_dist;
        //        }
        //        result -= Math.Round((double)50 / num_1, 2);

        //        result += (BluePlayer.WallsCount - RedPlayer.WallsCount);
        //        if (BluePlayer.posY == 0) result += 100;
        //        if (player_path_len == 0 && BluePlayer.posY != 0) result -= 500;
        //    }
        //    return result;
        //}

        private GameState ApplyAction(GameState state, (bool isMove, int x, int y, bool horizontal) action)
        {
            var newState = new GameState(
                state.BlueChess,
                state.RedChess,
                state.CurrentPlayer,
                state.Walls,
                state.Field
            );

            if (action.isMove)
            {
                if (newState.CurrentPlayer)
                {
                    newState.RedChess.posX = action.x;
                    newState.RedChess.posY = action.y;
                }
                else
                {
                    newState.BlueChess.posX = action.x;
                    newState.BlueChess.posY = action.y;
                }
            }
            else
            {
                newState.Walls.Add((action.x, action.y, action.horizontal));
                if (newState.CurrentPlayer)
                    newState.RedChess.WallsCount--;
                else
                    newState.BlueChess.WallsCount--;

                UpdateGraphWithWall(newState, action.x, action.y, action.horizontal);
            }

            newState.CurrentPlayer = !newState.CurrentPlayer;
            return newState;
        }

        private void UpdateGraphWithWall(GameState state, int x, int y, bool isHorizontal)
        {
            if (isHorizontal)
            {
                RemoveEdgeBetween(state.Field, (x, y), (x, y + 1));
                RemoveEdgeBetween(state.Field, (x + 1, y), (x + 1, y + 1));
            }
            else
            {
                RemoveEdgeBetween(state.Field, (x, y), (x + 1, y));
                RemoveEdgeBetween(state.Field, (x, y + 1), (x + 1, y + 1));
            }
        }

        private void RemoveEdgeBetween(
            AdjacencyGraph<(int x, int y), TaggedEdge<(int x, int y), int>> graph,
            (int x, int y) source, (int x, int y) target)
        {
            if (graph.TryGetEdge(source, target, out var edge))
                graph.RemoveEdge(edge);
            if (graph.TryGetEdge(target, source, out var reverseEdge))
                graph.RemoveEdge(reverseEdge);
        }
    }
}