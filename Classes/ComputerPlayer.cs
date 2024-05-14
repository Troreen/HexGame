using System;

namespace HexGame
{
    public class ComputerPlayer : Player
    {
        public ComputerPlayer(char marker) : base(marker) { }

        public override (int, int) MakeMove(GameBoard board)
        {
            // Simple AI: Randomly choose an empty spot
            Random rand = new Random();
            int row, column;
            do
            {
                row = rand.Next(board.Size);
                column = rand.Next(board.Size);
            }
            while (board.IsCellOccupied(row, column));
            Console.WriteLine($"Computer {Marker} moves to {row} {column}");
            return (row, column);
        }
    }
}
