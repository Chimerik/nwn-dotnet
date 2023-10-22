using Anvil.API;
using Anvil.Services;
using NLog;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ItemSystem))]
  public partial class ItemSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static FeedbackService feedbackService;
    //private static Effect corePotionEffect;
    public ItemSystem(FeedbackService feedback)
    {
      feedbackService = feedback;
      //scriptHandleFactory = scriptFactory;

      /*removeCoreHandle = scriptHandleFactory.CreateUniqueHandler(Potion.RemoveCore);

      corePotionEffect = Effect.RunAction(null, removeCoreHandle);
      corePotionEffect = Effect.LinkEffects(corePotionEffect, Effect.Icon(NwGameTables.EffectIconTable.GetRow(131)));
      corePotionEffect.Tag = "_CORE_EFFECT";
      corePotionEffect.SubType = EffectSubType.Supernatural;*/      
    }
  }
}
