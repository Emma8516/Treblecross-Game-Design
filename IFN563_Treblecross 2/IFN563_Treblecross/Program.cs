using System;
using IFN563_Treblecross;
namespace IFN563_Treblecross
{


    class Program
    {

            static void Main(string[] args)
            {
            
            string saveFilePath = "savefile.json"; 
         
            GameController gameController = new GameController(saveFilePath);
            gameController.DisplayMainMenu();
        }
        
    }


}
