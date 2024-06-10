using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ChatimentDivin(NwCreature caster)
    {
      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez vous équiper d'une arme de mêlée", ColorConstants.Red);
        return;
      }

      int chatimentLevel;

      if (Players.TryGetValue(caster, out Player player))
      {
        chatimentLevel = player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow)
          ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1;
      }
      else
      {
        float cr = caster.ChallengeRating;
        chatimentLevel = cr > 16 ? 5 : cr > 12 ? 4 : cr > 8 ? 3 : cr > 4 ? 2 : 1;
      }

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetChatimentDivinEffect(chatimentLevel, caster.GetClassInfo(ClassType.Paladin).Level)));

      player.oid.LoginCreature.DecrementRemainingFeatUses((Feat)CustomSkill.ChatimentDivin);
      player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).SetRemainingSpellSlots((byte)chatimentLevel,
        (byte)(player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots((byte)chatimentLevel) - 1));

      player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackChatimentDivin;
      player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackChatimentDivin;

      //if (targetObject is not NwCreature targetCreature || Utils.In(targetCreature.Race.RacialType, RacialType.Construct, RacialType.Undead))
      //return;
    }
  }
}
