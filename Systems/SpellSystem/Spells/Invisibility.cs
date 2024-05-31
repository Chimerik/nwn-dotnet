using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Invisibility(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);
      DelayInvisEffect(oCaster, oTarget, spellEntry);

      return new List<NwGameObject> { oTarget };
    }
    private static async void DelayInvisEffect(NwGameObject oCaster, NwGameObject oTarget, SpellEntry spellEntry)
    {
      await NwTask.NextFrame();

      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Invisibility(InvisibilityType.Normal)), NwTimeSpan.FromRounds(spellEntry.duration));

      if (oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.MonkLinceulDombre))
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.MonkLinceulDombre);
    }
  }
}
