using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnChatimentAmeliore(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinChatimentAmeliore))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.PaladinChatimentAmeliore);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChatimentAmelioreEffectTag))
      {
        NwItem weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

        if(weapon is null || !weapon.IsRangedWeapon)
        {
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChatimentAmeliore(player.oid.LoginCreature));
        }
        else
        {
          player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnequipChatimentAmeliore;
          player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnequipChatimentAmeliore;
        }
      }
        
      return true;
    }
  }
}
