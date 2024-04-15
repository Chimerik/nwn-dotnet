using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void FlameBlade(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;
      StringUtils.ForceBroadcastSpellCasting(caster, onSpellCast.Spell);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      NwItem blade = await NwItem.Create(BaseItems2da.baseItemTable[(int)BaseItemType.Scimitar].craftedItem, caster, 1, "_TEMP_FLAME_BLADE");
      blade.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
      blade.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Fire, IPDamageBonus.Plus2d6), EffectDuration.Permanent);
      blade.AddItemProperty(ItemProperty.NoDamage(), EffectDuration.Permanent);
      blade.AddItemProperty(ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Orange), EffectDuration.Permanent);

      caster.OnItemUnequip -= OnUnequipFlameBlade;
      caster.OnItemUnequip += OnUnequipFlameBlade;

      caster.RunEquip(blade, InventorySlot.RightHand);

      if (caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value != CustomSpell.FlameBlade)
        EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);

      CreatureUtils.HandleBonusActionCooldown(caster);
    }
    private static void OnUnequipFlameBlade(OnItemUnequip onUnEquip)
    {
      if (!onUnEquip.Item.IsValid || onUnEquip.Item.Tag != "_TEMP_FLAME_BLADE")
        return;

      onUnEquip.Item.Destroy();
    }
  }
}
