using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Serenity(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));

      string taggedEffect = "";

      foreach (var eff in oCaster.ActiveEffects)
      {
        if (string.IsNullOrEmpty(taggedEffect))
        {
          if(eff.EffectType == EffectType.Charmed || eff.Tag == EffectSystem.CharmEffectTag
            || eff.EffectType == EffectType.Frightened || eff.Tag == EffectSystem.FrightenedEffectTag)
          {
            taggedEffect = eff.Tag;
            oCaster.RemoveEffect(eff);
          }
        }
        else if(taggedEffect == eff.Tag)
          oCaster.RemoveEffect(eff);
      }
    }
  }
}
