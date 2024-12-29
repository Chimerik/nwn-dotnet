using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMetamagie(PlayerSystem.Player player, int customSkillId)
    {
      byte nbUses = 2;

      foreach (var metamagie in learnableDictionary.Values.Where(l => l is LearnableSkill learnable && learnable.category == Category.Metamagic))
      {
        if (player.oid.LoginCreature.KnowsFeat((Feat)metamagie.id))
        {
          nbUses = player.oid.LoginCreature.GetFeatRemainingUses((Feat)metamagie.id);
          break;
        }
      }

      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
      {
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

        byte sourceCost = customSkillId switch
        {
          CustomSkill.EnsoAmplification => 2,
          CustomSkill.EnsoIntensification or CustomSkill.EnsoAcceleration => 3,
          _ => 1,
        };

        if (sourceCost > nbUses)
          nbUses = 0;

        player.oid.LoginCreature.SetFeatRemainingUses((Feat)customSkillId, nbUses);
      }

      return true;
    }
  }
}
