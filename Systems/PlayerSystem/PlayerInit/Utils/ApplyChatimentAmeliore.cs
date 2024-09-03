using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyChatimentAmeliore()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinChatimentAmeliore)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChatimentAmelioreEffectTag))
        {
          NwItem weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || !weapon.IsRangedWeapon)
          {
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChatimentAmeliore(oid.LoginCreature));
          }
          else
          {
            oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnequipChatimentAmeliore;
            oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnequipChatimentAmeliore;
          }
        }
      }
    }
  }
}
