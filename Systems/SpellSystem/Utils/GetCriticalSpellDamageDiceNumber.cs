using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetCriticalSpellDamageDiceNumber(NwGameObject oCaster, SpellEntry spell, int numDice)
    {
      numDice *= 2;

      if (oCaster is not NwCreature caster)
        return numDice;

      if (caster.KnowsFeat((Feat)CustomSkill.Broyeur) && spell.damageType.Contains(DamageType.Bludgeoning))
        numDice += 1;

      if (caster.KnowsFeat((Feat)CustomSkill.Empaleur) && spell.damageType.Contains(DamageType.Piercing))
        numDice += 1;

      return numDice;
    }
  }
}
