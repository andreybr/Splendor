using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace Splendor
{
    class GameOptions
    {
        XNACS1Rectangle[] oBoxes = new XNACS1Rectangle[3];
        XNACS1Rectangle[] oPlayerAvatars = new XNACS1Rectangle[3];
        XNACS1Rectangle[] oTopButtons = new XNACS1Rectangle[3];
        XNACS1Rectangle[] oBottomButtons = new XNACS1Rectangle[3];
        public int[] nBotsSelected = new int[3];
        XNACS1Rectangle oAcceptButton;   
        int nBoxSelected = 0;
        bool bButtonPressReset = true;

        public GameOptions()
        {
            XNACS1Base.World.SetBackgroundTexture("Background3");

            for (int x = 0; x < oBoxes.Count(); x++)
            {
                oBoxes[x] = new XNACS1Rectangle(new Vector2(MyLibrary.Resize(330) + x * MyLibrary.Resize(250), MyLibrary.Resize(300)), MyLibrary.Resize(220), MyLibrary.Resize(80));
                oBoxes[x].Visible = false;

                oPlayerAvatars[x] = new XNACS1Rectangle(new Vector2(MyLibrary.Resize(300) + x * MyLibrary.Resize(270), MyLibrary.Resize(400)), MyLibrary.Resize(220), MyLibrary.Resize(220), "Facetest3");
                oPlayerAvatars[x].LabelColor = Color.Black;
                oTopButtons[x] = new XNACS1Rectangle(oPlayerAvatars[x].Center + new Vector2(0, MyLibrary.Resize(140)), MyLibrary.Resize(66), MyLibrary.Resize(36), "ArrowUp");
                oBottomButtons[x] = new XNACS1Rectangle(oPlayerAvatars[x].Center + new Vector2(0, MyLibrary.Resize(-140)), MyLibrary.Resize(66), MyLibrary.Resize(36), "ArrowDown");
            }

            oAcceptButton = new XNACS1Rectangle(new Vector2(MyLibrary.Resize(900), MyLibrary.Resize(100)), MyLibrary.Resize(200), MyLibrary.Resize(100));
            oAcceptButton.Label = "Accept";
        }

        public Game.MENU_STATE Update(Cursor oCursor, KeyboardState oKeyState)
        {
            //XNACS1Base.EchoToTopStatus("gameoptions:" + nBoxSelected);
            oKeyState = Keyboard.GetState();

            for (int x = 0; x < oTopButtons.Count(); x++) // left and right buttons
            {
                #region left/right button clicks = change bot
                if (oCursor.LeftClick())
                {
                    if (oCursor.oHitbox.Collided(oBottomButtons[x]))
                    {
                        if (nBotsSelected[x] > 0)
                            nBotsSelected[x]--;
                    }
                    else if (oCursor.oHitbox.Collided(oTopButtons[x]))
                    {
                        if (nBotsSelected[x] < 3)
                            nBotsSelected[x]++;
                    }
                }
                #endregion
            }

            if (oCursor.oHitbox.Collided(oAcceptButton)) // accept options button
            {
                if(oCursor.LeftClick())
                    return Game.MENU_STATE.GAME;
            }

            for (int x = 0; x < oBoxes.Count(); x++)
            {
                if (oCursor.LeftClick())
                {
                    if (oCursor.oHitbox.Collided(oBoxes[x]))
                    {
                        nBoxSelected = x;
                    }
                }

                if (oKeyState.IsKeyDown(Keys.Down))
                {
                    if (bButtonPressReset == true)
                    {
                        if (nBotsSelected[nBoxSelected] > 0)
                            nBotsSelected[nBoxSelected]--;
                    }

                    bButtonPressReset = false;
                }
                else if (oKeyState.IsKeyDown(Keys.Up))
                {
                    if (bButtonPressReset == true)
                    {
                        if (nBotsSelected[nBoxSelected] < 5)
                            nBotsSelected[nBoxSelected]++;
                    }

                    bButtonPressReset = false;
                }
                else if (oKeyState.IsKeyDown(Keys.Right))
                {
                    if (bButtonPressReset == true)
                    {
                        if (nBoxSelected < oBoxes.Count() - 1)
                            nBoxSelected++;
                    }

                    bButtonPressReset = false;
                }
                else if (oKeyState.IsKeyDown(Keys.Left))
                {
                    if (bButtonPressReset == true)
                    {
                        if (nBoxSelected > 0)
                            nBoxSelected--;
                    }

                    bButtonPressReset = false;
                }
                else
                {
                    bButtonPressReset = true;
                }

                if (nBotsSelected[x] != 0)
                {
                    oPlayerAvatars[x].Texture = "FaceTest" + (nBotsSelected[x] + 2);
                    //oPlayerAvatars[x].Label = "FaceTest " + oBotsSelected[x];
                }
                else
                {
                    oPlayerAvatars[x].Texture = "FaceEmpty";
                }

                #region update box labels
                //if (oBotsSelected[x] == 0)
                //{
                //    oPlayerAvatars[x].Texture = "FaceTest " + (oBotsSelected[x] + 1);
                //}
                //else if (oBotsSelected[x] == 1)
                //{
                //    oPlayerAvatars[x].Texture = "FaceTest " + oBotsSelected[x];
                //}
                //else if (oBotsSelected[x] == 2)
                //{
                //    oPlayerAvatars[x].Texture = "FaceTest " + oBotsSelected[x];
                //}
                //if (oBotsSelected[x] == 0)
                //{
                //    oBoxes[x].Label = "TrapClap " + oBotsSelected[x];
                //    oBoxes[x].Color = Color.Green;
                //}
                //else if (oBotsSelected[x] == 1)
                //{
                //    oBoxes[x].Label = "Error " + oBotsSelected[x];
                //    oBoxes[x].Color = Color.Orange;
                //}
                //else if (oBotsSelected[x] == 2)
                //{
                //    oBoxes[x].Label = "Andrey3000 " + oBotsSelected[x];
                //    oBoxes[x].Color = Color.Red;
                //}
                #endregion
            }      

            return Game.MENU_STATE.GAME_OPTIONS;
        }

        public void Clear()
        {
            for (int x = 0; x < oBoxes.Count(); x++)
            {
                oPlayerAvatars[x].RemoveFromAutoDrawSet();
                oBoxes[x].RemoveFromAutoDrawSet();
                oTopButtons[x].RemoveFromAutoDrawSet();
                oBottomButtons[x].RemoveFromAutoDrawSet();           
            }
            oAcceptButton.RemoveFromAutoDrawSet();
        }
    }
}
