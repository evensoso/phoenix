
using System;
using System.Linq;

using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
  [Robot(TimeZone = TimeZones.TokyoStandardTime, AccessRights = AccessRights.None)]

  public partial class Batiboko : Robot
  {
    private void identifyExistingTrades()
    {
        foreach (var pos in Positions)
        {
            //countingPosPlus(pos);
        }
    }
    //--------------------------------------------------------------------------------------
    private void moneyCheck()
    {
        if (Account.Equity >= C * TM)
        {
            Print("Achieved Target Amount. All positions closed.");
            closeAll();
            Stop();
        }

        if (Account.Equity <= ES + 600)
        {
            protection 20000
             Print("Money protection " + (ES) + " stop triggered. All positions closed.");
            closeAll();
             Stop();
         }

        if (Account.Balance / Account.Equity < EquityStop)
        {
        Print("Equity protection stop triggered. All positions closed.");
        closeAll();
        Stop();
        }

        if (Account.Equity < Account.Balance * BalanceStop)
        {
        Print("Account balance protection triggered. All positions closed.");
        closeAll();
        Stop();
      }
    }

    private void nearshiftCheck()
    {
        //Nearshift
        foreach (var pos in Positions.FindAll(botName, Symbol))
        {
            sym = MarketData.GetSymbol(pos.SymbolCode);
            if (isNearTargetSignal())
            {
                CalcNewValues();
                AdjustPos();
            }
        }
    }
    private void trailCheck()
    {
        foreach (var pos in Positions.FindAll(botName, Symbol))
        {
            if (pos.Pips > Trailstart)
            {
                Print("Trailstart");
                // true:false
                double actualPrice = isBuy(pos) ? Symbol.Bid : Symbol.Ask;
                int factor = isBuy(pos) ? 1 : -1;
                double? newStopLossPR = pos.StopLoss;

                // Stop a ZERO
                if ((pos.EntryPrice - newStopLossPR) * factor > 0)
                    newStopLossPR = pos.EntryPrice;
                //Trail
                if ((actualPrice - newStopLossPR) * factor > Trail * Symbol.PipSize)
                {
                    newStopLossPR += factor * Trail * Symbol.PipSize;
                    if (newStopLossPR != pos.StopLoss)
                        ModifyPosition(pos, newStopLossPR, pos.TakeProfit.Value);
                }
            }
        }
    }
    //-------------------------------------------------------------------------------------------
    private void filtercheck()
    {
        //Filter
        if (TimeFilter)
        {
            if (BarCount <= TradeDelayBC)
                tradeSafe = false;

            if (isNotTradingTime())
                tradeSafe = false;
        }
        if (ChaosFilterOn)
        {
            if (isChaosSignal())
            {
                CBarCount = 0;
                Print("isChaosSignal CBarCount=0");
            }

            if (CBarCount <= ChaosWaitBC)
                tradeSafe = false;
        }

        if (TradeDelayTFilterOn)
        {
            if (TradeCount < TradeDelayTC)
            {
                tradeSafe = false;
            }
        }

        if (PibotFilterOn)
        {
            if (isPibotLiskSignal())
            {
                tradeSafe = false;

                if (FsB > Symbol.Bid)
                {
                    close(TradeType.Buy, Volume);
                }

                else if (FrB < Symbol.Ask)
                {
                    close(TradeType.Sell, Volume);
                }
            }
            if (isPibotSignal())
            {
            }
        }
    }

    //-------------------------------------------------------------------------------------
    private void CalcNewValues()
    {
        if (pos.TradeType == TradeType.Buy)
        {
            newStopLossPR = Symbol.Bid - (Symbol.PipSize * TrailSLPips);
            newTakeProfitPR = Symbol.Bid + (Symbol.PipSize * TrailSLPips);
        }
        else
        {
            newStopLossPR = Symbol.Ask + (Symbol.PipSize * TrailSLPips);
            newTakeProfitPR = Symbol.Ask - (Symbol.PipSize * TrailSLPips);
        }
    }
    private void AdjustPos()
    {
        if (newStopLossPR == 0 || newTakeProfitPR == 0)
            return;
        ModifyPosition(pos, newStopLossPR, newTakeProfitPR);
    }
    //-------------------------------------------------------------------------
    private TradeType GetRandomTradeType()
    {
        return rand.Next(2) == 0 ? TradeType.Buy : TradeType.Sell;
    },

    //-------------------------------------------------------------------------
    //counting
    private void countingPosPlus(Position pos)
    {
        switch (pos.TradeType)
        {
            case TradeType.Buy:
                LongPositions++;
                break;
            case TradeType.Sell:
                ShortPositions++;
                break;
        }
    }
    private void countingPosMinus(Position pos)
    {
        switch (pos.TradeType)
        {
            case TradeType.Buy:
                LongPositions--;
                break;
            case TradeType.Sell:
                ShortPositions--;
                break;
        }
    }
    private void countingMartingale(Position pos)
    {
        if (pos.GrossProfit < 0)
        {
            MartingaleActive++;
            Volume = Volume * 2;
        }

        else if (MartingaleActive > 0)
        {
            MartingaleActive--;
            Volume = (int)(Volume / 2);
            //update
        }
    }

  }
}
