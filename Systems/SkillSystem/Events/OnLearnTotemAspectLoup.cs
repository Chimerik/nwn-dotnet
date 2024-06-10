using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTotemAspectLoup(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (player.learnableSkills.TryGetValue(CustomSkill.StealthProficiency, out var learnable))
      {
        if (learnable.currentLevel < 1)
          learnable.LevelUp(player);
      }
      else
      {
        LearnableSkill learnableSkill = new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.StealthProficiency], player);
        player.learnableSkills.Add(learnableSkill.id, learnableSkill);
        learnableSkill.LevelUp(player);

        player.oid.SendServerMessage($"Vous apprenez la maîtrise {StringUtils.ToWhitecolor(learnableSkill.name)}", ColorConstants.Orange);
      }

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.WolAspectAuraEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.wolfAspectAura));

      return true;
    }
  }
}
