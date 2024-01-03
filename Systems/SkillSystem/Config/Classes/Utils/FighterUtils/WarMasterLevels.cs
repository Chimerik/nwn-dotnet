using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleWarMasterLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 3;
          player.InitializeManoeuvreChoice();

          break;

        case 7:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          player.learnableSkills.TryAdd(CustomSkill.WarMasterObservation, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarMasterObservation]));
          player.learnableSkills[CustomSkill.WarMasterObservation].LevelUp(player);
          player.learnableSkills[CustomSkill.WarMasterObservation].source.Add(Category.Class);

          break;

        case 11:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          break;

        case 15:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Value = 2;
          player.InitializeManoeuvreChoice();

          player.oid.OnCombatStatusChange -= FighterUtils.OnCombatWarMasterRecoverManoeuvre;
          player.oid.OnCombatStatusChange += FighterUtils.OnCombatWarMasterRecoverManoeuvre;

          break;

      }
    }
  }
}
