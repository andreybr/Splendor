using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace Splendor
{
    class MainMenu
    {
        XNACS1Rectangle[] oButtons = new XNACS1Rectangle[2];
        Color oButtonHover = new Color(100, 100, 100, 100);
        Color oButtonNormal = new Color(0, 0, 0, 0);

        public MainMenu()
        {
            //XNACS1Base.World.SetBackgroundColor(Color.Black);
            XNACS1Base.World.SetBackgroundTexture("TitleScreen3");

            for (int x = 0; x < oButtons.Count(); x++)
            {
                //oButtons[x] = new XNACS1Rectangle(new Vector2(MyLibrary.Resize(600), MyLibrary.Resize(210) - x * MyLibrary.Resize(70)), MyLibrary.Resize(200), MyLibrary.Resize(50));
                oButtons[x] = new XNACS1Rectangle(new Vector2(MyLibrary.Resize(600), MyLibrary.Resize(210) - x * MyLibrary.Resize(70)), MyLibrary.Resize(320), MyLibrary.Resize(20));
                oButtons[x].LabelColor = Color.White;
                oButtons[x].Color = oButtonNormal;
                oButtons[x].LabelFont = "Title";
            }

            oButtons[0].Label = "Play Game";
            oButtons[1].Label = "Quit";
        }

        public Game.MENU_STATE Update(Game oGame, Cursor oCursor)        
        {
            if (oCursor.Collided(oButtons[0]))
            {
                //oButtons[0].Color = oButtonHover;
                oButtons[0].LabelColor = Color.Gold;
                oButtons[0].Texture = "Selector";
                //oButtons[1].Color = oButtonNormal;
                oButtons[1].LabelColor = Color.White;
                oButtons[1].Texture = "Empty";

                if (oCursor.LeftClick())
                    return Game.MENU_STATE.GAME_OPTIONS;
            }
            else if (oCursor.Collided(oButtons[1]))
            {
                //oButtons[0].Color = oButtonNormal;
                oButtons[0].LabelColor = Color.White;
                oButtons[0].Texture = "Empty";
                //oButtons[1].Color = oButtonHover;
                oButtons[1].LabelColor = Color.Gold;
                oButtons[1].Texture = "Selector";

                if (oCursor.LeftClick())
                    oGame.Exit();
            }
            else
            {
                oButtons[0].LabelColor = Color.White;
                oButtons[1].LabelColor = Color.White;
                oButtons[0].Texture = "Empty";
                oButtons[1].Texture = "Empty";
                //oButtons[0].Color = oButtonNormal;
                //oButtons[1].Color = oButtonNormal;
            }

            return Game.MENU_STATE.MAIN;
        }

        public void Clear()
        {
            for (int x = 0; x < oButtons.Count(); x++)
            {
                oButtons[x].RemoveFromAutoDrawSet();
                oButtons[x] = null;
            }
            XNACS1Base.World.SetBackgroundTexture("");
        }
    }
}
