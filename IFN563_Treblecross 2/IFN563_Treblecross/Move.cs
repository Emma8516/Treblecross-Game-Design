using System;
using System.Numerics;

namespace IFN563_Treblecross
{
    public class Move
    {
        public int StartPosition {get;private set;}
        
        public Player Player {get;private set;}
        public int Position {get; set;}

        public Move(int startPosition, Player player)
        {
            StartPosition = startPosition;
            
            Player = player;
        }

        public void Execute(OneDimensionalBoard board)
        {
            
            board.Tiles[StartPosition].OccupyTile(Player);
        }

        public void Revert(OneDimensionalBoard board)
        {
            
            board.Tiles[StartPosition].ClearTile();
        }

    }
}







