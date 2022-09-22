/*   Parameter    =
     Symbole      = GBPUSD
     Timeframe    = m15
     Source       = Open
     Volume       = 2000
     TakeProfit   = 300
     StopLoss     = 100
     TrailStart   = 29
     Trail        = 3
     Chaos Wait (bars)
     Trade Delay (bars)
     Trade Probability(Buy%)
     Trade Probability(Sell%)

     Results :
     Resultats         = 01/04/2011 - 17/7/2014
     gain              = 5303 euros(+11%).
     Net profit        = 5303,01 euros
     Ending Equity     = 5303,01 euros
     Ratio de Sharpe   = 0.13
     Ratio de Storino  = 0.17*/

//cBot for GBPUSD - M15 for H1

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

    public class Batiboko : Robot
    {
        //Prameters
        //Main
        [Parameter("Source")]
        public DataSeries Source { get; set; }
        [Parameter("Volume", DefaultValue = 2000, MinValue = 1000, Step = 1000)]
        public int Volume { get; set; }
        [Parameter("TakeProfit(pips)", DefaultValue = 300, MinValue = 1, Step = 10)]
        public int TakeProfitPI { get; set; }
        [Parameter("Stop Loss (pips)", DefaultValue = 100, MinValue = 1, Step = 10)]
        public int StopLossPI { get; set; }
        [Parameter("Max Slippage (pips)", DefaultValue = 0.2, MinValue = 0)]
        public double SlippagePI { get; set; }
        [Parameter("EquityStop (Equity)", DefaultValue = 20000)]
        public int ES { get; set; }
        [Parameter("Compression Rario", DefaultValue = 4)]
        public int CR { get; set; }
        [Parameter("Capital", DefaultValue = 200000)]
        public int C { get; set; }
        [Parameter("target magnification", DefaultValue = 3)]
        public int TM { get; set; }

        //STO
        [Parameter("K Periods", DefaultValue = 5)]
        public int KPeriods { get; set; }
        [Parameter("D Periods", DefaultValue = 3)]
        public int DPeriods { get; set; }
        [Parameter("K Slowing", DefaultValue = 3)]
        public int KSlowing { get; set; }
        //for divergence
        [Parameter("STOcycle", DefaultValue = 6)]
        public int SC { get; set; }
        //RSI
        [Parameter("Periods RSI", DefaultValue = 14)]
        public int PeriodsRSI { get; set; }
        //SMA
        [Parameter("Periods SMA-H48", DefaultValue = 48)]
        public int PeriodsSMAH48 { get; set; }
        //MACD
        [Parameter("Periods MACD", DefaultValue = 9)]
        public int PeriodsMACD { get; set; }
        [Parameter("ShortCycle MACD", DefaultValue = 12)]
        public int ShortCycleMACD { get; set; }
        [Parameter("LongCycle MACD", DefaultValue = 26)]
        public int LongCycleMACD { get; set; }
        //for divergence
        [Parameter("MACDcycle", DefaultValue = 20)]
        public int MC { get; set; }
        //BB
        [Parameter("Bollinger Bands Periods", DefaultValue = 21)]
        public int BBPeriods { get; set; }
        [Parameter("Band Height (pips)", DefaultValue = 40.0, MinValue = 0)]
        public double BHPI { get; set; }
        [Parameter("Consolidation Periods", DefaultValue = 2)]
        public int CPeriods { get; set; }

        [Parameter("Max Long Positions", DefaultValue = 2, MinValue = 0)]
        public int MaxLongP { get; set; }
        [Parameter("Max Short Positions", DefaultValue = 2, MinValue = 0)]
        public int MaxShortP { get; set; }
        [Parameter("Trade Delay Count (bars)", DefaultValue = 10, MinValue = 1)]
        public int TradeDelayBC { get; set; }
        [Parameter("Trade Delay Count (trade entry times)", DefaultValue = 10, MinValue = 1)]
        public int TradeDelayTC { get; set; }
        [Parameter("Chaos Wait (bars)", DefaultValue = 8, MinValue = 2)]
        public int ChaosWaitBC { get; set; }

        [Parameter("Time Filter(AM4-AM8)", DefaultValue = true)]
        public bool TimeFilter { get; set; }
        [Parameter("Trade Delay (bars)", DefaultValue = true)]
        public bool TradeDelayBFilterOn { get; set; }
        [Parameter("Trade Delay (entry times)", DefaultValue = true)]
        public bool TradeDelayTFilterOn { get; set; }
        [Parameter("Wait on Loss (bars)", DefaultValue = 0, MinValue = 0)]
        public int WaitOnLoss { get; set; }

        [Parameter("Chaos Filter", DefaultValue = false)]
        public bool ChaosFilterOn { get; set; }
        [Parameter("Pibot Filter", DefaultValue = false)]
        public bool PibotFilterOn { get; set; }

        //---------------------------------------------------------------------------------------------

        [Parameter("Periods SMA-M720", DefaultValue = 144)]
        public int PeriodsSMAM720 { get; set; }
        [Parameter("Periods SMA-D21", DefaultValue = 21)]
        public int PeriodsSMAD21 { get; set; }
        [Parameter("Periods SMA-D200", DefaultValue = 200)]
        public int PeriodsSMAD200 { get; set; }

        [Parameter("Close All on Bot Stop", DefaultValue = false)]
        public bool CloseOnStopOn { get; set; }
        [Parameter("OCO Mode", DefaultValue = false)]
        public bool OCOModeOn { get; set; }
        [Parameter("MartingaleOn", DefaultValue = false)]
        public bool MartingaleOn { get; set; }

        [Parameter("Trade Probability(Buy%)", DefaultValue = 100, MinValue = 0, MaxValue = 100)]
        public int BP { get; set; }
        [Parameter("Trade Probability(Sell%)", DefaultValue = 80, MinValue = 0, MaxValue = 100)]
        public int SP { get; set; }

        [Parameter("Expansion", DefaultValue = 3)]
        public int ER { get; set; }

        //Trail
        [Parameter("Trail start(pips)", DefaultValue = 29, MinValue = 1)]
        public int Trailstart { get; set; }
        [Parameter("Trail", DefaultValue = 3, MinValue = 1)]
        public int Trail { get; set; }
        //near Trail
        [Parameter("Trigger (pips)", DefaultValue = 2.0, MinValue = 0)]
        public double TriggerPips { get; set; }
        [Parameter("Trail SL (pips)", DefaultValue = 10.0, MinValue = 0)]
        public double TrailSLPips { get; set; }
        [Parameter("Trail TP (pips)", DefaultValue = 5.0, MinValue = 0)]
        public double TrailTPPips { get; set; }

        [Parameter("EquityStop (Balance/Equity)", DefaultValue = 2)]
        public int EquityStop { get; set; }
        [Parameter("Balance Stop Loss (ratio)", DefaultValue = 0.2, MinValue = 0, MaxValue = 1)]
        public double BalanceStop { get; set; }

        [Parameter("Trade Biasing", DefaultValue = 3, MinValue = 0, MaxValue = 3)]
        public int Bias { get; set; }

        [Parameter("Number", DefaultValue = 1, MinValue = 1)]
        public int Number { get; set; }
        [Parameter("Max Tries", DefaultValue = 20, MinValue = 2)]
        public double MaxTries { get; set; }

        private Random rand = new Random();
        //--------------------------------------------------------------------------------
        // Globals
        private StochasticOscillator STO;
        private StochasticOscillator STO4;
        private RelativeStrengthIndex RSI;

        private SimpleMovingAverage SMAH48;
        private SimpleMovingAverage SMAD21;
        private MacdHistogram MACD4;
        private BollingerBands BB;

        //private HighMinusLow HL;
        //private MarketSeries M1Series;

        private Symbol sym = null;
        private Position pos = null;

        //defination
        private const int STOhigh = 80, STOlow = 20, RSIhigh = 70, RSIlow = 30;
        private const double MACDhigh = 0.0006, MACDlow = -0.0006;

        private string botName, type;
        private double openingBalance = 0;
        private bool tradeSafe = true, buySafe = true, sellSafe = true;
        private double? newStopLossPR = 0, newTakeProfitPR = 0;

        private int randomNumber;

        private double highp, lowp, closep;
        private double pivot, Wpivot, FrB, FsB, Wr3, Ws3, top, bottom;
        private int LongPositions = 0, ShortPositions = 0, MartingaleActive = 0, c = 0, BarCount, CBarCount, TradeCount;
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnStart()
        {
            Print("Initializing...");

            BarCount = TradeDelayBC;
            TradeCount = TradeDelayTC;
            CBarCount = ChaosWaitBC;

            openingBalance = Account.Balance;

            STO = Indicators.StochasticOscillator(KPeriods, KSlowing, DPeriods, MovingAverageType.Exponential);
            STO4 = Indicators.StochasticOscillator(KPeriods * CR, KSlowing * CR, DPeriods * CR, MovingAverageType.Exponential);
            RSI = Indicators.RelativeStrengthIndex(Source, PeriodsRSI);
            SMAH48 = Indicators.SimpleMovingAverage(Source, 48 * CR);
            SMAD21 = Indicators.SimpleMovingAverage(Source, 21 * (CR * 24));
            MACD4 = Indicators.MacdHistogram(LongCycleMACD * CR, ShortCycleMACD * CR, PeriodsMACD * CR);

            BB = Indicators.BollingerBands(Source, BBPeriods * CR, 2, MovingAverageType.Simple);
            top = BB.Top.Last(1);
            bottom = BB.Bottom.Last(1);

            //HL = Indicators.HighMinusLow(M1Series);
            //M1Series = MarketData.GetSeries(TimeFrame.Minute);

            highp = MarketSeries.High.LastValue;
            lowp = MarketSeries.Low.LastValue;
            closep = MarketSeries.Close.LastValue;

            pivot = (highp + lowp + closep) / 3;
            Wpivot = (highp + lowp + (2 * closep)) / 4;
            FrB = pivot + (highp - lowp) * 1.382;
            FsB = pivot - (highp - lowp) * 1.382;
            Wr3 = Wpivot + 2 * (highp - lowp);
            Ws3 = Wpivot - 2 * (highp - lowp);

            Positions.Opened += onPositionsOpened;
            Positions.Closed += onPositionsClosed;

            //identifyExistingTrades();

            Print("Initializing Completed!");
        }
        protected override void OnStop()
        {
            if (CloseOnStopOn)
                closeAll();
        }
        //------------------------------------------------------------
        protected override void OnBar()
        {
            BarCount++;
            CBarCount++;

            if (BarCount == TradeDelayBC)
            {
                Print("Barcount = TradeDelayBC(" + TradeDelayBC + ")");
            }
            openPosActionOnBar();
        }

        protected override void OnTick()
        {
            tradeSafe = true;
            buySafe = true;
            sellSafe = true;

            moneyCheck();
            //nearshiftCheck();
            //trailCheck();

            filtercheck();

            closePosActionOnTick();
            openPosActionOnTick();
        }
        //------------------------------------------------------------
        //------------------------------------------------------------
        //------------------------------------------------------------
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
            //if (Account.Equity >= C * TM)
            //{
            //    Print("Achieved Target Amount. All positions closed.");
            //    closeAll();
            //    Stop();
            //}

            //}


            //if (Account.Equity <= ES + 600)
            //{
            //    //protection 20000
            //     Print("Money protection " + (ES) + " stop triggered. All positions closed.");
            //    closeAll();
            //     Stop();
            // }

            //if (Account.Balance / Account.Equity < EquityStop)
            //{
            //Print("Equity protection stop triggered. All positions closed.");
            //closeAll();
            //Stop();
            //}
            //if (Account.Equity < Account.Balance * BalanceStop)
            //{
            //Print("Account balance protection triggered. All positions closed.");
            //closeAll();
            //Stop();
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
        //--------------------------------------------------------------------------------------------------------------------
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
        }
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //cBot Signal
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
        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------
        //cBot Positions
        private bool isBuy(Position position)
        {
            return TradeType.Buy == position.TradeType;
        }
        private bool isSell(Position position)
        {
            return TradeType.Sell == position.TradeType;
        }
        private bool isBuyPositions
        {
            get { return Positions.Find(botName, Symbol, TradeType.Buy) != null; }
        }
        private bool isSellPositions
        {
            get { return Positions.Find(botName, Symbol, TradeType.Sell) != null; }
        }
        private bool isAnyPosition
        {
            get { return isBuyPositions || isSellPositions; }
        }
        private bool isNoPosition
        {
            get { return !isAnyPosition; }
        }
    }
}
