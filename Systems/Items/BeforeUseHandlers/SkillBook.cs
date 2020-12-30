using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Items.BeforeUseHandlers
{
  public static class SkillBook
  {
    public static void HandleActivate(uint skillBook, Player player)
    {
      int FeatId = NWScript.GetLocalInt(skillBook, "_SKILL_ID");

      if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, FeatId) == (int)Feat.Invalid)
      {
        if (!player.learnableSkills.ContainsKey(FeatId))
        {
          Systems.SkillBook.pipeline.Execute(new Systems.SkillBook.Context(
            oItem: skillBook,
            oActivator: player,
            SkillId: FeatId
          ));
        }
        else
          NWScript.SendMessageToPC(player.oid, "Cette capacité se trouve déjà dans votre liste d'apprentissage.");
      }
      else
        NWScript.SendMessageToPC(player.oid, "Vous connaissez déjà les bases d'entrainement de cette capacité.");
    }
  }
}
