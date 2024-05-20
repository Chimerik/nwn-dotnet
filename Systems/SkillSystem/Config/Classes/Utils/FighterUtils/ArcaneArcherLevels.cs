using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleArcherMageLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(8).SetPlayerOverride(player.oid, "Archer-Mage");
          player.oid.SetTextureOverride("fighter", "arcanearcher");

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value = 2;

          if (!player.windows.TryGetValue("tirArcaniqueChoice", out var value)) player.windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(player, 2));
          else ((TirArcaniqueChoiceWindow)value).CreateWindow(2);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.ArcaneArcherTirIncurve, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcaneArcherTirIncurve], player));
          player.learnableSkills[CustomSkill.ArcaneArcherTirIncurve].LevelUp(player);
          player.learnableSkills[CustomSkill.ArcaneArcherTirIncurve].source.Add(Category.Class);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value = 1;

          if (!player.windows.TryGetValue("tirArcaniqueChoice", out var window)) player.windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(player));
          else ((TirArcaniqueChoiceWindow)window).CreateWindow();

          break;

        case 10:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value = 1;

          if (!player.windows.TryGetValue("tirArcaniqueChoice", out var tirArca)) player.windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(player));
          else ((TirArcaniqueChoiceWindow)tirArca).CreateWindow();

          break;

        case 15:

          player.oid.OnCombatStatusChange -= FighterUtils.OnCombatArcaneArcherRecoverTirArcanique;
          player.oid.OnCombatStatusChange += FighterUtils.OnCombatArcaneArcherRecoverTirArcanique;

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value = 1;

          if (!player.windows.TryGetValue("tirArcaniqueChoice", out var tirArcanique)) player.windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(player));
          else ((TirArcaniqueChoiceWindow)tirArcanique).CreateWindow();

          break;

        case 18:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TIR_ARCANIQUE_CHOICE").Value = 1;

          if (!player.windows.TryGetValue("tirArcaniqueChoice", out var tirArcani)) player.windows.Add("tirArcaniqueChoice", new TirArcaniqueChoiceWindow(player));
          else ((TirArcaniqueChoiceWindow)tirArcani).CreateWindow();

          break;
      }
    }
  }
}
