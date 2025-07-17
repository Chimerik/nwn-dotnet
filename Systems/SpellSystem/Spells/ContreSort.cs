using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ContreSort(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (target.CurrentAction == Action.CastSpell)
      {
        SavingThrowResult saveResult = CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, oCaster, spellDC, spellEntry);

        if (saveResult == SavingThrowResult.Failure)
        {
          target.ClearActionQueue();
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispelDisjunction));
        }
      }

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamBlack, oCaster, BodyNode.Hand)));
    }
  }
}
