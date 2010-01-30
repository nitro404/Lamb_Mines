



using System;

namespace Scallywags
{
    public static class Settings
    {
#if XBOX
        public static bool START_FULL_SCREEN = true;                        ///< Should the app start in full screen?
#else
        public static bool START_FULL_SCREEN = false;                       ///< Should the app start in full screen?
#endif
        public static bool SHOW_FPS          = false;
        public static bool START_GAME_MODULE = false;                        ///< Should the app start with the game module?
        public static bool DRAW_SAFE_REGION  = false;                       ///< Do you want the hud to draw with the safe region highlighted?
        public static bool USE_COLLISION     = true;                        ///< Turn the collision on or off
        public static bool WOBBLE            = true;                        ///< Toggles the Wobble effect of hte firing Targetter
        public static bool FAST_MODE         = false;                       ///< A Framerate fix for working on slow computers
        public static bool DISABLE_AI        = false;                       ///< Disables AI
        public static bool ALL_AI            = false;                       ///< Forces all the players to be controlled by the AI
        public static bool DEBUG_PLAYER_COUNT = true;                       ///< Allows us to atomatically set the Number of Players
        public static int  DEBUG_NUM_PLAYERS = 4;                           ///< The Number of Players to debug to
        public static bool SHOW_INTRO        = true;                        ///< Turns off Camera Intro

        ////////////////////////////////////////////////
        //Gameplay settings
        public const float MOVE_POINTS              = 75.0f;                ///< The number of move points players will get at the start of their turns
        public const float SHIP_MOVE_SPEED          = 35.0f;                ///< The forward move speed of the ships in world units per second
        public const float SHIP_MOVE_COST           = 0.4f;                 ///< The cost, in move points, to move the ship ahead one step
        public const float IDLE_COST                = 0.05f;                ///< The cost, in move points, to move the ship ahead one step
        public const float SHIP_TURN_SPEED          = (float)Math.PI / 2;   ///< The speed the ship will turn at in Radians per second (roughly 60 degrees per second  at PI/3 )                               
        public const float SHIP_FIRE_TIMER          = 5.0f;                 ///< Time it will take to fire the cannons
        public const float TARGET_RADIUS            = 10.0f;                ///< The Target's radius for checking
        public const float BOAT_BONUS               = 25.0f;
        public const float TOWER_COIN_HEIGHT        = 25.0f;

        public const float WIND_MODIFIER            = 0.5f;                 ///< The amount of bonus the player will get when moving with the wind, in percent ( 1.0 == 100 == double speed )
        public const int   NUM_ARC_TICKS            = 20;                   ///< The number of ticks in the firing arc
        public const float GRAVITY                  = 100.0f;               ///< The Acceleration due to Gravity (For the CannonBall)
                                                                            ///
        public const int   MAX_PORT_COINS           = 5;                    ///< The number of coins a port is created with

        public const float TOWER_HEALTH             = 100.0f;               ///< The max health of the town's towers
        public const float TOWER_RANGE              = 100.0f;               ///< The range of the tower
        public const float TOWER_ATTACK_ANGLE       = (float)Math.PI/2;     ///< the angle of the attack region of the tower  
        public const float TOWER_WAIT_TIME          = 2.5f;                 ///< The amount of time the tower waits after a shot

        public const float CANNON_VELOCITY          = 100.0f;               ///< The Horizontal Velocity of the Cannonball
        public const float CANNON_COLLIDE_RADIUS    = 10.0f;                ///< The hit radius of the cannonball                                                                

        public const float CAMERA_TURN_SPEED        = (float)Math.PI / 2.0f;///< The speed the camera will orbit at in Radians per second
        public const float CAMERA_ZOOM_SPEED        = 50.0f;                ///< Zoom units per second
        public const float CAMERA_READY_DISTANCE    = 5.0f;                 ///< Distance to the moveto target in which the camera is considered to have arrived
        public const float CAMERA_HEIGHT            = 30.0f;
        public const float CAMERA_LOOKAT_HEIGHT     = 15.0f;
        public const float CAMERA_DEFAULT_RADIUS    = 75.0f;
        public const float DEAD_ZONE                = 0.1f;                 ///< The Controllers Dead Zone

        //temporary, 2000 seemed a little extreme (10 seconds passed, and the AI wasn't even past double digits in regards to frame delay)
        public const float AI_TOTAL_DELAY = 0.75f;                           ///<The amount of pause between the AI actions                                                                    
//        public const float AI_TOTAL_DELAY = 2000;                           ///<The amount of pause between the AI actions
                                                                
        ////////////////////////////////////////////////
        //HUD settings
        public const float SAFE_REGION_PERCENTAGE   = 0.05f;         ///< Amount of screen not to draw the HUD to

        ///////////////////////////////////////////
        //Application settings
        public const float SWITCH_INPUT_DELAY       = 0.3f;         ///< The delay, when switching modules, before processing input
  
        //These will be ignored if the startup is full screen
        public const int PREFFERED_WINDOW_WIDTH     = 1280;         ///< The preffered window width
        public const int PREFFERED_WINDOW_HEIGHT    = 720;          ///< The preffered window height
        
        ///////////////////////////////////////////
        //Loader settings
        public const float LOADER_FADE_OUT_TIME     = 1.0f;         ///< Time to pause and fade out the load screen
                                                                
        ///////////////////////////////////////////////////
        //Splash screen properties
        public const float SPLASH_TIME              = 4.0f;         ///< Time to keep the splash screen visible
        public const float SPLASH_FADE_IN_TIME      = 1.0f;         ///< Time to fade in the splash screen, in seconds
        public const float SPLASH_FADE_OUT_TIME     = 1.0f;         ///< Time to fade out the splash screen, in seconds
                                                                    
        ///////////////////////////////////////////////////
        //Edge Detection Shader Catch
        public static bool DETECT_EDGES             = false;
    }
}
