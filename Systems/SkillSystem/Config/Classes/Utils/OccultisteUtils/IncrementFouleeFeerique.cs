using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void IncrementFouleeFeerique(NwCreature creature, SpellSchool spellSchool, byte spellLevel, NwFeat feat)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.FouleeEnjoleuse) && feat is null && Utils.In(spellSchool, SpellSchool.Illusion, SpellSchool.Enchantment) && 0 < spellLevel && spellLevel < 10)
      {
        await NwTask.NextFrame();

        creature.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeRafraichissante);
        creature.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeProvocatrice);
        creature.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeEvanescente);
        creature.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeRedoutable);
      }
    }
  }
}
