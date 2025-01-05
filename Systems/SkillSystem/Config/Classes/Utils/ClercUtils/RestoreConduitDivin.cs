using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void RestoreConduitDivin(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Cleric)?.Level;

      if (!level.HasValue)
        return;

      byte conduitUses = (byte)(level.Value > 17 ? 4 : level.Value > 5 ? 3 : level.Value > 1 ? 2 : 0);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(Feat.TurnUndead, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercEtincelleDivine, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercRepliqueInvoquee, conduitUses);
      //creature.SetFeatRemainingUses((Feat)CustomSkill.ClercLinceulDombre, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercRadianceDeLaube, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercCharmePlanteEtAnimaux, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercSavoirAncestral, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercDetectionDesPensees, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercFureurDestructrice, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercPreservationDeLaVie, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercBenedictionDuDieuDeLaGuerre, conduitUses);
    }
  }
}
