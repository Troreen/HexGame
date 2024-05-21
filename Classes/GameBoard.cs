using System;
using System.Collections.Generic;

namespace HexGame
{
    public class GameBoard
    {
        private char[,] board;
        private int rows;
        private int cols;
        private Stack<(int row, int col, char prev)> moveHistory; // Stack to track moves for undoing
        private static Random random = new Random();

        public GameBoard(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            board = new char[rows, cols];
            moveHistory = new Stack<(int row, int col, char prev)>();
            InitializeBoard();
            BlockRandomCell();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = '.';
                }
            }
        }

        private void BlockRandomCell()
        {
            int blockRow = random.Next(rows);
            int blockCol = random.Next(cols);
            board[blockRow, blockCol] = '#';  // '#' represents a blocked cell
        }

        public void DisplayBoard(List<(int, int)> winningPath = null)
        {
            Console.Write(new string(' ', 3)); // Initial space for row numbers
            for (int i = 0; i < cols; i++)
            {
                Console.Write(i + " "); // Print column numbers
            }
            Console.WriteLine();

            for (int i = 0; i < rows; i++)
            {
                Console.Write(new string(' ', 2 + i)); // Offset for hexagonal appearance
                Console.Write(i + " "); // Print row number
                for (int j = 0; j < cols; j++)
                {
                    bool isWinningCell = winningPath != null && winningPath.Contains((i, j));

                    if (board[i, j] == 'X')
                    {
                        Console.ForegroundColor = isWinningCell ? ConsoleColor.Yellow : ConsoleColor.Red;
                    }
                    else if (board[i, j] == 'O')
                    {
                        Console.ForegroundColor = isWinningCell ? ConsoleColor.Yellow : ConsoleColor.Blue;
                    }
                    else if (board[i, j] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.Write(board[i, j] + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public bool MakeMove(int row, int column, char marker)
        {
            if (row >= 0 && row < rows && column >= 0 && column < cols && !IsCellOccupied(row, column))
            {
                moveHistory.Push((row, column, board[row, column])); // Save the current state before changing
                board[row, column] = marker;
                return true;
            }
            return false;
        }

        public void UndoMove()
        {
            if (moveHistory.Count > 0)
            {
                var lastMove = moveHistory.Pop();
                board[lastMove.row, lastMove.col] = lastMove.prev; // Revert the move
            }
        }

        public bool IsCellOccupied(int row, int column)
        {
            return board[row, column] != '.';
        }

        public bool CheckWin(char playerMarker, out List<(int, int)> winningPath)
        {
            winningPath = BFS(playerMarker);
            return winningPath.Count > 0;
        }

        private List<(int, int)> BFS(char playerMarker)
        {
            var startCells = playerMarker == 'X'
                ? new List<(int, int)> { }
                : new List<(int, int)> { };

            if (playerMarker == 'X')
            {
                for (int c = 0; c < cols; c++)
                {
                    startCells.Add((0, c));
                }
            }
            else
            {
                for (int r = 0; r < rows; r++)
                {
                    startCells.Add((r, 0));
                }
            }

            int targetRow = playerMarker == 'X' ? rows - 1 : -1;
            int targetCol = playerMarker == 'O' ? cols - 1 : -1;

            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int, List<(int, int)>)>();

            foreach (var (r, c) in startCells)
            {
                if (board[r, c] == playerMarker)
                {
                    queue.Enqueue((r, c, new List<(int, int)> { (r, c) }));
                    visited.Add((r, c));
                }
            }

            while (queue.Count > 0)
            {
                var (r, c, path) = queue.Dequeue();

                if ((playerMarker == 'X' && r == targetRow) || (playerMarker == 'O' && c == targetCol))
                {
                    return path;  // Found a path to the other side
                }

                var directions = new List<(int, int)>
                {
                    (r - 1, c),
                    (r + 1, c),
                    (r, c - 1),
                    (r, c + 1),
                    (r - 1, c + 1),
                    (r + 1, c - 1)
                };

                foreach (var (nr, nc) in directions)
                {
                    if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && !visited.Contains((nr, nc)))
                    {
                        if (board[nr, nc] == playerMarker)
                        {
                            queue.Enqueue((nr, nc, new List<(int, int)>(path) { (nr, nc) }));
                            visited.Add((nr, nc));
                        }
                    }
                }
            }

            return new List<(int, int)>();  // No path found
        }

        public int Rows => rows; // Expose the number of rows publicly
        public int Cols => cols; // Expose the number of columns publicly

        // Indexer to allow board[row, col] access
        public char this[int row, int col]
        {
            get { return board[row, col]; }
            set { board[row, col] = value; }
        }
    }
}
