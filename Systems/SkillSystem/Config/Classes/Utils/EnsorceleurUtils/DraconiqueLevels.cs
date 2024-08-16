﻿using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static void HandleDraconiqueLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(9).SetPlayerOverride(player.oid, "Lignée Draconique");
          player.oid.SetTextureOverride("ensorceleur", "enso_draconique");

          player.learnableSkills.TryAdd(CustomSkill.EnsoResistanceDraconique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoResistanceDraconique], player));
          player.learnableSkills[CustomSkill.EnsoResistanceDraconique].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoResistanceDraconique].source.Add(Category.Class);

          foreach (var ensoLevel in player.oid.LoginCreature.LevelInfo)
            if (ensoLevel.ClassInfo.Class.ClassType == ClassType.Sorcerer)
              ensoLevel.HitDie += 1;

          break;

        case 6:

          if (!player.windows.TryGetValue("ensoDracoAffiniteElementaireSelection", out var style)) player.windows.Add("ensoDracoAffiniteElementaireSelection", new EnsoDracoAffiniteElementaireSelectionWindow(player));
          else ((EnsoDracoAffiniteElementaireSelectionWindow)style).CreateWindow();

          break;


        case 14:

          player.learnableSkills.TryAdd(CustomSkill.EnsoDracoWings, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoDracoWings], player));
          player.learnableSkills[CustomSkill.EnsoDracoWings].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoDracoWings].source.Add(Category.Class);

          break;

        case 18:

          player.learnableSkills.TryAdd(CustomSkill.EnsoCompagnonDraconique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnsoCompagnonDraconique], player));
          player.learnableSkills[CustomSkill.EnsoCompagnonDraconique].LevelUp(player);
          player.learnableSkills[CustomSkill.EnsoCompagnonDraconique].source.Add(Category.Class);

          break;
      }
    }
  }
}
