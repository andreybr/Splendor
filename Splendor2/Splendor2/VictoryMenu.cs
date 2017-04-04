using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    class VictoryMenu : XNACS1Rectangle
    {
        private float fWidth = MyLibrary.Resize(960);
        private float fHeight = MyLibrary.Resize(600);

        XNACS1Rectangle[] oBoxes = new XNACS1Rectangle[4];
        List<List<XNACS1Rectangle>> oCardIcons = new List<List<XNACS1Rectangle>>();

        public VictoryMenu(List<Player> oPlayerList)
        {
            this.Width = fWidth;
            this.Height = fHeight;
            this.Color = Color.Black;
            this.LabelColor = Color.White;
            this.LabelFont = "VictoryMenu";
            this.Texture = "VictoryMenuBox2";

            for (int x = 0; x < oPlayerList.Count(); x++)
                oCardIcons.Add(new List<XNACS1Rectangle>());

            //// test data for sorting
            //Random rand = new Random();
            //for (int x = 0; x < oPlayerList.Count(); x++)
            //    oPlayerList[x].nPoints = (rand.Next() % 20);
            //// test data for displaying cards
            //for (int x = 0; x < oPlayerList.Count(); x++)
            //    for (int y = 0; y < 3; y++)
            //        oPlayerList[x].nBoughtCardTiers[y] = (rand.Next() % 4 + 1);

            int nHighestPoints;
            int nHighestIndex;
            int nEnd = oBoxes.Count();
            Player oPlayerTemp;
            nEnd = oPlayerList.Count();
            while (nEnd > 1)
            {
                nHighestIndex = -1;
                nHighestPoints = 200;

                for (int x = 0; x < nEnd; x++)
                {
                    if (oPlayerList[x].Points() < nHighestPoints)
                    {
                        nHighestPoints = oPlayerList[x].Points();
                        nHighestIndex = x;
                    }
                }

                if (nHighestIndex > -1)
                {
                    oPlayerTemp = oPlayerList[nEnd - 1];
                    oPlayerList[nEnd - 1] = oPlayerList[nHighestIndex];
                    oPlayerList[nHighestIndex] = oPlayerTemp;
                }

                nEnd--;
            }
          
            for (int x = 0; x < oPlayerList.Count(); x++)
            {
                oBoxes[x] = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(860), MyLibrary.Resize(70));
                oBoxes[x].LabelColor = Color.White;
                oBoxes[x].Color = new Color(70, 70, 70, 70);
                //oBoxes[x].Texture = "VictoryMenuBar";
                oBoxes[x].LabelFont = "VictoryMenu";
                oBoxes[x].TextureTintColor = Color.White;

                // position text correctly
                oBoxes[x].Label = oPlayerList[x].Points() + "";
                while(oBoxes[x].Label.Length < 7)
                    oBoxes[x].Label += " ";

                oBoxes[x].Label += oPlayerList[x].GetName() + "";
                while(oBoxes[x].Label.Length < 59)
                    oBoxes[x].Label += " ";

                // draw cards
                int nTotalCards = oPlayerList[x].nBoughtCardTiers[0] + oPlayerList[x].nBoughtCardTiers[1] + oPlayerList[x].nBoughtCardTiers[2];
                for (int y = 0; y < nTotalCards; y++)
                {
                    oCardIcons[x].Add(new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(34), MyLibrary.Resize(50)));

                    if (y < oPlayerList[x].nBoughtCardTiers[0])
                        oCardIcons[x].Last().Texture = "CardBackSmall1";
                    else if (y < oPlayerList[x].nBoughtCardTiers[0] + oPlayerList[x].nBoughtCardTiers[1])
                        oCardIcons[x].Last().Texture = "CardBackSmall2";
                    else
                        oCardIcons[x].Last().Texture = "CardBackSmall3";
                }
            }
            Recenter();
        }

        public void Recenter()
        {
            this.CenterX = MyLibrary.Resize(MyLibrary.nWindowPixelWidth / 2);
            this.CenterY = MyLibrary.Resize(MyLibrary.nWindowPixelHeight / 2);

            for (int x = 0; x < oBoxes.Count(); x++)
            {
                if(oBoxes[x] != null)
                    oBoxes[x].Center = this.Center + new Vector2(0, MyLibrary.Resize(80) - x * MyLibrary.Resize(85));
            }

            for (int x = 0; x < oCardIcons.Count(); x++)
            {
                for (int y = 0; y < oCardIcons[x].Count(); y++)
                {
                    oCardIcons[x][y].Center = oBoxes[x].Center + new Vector2(-5f + 1.8f * y, 0f);
                }
            }
        }
    }
}
