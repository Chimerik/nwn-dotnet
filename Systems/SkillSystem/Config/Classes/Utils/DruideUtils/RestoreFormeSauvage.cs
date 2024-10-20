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
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageBlaireau, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageChat, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageAraignee, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageLoup, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageRothe, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvagePanthere, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageOursHibou, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageDilophosaure, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DruideAssistanceTerrestre, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DruideSanctuaireNaturel, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageOurs, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageCorbeau, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageTigre, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageAir, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageTerre, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageFeu, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.FormeSauvageEau, nbSource);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DruideFureurDesFlots, nbSource);
    }
  }
}
