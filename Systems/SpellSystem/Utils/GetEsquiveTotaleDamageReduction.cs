using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetEsquiveTotaleDamageReduction(NwCreature creature, int damage, bool saveFailed)
    {
      if (creature.KnowsFeat(Feat.ImprovedEvasion))
      {
        creature?.LoginPlayer.DisplayFloatingTextStringOnCreature(creature, "Esquive Totale".ColorString(StringUtils.gold));

        if (saveFailed)
        {
          damage /= 2;
          LogUtils.LogMessage($"Esquive totale - JDS échoué : dégâts {damage}", LogUtils.LogType.Combat);
        }
        else
        {
          damage = 0;
          LogUtils.LogMessage($"EEsquive totale - JDS réussi : dégâts {damage}", LogUtils.LogType.Combat);
        }
      }

      return damage;
    }
  }
}
