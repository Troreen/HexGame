namespace HexGame
{
    public abstract class Player
    {
        public char Marker { get; }

        protected Player(char marker)
        {
            Marker = marker;
        }

        public abstract (int, int) MakeMove(GameBoard board);
    }
}
