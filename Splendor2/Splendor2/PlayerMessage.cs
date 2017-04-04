using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace Splendor
{
    public class PlayerMessage
    {
        XNACS1Rectangle[] oMessageBoxes;
        int[] nMessageTimers;
        int nMessageDuration = 200;
        //Vector2 oMessageBoxOffset = new Vector2(-19f, 0f);
        Vector2 oMessageBoxOffset = new Vector2(MyLibrary.Resize(130), MyLibrary.Resize(90));
        int nCharactersPerLine = 55; 
        
        public PlayerMessage(int nNumberOfPlayers)
        {
            nMessageTimers = new int[nNumberOfPlayers];
            oMessageBoxes = new XNACS1Rectangle[nNumberOfPlayers];
            for (int x = 0; x < oMessageBoxes.Count(); x++)
            {
                oMessageBoxes[x] = new XNACS1Rectangle(new Vector2(), 29f, 6f);
                oMessageBoxes[x].Color = new Color(0, 0, 0, 180);
                oMessageBoxes[x].LabelColor = Color.White;
                oMessageBoxes[x].LabelFont = "PlayerMessage";
                oMessageBoxes[x].Visible = false;
            }
        }

        public void Update()
        {
            for (int x = 0; x < nMessageTimers.Count(); x++)
            {
                if (nMessageTimers[x] > 0)
                    nMessageTimers[x]--;
                else if (nMessageTimers[x] == 0)
                {
                    nMessageTimers[x]--;
                    DeleteMessage(x);
                }
            }
        }

        public void Recenter(Player[] oPlayers)
        {
            for (int x = 0; x < oMessageBoxes.Count(); x++)
            {
                oMessageBoxes[x].CenterX = oPlayers[x].Center().X + oMessageBoxOffset.X;

                if (x > 1)
                    oMessageBoxes[x].CenterY = oPlayers[x].Center().Y + oMessageBoxOffset.Y;
                else
                    oMessageBoxes[x].CenterY = oPlayers[x].Center().Y - oMessageBoxOffset.Y;
            }
        }

        public void DeleteMessage(int nID)
        {
            oMessageBoxes[nID].RemoveFromAutoDrawSet();
        }

        public void ShowMessage(int nID, string strMessage)
        {
            if(oMessageBoxes[nID] != null)
            {
                nMessageTimers[nID] = 0;
                oMessageBoxes[nID].RemoveFromAutoDrawSet();
                oMessageBoxes[nID] = null;
            }

            oMessageBoxes[nID] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(580), MyLibrary.Resize(46));
            oMessageBoxes[nID].Color = new Color(0, 0, 0, 180);
            oMessageBoxes[nID].LabelColor = Color.White;
            oMessageBoxes[nID].LabelFont = "PlayerMessage";

            int nCharCount = 0; 
            int nLastBreak = -1;
            List<string> oSubStrings = new List<string>();
            bool bEnd = false;

            while (strMessage.Length > 0)
            {
                while (nCharCount < strMessage.Length && nCharCount < nCharactersPerLine)
                {
                    if(strMessage[nCharCount] == ' ')
                        nLastBreak = nCharCount;

                    if (nCharCount == strMessage.Length - 1)
                    {
                        nLastBreak = nCharCount + 1;
                        bEnd = true;
                    }

                    nCharCount++;
                }

                if (bEnd == true)
                {
                    oSubStrings.Add(strMessage.Substring(0, nLastBreak));
                    strMessage = strMessage.Substring(nLastBreak);
                }

                else  if (nLastBreak > -1)
                {
                    oSubStrings.Add(strMessage.Substring(0, nLastBreak));
                    strMessage = strMessage.Substring(nLastBreak + 1);
                }
                else
                {
                    oSubStrings.Add(strMessage.Substring(0, nCharCount));
                    strMessage = "";
                }

                while(oSubStrings.Last().Length < nCharactersPerLine - 1)
                {
                    oSubStrings[oSubStrings.Count() - 1] += " ";
                }
                oSubStrings[oSubStrings.Count() - 1] += "\n";

                nCharCount = 0;
                nLastBreak = -1; 
            }

            // recombine the substrings
            oMessageBoxes[nID].Label = "";
            for (int x = 0; x < oSubStrings.Count(); x++)         
                oMessageBoxes[nID].Label += oSubStrings[x];          

            nMessageTimers[nID] = nMessageDuration;
        }
    }
}
