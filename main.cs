
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
      /*
      [Parameter("OCO Mode", DefaultValue = false)]
      public bool OCOModeOn { get; set; }
      [Parameter("MartingaleOn", DefaultValue = false)]
      public bool MartingaleOn { get; set; }
      */

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
      /*
      [Parameter("Trigger (pips)", DefaultValue = 2.0, MinValue = 0)]
      public double TriggerPips { get; set; }
      [Parameter("Trail SL (pips)", DefaultValue = 10.0, MinValue = 0)]
      public double TrailSLPips { get; set; }
      [Parameter("Trail TP (pips)", DefaultValue = 5.0, MinValue = 0)]
      public double TrailTPPips { get; set; }
      */

      [Parameter("EquityStop (Balance/Equity)", DefaultValue = 2)]
      public int EquityStop { get; set; }
      [Parameter("Balance Stop Loss (ratio)", DefaultValue = 0.2, MinValue = 0, MaxValue = 1)]
      public double BalanceStop { get; set; }

      /*
      [Parameter("Trade Biasing", DefaultValue = 3, MinValue = 0, MaxValue = 3)]
      public int Bias { get; set; }
      */

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

      //--------------------------------------------
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

  }
}
