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
      NwItem shield = onHB.Creature.GetItemInSlot(InventorySlot.LeftHand);
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
        int wisMod = onHB.Creature.GetAbilityModifier(Ability.Wisdom);

        var eff = onHB.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.MonkUnarmoredDefenceEffectTag && e.EffectType == EffectType.AcIncrease);

        if (eff is not null && eff.IntParams[1] != wisMod)
          EffectSystem.ApplyMonkUnarmoredDefenseEffect(onHB.Creature);

        if (onHB.Creature.GetObjectVariable<LocalVariableInt>("_MONK_SPEED_DISABLED").HasNothing
          && onHB.Creature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
          && !onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
        {
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(onHB.Creature.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Monk).Level));
        }
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckMonkUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.MonkUnarmoredDefenceEffectTag, EffectSystem.MonkSpeedEffectTag);
      }
    }
  }
}
