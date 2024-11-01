using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void DecrementFouleeFeerique(NwCreature creature)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeRafraichissante);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeProvocatrice);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeEvanescente);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeRedoutable);
    }
  }
}
