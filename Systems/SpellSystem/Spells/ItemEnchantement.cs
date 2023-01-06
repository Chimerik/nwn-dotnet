using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Enchantement(OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
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

      if (!player.windows.ContainsKey("enchantementSelection")) player.windows.Add("enchantementSelection", new PlayerSystem.Player.EnchantementSelectionWindow(player, onSpellCast.Spell, targetItem));
      else ((PlayerSystem.Player.EnchantementSelectionWindow)player.windows["enchantementSelection"]).CreateWindow(onSpellCast.Spell, targetItem);

      // TODO : la réactivation fonctionnera via OnExamineItem. Mais gardons ça dans un coin en attendant
      /*if (oTarget.ItemProperties.Any(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{spellId}") && ip.Tag.Contains("INACTIVE")))
      {
        string inactiveIPTag = oTarget.ItemProperties.FirstOrDefault(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{spellId}") && ip.Tag.Contains("INACTIVE")).Tag;
        string[] IPproperties = inactiveIPTag.Split("_");
        player.craftJob.Start(Craft.Job.JobType.EnchantementReactivation, player, null, oTarget, $"{spellId}_{IPproperties[5]}_{IPproperties[6]}");
        return;
      }*/
    }
  }
}
