using Anvil.API;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class SkillBook
  {
    public static void HandleActivate(NwItem skillBook, NwCreature oPC)
    {
      int id = skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value;
      Feat FeatId = (Feat)id;

      PlayerSystem.Log.Info($"{oPC.Name} used skillBook {FeatId}");

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (player.learntCustomFeats.ContainsKey(FeatId))
      {
        player.oid.SendServerMessage("Vous connaissez déjà les bases d'entrainement de cette capacité.", ColorConstants.Pink);
        return;
      }

      if (player.learnables.ContainsKey($"F{id}"))
      {
        player.oid.SendServerMessage("Cette capacité se trouve déjà dans votre liste d'apprentissage.", ColorConstants.Rose);
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
