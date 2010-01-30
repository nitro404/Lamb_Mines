using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Scallywags 
{
    //base class for the menus
    public abstract class MainMenuScreen 
    {
        private List< MenuItem >    m_menuItems;        //the list of all the menu items for each menu
        /*  @fn MainMenuScreen
         *  @brief Constructor
         */
        public MainMenuScreen() 
        {
            m_menuItems = new List< MenuItem >();
        }
        #region INTERFACE

        /** @fn     void AddMenuItem( MenuItem item )
         *  @brief  add a menu item to the menu screen
            @param  item [in] the item to add
         */
        public void AddMenuItem( MenuItem item )
        {
            if( item != null )
                m_menuItems.Add( item );
        }

        #endregion

        #region MUST_INHERIT
        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public abstract void Init(GraphicsDevice device, Dictionary< string, SpriteFont > fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures);

#endregion

#region CAN INHERIT VIRTUAL

        /** @fn     void ShutDown()
         *  @brief  cleanup the menu screen
         */
        public virtual void ShutDown()
        {
            m_menuItems.Clear();
        }
        
        /** @fn     GameMenuID Update( double fTotalTime, float fElapsedTIme, InputManager input )
         *  @brief  updates the menu items
         *  @return the ID of the menu screen to switch to, or GMN_THIS if this screen is to continue to run
         *  @param  fTotalTime [in] the total time the application has been running
         *  @param  fElapsedTime [in] the time, in seconds, since the previous frame
         *  @param  input [in] the application's input manager
         */
        public virtual GameMenuID Update(double fTotalTime, float fElapsedTime, InputManager input, SoundManager sounds, int Control)
        {
            foreach( MenuItem item in m_menuItems )
                item.Update( fElapsedTime, input );

            return GameMenuID.GMN_THIS;
        }

        /*  @fn Draw
         *  @brief This is the default draw function for the menu
         *      It will draw all the information that is stored
         *      in the MenuItemInfo list.
         *  @param device [in] The graphics device being used
         *  @param gameTime [in] not used?
         */
        public virtual void Draw( SpriteBatch sb ) 
        {
            foreach( MenuItem item in m_menuItems )
                item.Draw( sb );
        }
#endregion

    }

    public class ReferenceableInt
    {
        public int Integer;
    }
}
