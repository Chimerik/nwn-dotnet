using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CourrouxDeLaNature(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target) 
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Charisma);
      Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity) ? Ability.Strength : Ability.Dexterity;

      if(CreatureUtils.GetSavingThrowResult(target, saveAbility, caster, spellDC, spellEntry) == SavingThrowResult.Failure)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.CourrouxDeLaNature, NwTimeSpan.FromRounds(10)));

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
