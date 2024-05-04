using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleNecromancieLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 2: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Nécromancien");
          player.oid.SetTextureOverride("wizard", "necromancie");

          player.learnableSkills.TryAdd(CustomSkill.NecromancieMoissonDuFiel, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NecromancieMoissonDuFiel], player));
          player.learnableSkills[CustomSkill.NecromancieMoissonDuFiel].LevelUp(player);
          player.learnableSkills[CustomSkill.NecromancieMoissonDuFiel].source.Add(Category.Class);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.NecromancieUndeadThralls, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NecromancieUndeadThralls], player));
          player.learnableSkills[CustomSkill.NecromancieUndeadThralls].LevelUp(player);
          player.learnableSkills[CustomSkill.NecromancieUndeadThralls].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue((int)Spell.AnimateDead, out var learnable))
          {
            if (!learnable.learntFromClasses.Any(c => c == (int)ClassType.Wizard))
            {
              learnable.learntFromClasses.Add((int)ClassType.Wizard);

              if (learnable.currentLevel < 1)
                learnable.LevelUp(player);
            }
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.AnimateDead], (int)ClassType.Wizard);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);

            player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Animation des Morts")}", ColorConstants.Orange);
          }

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.NecromancieInsensible, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NecromancieInsensible], player));
          player.learnableSkills[CustomSkill.NecromancieInsensible].LevelUp(player);
          player.learnableSkills[CustomSkill.NecromancieInsensible].source.Add(Category.Class);

          NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.NecroticResistance));

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.NecromancieUndeadControl, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.NecromancieUndeadControl], player));
          player.learnableSkills[CustomSkill.NecromancieUndeadControl].LevelUp(player);
          player.learnableSkills[CustomSkill.NecromancieUndeadControl].source.Add(Category.Class);

          break;
      }
    }
  }
}
