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
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
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

      EffectUtils.ClearChatimentEffects(caster);
      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetChatimentDivinEffect(chatimentLevel)));

      player.oid.LoginCreature.DecrementRemainingFeatUses((Feat)CustomSkill.ChatimentDivin);
      player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).SetRemainingSpellSlots((byte)chatimentLevel,
        (byte)(player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots((byte)chatimentLevel) - 1));

      player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackChatimentDivin;
      player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackChatimentDivin;

      if(player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DevotionChatimentProtecteur))
      {
        int range = player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level > 17 ? 9 : 3;
        foreach(var target in player.oid.LoginCreature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, range, false))
        {
          if (player.oid.LoginCreature.IsReactionTypeHostile(target))
            continue;

          target.ApplyEffect(EffectDuration.Temporary, Effect.ACIncrease(2), NwTimeSpan.FromRounds(1));
        }
      }
    }
  }
}
