
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
    //cBot Action
    private void open(TradeType tradeType, int Volume)
    {
        if (tradeType == TradeType.Sell)
        {
            randomNumber = rand.Next(1, 100);

            if (randomNumber <= SP)
            {
                TradeCount++;

                if (TradeCount == TradeDelayTC)
                {
                    Print("TradeCount = TradeDelayTC(" + TradeDelayBC + ")");
                }
                if (tradeSafe == true)
                {
                    botName = type + "-" + "Batiboko" + "-" + MarketSeries.TimeFrame;

                    ExecuteMarketOrder(tradeType, Symbol, Volume, botName, StopLossPI, TakeProfitPI, SlippagePI, type);
                }
            }
            else if (randomNumber > SP)
            {
                return;
            }
        }
        else if (tradeType == TradeType.Buy)
        {
            randomNumber = rand.Next(1, 100);

            if (randomNumber <= BP)
            {
                TradeCount++;

                if (TradeCount == TradeDelayTC)
                {
                    Print("TradeCount = TradeDelayTC(" + TradeDelayBC + ")");
                }
                if (tradeSafe == true)
                {
                    botName = type + "-" + "Batiboko" + "-" + MarketSeries.TimeFrame;

                    ExecuteMarketOrder(tradeType, Symbol, Volume, botName, StopLossPI, TakeProfitPI, SlippagePI, type);
                }
            }
            else if (randomNumber > BP)
            {
                return;
            }
        }

    }

    //---------------------------------------------------------------------------------
    private void close(TradeType tradeType, int Volume)
    {
        foreach (var position in Positions.FindAll(botName, Symbol, tradeType))
        {
            ClosePosition(position, Volume);
            return;
        }
    }
    private void closeAll()
    {
        close(TradeType.Buy, Volume);
        close(TradeType.Sell, Volume);
    }

    //--------------------------------------------------------------
    private void openPosActionOnBar()
    {

        if (isBBSellSignal())
        {
            type = "BB";
            open(TradeType.Sell, Volume);
            c = 0;
        }
        else if (isBBBuySignal())
        {
            type = "BB";
            open(TradeType.Buy, Volume);
            c = 0;
        }
    }

    private void openPosActionOnTick()
    {
        if (buySafe == true)
        {
            //Range
            if (isSTOBuySignal() || isRSIBuySignal())
            {
                //type = "Range";
                open(TradeType.Buy, Volume);
            }
            //Trend
            if (isSMABuySignal() || isMACDBuySignal())
            {
                type = "Trend";
                //open(TradeType.Buy, Volume);
            }
        }
        if (sellSafe == true)
        {
            //Range
            if (isSTOSellSignal() || isRSISellSignal())
            {
                //type = "Range";
                open(TradeType.Sell, Volume);
            }
            //Trend
            if (isSMASellSignal() || isMACDSellSignal())
            {
                type = "Trend";
                //open(TradeType.Sell, Volume);
            }
        }
    }

    private void closePosActionOnTick()
    {
        //Range
        if (isSTOBuyCloseSignal() && isRSIBuyCloseSignal())
        {
            close(TradeType.Sell, Volume);
        }
        //Trend
        if (isSMABuyCloseSignal() && isMACDBuyCloseSignal())
        {
            close(TradeType.Sell, Volume);
        }
        //Range
        if (isSTOSellCloseSignal() && isRSISellCloseSignal())
        {
            close(TradeType.Buy, Volume);
        }
        //Trend
        if (isSMASellCloseSignal() && isMACDSellCloseSignal())
        {
            close(TradeType.Buy, Volume);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------

    private void onPositionsOpened(PositionOpenedEventArgs args)
    {
        var pos = args.Position;

        if (pos.Label != botName)
            return;
        else
        {
            //countingPosPlus(pos);
        }
        Print("(Open) c={0}, BarCount={1}, TradeCount={2}, CBarCount={3}", c, BarCount, TradeCount, CBarCount);
        BarCount = 0;
        Print("(Execute) BarCount=0");
    }

    private void onPositionsClosed(PositionClosedEventArgs args)
    {
        var pos = args.Position;

        if (pos.Label != botName)
            return;

        Print("(Close) c={0}, BarCount={1}, TradeCount={2}, CBarCount={3}", c, BarCount, TradeCount, CBarCount);

        //countingPosMinus(pos);

        if (MartingaleOn)
        {
            //countingMartingale(pos);
        }

        if (pos.GrossProfit < 0)
        {
            BarCount = 0;
            TradeCount = 0;

            Print("(Close pos.GrossProfit = {0} < 0) BarCount=0 TradeCount=0", pos.GrossProfit);

        }
        else if (pos.GrossProfit >= 0)
        {
            Print("(Close pos.GrossProfit = {0} >=0)", pos.GrossProfit);
        }
    }
  }
}
