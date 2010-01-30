using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Scallywags
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
            GamerServicesComponent services = new GamerServicesComponent( null );
           
            if( Settings.START_GAME_MODULE )
            {
                try
                {
                    game = new ScallyWagsApp( gameModule );
                    game.AddModule(new MenuModule());    
                    game.AddModule(new CreditsModule());
                    game.AddModule(new SplashModule() );
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
                game.AddModule(new CreditsModule());
                game.Run();
            }
        }
    }
}
