using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Manhattanville
{
    // declares the enum
    public enum AppState : int
    {
        Browse = 0,
        Edit = 1,
        Info = 2
    }

    class AppStateMgr
    {
        private static Manhattanville app;
        private static GraphicsDeviceManager graphics;
        private static AppState currentState;
        private static bool initialized = false;

        // Status Icon related stuff
        private const int iconWidth = 108; //216
        private const int iconHeight = 81; //162
        private const int iconPaddingX = 5;
        private const int iconPaddingY = 5;
        public static Rectangle iconPlaceHolder;
        private static List<Texture2D> icons;
        public static bool showIcon = false;

        public static void initialize(Manhattanville m, GraphicsDeviceManager g)
        {
            app = m;
            graphics = g;

            icons = new List<Texture2D>(Enum.GetNames(typeof(AppState)).Length);
            icons.Insert((int)AppState.Browse, app.Content.Load<Texture2D>("Icons\\height"));
            icons.Insert((int)AppState.Edit, app.Content.Load<Texture2D>("Icons\\footprint"));
            icons.Insert((int)AppState.Info, app.Content.Load<Texture2D>("Icons\\info"));

            iconPlaceHolder = new Rectangle(
                graphics.PreferredBackBufferWidth - iconWidth - iconPaddingX,
                graphics.PreferredBackBufferHeight - iconHeight - iconPaddingY,
                iconWidth,
                iconHeight);

            Log.Write("statusIconPlaceHolder: " + iconPlaceHolder.ToString() + "\n");
        }

        /*****************************************************************/
        /*                             ENTER                             */
        /*****************************************************************/
        public static void enter(AppState s)
        {

            if (s.Equals(currentState)) return;

            if (initialized)
            {
                exit(currentState);
            }

            currentState = s;

            Log.Write("Entering State: " + name(currentState) + "\n");

            switch (s)
            {
                case AppState.Browse:
                    /* ----------BROWSE---------*/
                    showIcon = false;
                    
                    break;
                case AppState.Edit:
                    /* -----------EDIT----------*/
                    showIcon = true;

                   
                    break;
                case AppState.Info:
                    /* -----------INFO----------*/
                    showIcon = true;

                    
                    break;
            }

            initialized = true; // Only significant first time enter is called
        }
        /*****************************************************************/
        /*                             EXIT                              */
        /*****************************************************************/
        public static void exit(AppState s)
        {

            Log.Write("Exiting State: " + name(currentState) + "\n");

            switch (s)
            {
                case AppState.Browse:
                    /* ----------BROWSE---------*/




                    break;
                case AppState.Edit:
                    /* -----------EDIT----------*/




                    break;
                case AppState.Info:
                    /* -----------INFO----------*/




                    break;
            }
        }

        public static String name(AppState s)
        {
            return Enum.GetName(typeof(AppState), s);
        }

        public static Texture2D icon()
        {
            return icons[(int)currentState];
        }
    }
}
