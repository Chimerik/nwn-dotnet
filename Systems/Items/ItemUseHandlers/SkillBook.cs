using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class SkillBook
  {
    public static void HandleActivate(NwItem skillBook, NwPlayer oPC)
    {
      Feat FeatId = (Feat)skillBook.GetLocalVariable<int>("_SKILL_ID").Value;

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (player.learntCustomFeats.ContainsKey(FeatId))
      {
        oPC.SendServerMessage("Vous connaissez déjà les bases d'entrainement de cette capacité.", Color.PINK);
        return;
      }

      if (player.learnableSkills.ContainsKey(FeatId))
      {
        oPC.SendServerMessage("Cette capacité se trouve déjà dans votre liste d'apprentissage.", Color.ROSE);
        return;
      }

      Systems.SkillBook.pipeline.Execute(new Systems.SkillBook.Context(
        oItem: skillBook,
        oActivator: player,
        SkillId: FeatId
      ));
    }
  }
}
