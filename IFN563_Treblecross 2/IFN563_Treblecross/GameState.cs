
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace IFN563_Treblecross
{
    public class GameState
    {

        public Player player1 {get;set;}
        public Player player2 {get;set;}
        public Player CurrentPlayer {get;set;}  
        public string GamePhase {get;set;}      
        public Player WinningPlayer {get;set;}  
        public OneDimensionalBoard GameBoard {get;set;}  
        public bool IsGameOver {get;set;}
        private GameSaver gameSaver;
        public int BoardSize {get;set;}



        public GameState() { }

        public GameState(int boardSize, Player playerOne, Player playerTwo, string saveFilePath)
        {
            BoardSize = boardSize;
            GameBoard = new OneDimensionalBoard(boardSize);
            GamePhase = "initializing";
            WinningPlayer = null;
            player1 = playerOne;
            player2 = playerTwo;
            CurrentPlayer = player1;
            gameSaver = new GameSaver(saveFilePath);

        }


        public void RebindCurrentPlayer()
        {
           
            if (CurrentPlayer.PlayerID == player1.PlayerID)
            {
                CurrentPlayer = player1;
            }
            else if (CurrentPlayer.PlayerID == player2.PlayerID)
            {
                CurrentPlayer = player2;
            }
        }

        public Player FindPlayerById(string playerId)
        {
            
            if (player1 != null && player1.PlayerID == playerId)
            {
                return player1;
            }

            
            if (player2 != null && player2.PlayerID == playerId)
            {
                return player2;
            }

           
            return null;
        }

        public Player GetPlayerById(string playerId)
        {
            if (player1?.PlayerID == playerId) return player1;
            if (player2?.PlayerID == playerId) return player2;
            return null;
        }

            public void SwitchPlayer()
        {
          

            try
            {
                // empty check
                if (player1 == null || player2 == null)
                {
                    
                    return;
                }

                if (CurrentPlayer == player1)
                {
                    CurrentPlayer = player2;
                    
                }
                else
                {
                    CurrentPlayer = player1;
                   
                }
            }
            catch (Exception ex)
            {
                
            }
        }


        public void SetGamePhase(string newPhase)
        {
            GamePhase = newPhase;
        }

        public void SetWinningPlayer(Player player)
        {
            WinningPlayer = player;
        }

        public void InitializeOrResetGameBoard()
        {
            if (GameBoard == null || BoardSize != GameBoard.Tiles.Length)
            {
                GameBoard = new OneDimensionalBoard(BoardSize);
            }
        }

        public void RestoreTileStates()
        {
            
            if (GameBoard?.Tiles == null)
            {
               
                return; 
            }

            foreach (var tile in GameBoard.Tiles)
            {
                if (tile == null)
                {
                    
                    continue; 
                }

                

                if (tile.Occupied)
                {
                    if (tile.OccupiedBy == null)
                    {
                        
                        continue; 
                    }
                    tile.OccupyTile(tile.OccupiedBy);
                }
                else
                {
                    tile.ClearTile();
                }
            }
        }



    }




    public class GameSaver
    {
        private string saveFilePath;

        public GameSaver(string path)
        {
            saveFilePath = path;
        }

        public void SaveGame(GameState gameState)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new PlayerJsonConverter() },
                PropertyNameCaseInsensitive = true
            };

            
            string gameStateJson = JsonSerializer.Serialize(gameState, options);

            
            File.WriteAllText(saveFilePath, gameStateJson);

            Console.WriteLine("Game has been saved.");
        }

        public GameState LoadGame()
        {
            string gameStateJson;
            try
            {
                gameStateJson = File.ReadAllText(saveFilePath);
            }
            catch
            {
                
                return null;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new PlayerJsonConverter() },
                PropertyNameCaseInsensitive = true
            };

            GameState gameState;
            try
            {
                gameState = JsonSerializer.Deserialize<GameState>(gameStateJson, options);
            }
            catch
            {
                
                return null;
            }

            if (gameState == null || gameState.GameBoard == null || gameState.GameBoard.Tiles == null)
            {
                
                return null;
            }

            gameState.RebindCurrentPlayer();

            try
            {
                gameState.RestoreTileStates();
            }
            catch
            {
                
            }

            return gameState;
        }

    }
}






