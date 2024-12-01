using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void DecrementFouleeFeerique(NwCreature creature, NwFeat feat)
    {
      var occultisteClass = creature.GetClassInfo((ClassType)CustomClass.Occultiste);
      byte? occultisteLevel = occultisteClass?.Level;

      if(occultisteLevel.HasValue)
      {
        if (feat is not null && Utils.In(feat.Id, CustomSkill.FouleeEvanescente, CustomSkill.FouleeProvocatrice, CustomSkill.FouleeRafraichissante, CustomSkill.FouleeRedoutable))
        {
          var maxSpellSlot = NwClass.FromClassId(CustomClass.Occultiste).SpellGainTable[occultisteLevel.Value][2];

          if (creature.GetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante) <= maxSpellSlot)
          {
            byte remainingSlots = occultisteClass.GetRemainingSpellSlots(1);

            for (byte i = 1; i < 10; i++)
              occultisteClass.SetRemainingSpellSlots(i, remainingSlots);
          }
        }

        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeRafraichissante);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeProvocatrice);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeEvanescente);
        creature.DecrementRemainingFeatUses((Feat)CustomSkill.FouleeRedoutable);
      }
    }
  }
}
