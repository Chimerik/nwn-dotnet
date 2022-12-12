using System;

namespace NWN.Systems
{
  public class Subscription
  {
    public readonly Utils.SubscriptionType type;
    public readonly DateTime nextDueDate;
    public readonly int daysToNextDueDate;
    public readonly int fee;

    public Subscription(Utils.SubscriptionType type, int fee, DateTime nextDueDate, int daysToNextDueDate)
    {
      this.type = type;
      this.fee = fee;
      this.nextDueDate = nextDueDate;
      this.daysToNextDueDate = daysToNextDueDate;
    }
    public Subscription()
    {

    }
    public Subscription(SerializableSubscription serializedSubscription)
    {
      type = serializedSubscription.type;
      fee = serializedSubscription.fee;
      nextDueDate = serializedSubscription.nextDueDate;
      daysToNextDueDate = serializedSubscription.daysToNextDueDate;
    }

    public class SerializableSubscription
    {
      public Utils.SubscriptionType type { get; set; }
      public int fee { get; set; }
      public DateTime nextDueDate { get; set; }
      public int daysToNextDueDate { get; set; }


      public SerializableSubscription()
      {

      }
      public SerializableSubscription(Subscription subscriptionBase)
      {
        type = subscriptionBase.type;
        fee = subscriptionBase.fee;
        nextDueDate = subscriptionBase.nextDueDate;
        daysToNextDueDate = subscriptionBase.daysToNextDueDate;
      }
    }
  }
}
