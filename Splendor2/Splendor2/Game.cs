using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNACS1Lib;

namespace Splendor
{
    public class Game : XNACS1Base
    {
        KeyboardState oKeyState = new KeyboardState();
        Cursor oCursor;

        public enum MENU_STATE { NO_CHANGE, MAIN, GAME_OPTIONS, GAME };
        MENU_STATE eMenuState = MENU_STATE.GAME;
        GameMenu oGameMenu;
        MainMenu oMainMenu;
        GameOptions oGameOptions;

        protected override void InitializeWorld()
        {
            SetAppWindowPixelDimension(false, MyLibrary.nWindowPixelWidth, MyLibrary.nWindowPixelHeight);
            IsMouseVisible = true;
            oCursor = new Cursor();
        }

        protected override void UpdateWorld()
        {
            //XNACS1Base.EchoToTopStatus("Andrey:" + oBestTarget.X + " " + oBestTarget.Y);
            oKeyState = Keyboard.GetState();
            oCursor.Update();

            #region close application (F1)
            if (oKeyState.IsKeyDown(Keys.F1))
                this.Exit();
            #endregion

            if (eMenuState == MENU_STATE.MAIN)
            {
                if (oMainMenu == null)
                    oMainMenu = new MainMenu();

                eMenuState = oMainMenu.Update(this, oCursor);

                if (eMenuState != MENU_STATE.MAIN)
                    oMainMenu.Clear();
            }
            else if (eMenuState == MENU_STATE.GAME_OPTIONS)
            {
                if (oGameOptions == null)
                    oGameOptions = new GameOptions();

                eMenuState = oGameOptions.Update(oCursor, oKeyState);

                if (eMenuState == MENU_STATE.GAME)
                {
                    oGameMenu = new GameMenu(oCursor, oGameOptions.nBotsSelected);
                    oGameOptions.Clear();
                }
            }
            else if (eMenuState == MENU_STATE.GAME)
            {
                if (oGameMenu == null)
                    oGameMenu = new GameMenu(oCursor);

                oGameMenu.Update(oCursor);

                if (eMenuState != MENU_STATE.GAME)
                    oGameMenu.Clear();
            }
        }
    }
}
