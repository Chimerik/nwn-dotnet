using System.Linq;
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

        var eff = onHB.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ResistanceDraconiqueEffectTag && e.EffectType == EffectType.AcIncrease);

        if(eff is not null && eff.IntParams[1] != chaMod)
          EffectSystem.ApplyResistanceDraconiqueEffect(onHB.Creature);
      }
      else
      {
        onHB.Creature.OnHeartbeat -= OnHeartBeatCheckResistanceDraconique;
        EffectUtils.RemoveTaggedEffect(onHB.Creature, EffectSystem.ResistanceDraconiqueEffectTag);
      }
    }
  }
}
