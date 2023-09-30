using System;

namespace NWN.Systems
{
  public class BuyOrder
  {
    public readonly int buyerId;
    public readonly ResourceType resourceType;
    public int quantity;
    public readonly int unitPrice;
    public DateTime expirationDate;
    

    public BuyOrder(int buyerId, ResourceType resourceType, int quantity, DateTime expirationDate, int unitPrice)
    {
      this.buyerId = buyerId;
      this.resourceType = resourceType;
      this.quantity = quantity;
      this.expirationDate = expirationDate;
      this.unitPrice = unitPrice;
    }
    public BuyOrder()
    {

    }
    public BuyOrder(SerializableBuyOrder serializedBuyOrder)
    {
      this.buyerId = serializedBuyOrder.buyerId;
      this.resourceType = (ResourceType)serializedBuyOrder.resourceType;
      this.quantity = serializedBuyOrder.quantity;
      this.expirationDate = serializedBuyOrder.expirationDate;
      this.unitPrice = serializedBuyOrder.unitPrice;
    }

    public class SerializableBuyOrder
    {
      public int buyerId { get; set; }
      public int resourceType { get; set; }
      public int quantity { get; set; }
      public int unitPrice { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableBuyOrder()
      {

      }
      public SerializableBuyOrder(BuyOrder BuyOrderBase)
      {
        buyerId = BuyOrderBase.buyerId;
        resourceType = (int)BuyOrderBase.resourceType;
        quantity = BuyOrderBase.quantity;
        unitPrice = BuyOrderBase.unitPrice;
        expirationDate = BuyOrderBase.expirationDate;
      }
    }
    public int GetTotalCost() { return quantity * unitPrice; }
  }
}
