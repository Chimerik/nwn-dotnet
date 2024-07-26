using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetPreparableSpellsCount(PlayerSystem.Player player, NwClass selectedClass)
    {
      int alwaysPreparedSpells = selectedClass.ClassType switch
      {
        ClassType.Paladin => player.learnableSpells.Values.Where(s => s.paladinSerment).Count(),
        ClassType.Cleric => player.learnableSpells.Values.Where(s => s.clericDomain).Count(),
        _ => 0,
      };

      int classLevel = selectedClass.ClassType == ClassType.Paladin ? player.oid.LoginCreature.GetClassInfo(selectedClass).Level / 2 : player.oid.LoginCreature.GetClassInfo(selectedClass).Level;
      return player.oid.LoginCreature.GetAbilityModifier(selectedClass.SpellCastingAbility) + classLevel + alwaysPreparedSpells;
    }
  }
}
