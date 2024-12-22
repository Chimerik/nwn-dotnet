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
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.UnarmoredDefenceEffectTag);
        WaitNextFrameToApplyConEffect(onHB.Creature, conMod);

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
    private static async void WaitNextFrameToApplyConEffect(NwCreature creature, int conMod)
    {
      if (conMod > 0)
      {
        await NwTask.NextFrame();
        EffectSystem.ApplyUnarmoredDefenseEffect(creature);
      }
    }
  }
}
