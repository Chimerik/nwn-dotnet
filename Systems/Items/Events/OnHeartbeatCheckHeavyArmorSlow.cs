using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnHeartbeatCheckHeavyArmorSlow(CreatureEvents.OnHeartbeat onHB)
    {
      if(onHB.Creature.GetAbilityScore(Ability.Strength) > 14)
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.heavyArmorSlowEffectTag);
      else if(!onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.heavyArmorSlow.Tag))
        onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.heavyArmorSlow);

      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 6 || onHB.Creature.KnowsFeat(Feat.ArmorProficiencyHeavy))
      {
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.heavyArmorSlowEffectTag);
        onHB.Creature.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
      }
    }
  }
}
