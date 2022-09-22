
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

    private bool isRSIBuySignal()
    {
        if ((RSI.Result.Last(1) < RSI.Result.Last(0)) && (RSI.Result.Last(1) == RSIlow))
        {
            //Print("isRSIBuySignal");
            type = "STOBuy";
            return true;
        }
        return false;
    }
    private bool isRSIBuyCloseSignal()
    {
        if (RSI.Result.LastValue == RSIhigh)
        {
            return true;
        }
        return false;
    }
    private bool isRSISellSignal()
    {
        if ((RSI.Result.Last(1) > RSI.Result.Last(0)) && (RSI.Result.Last(1) == RSIhigh))
        {
            //Print("isRSISellSignal");
            type = "STOBuy";
            return true;
        }
        return false;
    }
    private bool isRSISellCloseSignal()
    {
        if (RSI.Result.LastValue == RSIlow)
        {
            return true;
        }
        return false;
    }
    private bool isSTOBuySignal()
    {
        if ((STO.PercentD.Last(1) < STOlow) && (Functions.IsRising(STO4.PercentD)) && (Functions.IsRising(STO.PercentD)))
        {
            if ((Functions.Minimum(STO.PercentD, SC * 2) < STOlow) && (Functions.Minimum(STO.PercentD, SC * 2)) < (Functions.Minimum(STO.PercentD, SC)))
            {
                if (Functions.IsFalling(SMAH48.Result))
                {
                    //Print("isSTOBuySignal3");
                    type = "STOBuy Divergence(SMA-STO)";
                    return true;
                }
                //Print("isSTOBuySignal2");
                type = "STOBuy Divergence(STO)";
                return true;
            }
            //Print("isSTOBuySignal1");
            type = "STOBuy";
            return true;
        }
        return false;
    }
    private bool isSTOBuyCloseSignal()
    {
        if (STO4.PercentD.LastValue == STOhigh)
        {
            return true;
        }
        return false;
    }
    private bool isSTOSellSignal()
    {
        if ((STO.PercentD.Last(1) > STOhigh) && (Functions.IsFalling(STO4.PercentD)) && (Functions.IsFalling(STO.PercentD)))
        {
            if ((Functions.Maximum(STO.PercentD, SC * 2) > STOhigh) && (Functions.Maximum(STO.PercentD, SC * 2)) > (Functions.Maximum(STO.PercentD, SC)))
            {
                if (Functions.IsFalling(SMAH48.Result))
                {
                    //Print("isSTOSellSignal3");
                    type = "STOSell Divergence(SMA-STO)";
                    return true;
                }
                //Print("isSTOSellSignal2");
                type = "STOSell Divergence(STO)";
                return true;
            }
            //Print("isSTOSellSignal1");
            type = "STOSell";
            return true;
        }
        return false;
    }
    private bool isSTOSellCloseSignal()
    {
        if (STO4.PercentD.LastValue == STOlow)
        {
            return true;
        }
        return false;
    }
    //-----------------------------------------------------------------------------------------------------------------------------------
    private bool isSMABuySignal()
    {
        if ((SMAH48.Result.Last(1) < SMAH48.Result.Last(0)) && (SMAH48.Result.Last(1) == Symbol.Ask) && (Functions.IsRising(SMAD21.Result)))
        {
            if (SMAH48.Result.Last(2) < Symbol.Ask && SMAH48.Result.Last(0) < Symbol.Ask && Time.Hour > 14 && Time.Hour < 18)
            {
                //Print("isSMABuySignal2");
                type = "SMABuy Touch";
                return true;
            }
            //Print("isSMABuySignal1");
            type = "SMABuy CrossUp";
            return false;
        }
        return false;
    }
    private bool isSMABuyCloseSignal()
    {
        if (Functions.IsFalling(SMAH48.Result))
        {
            return true;
        }
        return false;
    }
    private bool isSMASellSignal()
    {
        if ((SMAH48.Result.Last(1) > SMAH48.Result.Last(0)) && (SMAH48.Result.Last(1) == Symbol.Bid) && (Functions.IsFalling(SMAD21.Result)))
        {
            if (SMAH48.Result.Last(2) > Symbol.Bid && SMAH48.Result.Last(0) > Symbol.Bid && Time.Hour > 14 && Time.Hour < 18)
            {
                //Print("isSMASellSignal2");
                type = "SMASell Touch";
                return true;
            }
            //Print("isSMASellSignal1");
            type = "SMABuySell CrossDown";
            return false;
        }
        return false;
    }
    private bool isSMASellCloseSignal()
    {
        if (Functions.IsRising(SMAH48.Result))
        {
            return true;
        }
        return false;
    }
    private bool isMACDBuySignal()
    {
        if (MACD4.Histogram.LastValue < MACDlow)
        {
            if ((MACD4.Histogram.Last(2) > MACD4.Histogram.Last(1)) && (MACD4.Histogram.Last(1) < MACD4.Histogram.Last(0)))
            {
                if (Functions.Minimum(MACD4.Histogram, MC * 4) > Functions.Minimum(MACD4.Histogram, MC))
                {
                    if (Functions.IsFalling(SMAH48.Result))
                    {
                        //Print("isMACDBuySignal3");
                        type = "MACD Divergence(SMA-MACD)";
                        return true;
                    }
                    //Print("isMACDBuySignal2");
                    type = "MACD Divergence(MACD)";
                    return true;
                }
                //Print("isMACDBuySignal1");
                type = " MACD";
                return true;
            }
            return false;
        }
        return false;
    }
    private bool isMACDBuyCloseSignal()
    {
        if ((MACD4.Histogram.Last(2) > MACD4.Histogram.Last(1)) && (MACD4.Histogram.Last(1) > MACD4.Histogram.Last(0)))
        {
            return true;
        }
        return false;
    }
    private bool isMACDSellSignal()
    {
        if (MACD4.Histogram.LastValue > MACDhigh)
        {
            if ((MACD4.Histogram.Last(2) < MACD4.Histogram.Last(1)) && (MACD4.Histogram.Last(1) > MACD4.Histogram.Last(0)))
            {
                if (Functions.Maximum(MACD4.Histogram, MC * 4) > Functions.Maximum(MACD4.Histogram, MC))
                {
                    if (Functions.IsRising(SMAH48.Result))
                    {
                        //Print("isMACDSellSignal3");
                        type = "MACD Divergence(SMA-MACD)";
                        return true;
                    }
                    //Print("isMACDSellSignal2");
                    type = "MACD Divergence(MACD)";
                    return true;
                }
                //Print("isMACDSellSignal1");
                type = " MACD";
                return true;
            }
            return false;
        }
        return false;
    }
    private bool isMACDSellCloseSignal()
    {
        if ((MACD4.Histogram.Last(2) < MACD4.Histogram.Last(1)) && (MACD4.Histogram.Last(1) < MACD4.Histogram.Last(0)))
        {
            return true;
        }
        return false;
    }
    //--------------------------------------------------------
    private bool isBBBuySignal()
    {
        if (top - bottom <= BHPI * Symbol.PipSize)
        {
            c = c + 1;
        }
        else
        {
            c = 0;
        }
        if (c >= CPeriods)
        {
            if (Symbol.Bid < bottom)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    private bool isBBSellSignal()
    {
        if (top - bottom <= BHPI * Symbol.PipSize)
        {
            c = c + 1;
        }
        else
        {
            c = 0;
        }

        if (c >= CPeriods)
        {
            if (Symbol.Ask > top)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    //-------------------------------------------------------------
    private bool isPibotLiskSignal()
    {
        if ((FsB > Symbol.Bid) || (FrB < Symbol.Ask))
        {
            Print("isPibotLiskSignal");
            return true;
        }
        return false;
    }
    private bool isPibotSignal()
    {
        if ((pivot == Symbol.Bid) || (Wpivot == Symbol.Bid) || (pivot == Symbol.Ask) || (Wpivot == Symbol.Ask))
        {
            Print("isPibotSignal");
            return true;
        }
        return false;
    }
    /*private bool isStartSignal()
    {
        //(HL.Result.LastValue > 0.0006) && Tick 260
        if (M1Series.TickVolume.LastValue > 260)
        {
            Print("isStartSignal");
            return true;
        }
        return false;
    }*/
    private bool isChaosSignal()
    {
        //(HL.Result.LastValue > 0.002) && TickM1 400
        if (MarketSeries.TickVolume.LastValue > 3000)
        {
            return true;
        }
        return false;
    }

    private bool isNearTargetSignal()
    {
        if (pos.TradeType == TradeType.Buy)
        {
            if (sym.Bid > pos.TakeProfit - (sym.PipSize * TriggerPips))
                Print("isNearTargetSignal");
            return true;
        }
        else if (pos.TradeType == TradeType.Sell)
        {
            if (sym.Ask < pos.TakeProfit + (sym.PipSize * TriggerPips))
                Print("isNearTargetSignal");
            return true;
        }
        return false;
    }

    private bool isNotTradingTime()
    {
        if ((Time.Hour >= 3 && Time.Hour <= 8))
        {
            //Print("isNotTradingTime");
            return true;
        }

        return false;
    }

  }
}
