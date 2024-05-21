using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select game mode:");
            Console.WriteLine("1. Player vs MinMaxPlayer");
            Console.WriteLine("2. Test MinMaxPlayer blocking move");
            Console.WriteLine("3. MinMaxPlayer vs MinMaxPlayer");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    PlayHumanVsMinMax();
                    break;
                case 2:
                    TestMinMaxBlocking();
                    break;
                case 3:
                    PlayMinMaxVsMinMax();
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }

        private static void PlayHumanVsMinMax()
        {
            Console.Write("Enter number of rows: ");
            int rows = int.Parse(Console.ReadLine());
            Console.Write("Enter number of columns: ");
            int cols = int.Parse(Console.ReadLine());

            Console.WriteLine("Welcome to Hex Game!");
            GameBoard board = new GameBoard(rows, cols);
            board.DisplayBoard();

            Player player1 = new HumanPlayer('X');
            Player player2 = new MinMaxPlayer('O');
            bool playerOneTurn = true;

            while (true)
            {
                Player currentPlayer = playerOneTurn ? player1 : player2;
                bool moveMade = false;

                while (!moveMade)
                {
                    var (row, column) = currentPlayer.MakeMove(board);
                    if (board.MakeMove(row, column, currentPlayer.Marker))
                    {
                        moveMade = true;
                        board.DisplayBoard();

                        if (board.CheckWin(currentPlayer.Marker, out List<(int, int)> winningPath))
                        {
                            board.DisplayBoard(winningPath);
                            Console.WriteLine($"Player {currentPlayer.Marker} wins!");
                            return; // Exit the game loop and method
                        }

                        if (IsBoardFull(board))
                        {
                            Console.WriteLine("Draw!");
                            return; // Exit the game loop and method
                        }

                        playerOneTurn = !playerOneTurn; // Switch turn only if a valid move was made
                    }
                    else
                    {
                        Console.WriteLine("Invalid move, try again.");
                    }
                }
            }
        }

        private static void TestMinMaxBlocking()
        {
            Console.Write("Enter number of rows: ");
            int rows = int.Parse(Console.ReadLine());
            Console.Write("Enter number of columns: ");
            int cols = int.Parse(Console.ReadLine());

            Console.WriteLine($"Testing MinMaxPlayer blocking move on a {rows}x{cols} board...");

            GameBoard board = new GameBoard(rows, cols);
            for (int i = 0; i < Math.Min(rows, cols) - 1; i++)
            {
                board.MakeMove(i, i, 'X');
            }

            Player ai = new MinMaxPlayer('O');
            var (row, col) = ai.MakeMove(board);
            board.MakeMove(row, col, 'O');
            board.DisplayBoard();

            if (board[row, col] == 'O')
            {
                Console.WriteLine("Test passed: AI blocked the winning move.");
            }
            else
            {
                Console.WriteLine("Test failed: AI did not block the winning move.");
            }
        }

        private static void PlayMinMaxVsMinMax()
        {
            Console.Write("Enter number of rows: ");
            int rows = int.Parse(Console.ReadLine());
            Console.Write("Enter number of columns: ");
            int cols = int.Parse(Console.ReadLine());

            int numberOfGames = 100;
            int ai1Wins = 0;
            int ai2Wins = 0;
            int draws = 0;

            for (int i = 0; i < numberOfGames; i++)
            {
                GameBoard board = new GameBoard(rows, cols);
                Player ai1 = new MinMaxPlayer('X');
                Player ai2 = new MinMaxPlayer('O');
                bool playerOneTurn = true;

                bool gameEnded = false;
                while (!gameEnded)
                {
                    Player currentPlayer = playerOneTurn ? ai1 : ai2;
                    bool moveMade = false;

                    while (!moveMade)
                    {
                        var (row, column) = currentPlayer.MakeMove(board);
                        if (board.MakeMove(row, column, currentPlayer.Marker))
                        {
                            moveMade = true;

                            if (board.CheckWin(currentPlayer.Marker, out List<(int, int)> winningPath))
                            {
                                board.DisplayBoard(winningPath);
                                Console.WriteLine($"Game {i + 1}: Player {currentPlayer.Marker} wins!");
                                if (currentPlayer == ai1) ai1Wins++;
                                else ai2Wins++;
                                gameEnded = true;
                                break;
                            }

                            if (IsBoardFull(board))
                            {
                                board.DisplayBoard();
                                Console.WriteLine($"Game {i + 1}: Draw");
                                draws++;
                                gameEnded = true;
                                break;
                            }

                            playerOneTurn = !playerOneTurn; // Switch turn only if a valid move was made
                        }
                    }
                }
            }

            Console.WriteLine($"AI1 Wins: {ai1Wins}");
            Console.WriteLine($"AI2 Wins: {ai2Wins}");
            Console.WriteLine($"Draws: {draws}");
        }

        private static bool IsBoardFull(GameBoard board)
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (!board.IsCellOccupied(row, col))
                        return false;
                }
            }
            return true;
        }
    }
}
