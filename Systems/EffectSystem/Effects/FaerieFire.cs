using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string faerieFireEffectTag = "_FAERIE_FIRE_EFFECT";
    public static readonly Native.API.CExoString faerieFireEffectExoTag = "_FAERIE_FIRE_EFFECT".ToExoString();
    public static Effect faerieFireEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessateNegative), Effect.VisualEffect(VfxType.DurGlowLightBlue));
        eff.Tag = faerieFireEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void CheckFaerieFire(OnEffectApply onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Effect.Tag != boneChillEffectTag || onEffect.Object is not NwGameObject target)
        return;

      if (onEffect.Effect.EffectType != EffectType.Invisibility && onEffect.Effect.EffectType != EffectType.ImprovedInvisibility)
        return;

      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == faerieFireEffectTag)
        {
          onEffect.PreventApply = true;
          return;
        }
    }
    public static void OnRemoveFaerieFire(OnEffectRemove onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Effect.Tag != faerieFireEffectTag || onEffect.Object is not NwGameObject target)
        return;

      target.OnEffectApply -= CheckFaerieFire;
    }
  }
}
