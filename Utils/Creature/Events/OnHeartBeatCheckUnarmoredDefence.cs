using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnHeartBeatCheckUnarmoredDefence(CreatureEvents.OnHeartbeat onHB)
    {
      NwItem armor = onHB.Creature.GetItemInSlot(InventorySlot.Chest);

      if (armor is null || armor.BaseACValue < 1)
      {
        if(onHB.Creature.GetAbilityModifier(Ability.Constitution) > 0)
          onHB.Creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetUnarmoredDefenseEffect(onHB.Creature.GetAbilityModifier(Ability.Constitution)), NwTimeSpan.FromRounds(1));
      }
      else
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckUnarmoredDefence;
    }
  }
}
