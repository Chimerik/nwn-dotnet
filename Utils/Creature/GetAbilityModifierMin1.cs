using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static byte GetAbilityModifierMin1(NwCreature creature, Ability ability)
    {
      return (byte)(creature.GetAbilityModifier(ability) > 1 ? creature.GetAbilityModifier(ability) : 1);
    }
    public static byte GetAbilityModifierMin1(NwCreature creature, Ability ability, bool rawModifier)
    {
      return (byte)(creature.GetRawAbilityScore(ability) > 9 ? (creature.GetRawAbilityScore(ability) - 10) / 2 : 1);
    }
  }
}
