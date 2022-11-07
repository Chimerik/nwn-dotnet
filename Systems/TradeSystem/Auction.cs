using System;

namespace NWN.Systems
{
  public class Auction
  {
    public readonly int auctionerId;
    public readonly string serializedItem;
    public readonly string itemName;
    public readonly int startingPrice;
    public readonly int buyoutPrice;
    public DateTime expirationDate;
    public int highestBidderId;
    public int highestBid;

    public Auction(int auctionerId, string serializedItem, string itemName, DateTime expirationDate, int startingPrice = 0, int buyoutPrice = 0, int highestBidderId = -1, int highestBid = 0)
    {
      this.auctionerId = auctionerId;
      this.serializedItem = serializedItem;
      this.itemName = itemName;
      this.startingPrice = startingPrice;
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
