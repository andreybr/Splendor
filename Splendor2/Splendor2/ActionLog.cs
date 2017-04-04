using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNACS1Lib;

namespace Splendor
{
    public class ActionLog
    {
        XNACS1Rectangle oMenuBox;
        List<XNACS1Rectangle> oLogs;
        int nMaxLogs = 11;
        int nCharactersPerLine = 50;

        public ActionLog()
        {
            oMenuBox = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(500), MyLibrary.Resize(390));
            oMenuBox.Color = new Color(0, 0, 0, 150);
            oMenuBox.Center = new Vector2(MyLibrary.Resize(950), MyLibrary.Resize(385));
            oLogs = new List<XNACS1Rectangle>();

            Recenter();
        }

        public void AddLog(string strMessage, Color oTextColor)
        {
            //strMessage = "and and and and and and and ad and and and and and and and and and and and and and and and and and and and and and";

            int nCharCount = 0;
            int nLastBreak = -1;
            List<string> oSubStrings = new List<string>();
            bool bEnd = false;

            while (strMessage.Length > 0)
            {
                while (nCharCount < strMessage.Length && nCharCount < nCharactersPerLine)
                {
                    if (strMessage[nCharCount] == ' ')
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

                else if (nLastBreak > -1)
                {
                    oSubStrings.Add(strMessage.Substring(0, nLastBreak));
                    strMessage = strMessage.Substring(nLastBreak + 1);
                }
                else
                {
                    oSubStrings.Add(strMessage.Substring(0, nCharCount));
                    strMessage = "";
                }

                while (oSubStrings.Last().Length < nCharactersPerLine - 1)
                {
                    oSubStrings[oSubStrings.Count() - 1] += " ";
                }
                oSubStrings[oSubStrings.Count() - 1] += "\n";

                nCharCount = 0;
                nLastBreak = -1;
            }

            // recombine the substrings
            strMessage = "";
            for (int x = 0; x < oSubStrings.Count(); x++)
                strMessage += oSubStrings[x]; 

            //while (strMessage.Length < nCharactersPerLine)
            //    strMessage += " ";

            oLogs.Add(new XNACS1Rectangle(new Vector2(), 40f, oSubStrings.Count() * 1.5f));
            oLogs.Last().Color = new Color(0, 0, 0, 150);
            oLogs.Last().Texture = "Empty";
            oLogs.Last().Label = strMessage;
            oLogs.Last().LabelFont = "Log";
            oLogs.Last().LabelColor = oTextColor;

            if (oLogs.Count > nMaxLogs)
            {
                oLogs[0].RemoveFromAutoDrawSet();
                oLogs.RemoveAt(0);
            }

            Recenter();
        }

        public void Recenter()
        {
            for (int x = 0; x < oLogs.Count(); x++)
            {
                if (x == 0)
                    oLogs[x].Center = new Vector2(oMenuBox.CenterX - MyLibrary.Resize(44), MyLibrary.Resize(570) - oLogs[x].Height / 2);
                else
                    oLogs[x].Center = new Vector2(oLogs[x - 1].CenterX, oLogs[x - 1].CenterY - (oLogs[x].Height / 2 + oLogs[x - 1].Height / 2 + 0.35f));
            }
        }
    }
}
