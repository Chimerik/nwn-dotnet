namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetKnockdownMeleeAdvantage(string tag, bool rangedAttack)
    {
      if (rangedAttack || tag != EffectSystem.KnockdownEffectTag)
        return false;

      LogUtils.LogMessage("Avantage - Attaque de mêlée sur une cible Déstabilisée", LogUtils.LogType.Combat);
      return true;
    }
  }
}
