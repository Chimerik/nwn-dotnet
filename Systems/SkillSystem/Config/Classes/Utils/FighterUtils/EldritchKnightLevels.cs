using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleEldritchKnightLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(8).SetPlayerOverride(player.oid, "Guerrier Occulte");
          player.oid.SetTextureOverride("fighter", "eldritchknight");

          int classPosition;

          for (classPosition = 0; classPosition < player.oid.LoginCreature.Classes.Count; classPosition++)
            if (player.oid.LoginCreature.Classes[classPosition].Class.ClassType == ClassType.Fighter)
              break;

          CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, classPosition, CustomClass.EldritchKnight);

          player.learnableSkills.TryAdd(CustomSkill.EldritchKnightArmeLiee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EldritchKnightArmeLiee], player));
          player.learnableSkills[CustomSkill.EldritchKnightArmeLiee].LevelUp(player);
          player.learnableSkills[CustomSkill.EldritchKnightArmeLiee].source.Add(Category.Class);

          if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 2, 3));
          else ((SpellSelectionWindow)cantrip1).CreateWindow((ClassType)CustomClass.EldritchKnight, 2, 3);

          break;

        case 4:

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell4).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell7).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          player.learnableSkills.TryAdd(CustomSkill.EldritchKnightMagieDeGuerre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EldritchKnightMagieDeGuerre], player));
          player.learnableSkills[CustomSkill.EldritchKnightMagieDeGuerre].LevelUp(player);
          player.learnableSkills[CustomSkill.EldritchKnightMagieDeGuerre].source.Add(Category.Class);

          break;

        case 8:

          if (!player.windows.TryGetValue("spellSelection", out var spell8)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell8).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 10:

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 1, 1));
          else ((SpellSelectionWindow)spell10).CreateWindow((ClassType)CustomClass.EldritchKnight, 1, 1);

          player.learnableSkills.TryAdd(CustomSkill.EldritchKnightFrappeOcculte, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EldritchKnightFrappeOcculte], player));
          player.learnableSkills[CustomSkill.EldritchKnightFrappeOcculte].LevelUp(player);
          player.learnableSkills[CustomSkill.EldritchKnightFrappeOcculte].source.Add(Category.Class);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell11).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell13).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 14:

          if (!player.windows.TryGetValue("spellSelection", out var spell14)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell14).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.EldritchKnightChargeArcanique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EldritchKnightChargeArcanique], player));
          player.learnableSkills[CustomSkill.EldritchKnightChargeArcanique].LevelUp(player);
          player.learnableSkills[CustomSkill.EldritchKnightChargeArcanique].source.Add(Category.Class);

          break;

        case 16:

          if (!player.windows.TryGetValue("spellSelection", out var spell16)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell16).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;


        case 19:

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell19).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;

        case 20:

          if (!player.windows.TryGetValue("spellSelection", out var spell20)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.EldritchKnight, 0, 1));
          else ((SpellSelectionWindow)spell20).CreateWindow((ClassType)CustomClass.EldritchKnight, 0, 1);

          break;
      }
    }
  }
}
