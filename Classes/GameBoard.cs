using System;
using System.Collections.Generic;

namespace HexGame
{
    public class GameBoard
    {
        private char[,] board;
        private int size;

        public GameBoard(int size)
        {
            this.size = size;
            board = new char[size, size];
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

        public void DisplayBoard()
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write(new string(' ', i)); // Offset for hexagonal appearance
                for (int j = 0; j < size; j++)
                {
                    Console.Write(board[i, j] + " "); // Corrected to display the board characters
                }
                Console.WriteLine();
            }
        }

        public bool IsCellOccupied(int row, int column)
        {
            return board[row, column] != '.';
        }

        public bool MakeMove(int row, int column, char marker)
        {
            if (row >= 0 && row < size && column >= 0 && column < size && !IsCellOccupied(row, column))
            {
                board[row, column] = marker;
                return true;
            }
            return false;
        }

        public bool CheckWin(char playerMarker)
        {
            bool[,] visited = new bool[size, size];

            if (playerMarker == 'X') // 'X' tries to connect East to West
            {
                for (int i = 0; i < size; i++)
                {
                    if (DFS(i, 0, playerMarker, visited))
                    {
                        return true;
                    }
                }
            }
            else if (playerMarker == 'O') // 'O' tries to connect North to South
            {
                for (int i = 0; i < size; i++)
                {
                    if (DFS(0, i, playerMarker, visited))
                    {
                        return true;
                    }
                }
            }

            return false; // Return false if no winning path is found for either player
        }

        private bool DFS(int row, int col, char playerMarker, bool[,] visited)
        {
            if (row < 0 || col < 0 || row >= size || col >= size || visited[row, col] || board[row, col] != playerMarker)
                return false;

            visited[row, col] = true;

            // Check if the current position has reached the opposite side
            if ((playerMarker == 'X' && col == size - 1) || (playerMarker == 'O' && row == size - 1))
                return true;

            // Explore all adjacent cells
            return DFS(row - 1, col, playerMarker, visited) ||
                   DFS(row + 1, col, playerMarker, visited) ||
                   DFS(row, col - 1, playerMarker, visited) ||
                   DFS(row, col + 1, playerMarker, visited) ||
                   DFS(row - 1, col + 1, playerMarker, visited) ||
                   DFS(row + 1, col - 1, playerMarker, visited);
        }

        public int Size => size; // Expose the board size publicly
    }
}
