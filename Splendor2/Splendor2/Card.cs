using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNACS1Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Splendor
{
    public class Card 
    {
        private float fWidth = MyLibrary.Resize(84);
        private float fHeight = MyLibrary.Resize(122);
        private float fCircleRadius = 0.9f;

        public XNACS1Rectangle oCardImage;
        public List<XNACS1Circle> oCardCosts;
        public Vector2 oCardCostOffset = new Vector2(MyLibrary.Resize(-25), MyLibrary.Resize(-43));

        public enum TYPE { WHITE = 0, BLUE = 1, GREEN = 2, RED = 3, BLACK = 4 };
        private TYPE eType;
        private int[] nCost = new int[6];
        private int nPoints;
        public int nTier;
        private XNACS1Rectangle oScore;

        public Card(Vector2 vNewLocation, TYPE eNewType, int[] nNewCost, int nNewPoints)
        {
            eType = eNewType;
            nPoints = nNewPoints;

            for (int x = 0; x < nNewCost.Length; x++)
                nCost[x] = nNewCost[x];
        }

        public void Recenter(Vector2 oNewCenter)
        {
            oCardImage.Center = oNewCenter;

            if(oScore != null)
                oScore.Center = oCardImage.Center + new Vector2(2.0f, 3.9f);

            for (int x = oCardCosts.Count() - 1; x > -1; x--)
            {
                oCardCosts[x].Center = new Vector2(oCardCostOffset.X + oCardImage.CenterX,
                                                   oCardCostOffset.Y + oCardImage.CenterY + ((oCardCosts.Count() - 1) - x) * MyLibrary.Resize(22));
            }
        }

        public int Tier()
         {
            int nToReturn = nTier;
            return nTier;
        }

        public void DrawFront()
        {
            oCardImage = new XNACS1Rectangle(new Vector2(), fWidth, fHeight);
            oCardImage.LabelFont = "Card";
            oCardImage.LabelColor = Color.White;

            oCardCosts = new List<XNACS1Circle>();
            for (int x = 0; x < 5; x++)
            {
                if (nCost[x] > 0)
                {
                    oCardCosts.Add(new XNACS1Circle(new Vector2(), fCircleRadius));
                    oCardCosts.Last().Label = "" + nCost[x];
                    oCardCosts.Last().LabelFont = "Card";

                    if (x == 0)
                        oCardCosts.Last().LabelColor = Color.Black;
                    else
                        oCardCosts.Last().LabelColor = Color.White;

                    switch (x)
                    {
                        case 0:
                            oCardCosts.Last().Color = Color.White;
                            break;
                        case 1:
                            oCardCosts.Last().Color = new Color(62, 127, 190);
                            break;
                        case 2:
                            oCardCosts.Last().Color = new Color(62, 190, 86);
                            break;
                        case 3:
                            oCardCosts.Last().Color = new Color(190, 62, 62);
                            break;
                        case 4:
                            oCardCosts.Last().Color = new Color(70, 50, 28);
                            break;
                    }
                }
                //else
                //{
                //    oCardCosts.Add(new XNACS1Circle(new Vector2(), fCircleRadius));
                //    oCardCosts.Last().Color = new Color(0, 0, 0, 190);
                //}
            }

            switch (eType)
            {
                case TYPE.WHITE:
                    oCardImage.Texture = "CardWhite5";
                    break;
                case TYPE.BLUE:
                    oCardImage.Texture = "CardBlue5";
                    break;
                case TYPE.GREEN:
                    oCardImage.Texture = "CardGreen5";
                    break;
                case TYPE.RED:
                    oCardImage.Texture = "CardRed5";
                    break;
                case TYPE.BLACK:
                    oCardImage.Texture = "CardBlack5";
                    break;
            }

            oScore = new XNACS1Rectangle();
            oScore.LabelColor = Color.White;
            oScore.LabelFont = "CardValue";
            oScore.Texture = "Empty";

            if (nPoints > 0)
                oScore.Label = "" + nPoints;

            //oCardImage.Label = "\n" + "\n" + "\n" + "\n" +
            //                nCost[0] + "     \n" + nCost[1] + "     \n" + nCost[2] + "     \n" + nCost[3] + "     \n" +
            //                nCost[4] + "     " + "\n" + "\n" + "\n " + "\n " + "\n " + "\n " + "\n " + "\n " + "\n ";          
        }

        public void DrawBack()
        {
            oCardImage = new XNACS1Rectangle(new Vector2(), fWidth, fHeight);
            oCardImage.LabelFont = "Card";
            oCardImage.LabelColor = Color.White;

            switch (nTier)
            {
                case 0:
                    oCardImage.Texture = "CardBack1";
                    break;
                case 1:
                    oCardImage.Texture = "CardBack2";
                    break;
                case 2:
                    oCardImage.Texture = "CardBack3";
                    break;
            }
        }

        public void ScaleSize(float fScale)
        {
            if (oCardImage != null)
            {
                oCardImage.Width = fWidth * fScale;
                oCardImage.Height = fHeight * fScale;
            }
        }

        public Vector2 Center()
        {
            return oCardImage.Center;
        }

        public int Points()
        {
            int nToReturn = nPoints;
            return nToReturn;
        }      

        public int[] Cost()
        {
            int[] arrnToReturn = new int[6];
            nCost.CopyTo(arrnToReturn, 0);
            return arrnToReturn;
        }

        public int NetCost()
        {
            return nCost[0] + nCost[1] + nCost[2] + nCost[3] + nCost[4] + nCost[5];
        }

        public TYPE Type()
        {
            TYPE eToReturn = eType;
            return eToReturn;
        }

        public Card Copy()
        {
            Card oToReturn = new Card(new Vector2(), eType, new int[] { nCost[0], nCost[1], nCost[2], nCost[3], nCost[4] }, nPoints);
            oToReturn.nTier = nTier;
            return oToReturn;
        }

        public void Clear()
        {
            if(oCardImage != null)
                oCardImage.RemoveFromAutoDrawSet();

            for (int x = oCardCosts.Count() - 1; x > -1; x--)
            {
                oCardCosts[x].RemoveFromAutoDrawSet();
                oCardCosts.RemoveAt(x);
            }
            
            if(oScore != null)
                oScore.RemoveFromAutoDrawSet();

            oScore = null;
            oCardImage = null;
        }
    }
}
