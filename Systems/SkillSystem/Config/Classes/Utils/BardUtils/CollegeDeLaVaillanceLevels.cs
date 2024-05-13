using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Bard
  {
    public static void HandleCollegeDeLaVaillanceLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(2).SetPlayerOverride(player.oid, "Collège de la Vaillance");
          player.oid.SetTextureOverride("bard", "vaillance");

          if(player.learnableSkills.TryAdd(CustomSkill.ShieldProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ShieldProficiency], player)))
            player.learnableSkills[CustomSkill.ShieldProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.ShieldProficiency].source.Add(Category.Class);

          if (player.learnableSkills.TryAdd(CustomSkill.MediumArmorProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MediumArmorProficiency], player)))
            player.learnableSkills[CustomSkill.MediumArmorProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.MediumArmorProficiency].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DefenseVaillante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DefenseVaillante], player));
          player.learnableSkills[CustomSkill.DefenseVaillante].LevelUp(player);
          player.learnableSkills[CustomSkill.DefenseVaillante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DegatsVaillants, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DegatsVaillants], player));
          player.learnableSkills[CustomSkill.DegatsVaillants].LevelUp(player);
          player.learnableSkills[CustomSkill.DegatsVaillants].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightFlailProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.BattleaxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BattleaxeProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.MorningstarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MorningstarProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.GreatswordProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreatswordProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.GreataxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.GreataxeProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.HalberdProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HalberdProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ScimitarProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.ThrowingAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ThrowingAxeProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.HeavyFlailProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyFlailProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.TridentProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TridentProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.HeavyCrossbowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HeavyCrossbowProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LongBowProficiency], player));
          player.learnableSkills.TryAdd(CustomSkill.WhipProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WhipProficiency], player));

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.VaillanceBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.VaillanceBonusAttack], player));
          player.learnableSkills[CustomSkill.VaillanceBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.VaillanceBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1;

          break;

        case 14:

          player.oid.LoginCreature.OnSpellCast -= SpellSystem.OnSpellCastMagieDeCombat;
          player.oid.LoginCreature.OnSpellCast += SpellSystem.OnSpellCastMagieDeCombat;

          break;
      }
    }
  }
}
