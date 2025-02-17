using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NoBonusActionEffectTag = "_NO_BONUS_ACTIONS_EFFECT";

    public static Effect NoBonusAction(NwGameObject target)
    {
      EffectUtils.RemoveTaggedEffect(target, BonusActionEffectTag);

      Effect eff = Effect.Icon(CustomEffectIcon.NoBonusAction);
      eff.Tag = NoBonusActionEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
