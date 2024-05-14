using System;

namespace HexGame
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(char marker) : base(marker) { }

        public override (int, int) MakeMove(GameBoard board)
        {
            // Input validation omitted for brevity
            Console.WriteLine($"Player {Marker}, enter your move (row column):");
            string[] inputs = Console.ReadLine().Split(' ');
            int row = int.Parse(inputs[0]);
            int column = int.Parse(inputs[1]);
            return (row, column);
        }
    }
}
