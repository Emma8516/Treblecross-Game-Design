using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace IFN563_Treblecross
{
    public class Tile
    {
        [JsonPropertyName("position")]
        public int Position {get; set;}

        [JsonPropertyName("occupied")]
        public bool Occupied {get; set;}

        
        [JsonPropertyName("occupiedBy")]
        public Player OccupiedBy {get; set;}

        public Tile(int position, bool occupied, Player occupiedBy)
        {
            Position = position;
            Occupied = occupied;
            OccupiedBy = occupiedBy;
        }

        public void OccupyTile(Player player)
        {
            Occupied = true;
            OccupiedBy = player;
        }

        public void ClearTile()
        {
            Occupied = false;
            OccupiedBy = null;
        }
    }

}

