using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace IFN563_Treblecross
{
    public abstract class Player
    {
        [JsonPropertyName("playerId")]
        public string PlayerID {get; set;}

        [JsonPropertyName("type")]
        public string Type {get; set;}

        protected Player() { }

        protected Player(string playerId)
        {
            PlayerID = playerId;
        }
        public void Initialize(string playerId)
        {
            PlayerID = playerId;
        }


        public abstract int MakeMove(OneDimensionalBoard board);
    }

    public class HumanPlayer:Player
    {
        public HumanPlayer() { }

        [JsonConstructor]
        public HumanPlayer(string playerId) : base(playerId)
        {
            Type = "Human";
        }

        




        public override int MakeMove(OneDimensionalBoard board)
        {
            
                while (true)
                {
                    Console.WriteLine($"{PlayerID}, enter your move (1-{board.Tiles.Length}): ");
                    if (int.TryParse(Console.ReadLine(), out int move))
                    {
                        move -= 1; 
                        if (move >= 0 && move < board.Tiles.Length && !board.Tiles[move].Occupied)
                        {
                            return move;
                        }
                        else
                        {
                            Console.WriteLine("Invalid move. Try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Try again.");
                    }
                }
            
        }
    }


    public class ComputerPlayer:Player
    {
        private static readonly Random random = new Random();

        public ComputerPlayer() { }
        [JsonConstructor]
        public ComputerPlayer(string playerId) : base(playerId)
        {
            Type = "Computer";
        }

        

        
        public override int MakeMove(OneDimensionalBoard board)
        {
            int move;
            do
            {
                move = random.Next(board.Tiles.Length);
            } while (board.Tiles[move].Occupied);

            return move;
        }
    }

    public class PlayerJsonConverter:JsonConverter<Player>
    {
        public override Player Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("type", out JsonElement typeElement) ||
            !root.TryGetProperty("playerId", out JsonElement playerIdElement))
            {
                return null;
            }

            
            return typeElement.GetString() switch
            {
                "Human" => new HumanPlayer(playerIdElement.GetString()),
                "Computer" => new ComputerPlayer(playerIdElement.GetString()),
                _ => null
            };
        }



        public override void Write(Utf8JsonWriter writer, Player value, JsonSerializerOptions options)
        {
            
            writer.WriteStartObject();
            writer.WriteString("playerId", value.PlayerID);
            writer.WriteString("type", value.Type);
            writer.WriteEndObject();
        }
    }


}
