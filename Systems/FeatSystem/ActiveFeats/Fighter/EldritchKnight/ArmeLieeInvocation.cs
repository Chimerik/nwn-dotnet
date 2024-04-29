using System;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ArmeLieeInovcation(NwCreature caster, NwFeat feat)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      string weaponIdString = feat.Id == CustomSkill.EldritchKnightArmeLieeInvocation2 ? "_ARME_LIEE_ID_2" : "_ARME_LIEE_ID_1";
      string weaponTagString = feat.Id == CustomSkill.EldritchKnightArmeLieeInvocation2 ? "_ARME_LIEE_TAG_2" : "_ARME_LIEE_TAG_1";
      InventorySlot slot = feat.Id == CustomSkill.EldritchKnightArmeLieeInvocation2 ? InventorySlot.LeftHand : InventorySlot.RightHand;

      if (Guid.TryParse(caster.GetObjectVariable<PersistentVariableString>(weaponIdString).Value, out var weaponId))
      {
        NwItem weapon = NwObject.FindObjectsWithTag<NwItem>(caster.GetObjectVariable<PersistentVariableString>(weaponTagString).Value).FirstOrDefault(i => i.UUID == weaponId);
        if(weapon is not null)
        {
          caster.AcquireItem(weapon);
          caster.RunEquip(weapon, slot);
          StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} invoquer son arme liée : {StringUtils.ToWhitecolor(weapon.Name)}", ColorConstants.Orange, true, true);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Votre arme liée ne répond pas à l'appel. Se trouve-t-elle dans un autre plan ?", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous n'êtes lié à aucune arme", ColorConstants.Red);
    }
  }
}
