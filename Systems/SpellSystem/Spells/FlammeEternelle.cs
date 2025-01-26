using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FlammeEternelle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.Gold < 50)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 50 po afin de faire usage de ce sort", ColorConstants.Red);
        return;
      }

      caster.Gold -= 25;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if(oTarget is NwItem item)
      {
        if (item.BaseItem.EquipmentSlots != EquipmentSlots.None)
          return;

        item.AddItemProperty(ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), EffectDuration.Permanent, policy:AddPropPolicy.KeepExisting);
      }
      else
      {
        oTarget.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect(VfxType.DurLightWhite20));
      }
    }
  }
}
