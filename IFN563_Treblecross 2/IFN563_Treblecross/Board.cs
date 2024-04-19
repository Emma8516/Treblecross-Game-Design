using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace IFN563_Treblecross
{
    public abstract class Board
    {
        public abstract void ResetBoard();
        public abstract void InitializeBoard(int size);
    }

    public class OneDimensionalBoard : Board
    {
        public Tile[] Tiles {get; set;}
        public int BoardSize {get; set;}

        [JsonConstructor]


        public OneDimensionalBoard(int boardSize)
        {
            InitializeBoard(boardSize);
        }

        public override void InitializeBoard(int size)
        {
            BoardSize = size;
            Tiles = new Tile[size];
            for (int i = 0; i < size; i++)
            {
                Tiles[i] = new Tile(i, false, null);
            }
        }

       

        public void RestoreTileStates(JsonElement tilesJson, GameState gameState)
        {
            
            for (int i = 0; i < Tiles.Length; i++)
            {
                var tileJson = tilesJson[i];
                bool occupied = tileJson.GetProperty("occupied").GetBoolean();
                if (occupied)
                {
                    JsonElement occupiedBy = tileJson.GetProperty("occupiedBy");
                    string playerId = occupiedBy.GetProperty("playerId").GetString();
                    // use gameState to find the player
                    Player player = gameState.FindPlayerById(playerId);
                    if (player != null)
                    {
                        Tiles[i].OccupyTile(player);
                    }
                }
                else
                {
                    Tiles[i].ClearTile();
                }
            }
        }




        public void DisplayBoard()
        {
            Console.Clear();
            
            if (Tiles == null || Tiles.Length != BoardSize)
            {
               
                return;
            }

            Console.WriteLine(new string('-', BoardSize * 4 + 1));
            for (int i = 0; i < Tiles.Length; i++)
            {
                Player x = Tiles[i].OccupiedBy;
                string symbol = "";
                if (x == null)
                {
                    symbol = Tiles[i].Occupied ? "X" : "";
                }
                else {

                    if (x.PlayerID == "Player1")
                    {
                        symbol = Tiles[i].Occupied ? "X" : "";
                    }
                    else
                    {

                        symbol = Tiles[i].Occupied ? "O" : " ";

                    }  
                }
           


                Console.Write($"| {symbol} ");

                if ((i + 1) % BoardSize == 0)
                {
                    Console.WriteLine("|");
                    Console.WriteLine(new string('-', BoardSize * 4 + 1));
                }
            }
        }
    
    



        public void UpdateTilePosition(int position, Player player)
        {
            if (position >= 0 && position < Tiles.Length && !Tiles[position].Occupied)
            {
                Tiles[position].OccupyTile(player);
            }
        }

        public override void ResetBoard()
        {
            foreach (var tile in Tiles)
            {
                tile.ClearTile();
            }
        }




    }

}






