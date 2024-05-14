namespace HexGame
{
    public abstract class Player
    {
        public char Marker { get; private set; }
    

        protected Player(char marker)
        {
            Marker = marker;
        }

        public abstract (int, int) MakeMove(GameBoard board);
    }

}