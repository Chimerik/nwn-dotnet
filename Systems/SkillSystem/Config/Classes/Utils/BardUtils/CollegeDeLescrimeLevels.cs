using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Bard
  {
    public static void HandleCollegeDeLescrimeLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(2).SetPlayerOverride(player.oid, "Collège de l'Escrime");
          player.oid.SetTextureOverride("bard", "escrime");

          if(player.learnableSkills.TryAdd(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ScimitarProficiency], player)))
            player.learnableSkills[CustomSkill.ScimitarProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.ScimitarProficiency].source.Add(Category.Class);

          if (player.learnableSkills.TryAdd(CustomSkill.MediumArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MediumArmorProficiency], player)))
            player.learnableSkills[CustomSkill.MediumArmorProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.MediumArmorProficiency].source.Add(Category.Class);

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.BardCollegeDeLescrime));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.BardCollegeDeLescrime);

          player.oid.LoginCreature.OnCreatureAttack -= BardUtils.OnAttackEscrime;
          player.oid.LoginCreature.OnCreatureAttack += BardUtils.OnAttackEscrime;

          player.learnableSkills.TryAdd(CustomSkill.BotteDefensive, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BotteDefensive], player));
          player.learnableSkills[CustomSkill.BotteDefensive].LevelUp(player);
          player.learnableSkills[CustomSkill.BotteDefensive].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.BotteTranchante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BotteTranchante], player));
          player.learnableSkills[CustomSkill.BotteTranchante].LevelUp(player);
          player.learnableSkills[CustomSkill.BotteTranchante].source.Add(Category.Class);

          DelayInspiInit(player.oid.LoginCreature);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.AttaqueSupplementaire, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AttaqueSupplementaire], player));
          player.learnableSkills[CustomSkill.AttaqueSupplementaire].LevelUp(player);
          player.learnableSkills[CustomSkill.AttaqueSupplementaire].source.Add(Category.Class);

          CreatureUtils.InitializeNumAttackPerRound(player.oid.LoginCreature);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.BotteDefensiveDeMaitre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BotteDefensiveDeMaitre], player));
          player.learnableSkills[CustomSkill.BotteDefensiveDeMaitre].LevelUp(player);
          player.learnableSkills[CustomSkill.BotteDefensiveDeMaitre].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.BotteTranchanteDeMaitre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BotteTranchanteDeMaitre], player));
          player.learnableSkills[CustomSkill.BotteTranchanteDeMaitre].LevelUp(player);
          player.learnableSkills[CustomSkill.BotteTranchanteDeMaitre].source.Add(Category.Class);

          break;
      }
    }
    public static async void DelayInspiInit(NwCreature creature)
    {
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.BotteDefensive, creature.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration));
      creature.SetFeatRemainingUses((Feat)CustomSkill.BotteTranchante, creature.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration));
    }
  }
}
