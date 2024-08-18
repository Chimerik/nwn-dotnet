using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static async void RestoreFormeSauvage(NwCreature creature, byte nbSource = 0, bool shortRest = false)
    {
      byte? level = creature.GetClassInfo(ClassType.Druid)?.Level;

      if (!level.HasValue)
        return;

      byte maxUses = (byte)(level.Value > 16 ? 4 : level.Value > 5 ? 3 : 2);

      if (shortRest) // et qu'une utilisation a été consommé, alors on ajoute une utilisation
      {
        byte currentUses = creature.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage);
        if (currentUses < maxUses)
          nbSource = (byte)(currentUses + 1);
        else
          return;
      }
      else
        nbSource = (byte)(nbSource > 0 ? nbSource : maxUses);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage, nbSource);

    }
  }
}
