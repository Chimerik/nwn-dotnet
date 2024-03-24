using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetPreparableSpellsCount(NwCreature creature, NwClass selectedClass)
    {
      return creature.GetAbilityModifier(selectedClass.SpellCastingAbility) + creature.GetClassInfo(selectedClass).Level;
    }
  }
}
