using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageConcentration(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.KnowsFeat((Feat)CustomSkill.InvocationConcentration)
        && NwSpell.FromSpellId(onDamage.Creature.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString)).SpellSchool == SpellSchool.Conjuration)
        return;

      if (onDamage.Creature.KnowsFeat((Feat)CustomSkill.RangerImplacable)
        && NwSpell.FromSpellId(onDamage.Creature.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString)).Id == CustomSpell.MarqueDuChasseur)
        return;

      var damager = (NwGameObject)NWScript.GetLastDamager(onDamage.Creature).ToNwObject<NwObject>();
      int totalDamage = onDamage.DamageAmount / 2;
      int concentrationDC = 10 > totalDamage ? 10 : totalDamage;
     
      if (GetSavingThrowResult(onDamage.Creature, Ability.Constitution, damager, concentrationDC, effectType:SpellConfig.SpellEffectType.Concentration) == SavingThrowResult.Failure)
      {
        SpellUtils.DispelConcentrationEffects(onDamage.Creature);
      }
    }
  }
}
