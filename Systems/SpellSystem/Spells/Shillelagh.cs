using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Shillelagh(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if(weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff))
      {
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));
        EffectSystem.ApplyShillelagh(caster, spell, casterClass.SpellCastingAbility);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un gourdin ou d'un bâton pour utiliser ce sort", ColorConstants.Red);
    }
  }
}
