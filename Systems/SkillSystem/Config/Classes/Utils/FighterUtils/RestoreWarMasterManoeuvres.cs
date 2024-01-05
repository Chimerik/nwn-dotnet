using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static async void RestoreManoeuvres(NwCreature creature)
    {
      byte? level = creature.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Fighter)?.Level;

      if (!level.HasValue)
        return;

      byte featUse = (byte)(level.Value > 14 ? 6 : level.Value > 6 ? 5 : 4);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterAttaquePrecise), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterBalayage), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRenversement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterDesarmement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterDiversion), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterFeinte), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterInstruction), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterJeuDeJambe), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterManoeuvreTactique), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterParade), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterProvocation), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRalliement), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterRiposte), featUse);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WarMasterEvaluationTactique), featUse);
    }
  }
}
