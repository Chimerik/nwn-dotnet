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

      byte conduitUses = (byte)(level.Value < 6 ? 1 : level.Value < 18 ? 2 : 3);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(Feat.TurnUndead, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercRepliqueInvoquee, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercLinceulDombre, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercRadianceDeLaube, conduitUses);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ClercCharmePlanteEtAnimaux, conduitUses);
    }
  }
}
