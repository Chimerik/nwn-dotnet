using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ConvocationFeerique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentrationList = new();

      if (oCaster is NwCreature caster)
      {
        if (caster.Gold > 299)
        {
          caster.Gold -= 300;

          SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

          NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, "feyspirit");

          NwItem weapon = summon.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus2d6), EffectDuration.Permanent);
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus6), EffectDuration.Permanent);

          summon.ApplyEffect(EffectDuration.Temporary, EffectSystem.SummonDuration, SpellUtils.GetSpellDuration(oCaster, spellEntry));
          summon.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Charm));
          summon.BaseAttackBonus = (byte)(NativeUtils.GetCreatureProficiencyBonus(caster) + CreatureUtils.GetAbilityModifierMin1(caster, casterClass.SpellCastingAbility));

          summon.Location = targetLocation;
          summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster2));

          concentrationList.Add(summon);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Vous devez disposer de 300 po pour faire usage de ce sort");
      }

      return concentrationList;
    }
  }
}
