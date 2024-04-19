using System;
using System.Numerics;
using System.Text.Json;

namespace IFN563_Treblecross
{
    public class GameController
    {

        public GameState GameState { get; set; }
        public MoveHistory MoveHistory { get; private set; } = new MoveHistory();

        private GameSaver gameSaver;
        private string saveFilePath;
        private Player player1;
        private Player player2;
        private TreblecrossRule rule = new TreblecrossRule();


        public GameController(string savePath)
        {
            this.saveFilePath = savePath;
            this.gameSaver = new GameSaver(this.saveFilePath);
            this.MoveHistory = new MoveHistory();

        }


        public void DisplayMainMenu()
        {
            while (true)
            {
                Console.WriteLine("Welcome to Treblecross! Place three 'X' or 'O' in a row to win.");
                Console.WriteLine("Please input letter choose an option:");
                Console.WriteLine("S: Start a new game");
                Console.WriteLine("C: Continue from the last game");
                Console.WriteLine("H: Help");
                Console.WriteLine("Q: Quit");



                string input = Console.ReadLine().ToUpper();
                switch (input)
                {
                    case "S":
                        StartGame();
                        break;
                    case "C":
                        ContinueGame();

                        break;
                    case "H":
                        DisplayHelp();
                        break;
                    case "Q":
                        Console.WriteLine("Exiting the game. Thank you for playing!");
                        return;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }

        public void StartGame()
        {
            
            int boardSize = GetBoardSize();
            player1 = new HumanPlayer("Player1");
            player2 = ChoosePlayer();
            GameState = new GameState(boardSize, player1, player2, saveFilePath);

            
            GameState.GameBoard.InitializeBoard(boardSize);
            GameState.GameBoard.DisplayBoard();
            Console.WriteLine("Type 'H' for help, 'S' to save, 'U' to undo, 'R' to redo,'Q' to quit.");

           
            GameState.SetGamePhase("playing");
            ManagePlayerTurns();
        }


        private Player ChoosePlayer()
        {
            while (true)
            {
                Console.WriteLine("Choose your opponent:\n1 = Human VS Human \n2 = Human VS Computer");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": return new HumanPlayer("Player2");
                    case "2": return new ComputerPlayer("Player2");
                    default:
                        Console.WriteLine("Invalid choice. Please choose again.");
                        break;
                }
            }
        }

       

        private int GetBoardSize()
        {
            int size;
            Console.WriteLine("Please enter the size of the board (5 or more):");
            while (!int.TryParse(Console.ReadLine(), out size) || size < 5)
            {
                Console.WriteLine("Invalid input. Please enter a number greater than or equal to 5:");
            }
            return size;
        }

        public void ManagePlayerTurns()
        {
            
            while (GameState.GamePhase == "playing")
            {
             
                if (GameState.CurrentPlayer is HumanPlayer)
                {

                    Console.WriteLine($"{GameState.CurrentPlayer.PlayerID}'s turn.");
                    Console.WriteLine("Enter your move, U to undo, R to redo, S to save or Q to quit:");
                    string input = Console.ReadLine().ToUpper();

                    switch (input)
                    {
                        case "Q":
                            GameState.SetGamePhase("ended");
                            Console.WriteLine("Game ended.");
                            return;
                        case "H":
                            DisplayHelp();
                            break;
                        case "S":
                            gameSaver.SaveGame(GameState);
                            Console.WriteLine("Game saved.");
                            break;
                        case "U":
                            UndoMove();

                            break;
                        case "R":
                            RedoMove();
                            break;

                        

                        default:
                            if (int.TryParse(input, out int position))
                            {
                                ProcessMove(position - 1); 
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a number or command.");
                                continue;
                            }
                            break;
                    }
                }
                
                else if (GameState.CurrentPlayer is ComputerPlayer)
                {
                    int move = GameState.CurrentPlayer.MakeMove(GameState.GameBoard);
                    ProcessMove(move); 
                }
            }
        }




        private void ProcessMove(int position)
        {
            int boardIndex = position;

            if (boardIndex < 0 || boardIndex >= GameState.GameBoard.Tiles.Length)
            {
                Console.WriteLine($"Invalid move. Please enter a number between 1 and {GameState.GameBoard.Tiles.Length}.");
                return;
            }
            //Console.WriteLine("Processing move at position: " + position);

            if (!GameState.GameBoard.Tiles[position].Occupied)
            {
                Move move = new Move(position, GameState.CurrentPlayer);
                move.Execute(GameState.GameBoard);
                MoveHistory.AddMove(move);

                GameState.GameBoard.DisplayBoard();

                if (rule.CheckWinCondition(GameState.GameBoard))
                {
                    EndGame();
                }
                else
                {
                    // check if the board full
                    bool isBoardFull = true;
                    foreach (var tile in GameState.GameBoard.Tiles)
                    {
                        if (!tile.Occupied)
                        {
                            isBoardFull = false;
                            break;
                        }
                    }

                    if (isBoardFull)
                    {
                        
                        
                        GameState.SetGamePhase("ended");
                        Console.WriteLine("It's a draw!");
                    }
                    else
                    {
                        
                        Console.WriteLine("Switching player.");
                        GameState.SwitchPlayer();
                    }
                }
            }
            else
            {
                Console.WriteLine($"The position is already occupied. Try again.");
            }
        }



        private void EndGame()
        {
            //Console.WriteLine("EndGame method called.");
            GameState.SetGamePhase("Ended");
            if (rule.CheckWinCondition(GameState.GameBoard))
            {
                GameState.SetWinningPlayer(GameState.CurrentPlayer);
                Console.WriteLine($"{GameState.CurrentPlayer.PlayerID} wins!");
            }
            else
            {
                
                    Console.WriteLine("Ended.");
                
            }
        }

        
        public void UpdateMove(Move move)
        {
            if (!GameState.GameBoard.Tiles[move.StartPosition].Occupied)
            {
                GameState.GameBoard.Tiles[move.StartPosition].OccupyTile(move.Player); 
                MoveHistory.AddMove(move);
            }
            else
            {
                Console.WriteLine("That space is already occupied. Please try another move.");
            }
        }


        
        public void UndoMove()
        {
            Move lastMove = MoveHistory.Undo();
            if (lastMove != null)
            {
                GameState.GameBoard.Tiles[lastMove.StartPosition].ClearTile();
                GameState.SwitchPlayer(); // Switch back to the opponent
            }
            else
            {
                Console.WriteLine("No moves to undo.");
                return;
            }

            if (GameState.CurrentPlayer is ComputerPlayer)
            {
                lastMove = MoveHistory.Undo();
                if (lastMove != null)
                {
                    GameState.GameBoard.Tiles[lastMove.StartPosition].ClearTile();
                    GameState.SwitchPlayer(); // Switch back to the human player
                }
                else
                {
                    Console.WriteLine("No additional moves to undo.");
                }
            }

            GameState.GameBoard.DisplayBoard(); 
        }

    

        public void RedoMove()
        {
            // Check if there is a move to redo
            Move nextMove = MoveHistory.Redo();
            if (nextMove != null)
            {
                // If there is, redo the move on the board
                GameState.GameBoard.Tiles[nextMove.StartPosition].OccupyTile(nextMove.Player);
                GameState.SwitchPlayer(); 
              
                GameState.GameBoard.DisplayBoard(); 
            }
            else
            {
                
                Console.WriteLine("No moves to redo.");
            }
        }



        public void DisplayHelp()
        {
            Console.WriteLine("___________________________________________________________________");
            Console.WriteLine("Game Rules:\n- The objective is to align three 'X's or 'O's horizontally.\n- Two players take turns placing their marker ('X' or 'O') on any unoccupied position on the board.\n- The first player to get three of their markers in a row wins the game. If the board is filled and no player has won, the game ends in a draw.\n");
            Console.WriteLine("How to Play:\n- The board consists of numbered positions, starting from 1 up to the total number of positions on the board.\n- When prompted during your turn, enter the number corresponding to the position where you want to place your marker.\n- For example, if you want to place an 'X' in the first position on the board, you would enter '1' when prompted.");
            Console.WriteLine("Undo and Redo Moves:\n- If you wish to undo your last action, enter 'U' during your turn. This will revert the board to the state before your last move.\n- To redo an action you have just undone, enter 'R'. This will reapply the move you just took back.\n");
            Console.WriteLine("Save the Game:\n- Enter 'S' at any time to save the current game state. This allows you to continue the game at a later time.\n");
            Console.WriteLine("Quit the Game:\n- To exit the game, simply enter 'Q'. Remember to save your game before quitting if you wish to continue later.\n");
            Console.WriteLine("Help:\n- If you need a refresher on these instructions during your turn, enter 'H' for help.");
            Console.WriteLine("Examples:\n- When prompted for your input, if you wish to place your marker in the third position, you should input '3'.\n- If you accidentally place a marker in the third position and wish to retract it, simply input 'U' to undo the last move.\n- If you've undone a move and wish to redo it, input 'R' to reapply that move.\n- Need to pause the game midway? Just input 'S', and the system will save your current progress.\n- When you want to revisit these instructions, remember to input 'H' for help.");
            Console.WriteLine("___________________________________________________________________");
        }

        public void ContinueGame()
        {
            try
            {
                // try to load game
                this.GameState = gameSaver.LoadGame();
                if (this.GameState == null)
                {

                    return;
                }

                // show board condition
                if (GameState.GameBoard == null)
                {
                }
                else
                {
                    GameState.GameBoard.DisplayBoard();
                }

                // check if the game end
                if (GameState.GamePhase.Equals("ended", StringComparison.OrdinalIgnoreCase))
                {
                }
                else
                {
                    // check if computerplayer
                    if (GameState.CurrentPlayer is ComputerPlayer)
                    {

                        int move = GameState.CurrentPlayer.MakeMove(GameState.GameBoard);
                        ProcessMove(move);
                    }
                    else
                    {
                        ManagePlayerTurns();
                    }
                }


            }
            catch (Exception ex)
            {
            }
        }
    }
}




