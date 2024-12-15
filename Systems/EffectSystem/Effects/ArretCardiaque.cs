using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ArretCardiaqueEffectTag = "_ARRET_CARDIAQUE_EFFECT";
    public static readonly Native.API.CExoString ArretCardiaqueEffectExoTag = ArretCardiaqueEffectTag.ToExoString();
    public static void ArretCardiaque(NwCreature target)
    {
      if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct))
        return;

      target.OnHeal -= OnHealRemoveExpertiseEffect;
      target.OnHeal += OnHealRemoveExpertiseEffect;

      Effect eff = Effect.Slow();
      eff.Tag = ArretCardiaqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
    private static void OnHealRemoveExpertiseEffect(OnHeal onHeal)
    {
      EffectUtils.RemoveTaggedEffect(onHeal.Target, ArretCardiaqueEffectTag, MutilationEffectTag, LacerationEffectTag, TranspercerEffectTag);
      onHeal.Target.OnHeal -= OnHealRemoveExpertiseEffect;
    }
  }
}

