using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static async void RestoreManoeuvres(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Fighter)?.Level;

      if (!level.HasValue)
        return;

      byte featUse = (byte)(level.Value > 14 ? 6 : level.Value > 6 ? 5 : 4);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterAttaqueMenacante, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterAttaquePrecise, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterBalayage, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterRenversement, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterDesarmement, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterDiversion, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterFeinte, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterInstruction, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterJeuDeJambe, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterManoeuvreTactique, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterParade, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterProvocation, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterRalliement, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterRiposte, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.WarMasterEvaluationTactique, featUse);
    }
  }
}
