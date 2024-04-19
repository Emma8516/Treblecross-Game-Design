using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IFN563_Treblecross
{
    public class MoveHistory
    {
        private List<Move> moves = new List<Move>();
        private int currentIndex = -1;

        public void AddMove(Move move) 
        {
            
            if (currentIndex < moves.Count - 1)
            {
                moves = moves.Take(currentIndex + 1).ToList();
            }

            moves.Add(move);
            currentIndex++;
        }

        public Move Undo()
        {
            if (currentIndex >= 0)
            {
                Move move = moves[currentIndex];
                currentIndex--;
                return move;
            }
            return null;
        }

        public Move Redo()
        {
            if (currentIndex < moves.Count - 1)
            {
                currentIndex++;
                return moves[currentIndex];
            }
            return null;
        }
    }


}

