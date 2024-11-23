using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static byte GetAbilityModifierMin1(NwCreature creature, Ability ability)
    {
      return (byte)(creature.GetAbilityModifier(Ability.Wisdom) > 1 ? creature.GetAbilityModifier(Ability.Wisdom) : 1);
    }
  }
}
