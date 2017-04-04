using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    class PlayerBot : BotBase
    {
        bool bCardActionActive = false;
        bool bCoinActionActive = false;
        Vector2 oTarget = new Vector2();
        int[] nSelectedCoins = new int[5];
        XNACS1Rectangle oCardActionMenu;

        bool bShowCardInfo = false;
        XNACS1Rectangle oCardInfoBox;
        float fCardInfoBoxWidth = MyLibrary.Resize(99);
        float fCardInfoBoxHeight = MyLibrary.Resize(100);      
        Vector2 oCardInfoBoxOffset = new Vector2(MyLibrary.Resize(95), MyLibrary.Resize(0));

        XNACS1Circle[] oCoinActionCircle = new XNACS1Circle[10];
        Color oCoinBlank = new Color(60, 60, 60, 100);

        List<XNACS1Primitive>[] oCardInfoCoins = new List<XNACS1Primitive>[5];
        Color oSelectedWhite = new Color(230, 230, 230);
        Color oSelectedBlue = new Color(62, 127, 190);
        Color oSelectedGreen = new Color(62, 190, 86);
        Color oSelectedRed = new Color(190, 62, 62);
        Color oSelectedBlack = new Color(70, 50, 28);
        Color oUnselectedWhite = new Color(57, 57, 57);
        Color oUnselectedBlue = new Color(15, 30, 47);
        Color oUnselectedGreen = new Color(15, 47, 22);
        Color oUnselectedRed = new Color(47, 15, 15);
        Color oUnselectedBlack = new Color(30, 20, 7);

        Vector2 oPreviouslyHovering = new Vector2(-1, -1);

        public PlayerBot(List<Card>[] oNewCardStack, List<Card>[] oNewPickedUpCards, List<Noble> oNewNobleStack, Player[] oNewPlayers,
                         ActionLog oNewActionLog, int[] nNewCoinCount, XNACS1Rectangle[] oNewCardBacks, XNACS1Rectangle[] oNewCardCounts,
                         Stats oNewStats, XNACS1Rectangle oNewCoinActionMenu,
                         Cursor oNewCursor, XNACS1Rectangle[] oNewCoinStack, TurnInfo oNewTurnInfo)
                         : base(oNewCardStack, oNewPickedUpCards, oNewNobleStack, oNewPlayers, oNewActionLog, nNewCoinCount, oNewCardBacks,
                                oNewCardCounts, oNewStats, oNewCoinActionMenu, oNewCursor, oNewCoinStack, oNewTurnInfo)
        {}

        public override void TakeTurn()
        {
            //XNACS1Base.EchoToTopStatus("PlayerBot:" + oTarget + oCardActionMenu.Center);

            #region show card info when hovering card
            Vector2 oCurrentlyHovering = new Vector2(-1, -1);            
            for (int y = 0; y < 3; y++) // tier
            {
                int nEnd;

                if (oCardStack[y].Count > 3)
                    nEnd = 4;
                else
                    nEnd = oCardStack[y].Count;

                for (int x = 0; x < nEnd; x++) // card
                {
                    if (oCursor.oHitbox.Collided(oCardStack[y][x].oCardImage) == true)
                    {
                        oCurrentlyHovering = new Vector2(x, y);
                    }
                }
            }

            for (int x = 0; x < CountCardsHeld(GetMyID()); x++)
            {
                if (oCursor.oHitbox.Collided(oPickedUpCards[GetMyID()][x].oCardImage) == true)
                {
                    oCurrentlyHovering = new Vector2(x, -1);
                }
            }

            if(oCurrentlyHovering.X == -1 && oCurrentlyHovering.Y == -1)
            {
                if (oPreviouslyHovering.X != -1)
                {
                    if (oPreviouslyHovering.Y != -1)
                    {
                        oCardStack[(int)oPreviouslyHovering.Y][(int)oPreviouslyHovering.X].ScaleSize(1.00f);           
                    }
                    else
                        if (oPreviouslyHovering.X < oPickedUpCards[GetMyID()].Count())
                            oPickedUpCards[GetMyID()][(int)oPreviouslyHovering.X].ScaleSize(1.00f);
                }

                oPreviouslyHovering = oCurrentlyHovering;

                if( oCardInfoBox != null)
                    HideCardInfo();           
            }
            else if (oPreviouslyHovering != oCurrentlyHovering)
            {
                if (oPreviouslyHovering.X != -1)
                {
                    if (oPreviouslyHovering.Y != -1)
                        oCardStack[(int)oPreviouslyHovering.Y][(int)oPreviouslyHovering.X].ScaleSize(1.00f);
                    else
                        if (oPreviouslyHovering.X < oPickedUpCards[GetMyID()].Count())
                            oPickedUpCards[GetMyID()][(int)oPreviouslyHovering.X].ScaleSize(1.00f);
                }
                
                oPreviouslyHovering = oCurrentlyHovering;

                if(oCardInfoBox != null)           
                    HideCardInfo();                  

                oCardInfoBox = new XNACS1Rectangle(new Vector2(), fCardInfoBoxWidth, fCardInfoBoxHeight, "CardInfoBox2");
                oCardInfoBox.TextureTintColor = new Color(255, 255, 255, 245);
                //oCardInfoBox.Label = "Cost" + "\n \n \n ";
                oCardInfoBox.LabelColor = Color.White;
                oCardInfoBox.LabelFont = "CardInfo";
                oCardInfoBox.Color = Color.Black;

                int[] nCardCost;
                if(oCurrentlyHovering.Y == -1)
                    nCardCost = CheckCardFromHand((int)oCurrentlyHovering.X).Cost();
                else
                    nCardCost = CheckCardFromBoard((int)oCurrentlyHovering.X, (int)oCurrentlyHovering.Y).Cost();                   

                int[] nCoinsOwned = CountCoinsHeld(GetMyID());
                int[] nCardsOwned = CountCardsBought(GetMyID());

                for (int nCoinColorIndex = 0; nCoinColorIndex < 5; nCoinColorIndex++) // coin color
                {
                    oCardInfoCoins[nCoinColorIndex] = new List<XNACS1Primitive>();
                    for (int b = 0; b < nCardCost[nCoinColorIndex]; b++) // coin x index
                    {
                        if (nCardsOwned[nCoinColorIndex] >= b + 1)
                        {
                            oCardInfoCoins[nCoinColorIndex].Add(new XNACS1Rectangle(new Vector2(), 1.1f, 1.4f, "CardIconTest"));
                            oCardInfoCoins[nCoinColorIndex].Last().Color = Color.Green;
                            oCardInfoCoins[nCoinColorIndex].Last().RotateAngle = 20;

                            if (nCoinColorIndex == 0)
                                oCardInfoCoins[nCoinColorIndex].Last().TextureTintColor = oSelectedWhite;
                            else if (nCoinColorIndex == 1)
                                oCardInfoCoins[nCoinColorIndex].Last().TextureTintColor = oSelectedBlue;
                            else if (nCoinColorIndex == 2)
                                oCardInfoCoins[nCoinColorIndex].Last().TextureTintColor = oSelectedGreen;
                            else if (nCoinColorIndex == 3)
                                oCardInfoCoins[nCoinColorIndex].Last().TextureTintColor = oSelectedRed;
                            else if (nCoinColorIndex == 4)
                                oCardInfoCoins[nCoinColorIndex].Last().TextureTintColor = oSelectedBlack;
                        }
                        else if (nCardsOwned[nCoinColorIndex] + nCoinsOwned[nCoinColorIndex] >= b + 1)
                        {
                            XNACS1Circle oNewCircle = new XNACS1Circle(new Vector2(), 0.7f, "CoinIconTest");
                            //oCardInfoCoins[nCoinColorIndex].Insert(0, oNewCircle);
                            oCardInfoCoins[nCoinColorIndex].Add(oNewCircle);

                            if (nCoinColorIndex == 0)
                                oNewCircle.TextureTintColor = oSelectedWhite;
                            else if (nCoinColorIndex == 1)
                                oNewCircle.TextureTintColor = oSelectedBlue;
                            else if (nCoinColorIndex == 2)
                                oNewCircle.TextureTintColor = oSelectedGreen;
                            else if (nCoinColorIndex == 3)
                                oNewCircle.TextureTintColor = oSelectedRed;
                            else if (nCoinColorIndex == 4)
                                oNewCircle.TextureTintColor = oSelectedBlack;
                        }
                        else
                        {
                            XNACS1Circle oNewCircle = new XNACS1Circle(new Vector2(), 0.7f, "CoinIconTest");
                            //oCardInfoCoins[nCoinColorIndex].Insert(0, oNewCircle);
                            oCardInfoCoins[nCoinColorIndex].Add(oNewCircle);

                            if (nCoinColorIndex == 0)
                                oNewCircle.TextureTintColor = oUnselectedWhite;
                            else if (nCoinColorIndex == 1)
                                oNewCircle.TextureTintColor = oUnselectedBlue;
                            else if (nCoinColorIndex == 2)
                                oNewCircle.TextureTintColor = oUnselectedGreen;
                            else if (nCoinColorIndex == 3)
                                oNewCircle.TextureTintColor = oUnselectedRed;
                            else if (nCoinColorIndex == 4)
                                oNewCircle.TextureTintColor = oUnselectedBlack;
                        }
                    }
                }

                if (oCurrentlyHovering.Y == -1)
                {
                    oCardInfoBox.CenterX = oPickedUpCards[GetMyID()][(int)oCurrentlyHovering.X].oCardImage.CenterX + oCardInfoBoxOffset.X;
                    oCardInfoBox.CenterY = oPickedUpCards[GetMyID()][(int)oCurrentlyHovering.X].oCardImage.CenterY + oCardInfoBoxOffset.Y;
                }
                else
                {
                    oCardInfoBox.CenterX = oCardStack[(int)oCurrentlyHovering.Y][(int)oCurrentlyHovering.X].oCardImage.CenterX + oCardInfoBoxOffset.X;
                    oCardInfoBox.CenterY = oCardStack[(int)oCurrentlyHovering.Y][(int)oCurrentlyHovering.X].oCardImage.CenterY + oCardInfoBoxOffset.Y;
                }

                // center coin images
                for (int a = 0; a < 5; a++)
                {
                    for (int b = 0; b < oCardInfoCoins[a].Count(); b++)
                    {
                        oCardInfoCoins[a][b].CenterX = -3.0f + oCardInfoBox.CenterX + 1f * b;
                        oCardInfoCoins[a][b].CenterY = oCardInfoBox.CenterY + 2.98f - a * 1.5f;
                    }
                }


                if (oCurrentlyHovering.X != -1)
                {
                    if (oCurrentlyHovering.Y != -1)
                        oCardStack[(int)oCurrentlyHovering.Y][(int)oCurrentlyHovering.X].ScaleSize(1.05f);
                    else
                        oPickedUpCards[GetMyID()][(int)oCurrentlyHovering.X].ScaleSize(1.05f);

                    XNACS1Base.PlayACue("tick");
                }

            }
            #endregion

            #region select coins for pickup
            if (bCoinActionActive == true)
            {
                #region select coins
                for (int x = 0; x < 5; x++)
                {
                    if (oCursor.oHitbox.Collided(oCoinStack[x]) == true)
                    {
                        if (oCursor.LeftClick() == true) // left click
                        {
                            if (CoinsRemaining()[x] > 0)
                            {
                                if (nSelectedCoins[x] == 0 && ArrayTotal(nSelectedCoins) < 3 && nSelectedCoins[0] < 2 && 
                                    nSelectedCoins[1] < 2 && nSelectedCoins[2] < 2 && nSelectedCoins[3] < 2 && nSelectedCoins[4] < 2)
                                {
                                    nSelectedCoins[x]++;
                                    //oCoinActionMenu.Label = nSelectedCoins[0] + "     \n" + nSelectedCoins[1] + "     \n" + nSelectedCoins[2] + "     \n" +
                                    //                        nSelectedCoins[3] + "     \n" + nSelectedCoins[4] + "     \n";

                                    if (x == 0)
                                        oCoinActionCircle[x * 2].Color = oSelectedWhite;
                                    else if (x == 1)
                                        oCoinActionCircle[x * 2].Color = oSelectedBlue;
                                    else if (x == 2)
                                        oCoinActionCircle[x * 2].Color = oSelectedGreen;
                                    else if (x == 3)
                                        oCoinActionCircle[x * 2].Color = oSelectedRed;
                                    else if (x == 4)
                                        oCoinActionCircle[x * 2].Color = oSelectedBlack;
                                }
                                else if (nSelectedCoins[x] == 1 && ArrayTotal(nSelectedCoins) == 1 && CoinsRemaining()[x] >= 4 )
                                {
                                    nSelectedCoins[x]++;
                                    //oCoinActionMenu.Label = nSelectedCoins[0] + "     \n" + nSelectedCoins[1] + "     \n" + nSelectedCoins[2] + "     \n" +
                                    //                        nSelectedCoins[3] + "     \n" + nSelectedCoins[4] + "     \n";

                                    if (x == 0)
                                        oCoinActionCircle[x * 2 + 1].Color = oSelectedWhite;
                                    else if (x == 1)
                                        oCoinActionCircle[x * 2 + 1].Color = oSelectedBlue;
                                    else if (x == 2)
                                        oCoinActionCircle[x * 2 + 1].Color = oSelectedGreen;
                                    else if (x == 3)
                                        oCoinActionCircle[x * 2 + 1].Color = oSelectedRed;
                                    else if (x == 4)
                                        oCoinActionCircle[x * 2 + 1].Color = oSelectedBlack;
                                }
                            }
                        }
                        else if (oCursor.RightClick() == true)
                        {
                            if (nSelectedCoins[x] > 0)
                            {
                                if (nSelectedCoins[x] == 2)
                                    oCoinActionCircle[x * 2 + 1].Color = oCoinBlank;
                                else if (nSelectedCoins[x] == 1)
                                    oCoinActionCircle[x * 2].Color = oCoinBlank;

                                nSelectedCoins[x]--;
                                //oCoinActionMenu.Label = nSelectedCoins[0] + "     \n" + nSelectedCoins[1] + "     \n" + nSelectedCoins[2] + "     \n" +
                                //                        nSelectedCoins[3] + "     \n" + nSelectedCoins[4] + "     \n";

                            }
                        }
                    }
                }
                #endregion

                #region pickup/cancel buttons
                if (oCursor.oHitbox.Collided(oCoinActionMenu) == true)
                {
                    if (oCursor.oHitbox.CenterY > oCoinActionMenu.CenterY + MyLibrary.Resize(172) &&
                        oCursor.oHitbox.CenterY < oCoinActionMenu.CenterY + MyLibrary.Resize(195))
                    {
                        oCoinActionMenu.Texture = "CoinActionPickup";
                        if (oCursor.LeftClick() == true)
                        {
                            PickUpCoins(nSelectedCoins);
                            oCoinActionMenu.Visible = false;
                            bCoinActionActive = false;


                            for (int x = 0; x < oCoinActionCircle.Count(); x++)
                            {
                                oCoinActionCircle[x].RemoveFromAutoDrawSet();
                                oCoinActionCircle[x] = null;
                            }

                            for(int x = 0; x < nSelectedCoins.Count(); x++)
                                nSelectedCoins[x] = 0;
                        }
                    }
                    else if (oCursor.oHitbox.CenterY < oCoinActionMenu.CenterY - MyLibrary.Resize(172) &&
                             oCursor.oHitbox.CenterY > oCoinActionMenu.CenterY - MyLibrary.Resize(195))
                    {
                        oCoinActionMenu.Texture = "CoinActionCancel";
                        if (oCursor.LeftClick() == true)
                        {
                            oCoinActionMenu.Visible = false;
                            bCoinActionActive = false;

                            for (int x = 0; x < oCoinActionCircle.Count(); x++)
                            {
                                oCoinActionCircle[x].RemoveFromAutoDrawSet();
                                oCoinActionCircle[x] = null;
                            }

                            for (int x = 0; x < nSelectedCoins.Count(); x++)
                                nSelectedCoins[x] = 0;
                        }
                    }
                    else
                    {
                        oCoinActionMenu.Texture = "CoinActionNeutral";
                    }
                }
                #endregion
            }
            #endregion                

            if (bCardActionActive == false)
            {
                #region select/deselect target
                if (oCursor.LeftClick() == true)
                {
                    // select card on board for action
                    for (int y = 0; y < 3; y++) // tier
                    {
                        int nEnd;

                        if (oCardStack[y].Count() > 3)
                            nEnd = 4;
                        else
                            nEnd = oCardStack[y].Count();

                        for (int x = 0; x < nEnd; x++) // card
                        { 
                             if (oCardStack[y][x] != null &&
                                oCursor.oHitbox.Collided(oCardStack[y][x].oCardImage))
                            {
                                oCardActionMenu = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "CardActionNeutral");
                                oTarget = new Vector2(x, y);
                                bCardActionActive = true;
                                oCardActionMenu.Texture = "CardActionNeutral";
                                oCardActionMenu.Center = oCardStack[y][x].Center();
                                oCardActionMenu.Visible = true;
                            }
                        }
                    }

                    // select reserved card for action
                    for (int x = 0; x < CountCardsHeld(GetMyID()); x++) // card
                    {
                        if (oCursor.oHitbox.Collided(oPickedUpCards[GetMyID()][x].oCardImage))
                        {
                            oCardActionMenu = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "CardActionNeutral");
                            oTarget = new Vector2(x, 4);
                            bCardActionActive = true;
                            oCardActionMenu.Texture = "CardActionNeutral";
                            oCardActionMenu.Center = oPickedUpCards[GetMyID()][x].Center();
                            oCardActionMenu.Visible = true;
                        }
                    }

                    // select deck for action
                    for (int x = 0; x < 3; x++)
                    {
                        if (oCursor.oHitbox.Collided(oCardBacks[x]) == true)
                        {
                            oCardActionMenu = new XNACS1Rectangle(new Vector2(), MyLibrary.Resize(84), MyLibrary.Resize(122), "DeckActionNeutral");
                            oTarget = new Vector2(x, -1);
                            bCardActionActive = true;
                            oCardActionMenu.Texture = "DeckActionNeutral";
                            oCardActionMenu.Center = oCardBacks[x].Center;
                            oCardActionMenu.Visible = true;
                        }
                    }

                    // select coin action
                    for (int x = 0; x < 6; x++)
                    {
                        if (oCursor.oHitbox.Collided(oCoinStack[x]) == true)
                        {
                            //oCoinActionMenu.Color = Color.Black;

                            if (bCoinActionActive == false)
                            {
                                for (int y = 0; y < oCoinActionCircle.Count(); y++)
                                {
                                    if (y % 2 == 0)
                                        oCoinActionCircle[y] = new XNACS1Circle(oCoinStack[y / 2].Center + new Vector2(-3.25f, 0.9f), 0.7f);
                                    else
                                        oCoinActionCircle[y] = new XNACS1Circle(oCoinStack[y / 2].Center + new Vector2(-3.25f, -0.9f), 0.7f);

                                    oCoinActionCircle[y].Color = oCoinBlank;
                                }

                                if(x != 5)
                                    nSelectedCoins[x]++;

                                if (x == 0)
                                    oCoinActionCircle[x * 2].Color = oSelectedWhite;
                                else if (x == 1)
                                    oCoinActionCircle[x * 2].Color = oSelectedBlue;
                                else if (x == 2)
                                    oCoinActionCircle[x * 2].Color = oSelectedGreen;
                                else if (x == 3)
                                    oCoinActionCircle[x * 2].Color = oSelectedRed;
                                else if (x == 4)
                                    oCoinActionCircle[x * 2].Color = oSelectedBlack;
                            }

                            bCoinActionActive = true;                           
                            oCoinActionMenu.Center = new Vector2(oCoinStack[0].CenterX - MyLibrary.Resize(12), (oCoinStack[2].CenterY + oCoinStack[3].CenterY) / 2);
                            oCoinActionMenu.Texture = "CoinActionNeutral";
                            oCoinActionMenu.Visible = true;
                            //oCoinActionMenu.Label = nSelectedCoins[0] + "     \n" + nSelectedCoins[1] + "     \n" + nSelectedCoins[2] + "     \n" +
                            //                        nSelectedCoins[3] + "     \n" + nSelectedCoins[4] + "     \n";
                        }
                    }
                }
                #endregion
            }
            else if (bCardActionActive == true)
            {
                #region perform action on target
                if (oCursor.oHitbox.Collided(oCardActionMenu) == true)
                {
                    if (oTarget.Y > -1 && oTarget.Y < 4) // card on board action
                    {
                        if (oCursor.oHitbox.Center.Y > (oCardActionMenu.CenterY + MyLibrary.Resize(12)) &&
                            oCursor.oHitbox.Center.Y < (oCardActionMenu.CenterY + MyLibrary.Resize(40)))
                        {
                            oCardActionMenu.Texture = "CardActionBuy";

                            if (oCursor.LeftClick())
                            {
                                if (BuyCardFromBoard((int)oTarget.X, (int)oTarget.Y) == true)
                                {
                                    HideCardInfo();
                                    oPreviouslyHovering = new Vector2(-1, -1);
                                }

                                oCardActionMenu.Visible = false;
                                bCardActionActive = false;
                            }
                        }
                        else if (oCursor.oHitbox.Center.Y < (oCardActionMenu.CenterY - MyLibrary.Resize(12)) &&
                                 oCursor.oHitbox.Center.Y > (oCardActionMenu.CenterY - MyLibrary.Resize(40)))
                        {
                            oCardActionMenu.Texture = "CardActionReserve";

                            if (oCursor.LeftClick())
                            {
                                if (ReserveCardFromBoard((int)oTarget.X, (int)oTarget.Y) == true)
                                {
                                    HideCardInfo();
                                    oPreviouslyHovering = new Vector2(-1, -1);
                                }

                                oCardActionMenu.Visible = false;
                                bCardActionActive = false;
                            }
                        }
                        else
                        {
                            oCardActionMenu.Texture = "CardActionNeutral";
                        }
                    }
                    else if (oTarget.Y == 4) // reserved card action
                    {
                        if (oCursor.oHitbox.Center.Y > (oCardActionMenu.CenterY + MyLibrary.Resize(12)) &&
                            oCursor.oHitbox.Center.Y < (oCardActionMenu.CenterY + MyLibrary.Resize(40)))
                        {
                            oCardActionMenu.Texture = "CardActionBuy";

                            if (oCursor.LeftClick())
                            {
                                BuyCardFromHand((int)oTarget.X);
                                oCardActionMenu.Visible = false;
                                bCardActionActive = false;
                            }
                        }

                        else
                        {
                            oCardActionMenu.Texture = "CardActionNeutral";
                        }
                    }
                    else if (oTarget.Y == -1)// deck action
                    {
                        if (oCursor.oHitbox.Center.Y < (oCardActionMenu.CenterY + MyLibrary.Resize(10)) &&
                            oCursor.oHitbox.Center.Y > (oCardActionMenu.CenterY - MyLibrary.Resize(10)))
                        {
                            oCardActionMenu.Texture = "DeckActionReserve";

                            if (oCursor.LeftClick())
                            {
                                ReserveCardFromDeck((int)oTarget.X);
                                oCardActionMenu.RemoveFromAutoDrawSet();
                                oCardActionMenu = null;
                                //oCardActionMenu.Visible = false;
                                bCardActionActive = false;
                            }
                        }
                        else
                        {
                            oCardActionMenu.Texture = "DeckActionNeutral";
                        }
                    }
                }
                #endregion

                #region cancel target
                if (oCursor.RightClick() == true)
                {
                    if (oCardActionMenu != null)
                    {
                        oCardActionMenu.RemoveFromAutoDrawSet();
                        oCardActionMenu = null;
                    }

                    bCardActionActive = false;
                }
                #endregion
            }
        }
        public override void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard)
        {

        }
        public override string Name()
        {
            return "Player";
        }
        public override string Picture()
        {
            return "FaceTest";
        }

        private void HideCardInfo()
        {
            oCardInfoBox.RemoveFromAutoDrawSet();
            oCardInfoBox = null;
            for (int nCoinColorIndex = 0; nCoinColorIndex < 5; nCoinColorIndex++) // coin color
            {
                if (oCardInfoCoins[nCoinColorIndex] != null)
                {
                    while (oCardInfoCoins[nCoinColorIndex].Count > 0)
                    {
                        oCardInfoCoins[nCoinColorIndex][0].RemoveFromAutoDrawSet();
                        oCardInfoCoins[nCoinColorIndex].RemoveAt(0);
                    }
                }
            } 
        }
    }
}
