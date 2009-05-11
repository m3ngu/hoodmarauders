using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Manhattanville.PieMenu;

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
        private static List<PieMenuNode> menus;
        public static bool handlesEnabled = false;
        public static bool continousMode = true;
        public static bool handleGrabbed = false;

        public static void initialize(Manhattanville m, GraphicsDeviceManager g)
        {
            app = m;
            graphics = g;

            icons = new List<Texture2D>(Enum.GetNames(typeof(AppState)).Length);
            icons.Insert((int)AppState.Browse, app.Content.Load<Texture2D>("Icons\\browse"));
            icons.Insert((int)AppState.Edit, app.Content.Load<Texture2D>("Icons\\edit"));
            icons.Insert((int)AppState.Info, app.Content.Load<Texture2D>("Icons\\info"));

            iconPlaceHolder = new Rectangle(
                graphics.PreferredBackBufferWidth - iconWidth - iconPaddingX,
                graphics.PreferredBackBufferHeight - iconHeight - iconPaddingY,
                iconWidth,
                iconHeight);

            Log.Write("statusIconPlaceHolder: " + iconPlaceHolder.ToString() + "\n");

            LoadMenu();
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
                    handlesEnabled = false;
                    app.toggleHandles();

                    break;
                case AppState.Edit:
                    /* -----------EDIT----------*/
                    showIcon = true;
                    handlesEnabled = true;
                    app.toggleHandles();
                   
                    break;
                case AppState.Info:
                    /* -----------INFO----------*/
                    showIcon = true;
                    handlesEnabled = false;
                    app.toggleHandles();
                    
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

        public static bool inState(AppState s)
        {
            return s.Equals(currentState);
        }

        private static void LoadMenu()
        {
            menus = new List<PieMenuNode>(Enum.GetNames(typeof(AppState)).Length);

            /*********************************************************************/
            int i = (int)AppState.Browse;
            /*********************************************************************/
            menus.Add(new PieMenuNode());

            menus[i].Add(new PieMenuNode());  // Placeholder

            menus[i].Add(new PieMenuNode("Edit",
               app.Content.Load<Texture2D>("Icons\\edit"),
               new SimpleDelegate(MenuAction),
               AppState.Edit));

            menus[i].Add(new PieMenuNode("Cancel",
                app.Content.Load<Texture2D>("Icons\\cancel"),
                null));

            menus[i].Add(new PieMenuNode("Info",
                app.Content.Load<Texture2D>("Icons\\info"),
                new SimpleDelegate(MenuAction),
                AppState.Info));
            
            /*********************************************************************/
            i = (int)AppState.Edit;
            /*********************************************************************/
            menus.Add(new PieMenuNode());

            menus[i].Add(new PieMenuNode("Grab",
                app.Content.Load<Texture2D>("Icons\\grab"),
                new SimpleDelegate(MenuAction)));

            menus[i].Add(new PieMenuNode("Accept",
                app.Content.Load<Texture2D>("Icons\\accept"),
                new SimpleDelegate(MenuAction),
                AppState.Browse));

            menus[i].Add(new PieMenuNode("Cancel",
                app.Content.Load<Texture2D>("Icons\\cancel"),
                null));

            menus[i].Add(new PieMenuNode("Reject",
                app.Content.Load<Texture2D>("Icons\\reject"),
                new SimpleDelegate(MenuAction),
                AppState.Browse));
            /*********************************************************************/
            i = (int)AppState.Info;
            /*********************************************************************/
            menus.Add(new PieMenuNode());

            menus[i].Add(new PieMenuNode());  // Empty placeholder

            menus[i].Add(new PieMenuNode("Edit",
                app.Content.Load<Texture2D>("Icons\\edit"),
                new SimpleDelegate(MenuAction),
                AppState.Edit));

            menus[i].Add(new PieMenuNode("Cancel",
                app.Content.Load<Texture2D>("Icons\\cancel"),
                null));

            menus[i].Add(new PieMenuNode("Browse",
                app.Content.Load<Texture2D>("Icons\\browse"),
                new SimpleDelegate(MenuAction),
                AppState.Browse));

        }

        public static void MenuAction(Object sender)
        {
            PieMenuNode sndr = (PieMenuNode)sender;

            GoblinXNA.UI.Notifier.AddMessage(sndr.Text + " Selected!");

            if (sndr.Text.Equals("Grab"))
            {
                ModificationManager.grabHandle();
                handleGrabbed = true;
                sndr.Text = "Release";
            }
            else if (sndr.Text.Equals("Release"))
            {
                ModificationManager.releaseHandle(); 
                handleGrabbed = false;
                sndr.Text = "Grab";
            }
            else if (sndr.Text.Equals("Accept"))
            {
                //TODO: Accept
                enter(AppState.Browse);
            }
            else if (sndr.Text.Equals("Reject"))
            {
                //TODO: Reject
                enter(AppState.Browse);
            }
            else if ((sndr.State == AppState.Edit) && (!app.buildingSelected()))
            {
                GoblinXNA.UI.Notifier.AddMessage("ERROR: Please select a building before selecting the edit menu!");
            }
            else
            {
                enter(sndr.State);
            }
            
        }

        public static PieMenuNode currentMenu()
        {
            return menus[(int)currentState];
        }

    }
}
