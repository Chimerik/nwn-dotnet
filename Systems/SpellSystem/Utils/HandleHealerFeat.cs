using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleHealerFeat(NwCreature caster, int dice, int nbDices = 1)
    {
      int healAmount = 0;

      if (caster.KnowsFeat((Feat)CustomSkill.Healer))
      {
        for (int i = 0; i < nbDices; i++)
        {
          int healRoll = Utils.Roll(dice);

          if (healRoll < 2)
            healRoll = Utils.Roll(dice);

          healAmount += healRoll;
        }
      }
      else
      {
        healAmount = Utils.Roll(dice, nbDices);
      }

      return healAmount;
    }
  }
}
