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
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.UnarmoredDefenceEffectTag);
      }
    }
    private static async void WaitNextFrameToApplyConEffect(NwCreature creature, int conMod)
    {
      if (conMod > 0)
      {
        await NwTask.NextFrame();
        creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(conMod));
      }
    }
  }
}
