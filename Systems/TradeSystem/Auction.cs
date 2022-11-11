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
    public Auction(SerializableAuction serializedAuction)
    {
      this.auctionerId = serializedAuction.auctionerId;
      this.serializedItem = serializedAuction.serializedItem;
      this.itemName = serializedAuction.itemName;
      this.startingPrice = serializedAuction.startingPrice;
      this.expirationDate = serializedAuction.expirationDate;
      this.buyoutPrice = serializedAuction.buyoutPrice;
      this.highestBidderId = serializedAuction.highestBidderId;
      this.highestBid = serializedAuction.highestBid;
    }

    public class SerializableAuction
    {
      public int auctionerId { get; set; }
      public string serializedItem { get; set; }
      public string itemName { get; set; }
      public int startingPrice { get; set; }
      public int buyoutPrice { get; set; }
      public int highestBidderId { get; set; }
      public int highestBid { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableAuction()
      {

      }
      public SerializableAuction(Auction AuctionBase)
      {
        auctionerId = AuctionBase.auctionerId;
        serializedItem = AuctionBase.serializedItem;
        itemName = AuctionBase.itemName;
        startingPrice = AuctionBase.startingPrice;
        buyoutPrice = AuctionBase.buyoutPrice;
        expirationDate = AuctionBase.expirationDate;
        highestBidderId = AuctionBase.highestBidderId;
        highestBid = AuctionBase.highestBid;
      }
    }
  }
}
