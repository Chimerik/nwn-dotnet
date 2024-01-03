using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class FeatUtils
  {
    public static void DecrementManoeuvre(NwCreature creature)
    {
      DecrementFeatUses(creature, CustomSkill.WarMasterAttaqueMenacante);
      DecrementFeatUses(creature, CustomSkill.WarMasterAttaquePrecise);
      DecrementFeatUses(creature, CustomSkill.WarMasterBalayage);
      DecrementFeatUses(creature, CustomSkill.WarMasterRenversement);
      DecrementFeatUses(creature, CustomSkill.WarMasterDesarmement);
      DecrementFeatUses(creature, CustomSkill.WarMasterDiversion);
      DecrementFeatUses(creature, CustomSkill.WarMasterFeinte);
      DecrementFeatUses(creature, CustomSkill.WarMasterInstruction);
      DecrementFeatUses(creature, CustomSkill.WarMasterJeuDeJambe);      
      DecrementFeatUses(creature, CustomSkill.WarMasterManoeuvreTactique);      
      DecrementFeatUses(creature, CustomSkill.WarMasterParade);      
      DecrementFeatUses(creature, CustomSkill.WarMasterProvocation);      
      DecrementFeatUses(creature, CustomSkill.WarMasterRalliement);      
      DecrementFeatUses(creature, CustomSkill.WarMasterRiposte);      
      DecrementFeatUses(creature, CustomSkill.WarMasterEvaluationTactique);      
    }
  }
}
