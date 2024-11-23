using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VagabondageEffectTag = "_VAGABONDAAGE_EFFECT";
    public static void ApplyVagabondage(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Nage, Escalade);
      eff.Tag = VagabondageEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      caster.OnItemEquip -= OnEquipVagabondage;
      caster.OnItemUnequip -= OnUnEquipVagabondage;
      caster.OnItemEquip += OnEquipVagabondage;
      caster.OnItemUnequip += OnUnEquipVagabondage;

      if (!ItemUtils.IsArmor(caster.GetItemInSlot(InventorySlot.Chest)))
      {
        caster.OnHeartbeat -= OnHeartBeatCheckVagabondage;
        caster.OnHeartbeat += OnHeartBeatCheckVagabondage;

        if(!caster.ActiveEffects.Any(e => e.Tag == VagabondageEffectTag))
          caster.ApplyEffect(EffectDuration.Permanent, eff);
      }
      else 
      {
        caster.OnHeartbeat -= OnHeartBeatCheckVagabondage;
        EffectUtils.RemoveTaggedEffect(caster, VagabondageEffectTag); 
      }
    }
    private static async void OnEquipVagabondage(OnItemEquip onEquip)
    {
      if (onEquip.EquippedBy is null || onEquip.Item is null)
        return;

      await NwTask.NextFrame();
      ApplyVagabondage(onEquip.EquippedBy);
    }
    private static async void OnUnEquipVagabondage(OnItemUnequip onUnequip)
    {
      if (onUnequip.Creature is null || onUnequip.Item is null)
        return;

      await NwTask.NextFrame();
      ApplyVagabondage(onUnequip.Creature);
    }
    private static void OnHeartBeatCheckVagabondage(CreatureEvents.OnHeartbeat onHB)
    {
      ApplyVagabondage(onHB.Creature);
    }
  }
}
