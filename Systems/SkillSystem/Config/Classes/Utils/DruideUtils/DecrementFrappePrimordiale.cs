using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void DecrementFrappePrimordiale(NwCreature creature)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFrappePrimordialeFroid);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFrappePrimordialeFeu);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFrappePrimordialeElec);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.DruideFrappePrimordialeTonnerre);
    }
  }
}
