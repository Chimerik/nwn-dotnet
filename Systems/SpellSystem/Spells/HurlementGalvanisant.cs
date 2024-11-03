using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void HurlementGalvanisant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        SpellUtils.SignalEventSpellCast(caster, caster, spell.SpellType);

        if (caster.Gender == Gender.Male)
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCry));
        else
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCryFemale));

        foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false, caster.Location.Position))
        {
          if (target.IsReactionTypeHostile(caster))
            continue;

          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.hurlementGalvanisant, NwTimeSpan.FromRounds(spellEntry.duration));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
        }
      }
      else
        caster.LoginPlayer?.SendServerMessage("Utilisable uniquement sous les effets de Rage du Barbare", ColorConstants.Red);

      caster.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 0);
    }
  }
}
