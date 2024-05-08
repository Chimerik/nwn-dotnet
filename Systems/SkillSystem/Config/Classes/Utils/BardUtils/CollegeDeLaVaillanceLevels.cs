using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
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

          if (player.learnableSkills.TryAdd(CustomSkill.MartialWeaponProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.MartialWeaponProficiency], player)))
            player.learnableSkills[CustomSkill.MartialWeaponProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.MartialWeaponProficiency].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DefenseVaillante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DefenseVaillante], player));
          player.learnableSkills[CustomSkill.DefenseVaillante].LevelUp(player);
          player.learnableSkills[CustomSkill.DefenseVaillante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.DegatsVaillants, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DegatsVaillants], player));
          player.learnableSkills[CustomSkill.DegatsVaillants].LevelUp(player);
          player.learnableSkills[CustomSkill.DegatsVaillants].source.Add(Category.Class);

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
