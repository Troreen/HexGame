using System;
using System.Diagnostics;
using System.Linq;

namespace HexGame
{
    public class MinMaxPlayer : Player
    {
        public static int MaxDepth = 3; // You can adjust this as needed
        private Random random = new Random();

        public MinMaxPlayer(char marker) : base(marker) { }

        public override (int, int) MakeMove(GameBoard board)
        {
            Stopwatch stopwatch = Stopwatch.StartNew(); // Start tracking time

            (int bestRow, int bestCol) = (-1, -1);
            int bestValue = int.MinValue;

            var startTime = DateTime.Now;
            var timeLimit = TimeSpan.FromSeconds(2); // Optional time limit

            for (int depth = 1; depth <= MaxDepth; depth++)
            {
                (int row, int col, int value) = IterativeDeepeningMinimax(board, depth, startTime, timeLimit);
                if (value > bestValue)
                {
                    bestValue = value;
                    bestRow = row;
                    bestCol = col;
                }

                // Check if time limit exceeded
                if (DateTime.Now - startTime > timeLimit)
                {
                    break;
                }
            }

            stopwatch.Stop(); // Stop tracking time

            // Return the best move
            return (bestRow, bestCol);
        }

        private (int, int, int) IterativeDeepeningMinimax(GameBoard board, int depth, DateTime startTime, TimeSpan timeLimit)
        {
            (int bestRow, int bestCol) = (-1, -1);
            int bestMoveValue = int.MinValue;

            for (int row = 0; row < board.Size; row++)
            {
                for (int col = 0; col < board.Size; col++)
                {
                    if (!board.IsCellOccupied(row, col))
                    {
                        board.MakeMove(row, col, Marker);
                        int moveValue = Minimax(board, 0, false, int.MinValue, int.MaxValue, depth);
                        board.UndoMove();

                        if (moveValue > bestMoveValue)
                        {
                            bestMoveValue = moveValue;
                            bestRow = row;
                            bestCol = col;
                        }

                        // Check if time limit exceeded
                        if (DateTime.Now - startTime > timeLimit)
                        {
                            return (bestRow, bestCol, bestMoveValue);
                        }
                    }
                }
            }

            return (bestRow, bestCol, bestMoveValue);
        }

        private int Minimax(GameBoard board, int depth, bool isMaximizing, int alpha, int beta, int maxDepth)
        {
            char opponentMarker = Marker == 'X' ? 'O' : 'X';

            if (board.CheckWin(Marker, out _) || board.CheckWin(opponentMarker, out _) || depth >= maxDepth)
                return EvaluateBoard(board, Marker);

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                var moves = GenerateMoves(board);

                foreach (var (row, col) in moves)
                {
                    board.MakeMove(row, col, Marker);
                    int eval = Minimax(board, depth + 1, false, alpha, beta, maxDepth);
                    board.UndoMove();
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                var moves = GenerateMoves(board);

                foreach (var (row, col) in moves)
                {
                    board.MakeMove(row, col, opponentMarker);
                    int eval = Minimax(board, depth + 1, true, alpha, beta, maxDepth);
                    board.UndoMove();
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(GameBoard board, char marker)
        {
            char opponentMarker = marker == 'X' ? 'O' : 'X';

            if (board.CheckWin(marker, out _))
                return 10;

            if (board.CheckWin(opponentMarker, out _))
                return -10;

            int score = 0;
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (board[row, col] == marker)
                    {
                        score += CountConnections(board, row, col, marker);
                        if (IsCenter(row, col, board.Size))
                            score += 2;
                    }
                    else if (board[row, col] == opponentMarker)
                    {
                        score -= CountConnections(board, row, col, opponentMarker);
                        if (IsCenter(row, col, board.Size))
                            score -= 2;
                    }
                }
            }

            return score;
        }

        private int CountConnections(GameBoard board, int row, int col, char marker)
        {
            int connections = 0;
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, 1 }, { 1, -1 } };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];
                int newRow = row + dr;
                int newCol = col + dc;
                if (newRow >= 0 && newRow < board.Rows && newCol >= 0 && newCol < board.Cols && board[newRow, newCol] == marker)
                {
                    connections++;
                }
            }

            return connections;
        }

        private bool IsCenter(int row, int col, int size)
        {
            int center = size / 2;
            return (row == center || row == center - 1 || row == center + 1) &&
                   (col == center || col == center - 1 || col == center + 1);
        }

        private (int, int)[] GenerateMoves(GameBoard board)
        {
            var moves = from row in Enumerable.Range(0, board.Rows)
                        from col in Enumerable.Range(0, board.Cols)
                        where !board.IsCellOccupied(row, col)
                        select (row, col);

            return moves.ToArray();
        }
    }
}
