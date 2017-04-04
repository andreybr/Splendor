using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor
{
    public class Stats
    {
        public int nNumberOfPlayers;
        public int nCurrentPlayer;
        public int nCurrentRound;
        public int nMaxCoinsHeld;
        public int nNobleCount;
        public int nMaxGoldCoins;
        public int nMaxRegularCoins;

        public Stats(int nNewNumberOfPlayers)
        {
            nMaxCoinsHeld = 10;
            nNumberOfPlayers = nNewNumberOfPlayers;        
            nNobleCount = nNumberOfPlayers + 1;
            nMaxGoldCoins = 5;

            if (nNumberOfPlayers == 1)
                nMaxRegularCoins = 4;
            else if (nNumberOfPlayers == 2)
                nMaxRegularCoins = 4;
            else if (nNumberOfPlayers == 3)
                nMaxRegularCoins = 5;
            else if (nNumberOfPlayers == 4)
                nMaxRegularCoins = 7;
        }

        public void AdvanceTurn()
        {         
            nCurrentPlayer++;

            if (nCurrentPlayer > (nNumberOfPlayers - 1))
            {
                nCurrentPlayer = 0;
                nCurrentRound++;
            }
        }
    }
}
