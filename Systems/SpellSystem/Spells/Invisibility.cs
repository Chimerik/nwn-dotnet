using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Invisibility(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);
      DelayInvisEffect(oCaster, oTarget, spellEntry, feat);

      return new List<NwGameObject> { oTarget };
    }
    private static async void DelayInvisEffect(NwGameObject oCaster, NwGameObject oTarget, SpellEntry spellEntry, NwFeat feat)
    {
      await NwTask.NextFrame();

      TimeSpan duration = NwTimeSpan.FromRounds(spellEntry.duration);

      if (oCaster is NwCreature caster && feat is not null)
      {
        duration = NwTimeSpan.FromRounds(10);

        switch (feat.Id)
        {
          case CustomSkill.MonkLinceulDombre:
            caster.IncrementRemainingFeatUses(feat.FeatType);
            
            break;

          case CustomSkill.ClercLinceulDombre:
            caster.IncrementRemainingFeatUses(feat.FeatType);
            ClercUtils.ConsumeConduitDivin(caster);
            break;
        }
      }

      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Invisibility(InvisibilityType.Normal)), duration);
    }
  }
}
