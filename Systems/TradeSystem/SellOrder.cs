using System;

namespace NWN.Systems
{
  public class SellOrder
  {
    public readonly int sellerId;
    public readonly ResourceType resourceType;
    public readonly int resourceLevel;
    public int quantity;
    public readonly int unitPrice;
    public DateTime expirationDate;


    public SellOrder(int sellerId, ResourceType resourceType, int resourceLevel, int quantity, DateTime expirationDate, int unitPrice)
    {
      this.sellerId = sellerId;
      this.resourceType = resourceType;
      this.resourceLevel = resourceLevel;
      this.quantity = quantity;
      this.expirationDate = expirationDate;
      this.unitPrice = unitPrice;
    }
    public SellOrder()
    {

    }
    public int GetTotalCost() { return quantity * unitPrice; }

  }
}
