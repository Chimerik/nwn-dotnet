using System;

namespace NWN.Systems.TradeSystem
{
  public class BuyOrder
  {
    public readonly int buyerId;
    public readonly ResourceType resourceType;
    public readonly int resourceLevel;
    public readonly int quantity;
    public readonly int unitPrice;
    public DateTime expirationDate;
    

    public BuyOrder(int buyerId, ResourceType resourceType, int resourceLevel, int quantity, DateTime expirationDate, int unitPrice)
    {
      this.buyerId = buyerId;
      this.resourceType = resourceType;
      this.resourceLevel = resourceLevel;
      this.quantity = quantity;
      this.expirationDate = expirationDate;
      this.unitPrice = unitPrice;
    }
    public BuyOrder()
    {

    }
    public int GetTotalCost() { return quantity * unitPrice; }

  }
}
