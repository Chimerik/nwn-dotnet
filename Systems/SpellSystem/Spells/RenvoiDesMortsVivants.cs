using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RenvoiDesMortsVivants(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);
      int clericLevel = caster.GetClassInfo(ClassType.Cleric).Level;
      int wisMod = caster.GetAbilityModifier(Ability.Wisdom) > 1 ? caster.GetAbilityModifier(Ability.Wisdom) : 1;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(target.Race.RacialType == RacialType.Undead 
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC) == SavingThrowResult.Failure)
        {
          if (clericLevel > 4)
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 8, wisMod),
              DamageType.Divine)));

          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSunstrike));
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetRenvoiDesImpiesEffect(target), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }

      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
