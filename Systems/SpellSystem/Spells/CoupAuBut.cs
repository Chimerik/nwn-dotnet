using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CoupAuBut(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if(weapon is null || NativeUtils.GetCreatureWeaponProficiencyBonus(caster, weapon) < 1)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'une arme que vous maîtrisez pour utiliser ce sort", ColorConstants.Red);
        return;
      }

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
      EffectSystem.ApplyCoupAuBut(caster, spell, casterClass.SpellCastingAbility);
    }
  }
}
