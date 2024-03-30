using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCriticalSpellDamageDiceNumber(NwCreature caster, SpellEntry spell, int numDice)
    {
      numDice *= 2;

      if (caster.KnowsFeat((Feat)CustomSkill.Broyeur) && spell.damageType == DamageType.Bludgeoning)
        numDice += 1;

      if (caster.KnowsFeat((Feat)CustomSkill.Empaleur) && spell.damageType == DamageType.Piercing)
        numDice += 1;

      return numDice;
    }
  }
}
