using Anvil.API;
using Anvil.API.Events;

using NWN.Native.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Enchantement(OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (player.craftJob != null)
      {
        player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
        return;
      }

      if (onSpellCast.TargetObject is not NwItem targetItem || targetItem == null || targetItem.Possessor != player.oid.ControlledCreature)
      {
        player.oid.SendServerMessage("Cible invalide.", ColorConstants.Red);
        return;
      }

      if (!player.learnableSkills.ContainsKey(CustomSkill.Enchanteur) || player.learnableSkills[CustomSkill.Enchanteur].totalPoints < 1)
      {
        player.oid.SendServerMessage("Il est nécessaire de connaître les bases de l'enchantement avant de pouvoir commencer ce travail !", ColorConstants.Red);
        return;
      }

      if (targetItem.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        player.oid.SendServerMessage($"{targetItem.Name.ColorString(ColorConstants.White)} n'a plus d'emplacement d'inscription disponible.", ColorConstants.Red);
        return;
      }

      // TODO : ajouter un coût en influx pur au lancement d'une inscription

      player.craftJob = new PlayerSystem.CraftJob(player, targetItem, onSpellCast.Spell, PlayerSystem.JobType.Enchantement);

      /*if (!player.windows.ContainsKey("enchantementSelection")) player.windows.Add("enchantementSelection", new PlayerSystem.Player.EnchantementSelectionWindow(player, onSpellCast.Spell, targetItem));
      else ((PlayerSystem.Player.EnchantementSelectionWindow)player.windows["enchantementSelection"]).CreateWindow(onSpellCast.Spell, targetItem);*/
    }
  }
}
