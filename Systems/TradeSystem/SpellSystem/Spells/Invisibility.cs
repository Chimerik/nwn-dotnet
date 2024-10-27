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
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach(var target in targets)
        DelayInvisEffect(oCaster, target, spellEntry, feat);

      return targets;
    }
    private static async void DelayInvisEffect(NwGameObject oCaster, NwGameObject oTarget, SpellEntry spellEntry, NwFeat feat)
    {
      await NwTask.NextFrame();

      TimeSpan duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

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
