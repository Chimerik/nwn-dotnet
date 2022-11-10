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
    public SellOrder(SerializableSellOrder serializedSellOrder)
    {
      this.sellerId = serializedSellOrder.sellerId;
      this.resourceType = (ResourceType)serializedSellOrder.resourceType;
      this.resourceLevel = serializedSellOrder.resourceLevel;
      this.quantity = serializedSellOrder.quantity;
      this.expirationDate = serializedSellOrder.expirationDate;
      this.unitPrice = serializedSellOrder.unitPrice;
    }

    public class SerializableSellOrder
    {
      public int sellerId { get; set; }
      public int resourceType { get; set; }
      public int resourceLevel { get; set; }
      public int quantity { get; set; }
      public int unitPrice { get; set; }
      public DateTime expirationDate { get; set; }

      public SerializableSellOrder()
      {

      }
      public SerializableSellOrder(SellOrder sellOrderBase)
      {
        sellerId = sellOrderBase.sellerId;
        resourceType = (int)sellOrderBase.resourceType;
        resourceLevel = sellOrderBase.resourceLevel;
        quantity = sellOrderBase.quantity;
        unitPrice = sellOrderBase.unitPrice;
        expirationDate = sellOrderBase.expirationDate;
      }
    }
    public int GetTotalCost() { return quantity * unitPrice; }

  }
}
