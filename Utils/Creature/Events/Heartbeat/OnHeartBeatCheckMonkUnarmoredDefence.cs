using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartBeatCheckMonkUnarmoredDefence(CreatureEvents.OnHeartbeat onHB)
    {
      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);
      NwItem shield = onHB.Creature.GetItemInSlot(InventorySlot.RightHand);
      bool hasShield = false;

      if(shield is not null)
        switch(shield.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield: hasShield = true; break;
        }


      if (!hasShield && (armor is null || armor.BaseACValue < 1))
      {
        if (onHB.Creature.GetAbilityModifier(Ability.Wisdom) > 0 && !onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkUnarmoredDefenceEffectTag))
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkUnarmoredDefenseEffect(onHB.Creature.GetAbilityModifier(Ability.Wisdom)));

        if (onHB.Creature.Classes.Any(c => c.Class.ClassType == ClassType.Monk && c.Level > 1)
          && !onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(onHB.Creature.Classes.FirstOrDefault(c => c.Class.ClassType == ClassType.Monk).Level));
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckMonkUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.MonkUnarmoredDefenceEffectTag);
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.MonkSpeedEffectTag);
      }
    }
  }
}
