using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnHeartBeatCheckResistanceDraconique(CreatureEvents.OnHeartbeat onHB)
    {
      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 1)
      {
        int chaMod = onHB.Creature.GetAbilityModifier(Ability.Charisma);
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.ResistanceDraconiqueEffectTag);
        WaitNextFrameToApplyChaEffect(onHB.Creature, chaMod);
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckResistanceDraconique;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.ResistanceDraconiqueEffectTag);
      }
    }
    private static async void WaitNextFrameToApplyChaEffect(NwCreature creature, int chaMod)
    {
      if (chaMod > 0)
      {
        await NwTask.NextFrame();
        EffectSystem.ApplyResistanceDraconiqueEffect(creature, chaMod);
      }
    }
  }
}
