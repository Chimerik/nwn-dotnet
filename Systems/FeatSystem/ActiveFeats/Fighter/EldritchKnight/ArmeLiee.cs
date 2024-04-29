using System;
using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ArmeLiee(NwCreature caster, NwFeat feat)
    {
      string weaponIdString = feat.Id == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_ID_2" : "_ARME_LIEE_ID_1";
      string weaponTagString = feat.Id == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_TAG_2" : "_ARME_LIEE_TAG_1";
      string weaponChargeString = feat.Id == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_2_NB_CHARGE" : "_ARME_LIEE_1_NB_CHARGE";

      if (Guid.TryParse(caster.GetObjectVariable<PersistentVariableString>(weaponIdString).Value, out var previousWeaponId))
      {
        NwItem previousWeapon = NwObject.FindObjectsWithTag<NwItem>(caster.GetObjectVariable<PersistentVariableString>(weaponTagString).Value).FirstOrDefault(i => i.UUID == previousWeaponId);
        
        if(previousWeapon is not null)
        {
          previousWeapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").Delete();
          caster.GetObjectVariable<PersistentVariableString>(weaponIdString).Delete();
          caster.GetObjectVariable<PersistentVariableString>(weaponTagString).Delete();
          caster.LoginPlayer?.SendServerMessage($"Votre lien avec {StringUtils.ToWhitecolor(previousWeapon.Name)} est rompu", ColorConstants.Orange);
        }
      }

      if (caster.LoginPlayer?.LoginCreature.GetObjectVariable<LocalVariableInt>(weaponChargeString).Value > 0)
      {
        caster.LoginPlayer?.EnterTargetMode(SelectWeapon, Config.selectItemTargetMode);
        caster.LoginPlayer?.SendServerMessage("Sélectionnez une arme à laquelle vous lier", ColorConstants.Orange);
        caster.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_SELECTION").Value = feat.Id;
      }
      else
        caster.LoginPlayer?.SendServerMessage("Aucune charge disponible. Un repos court est nécessaire", ColorConstants.Red);
    }
    private static void SelectWeapon(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      int featId = selection.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_SELECTION").Value;

      if (selection.IsCancelled || selection.TargetObject is not NwItem weapon || !ItemUtils.IsWeapon(weapon.BaseItem)
        || !Players.TryGetValue(selection.Player.LoginCreature, out Player player))
      {
        selection.Player.LoginCreature.SetFeatRemainingUses((Feat)featId, 0);
        return;
      }

      if(weapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasValue && weapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").Value != player.characterId)
      {
        player.oid.SendServerMessage("Cette arme est déjà liée, il ne vous est pas possible de remplacer le lien existant", ColorConstants.Red);
        return;
      }

      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_SELECTION").Delete();

      string weaponIdString = featId == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_ID_2" : "_ARME_LIEE_ID_1";
      string weaponTagString = featId == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_TAG_2" : "_ARME_LIEE_TAG_1";
      string weaponChargeString = featId == CustomSkill.EldritchKnightArmeLiee2 ? "_ARME_LIEE_2_NB_CHARGE" : "_ARME_LIEE_1_NB_CHARGE";

      weapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").Value = player.characterId;
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableString>(weaponTagString).Value = weapon.Tag;
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableString>(weaponIdString).Value = weapon.UUID.ToUUIDString();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(weaponChargeString).Value -= 1;
      player.oid.LoginCreature.SetFeatRemainingUses((Feat)featId, 100);

      StringUtils.DisplayStringToAllPlayersNearTarget(player.oid.LoginCreature, $"{player.oid.LoginCreature.Name.ColorString(ColorConstants.Cyan)} se lie rituellement à {StringUtils.ToWhitecolor(weapon.Name)}", ColorConstants.Orange, true, true);
    }
  }
}
