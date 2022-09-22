
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

    //状態
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
