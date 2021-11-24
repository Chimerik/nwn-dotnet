using Anvil.API;

namespace NWN.Systems.Items.ItemUseHandlers
{
  public static class SkillBook
  {
    public static void HandleActivate(NwItem skillBook, NwCreature oPC)
    {
      int id = skillBook.GetObjectVariable<LocalVariableInt>("_SKILL_ID").Value;

      PlayerSystem.Log.Info($"{oPC.Name} used skillBook {id}");

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (player.learnableSkills.ContainsKey(id))
      {
        player.oid.SendServerMessage("Vous connaissez déjà les bases d'entrainement de cette compétence.", ColorConstants.Red);
        return;
      }

      player.learnableSkills.Add(id, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[id]));
    }
  }
}
