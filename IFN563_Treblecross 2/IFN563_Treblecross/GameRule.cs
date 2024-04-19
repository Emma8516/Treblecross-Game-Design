using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using IFN563_Treblecross;

namespace IFN563_Treblecross
{
    public abstract class GameRule
    {
       
        public abstract bool IsMoveLegal(Move move, OneDimensionalBoard gameBoard);

        
        public abstract bool IsGameOver(OneDimensionalBoard gameState);

      
        public abstract bool CheckWinCondition(OneDimensionalBoard gameState);
    }

    public class TreblecrossRule : GameRule
    {
        public override bool IsMoveLegal(Move move, OneDimensionalBoard gameBoard)
        {
           
            return move.StartPosition >= 0 && move.StartPosition < gameBoard.Tiles.Length &&
                   !gameBoard.Tiles[move.StartPosition].Occupied;
        }

        public override bool IsGameOver(OneDimensionalBoard gameBoard)
        {
            
            return CheckWinCondition(gameBoard);
        }


        public override bool CheckWinCondition(OneDimensionalBoard gameBoard)
        {
            
            for (int i = 0; i < gameBoard.Tiles.Length - 2; i++)
            {
                if (gameBoard.Tiles[i].Occupied &&
                    gameBoard.Tiles[i].OccupiedBy == gameBoard.Tiles[i + 1].OccupiedBy &&
                    gameBoard.Tiles[i].OccupiedBy == gameBoard.Tiles[i + 2].OccupiedBy)
                {
                    
                    return true;
                }
            }
           
            return false;
        }

    }


}


public class MoveValidator
{
    private GameRule gameRule;

    public MoveValidator(GameRule rule)
    {
        gameRule = rule;
    }

    public bool ValidateMove(Move move, GameState gameState)
    {
        // Check if the move is legal according to the game rules and the game is not over.
        return gameRule.IsMoveLegal(move, gameState.GameBoard) && !gameState.IsGameOver;
    }

}
