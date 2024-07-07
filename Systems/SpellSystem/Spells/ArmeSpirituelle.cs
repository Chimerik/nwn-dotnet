using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void ArmeSpirituelle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      NWScript.AssignCommand(oCaster, () => targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.SummonCreature("X2_S_FAERIE001", VfxType.FnfSummonMonster1, NwTimeSpan.FromRounds(spellEntry.duration)), NwTimeSpan.FromRounds(spellEntry.duration)));
      NwCreature summon = UtilPlugin.GetLastCreatedObject(5).ToNwObject<NwCreature>();
      NwItem weapon = await NwItem.Create("NW_WSWDG001", summon);
      weapon.Droppable = false;

      weapon.AddItemProperty(ItemProperty.AttackBonus(NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(castingClass.SpellCastingAbility)),
        EffectDuration.Permanent);

      weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d8), EffectDuration.Permanent);
      summon.RunEquip(weapon, InventorySlot.RightHand);
    }
  }
}
