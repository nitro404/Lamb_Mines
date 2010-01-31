using System;
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ScallyWagsApp game              = null;
            GameModule gameModule           = new GameModule();
			WinScreenModule winScreenModule = new WinScreenModule();
			LooseScreenModule loseModule = new LooseScreenModule();

            GamerServicesComponent services = new GamerServicesComponent( null );
           
            if( Settings.START_GAME_MODULE )
            {
                try
                {
                    game = new ScallyWagsApp( gameModule );
                    //game.AddModule(new MenuModule());
					game.AddModule(winScreenModule);

                    game.Run();
                }
                catch( Exception ex )
                {
                    Error.Trace( "Error: " + ex.Message );
                    Error.Trace( "Stack Trace: \n\n" + ex.StackTrace );
                }
            }
            else
            {
                game = new ScallyWagsApp( new SplashModule() );
                game.AddModule(new MenuModule());
                game.AddModule( gameModule);
				game.AddModule(winScreenModule);
				game.AddModule(loseModule);

                //game.AddModule(new CreditsModule());
                game.Run();
            }
        }
    }
}

