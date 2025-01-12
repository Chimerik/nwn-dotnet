using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProvocationEffectTag = "_PROVOCATION_EFFECT";
    //public static readonly string ProvoqueurEffectTag = "_PROVOQUEUR_EFFECT";
    public static void ApplyProvocation(NwGameObject caster, NwCreature target, TimeSpan duration)
    {
      EffectDuration effectDurationType = duration == TimeSpan.Zero ? EffectDuration.Permanent : EffectDuration.Temporary; 

      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Taunted), Effect.VisualEffect(VfxType.DurMindAffectingNegative),
        Effect.RunAction());
      eff.Tag = ProvocationEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;

      target.ApplyEffect(effectDurationType, eff, duration);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpConfusionS));

      /*Effect provoqueur = Effect.RunAction();
      provoqueur.Tag = ProvoqueurEffectTag;
      provoqueur.SubType = EffectSubType.Supernatural;
      provoqueur.Creator = target;
      caster.ApplyEffect(effectDurationType, provoqueur, duration);*/
    }
  }
}
