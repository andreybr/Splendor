using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNACS1Lib;
using Microsoft.Xna.Framework;

namespace Splendor
{
    public class AndreyBot : BotBase
    {
        int[,] nTurnsNeededToBuy;
        int[,] nCoinsNeededToBuy;
        int[,] nCloseToNobles; // how close each player is to a noble
        int[] nNobleAffinity; // distribution of noble costs by color
        List<Vector2> oTargets = new List<Vector2>();
        Vector2 oBestTarget = new Vector2();
        Card oBestCard;

        public AndreyBot(List<Card>[] oNewCardStack, List<Card>[] oNewPickedUpCards, List<Noble> oNewNobleStack, Player[] oNewPlayers,
                         ActionLog oNewActionLog, int[] nNewCoinCount, XNACS1Rectangle[] oNewCardBacks, XNACS1Rectangle[] oNewCardCounts,
                         Stats oNewStats, XNACS1Rectangle oNewCoinActionMenu,
                         Cursor oNewCursor, XNACS1Rectangle[] oNewCoinStack, TurnInfo oNewTurnInfo)
                         : base(oNewCardStack, oNewPickedUpCards, oNewNobleStack, oNewPlayers, oNewActionLog, nNewCoinCount, oNewCardBacks,
                                oNewCardCounts, oNewStats, oNewCoinActionMenu, oNewCursor, oNewCoinStack, oNewTurnInfo)
        {}

        // required functions
        public override void TakeTurn()
        {
            //ShowMessage("this is a really really short test message");
            //XNACS1Base.EchoToTopStatus("Andrey:" + oBestTarget.X + " " + oBestTarget.Y);
            //XNACS1Base.EchoToTopStatus("Andrey:" + nLeastOwnedPriority[0] + "," + nLeastOwnedPriority[1] + "," + nLeastOwnedPriority[2] + ","
            //         + nLeastOwnedPriority[3] + "," + nLeastOwnedPriority[4] + ",");

            //if (CountCardsHeld(GetMyID()) > 0)
            //    BuyCardFromHand(0);
            //else
            //    ReserveCardFromDeck(0);

            ////BuyCardFromBoard(0, 0);
            //return;

            oBestTarget = new Vector2();

            int nTest1;
            int nTest2;
            int nTest3;
            int nTest4;
            int nTest5;
            int nTest6;
            int nTest7;

            #region populate possible cards+
            for (int nTier = 0; nTier < 4; nTier++)
            {
                if (nTier < 3)
                {
                    for (int nCard = 0; nCard < CardsOnBoard(nTier); nCard++)
                    {
                        oTargets.Add(new Vector2(nCard, nTier));
                    }
                }
                else
                {
                    for (int nCard = 0; nCard < CardsReservedByPlayer(GetMyID()).Count(); nCard++)
                    {
                        oTargets.Add(new Vector2(nCard, nTier));
                    }
                }
            }
            #endregion
            if (CountNetCardsBought(GetMyID()) < 5)
            {
                FilterByTier(1);
                FilterByTier(2);
            }
            nTest1 = oTargets.Count();

            FilterByCanAffordNow();
            nTest2 = oTargets.Count();

            //FilterByLeastTurns();
            nTest3 = oTargets.Count();

            FilterByLeastCoinsNeeded();
            nTest4 = oTargets.Count();

            if(CountNetCardsBought(GetMyID()) < 10)
                FilterByCardsLeastOwned();
            nTest5 = oTargets.Count();

            FilterByPoints();
            nTest6 = oTargets.Count();
     
            if (CountNoblesAvailable() > 0)
                FilterByNobleAffinity();
            nTest7 = oTargets.Count();

            oBestTarget = oTargets[oTargets.Count() - 1];
            int[] oTargetCost;

            if((int)oBestTarget.Y < 3)
                oTargetCost = CheckCardFromBoard((int)oBestTarget.X, (int)oBestTarget.Y).Cost();
            else
                oTargetCost = CardsReservedByPlayer(GetMyID())[(int)oBestTarget.X].Cost();

            //XNACS1Base.EchoToTopStatus("Andrey:" + oBestTarget.X + "," + oBestTarget.Y + "  |" + nTest1 + "|" + nTest2 + "|" + nTest3 + "|" + nTest4 + "|" + nTest5 + "|" + nTest6 + "|" + nTest7); 

            //XNACS1Base.EchoToTopStatus("Andrey:" + oBestTarget.X + "," + oBestTarget.Y + "  " +
            //                           oTargetCost[0] + "," + oTargetCost[1] + "," + oTargetCost[2] + "," + oTargetCost[3] + "," + oTargetCost[4] + ":\n::");

            #region if can afford target = buy card
            Card oCardToCheck;

            if (oBestTarget.Y < 3)
                oCardToCheck = CheckCardFromBoard((int)oBestTarget.X, (int)oBestTarget.Y);
            else
                oCardToCheck = CardsReservedByPlayer(GetMyID())[(int)oBestTarget.X];

            if (CanAfford(oCardToCheck) == true)
            {
                if (oBestTarget.Y < 3)
                    BuyCardFromBoard((int)oBestTarget.X, (int)oBestTarget.Y);
                else
                    BuyCardFromHand((int)oBestTarget.X);
            }
            #endregion
      
            #region if cant afford target = pick up coins
            if (CountNetCoinsHeld(GetMyID()) < 8) // shouldnt be check when picking up 2
            {
                int[] nLeastOwnedPriority = new int[5] { 0, 1, 2, 3, 4 }; // buying power in order of least owned
                int[] nTargetRequiredPriority = new int[5] { 0, 1, 2, 3, 4 }; // coin types in order to buy target 
                int[] nWantedCoins = new int[5]; // coins I have selected to be picked up

                #region sort required for target
                int nEnd = 5;
                int nTemp;
                while (nEnd >= 1)
                {
                    for (int x = 1; x < nEnd; x++)
                    {
                        if (oTargetCost[nTargetRequiredPriority[x - 1]] < oTargetCost[nTargetRequiredPriority[x]])
                        {
                            nTemp = nTargetRequiredPriority[x - 1];
                            nTargetRequiredPriority[x - 1] = nTargetRequiredPriority[x];
                            nTargetRequiredPriority[x] = nTemp;
                        }
                    }
                    nEnd--;
                }
                #endregion

                #region sort least owned buying power by type
                nEnd = 5;
                while (nEnd >= 1)
                {
                    for (int x = 1; x < nEnd; x++)
                    {
                        if (SumResources(GetMyID())[nLeastOwnedPriority[x - 1]] > SumResources(GetMyID())[nLeastOwnedPriority[x]])
                        {
                            nTemp = nLeastOwnedPriority[x - 1];
                            nLeastOwnedPriority[x - 1] = nLeastOwnedPriority[x];
                            nLeastOwnedPriority[x] = nTemp;
                        }
                    }
                    nEnd--;
                }
                #endregion

                #region try to pick up 3 different coins 
                int nCoinsSelected = 0;
                int nPriorityIndex = 0;
                while (nCoinsSelected < 3 && nPriorityIndex < 5) // try to pick up coins needed for target
                {
                    if (CoinsRemaining()[nTargetRequiredPriority[nPriorityIndex]] > 0 &&
                        SumResources(GetMyID())[nTargetRequiredPriority[nPriorityIndex]] < oTargetCost[nTargetRequiredPriority[nPriorityIndex]])
                    {
                        nWantedCoins[nTargetRequiredPriority[nPriorityIndex]]++;
                        nCoinsSelected++;
                    }
                    nPriorityIndex++;
                }
                nPriorityIndex = 0;

                while (nCoinsSelected < 3 && nPriorityIndex < 5) // try to pick up coins of least color owned
                {
                    if (CoinsRemaining()[nLeastOwnedPriority[nPriorityIndex]] > 0 && nWantedCoins[nLeastOwnedPriority[nPriorityIndex]] == 0)
                    {
                        nWantedCoins[nLeastOwnedPriority[nPriorityIndex]]++;
                        nCoinsSelected++;
                    }
                    nPriorityIndex++;
                }
                #endregion

                if (nCoinsSelected == 3)
                    PickUpCoins(nWantedCoins);
                else
                    nWantedCoins = new int[5];

                #region try to pick up 2 coins
                for (int x = 0; x < 5; x++)
                {
                    if (CoinsRemaining()[nLeastOwnedPriority[x]] >= 4)
                    {
                        nWantedCoins[nLeastOwnedPriority[x]] += 2;
                        break;
                    }
                }
                #endregion

                if (nCoinsSelected == 2)
                    PickUpCoins(nWantedCoins);
            }
            #region reserve card from deck as last resort
            if(bTurnTaken == false)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (CardsInDeck(x) > 0)
                        ReserveCardFromDeck(x);

                    //ShowMessage("I r dumb");
                }
            }
            #endregion
            #endregion

            ResetTargets(); 
        }
        public override void ReactToMove(string sName, int nPlayerID, TurnInfo.ActionType eAction, int[] arrnSelectedCoins, Card oCard)
        {
            Random rand = new Random();
            int nRandResult;

            if (eAction == TurnInfo.ActionType.NONE)
            {
                nRandResult = rand.Next() % 3; // 1

                if(nRandResult == 0)
                    ShowMessage("Wise choice " + sName + ".");
                else if(nRandResult == 1)
                    ShowMessage("Damn, you smart as hell " + sName + ".");
                else if (nRandResult == 2)
                    ShowMessage("If I had a dollar for every brain you don't have I would have one dollar.");            
            }
            else if (eAction == TurnInfo.ActionType.COINS)
            {
                if (rand.Next() % 5 > 2) // 2/5
                    return;

                ShowAvatar(1);
                nRandResult = rand.Next() % 6;

                if (nRandResult == 0)
                    ShowMessage("I need those coins! I have a family to feed!");
                else if (nRandResult == 1)
                    ShowMessage("Orphan children need those coins!");
                else if (nRandResult == 2)
                    ShowMessage("Coins are for poor people like you " + sName + ".");
                else if (nRandResult == 3)
                    ShowMessage("Damn, you could buy 4 paperclips with that.");
                else if (nRandResult == 4)
                    ShowMessage("I will trade you some magic beans for those.");
                else if (nRandResult == 5)
                    ShowMessage("If I had money like that I wouldn't be playing this game.");
            }
            else if (eAction == TurnInfo.ActionType.BUY_BOARD)
            {
                if (oBestCard != null && CompareCard(oBestCard, oCard) == true)
                {
                    if (rand.Next() % 4 > 2) // 3/4
                        return;

                    nRandResult = rand.Next() % 3;

                    if (nRandResult == 0)
                        ShowMessage("Nooo! I was gona buy that for little Timmy!");
                    else if (nRandResult == 1)
                        ShowMessage("Ya cool jerk. I needed that.");
                    else if (nRandResult == 2)
                        ShowMessage("That was the only card I ever loved!");
                }
                else if (oCard.Points() == 3)
                {
                    if (rand.Next() % 5 > 2) // 3/5
                        return;

                    nRandResult = rand.Next() % 2;

                    if (nRandResult == 0)
                        ShowMessage("Gimme that! You have enough points already.");
                    else if (nRandResult == 1)
                        ShowMessage("I make more points than that in an hour.");
                }
                else if (oCard.Points() == 4)
                {
                    if (rand.Next() % 5 > 2) // 3/5
                        return;

                    nRandResult = rand.Next() % 2;

                    if (nRandResult == 0)
                        ShowMessage("If I had hands " + sName + " I would slap you.");
                    else if (nRandResult == 1)
                        ShowMessage("Share the wealth gaylord.");
                }
                else if (oCard.Points() == 5)
                {
                    if (rand.Next() % 5 > 2) // 3/5
                        return;

                    nRandResult = rand.Next() % 2;

                    if (nRandResult == 0)
                        ShowMessage("You need to die in a fire.");
                    else if (nRandResult == 1)
                        ShowMessage("NOOOOOO!");
                }
                else
                {
                    if (rand.Next() % 5 > 2) // 2/5
                        return;

                    ShowAvatar(2);
                    nRandResult = rand.Next() % 5;

                    if (nRandResult == 0)
                        ShowMessage("Slow down there big spender.");
                    else if (nRandResult == 1)
                        ShowMessage("A couple more moves like that and you will win for sure!");
                    else if (nRandResult == 2)
                        ShowMessage("It's all yours my friend!");
                    else if (nRandResult == 3)
                        ShowMessage("Take it. I didn't want it anyway.");
                    else if (nRandResult == 4)
                        ShowMessage("A card that fine should go to a cool guy like me.");
                }
            }
            else if (eAction == TurnInfo.ActionType.BUY_HAND)
            {
                if (rand.Next() % 3 > 0) // 2/3
                    return;

                nRandResult = rand.Next() % 3;

                if (nRandResult == 0)
                    ShowMessage("Hey I didn't know you could do that!");
                else if (nRandResult == 1)
                    ShowMessage("Ah, so that is where all the good cards are.");
                else if (nRandResult == 2)
                    ShowMessage("Pretty good move I guess....");
            }
            else if (eAction == TurnInfo.ActionType.RESERVE_DECK)
            {
                if (rand.Next() % 3 > 0) // 2/3
                    return;

                nRandResult = rand.Next() % 2;

                if (nRandResult == 0)
                    ShowMessage("Damn! Why didn't I think of that?");
                else if (nRandResult == 1)
                    ShowMessage("Ah, so that is where all the good cards are.");
            }
            else if (eAction == TurnInfo.ActionType.RESERVE_BOARD)
            {
                if (rand.Next() % 2 == 0) // 1/2
                    return;

                nRandResult = rand.Next() % 3;

                if (nRandResult == 0)
                    ShowMessage("Only an idiot would want that card.");
                else if (nRandResult == 1)
                    ShowMessage("Put that back " + sName + "!");
                else if (nRandResult == 2)
                    ShowMessage("No fair, I licked that card.");
            }
        }
        public override string Name()
        {
            return "Andrey3000";
        }
        public override string Picture()
        {
            return "FaceTest4";
        }

        public bool CompareCard(Card oCard1, Card oCard2)
        {
            if (oCard1.Points() != oCard2.Points())
                return false;

            if(oCard1.Type() != oCard2.Type())
                return false;

            if (oCard1.Cost() != oCard2.Cost())
                return false;

            return true;
        }

        public void FilterByTier(int nTier)
        {
            for (int x = 0; x < oTargets.Count(); x++)
            {
                if (oTargets[x].Y == nTier)
                {
                    oTargets.RemoveAt(x);
                    x--;
                }
            }
        }
        public void FilterByPoints()
        {
            int nHighestPoints = 0;

            // find the highest score amongst targets
            for (int nTargetIndex = 0; nTargetIndex < oTargets.Count(); nTargetIndex++)
            {
                if (oTargets[nTargetIndex].Y < 3) // cards on board
                {
                    if (CheckCardFromBoard((int)oTargets[nTargetIndex].X, (int)oTargets[nTargetIndex].Y).Points() > nHighestPoints)
                        nHighestPoints = CheckCardFromBoard((int)oTargets[nTargetIndex].X, (int)oTargets[nTargetIndex].Y).Points();
                }
                else // cards in hand
                {
                    if (CardsReservedByPlayer(GetMyID())[(int)oTargets[nTargetIndex].X].Points() > nHighestPoints)
                        nHighestPoints = CardsReservedByPlayer(GetMyID())[(int)oTargets[nTargetIndex].X].Points();
                }
            }

            // remove all cards worth less than the most valuable card
            for (int nTargetIndex = 0; nTargetIndex < oTargets.Count(); nTargetIndex++)
            {
                if (oTargets[nTargetIndex].Y < 3) // cards on board
                {
                    if (CheckCardFromBoard((int)oTargets[nTargetIndex].X, (int)oTargets[nTargetIndex].Y).Points() < nHighestPoints)
                    {
                        oTargets.RemoveAt(nTargetIndex);
                        nTargetIndex--;
                    }
                }
                else // cards in hand
                {
                    if (CardsReservedByPlayer(GetMyID())[(int)oTargets[nTargetIndex].X].Points() < nHighestPoints)
                    {
                        oTargets.RemoveAt(nTargetIndex);
                        nTargetIndex--;
                    }
                }
            }

            if (oTargets.Count() == 0)
            {
                nHighestPoints = 0;
            }
        }
        public void FilterByCanAffordNow()
        {
            if (oTargets.Count() == 1)
                return;

            Card oCardToCheck;        
            bool bCanAffordSomething = false;

            // try to  find a card I can afford
            for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
            {
                if (oTargets[nIndex].Y < 3) // cards on board
                    oCardToCheck = CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y);
                else // reserved cards
                    oCardToCheck = CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X];

                if (CanAfford(oCardToCheck) == true)
                {
                    bCanAffordSomething = true;
                    break;
                }
            }

            // remove all cards I cant afford
            if (bCanAffordSomething == true)
            {
                for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
                {
                    if (oTargets[nIndex].Y < 3) // cards on board
                        oCardToCheck = CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y);
                    else // reserved cards
                        oCardToCheck = CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X];

                    if (CanAfford(oCardToCheck) == false)
                    {
                        oTargets.RemoveAt(nIndex);
                        nIndex--;
                    }
                }
            }
        }
        public void FilterByLeastTurns()
        {
            if (oTargets.Count() == 1)
                return;

            CalculateTurnsNeeded();
            int nSmallest = 100;
            
            // find the best priority
            for (int nTier = 0; nTier < 3; nTier++)
            {
                for (int nCard = 0; nCard < CardsOnBoard(nTier) - 1; nCard++)
                {
                    if (nTurnsNeededToBuy[nCard, nTier] < nSmallest)
                        nSmallest = nTurnsNeededToBuy[nCard, nTier];
                }
            }

            // remove all cards that are worse than the best found priority
            for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
            {
                if (oTargets[nIndex].Y == 3)
                    continue;

                if (nTurnsNeededToBuy[(int)oTargets[nIndex].X, (int)oTargets[nIndex].Y] > nSmallest)
                {
                    oTargets.RemoveAt(nIndex);
                    nIndex--;
                }
            }
        }
        public void FilterByLeastCoinsNeeded()
        {
            if (oTargets.Count() == 1)
                return;

            CalculateCoinsNeeded();
            int nMinimumCost = 100;

            for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
            {
                if (nCoinsNeededToBuy[(int)oTargets[nIndex].X, (int)oTargets[nIndex].Y] < nMinimumCost)
                    nMinimumCost = nCoinsNeededToBuy[(int)oTargets[nIndex].X, (int)oTargets[nIndex].Y];
            }
          
            for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
            {
                if (nCoinsNeededToBuy[(int)oTargets[nIndex].X, (int)oTargets[nIndex].Y] > nMinimumCost)
                {
                    oTargets.RemoveAt(nIndex);
                    nIndex--;
                }
            }
        }
        public void FilterByCardsLeastOwned()
        {
            if (oTargets.Count() == 1)
                return;

            int[] nColorPriority = new int[] { 0, 1, 2, 3, 4 };
            int nEnd = 4;

            // sort priority by least cards of each type owned, least at index 0
            while (nEnd > 1)
            {
                for (int x = 0; x < nEnd; x++)
                {
                    if (CountCardsBought(GetMyID())[nColorPriority[x]] > CountCardsBought(GetMyID())[nColorPriority[x + 1]])
                    {
                        int nTemp = nColorPriority[x];
                        nColorPriority[x] = nColorPriority[x + 1];
                        nColorPriority[x + 1] = nTemp;
                    }
                }
                nEnd--;
            }

            // find best color available
            int nCount = 0;
            int nPriority = -1;
            while (nCount == 0)
            {
                nPriority++;
                for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
                {
                    if (oTargets[nIndex].Y < 3)
                    {
                        if ((int)CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y).Type() == nColorPriority[nPriority])
                            nCount++;
                    }
                    else
                    {
                        if ((int)CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X].Type() == nColorPriority[nPriority])
                            nCount++;
                    }
                }             
            }

            // remove all but best color
            for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
            {
                if (oTargets[nIndex].Y < 3)
                {
                    if ((int)CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y).Type() != nColorPriority[nPriority])
                    {
                        oTargets.RemoveAt(nIndex);
                        nIndex--;
                    }
                }
                else     
                {
                    if ((int)CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X].Type() != nColorPriority[nPriority])
                    {
                        oTargets.RemoveAt(nIndex);
                        nIndex--;
                    }
                }
            }
        }
        public void FilterByNobleAffinity()
        {
            if (oTargets.Count() == 1)
                return;
            
            CalculateNobleAffinity();
            int[] nColorPriority = new int[] { 0, 1, 2, 3, 4 };
            int nEnd = 4;
            int nMaxCardsOfAType = 4;

            // sort priority by most common noble colors, highest at 0
            while (nEnd > 1)
            {
                for (int x = 0; x < nEnd; x++)
                {
                    if (nNobleAffinity[nColorPriority[x]] < nNobleAffinity[nColorPriority[x + 1]])
                    {
                        int nTemp = nColorPriority[x];
                        nColorPriority[x] = nColorPriority[x + 1];
                        nColorPriority[x + 1] = nTemp;
                    }
                }
                nEnd--;
            }

            // find best color available
            int nCount = 0; // counts how many cards of best color found, if 0 move to next best color
            int nPriority = -1;
            while (nCount == 0 && nPriority < 4)
            {
                nPriority++;

                if (CountCardsBought(GetMyID())[nColorPriority[nPriority]] >= nMaxCardsOfAType)
                    continue;

                for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
                {
                    if (oTargets[nIndex].Y < 3)
                    {
                        if ((int)CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y).Type() == nColorPriority[nPriority])
                            nCount++;
                    }
                    else
                    {
                        if ((int)CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X].Type() == nColorPriority[nPriority])
                            nCount++;
                    }
                }
            }

            if (nCount > 0)
            {
                // remove all but best color
                for (int nIndex = 0; nIndex < oTargets.Count(); nIndex++)
                {
                    if (oTargets[nIndex].Y < 3)
                    {
                        if ((int)CheckCardFromBoard((int)oTargets[nIndex].X, (int)oTargets[nIndex].Y).Type() != nColorPriority[nPriority])
                        {
                            oTargets.RemoveAt(nIndex);
                            nIndex--;
                        }
                    }
                    else
                    {
                        if ((int)CardsReservedByPlayer(GetMyID())[(int)oTargets[nIndex].X].Type() != nColorPriority[nPriority])
                        {
                            oTargets.RemoveAt(nIndex);
                            nIndex--;
                        }
                    }
                }
            }
        }

        // helpers
        public void ResetTargets()
        {
            while (oTargets.Count() > 0)
            {
                oTargets.RemoveAt(0);
            }
        }
        public void CalculateNobleCloseness()
        {
            nCloseToNobles = new int[CountNobles(), NumberOfPlayers()];
            Noble oNoble;

            for (int nNoble = 0; nNoble < CountNobles(); nNoble++) // per noble 
            {
                oNoble = CheckNoble(nNoble);

                for (int nPlayer = 0; nPlayer < NumberOfPlayers(); nPlayer++) //per player
                {
                    for (int nColor = 0; nColor < 5; nColor++)
                    {
                        if (oNoble.Cost()[nColor] > CountCardsBought(GetMyID())[nColor])
                            nCloseToNobles[nNoble, nPlayer] += oNoble.Cost()[nColor] - CountCardsBought(GetMyID())[nColor];
                    }

                }
            }
        } // unused
        public bool CanAfford(Card oCard)
        {
            int nCost = 0;

            for (int x = 0; x < 5; x++)
                if (oCard.Cost()[x] > CountCoinsHeld(GetMyID())[x] + CountCardsBought(GetMyID())[x])
                    nCost += oCard.Cost()[x] - (CountCoinsHeld(GetMyID())[x] + CountCardsBought(GetMyID())[x]);

            if (CountCoinsHeld(GetMyID())[5] >= nCost)
                return true;
            else
                return false;
        }
        public void CalculateNobleAffinity()
        {
            Noble oNoble;
            nNobleAffinity = new int[5];

            for (int nIndex = 0; nIndex < CountNobles(); nIndex++) // per noble
            {
                if (CheckNoble(nIndex).IsAvailable() == false)
                    continue;

                oNoble = CheckNoble(nIndex);
                for (int nColor = 0; nColor < 5; nColor++) // per color
                {
                    nNobleAffinity[nColor] += oNoble.Cost()[nColor];
                }              
            }

        }
        public void CalculateCoinsNeeded()
        {
            nCoinsNeededToBuy = new int[4, 4]; // x,y
            Card oCard;
            int nEnd;

            // initialize array 
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    nCoinsNeededToBuy[x, y] = 99;

            for (int nTier = 0; nTier < 4; nTier++) // per tier
            {
                if(nTier < 3)
                    nEnd = CardsOnBoard(nTier);
                else
                    nEnd = CountCardsHeld(GetMyID());

                for (int nCard = 0; nCard < nEnd; nCard++) // per card
                {
                    if (nTier < 3)
                        oCard = CheckCardFromBoard(nCard, nTier);
                    else
                        oCard = CardsReservedByPlayer(GetMyID())[nCard];

                    nCoinsNeededToBuy[nCard, nTier] = 0;

                    for (int nColor = 0; nColor < 5; nColor++) // per color
                    {
                        if(oCard.Cost()[nColor] > CountCardsBought(GetMyID())[nColor])
                            nCoinsNeededToBuy[nCard, nTier] += oCard.Cost()[nColor] - CountCardsBought(GetMyID())[nColor];
                    }
                }
            }

            //XNACS1Base.SetBottomEchoColor(Color.Red);
            //XNACS1Base.EchoToBottomStatus("                                     AndreyCalcCoins:" + nCoinsNeededToBuy[0, 3] + "," + nCoinsNeededToBuy[1, 3] + "," 
            //                                                 + nCoinsNeededToBuy[2, 3] + "," + nCoinsNeededToBuy[3, 3] + "");
        }

        // wont consider 2 coin draws
        // 0 turns is best priority
        public void CalculateTurnsNeeded()
        {
            nTurnsNeededToBuy = new int[4, 3];
            int nMyFastest;

            for (int nY = 0; nY < 3; nY++) // per tier
            {
                for (int nX = 0; nX < CardsOnBoard(nY); nX++) // per card
                {
                    nMyFastest = TurnsNeededToBuy(nX, nY, GetMyID());

                    for (int nPlayer = 0; nPlayer < NumberOfPlayers(); nPlayer++) // per player
                    {
                        if (nPlayer == GetMyID())
                            continue;

                        if (TurnsNeededToBuy(nX, nY, nPlayer) < nMyFastest)
                            nTurnsNeededToBuy[nX, nY]++;
                    }
                }
            }
        }
        public int TurnsNeededToBuy(int nX, int nY, int nID)
        {
            int[] nOwned = new int[] { 0, 0, 0, 0, 0 };
            int[] nCost = CheckCardFromBoard(nX, nY).Cost();
            int nTurnCount = 0;

            for(int x = 0; x < nOwned.Count(); x++) // add up buying power
                nOwned[x] += CountCoinsHeld(nID)[x];
             
            for (int x = 0; x < nOwned.Count(); x++) // add up buying power
                nOwned[x] += CountCardsBought(nID)[x];

            for (int x = 0; x < nOwned.Count(); x++) // subtract total buying power from cost
                nCost[x] -= nOwned[x];

            for (int x = 0; x < nOwned.Count(); x++) // normalize costs below 0
                if (nCost[x] < 0)
                    nCost[x] = 0;

            int n1st;
            int n2nd;
            int n3rd;

            while (ArraySum(nCost) > CountCoinsHeld(nID)[5])
            {
                n1st = 0;
                n2nd = 0;
                n3rd = 0;

                for (int x = 0; x < nOwned.Count(); x++) // determine higher costs
                {
                    if (nCost[x] >= n1st)
                    {
                        n3rd = n2nd;
                        n2nd = n1st;
                        n1st = nCost[x];
                    }
                    else if (nCost[x] >= n2nd)
                    {
                        n3rd = n2nd;
                        n2nd = nCost[x];
                    }
                    else if (nCost[x] >= n3rd)
                    {
                        n3rd = nCost[x];
                    }
                }

                for (int x = 0; x < nOwned.Count(); x++)
                {
                    if (nCost[x] == n1st && n1st > 0)
                    {
                        nCost[x]--;
                        n1st = -100;
                    }
                    else if (nCost[x] == n2nd && n2nd > 0)
                    {
                        nCost[x]--;
                        n2nd = -100;
                    }
                    else if (nCost[x] == n3rd && n3rd > 0)
                    {
                        nCost[x]--;
                        n3rd = -100;
                    }
                }
                nTurnCount++;
            }
            return nTurnCount;
        }
        public int ArraySum(int[] nCost)
        {
            int nToReturn = nCost[0] + nCost[1] + nCost[2] + nCost[3] + nCost[4];
            return nToReturn;
        }
    }
}
