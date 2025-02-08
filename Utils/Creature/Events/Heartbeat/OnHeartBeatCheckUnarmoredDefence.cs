using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartBeatCheckUnarmoredDefence(CreatureEvents.OnHeartbeat onHB)
    {
      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 1)
      {
        int conMod = onHB.Creature.GetAbilityModifier(Ability.Constitution);

        var eff = onHB.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.UnarmoredDefenceEffectTag);

        if (eff is not null && eff.IntParams[1] != conMod)
          EffectSystem.ApplyUnarmoredDefenseEffect(onHB.Creature);

        if (onHB.Creature.Classes.Any(c => c.Class.Id == CustomClass.Barbarian && c.Level > 4)
            && !onHB.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianSpeedEffectTag))
          onHB.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BarbarianSpeed);
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.UnarmoredDefenceEffectTag, EffectSystem.BarbarianSpeedEffectTag);
      }
    }
  }
}
