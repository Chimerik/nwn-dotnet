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
    //private readonly ScriptHandleFactory scriptHandleFactory;
    //public static ScriptCallbackHandle removeCoreHandle;
    //private static Effect corePotionEffect;
    private static Effect shieldArmorDisadvantageEffect;
    private static Effect slow;
    public ItemSystem(FeedbackService feedback/*, ScriptHandleFactory scriptFactory*/)
    {
      feedbackService = feedback;
      //scriptHandleFactory = scriptFactory;

      /*removeCoreHandle = scriptHandleFactory.CreateUniqueHandler(Potion.RemoveCore);

      corePotionEffect = Effect.RunAction(null, removeCoreHandle);
      corePotionEffect = Effect.LinkEffects(corePotionEffect, Effect.Icon(NwGameTables.EffectIconTable.GetRow(131)));
      corePotionEffect.Tag = "_CORE_EFFECT";
      corePotionEffect.SubType = EffectSubType.Supernatural;*/

      shieldArmorDisadvantageEffect = Effect.RunAction();
      shieldArmorDisadvantageEffect =  Effect.LinkEffects(shieldArmorDisadvantageEffect, Effect.Icon(NwGameTables.EffectIconTable.GetRow(34)));
      shieldArmorDisadvantageEffect.Tag = StringUtils.shieldArmorDisadvantageEffectTag;
      shieldArmorDisadvantageEffect.SubType = EffectSubType.Unyielding;

      slow = Effect.MovementSpeedDecrease(30);
      slow = Effect.LinkEffects(slow, Effect.Icon(NwGameTables.EffectIconTable.GetRow(38)));
      slow.Tag = "_EFFECT_HEAVY_ARMOR_SLOW";
      slow.SubType = EffectSubType.Unyielding;
    }
  }
}
