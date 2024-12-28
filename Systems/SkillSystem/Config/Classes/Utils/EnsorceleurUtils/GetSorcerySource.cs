using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static int GetSorcerySource(NwCreature creature)
    {
      foreach(var metamagie in SkillSystem.learnableDictionary.Values.Where(l => l is LearnableSkill learnable && learnable.category == SkillSystem.Category.Metamagic))
      {
        if (creature.KnowsFeat((Feat)metamagie.id))
          return creature.GetFeatRemainingUses((Feat)metamagie.id);
      }

      return 0;
    }
  }
}
