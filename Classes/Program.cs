using System;

namespace HexGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hex Game!");
            GameBoard board = new GameBoard(5);
            board.DisplayBoard();

            Player player1 = new HumanPlayer('X');
            Player player2 = new HumanPlayer('O');
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

                        if (board.CheckWin(currentPlayer.Marker))
                        {
                            Console.WriteLine($"Player {currentPlayer.Marker} wins!");
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
    }
}
