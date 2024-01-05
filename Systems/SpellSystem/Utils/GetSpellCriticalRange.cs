using System.ComponentModel.DataAnnotations;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellCriticalRange(NwCreature creature)
    {
      if (!PlayerSystem.Players.TryGetValue(creature, out var player) || player.learnableSkills.TryGetValue(CustomSkill.FighterChampion, out var champion))
        return 20;

      return champion.currentLevel > 14 ? 18 : champion.currentLevel > 2 ? 19 : 20;
    }
  }
}
