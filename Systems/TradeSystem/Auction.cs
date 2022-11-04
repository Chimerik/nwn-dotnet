using System;

namespace NWN.Systems.TradeSystem
{
  public class Auction
  {
    public readonly int auctionerId;
    public readonly string serializedItem;
    public readonly string itemName;
    public readonly int buyoutPrice;
    public DateTime expirationDate;
    public int highestBidderId;
    public int highestBid;

    public Auction(int auctionerId, string serializedItem, string itemName, int buyoutPrice, DateTime expirationDate, int highestBidderId, int highestBid)
    {
      this.auctionerId = auctionerId;
      this.serializedItem = serializedItem;
      this.itemName = itemName;
      this.buyoutPrice = buyoutPrice;
      this.expirationDate = expirationDate;
      this.highestBidderId = highestBidderId;
      this.highestBid = highestBid;
    }
    public Auction()
    {

    }
  }
}
