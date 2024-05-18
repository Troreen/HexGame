using System;
using System.Collections.Generic;

namespace HexGame
{
    public class GameBoard
    {
        private char[,] board;
        private int size;
        private Stack<(int row, int col, char prev)> moveHistory; // Stack to track moves for undoing

        public GameBoard(int size)
        {
            this.size = size;
            board = new char[size, size];
            moveHistory = new Stack<(int row, int col, char prev)>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = '.';
                }
            }
        }

        public void DisplayBoard(List<(int, int)> winningPath = null)
        {
            Console.Write(new string(' ', 3)); // Initial space for row numbers
            for (int i = 0; i < size; i++)
            {
                Console.Write(i + " "); // Print column numbers
            }
            Console.WriteLine();

            for (int i = 0; i < size; i++)
            {
                Console.Write(new string(' ', 2 + i)); // Offset for hexagonal appearance
                Console.Write(i + " "); // Print row number
                for (int j = 0; j < size; j++)
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
            if (row >= 0 && row < size && column >= 0 && column < size && !IsCellOccupied(row, column))
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
            winningPath = new List<(int, int)>();
            bool[,] visited = new bool[size, size];
            if (playerMarker == 'X')
            {
                for (int i = 0; i < size; i++)
                {
                    if (DFS(i, 0, playerMarker, visited, winningPath))
                    {
                        return true;
                    }
                }
            }
            else if (playerMarker == 'O')
            {
                for (int i = 0; i < size; i++)
                {
                    if (DFS(0, i, playerMarker, visited, winningPath))
                    {
                        return true;
                    }
                }
            }
            winningPath.Clear();
            return false;
        }

        private bool DFS(int row, int col, char playerMarker, bool[,] visited, List<(int, int)> path)
        {
            if (row < 0 || col < 0 || row >= size || col >= size || visited[row, col] || board[row, col] != playerMarker)
                return false;

            visited[row, col] = true;
            path.Add((row, col));
            if ((playerMarker == 'X' && col == size - 1) || (playerMarker == 'O' && row == size - 1))
                return true;

            bool result = DFS(row - 1, col, playerMarker, visited, path) ||
                          DFS(row + 1, col, playerMarker, visited, path) ||
                          DFS(row, col - 1, playerMarker, visited, path) ||
                          DFS(row, col + 1, playerMarker, visited, path) ||
                          DFS(row - 1, col + 1, playerMarker, visited, path) ||
                          DFS(row + 1, col - 1, playerMarker, visited, path);

            if (!result)
            {
                path.RemoveAt(path.Count - 1);
            }

            return result;
        }

        public int Size => size; // Expose the board size publicly

        // Indexer to allow board[row, col] access
        public char this[int row, int col]
        {
            get { return board[row, col]; }
            set { board[row, col] = value; }
        }
    }
}
