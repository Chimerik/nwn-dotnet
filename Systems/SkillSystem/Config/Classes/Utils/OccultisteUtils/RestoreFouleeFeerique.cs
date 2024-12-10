using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void RestoreFouleeFeerique(NwCreature creature)
    {
      var occultiste = creature.GetClassInfo((ClassType)CustomClass.Occultiste);

      if (occultiste != null)
      {
        await NwTask.NextFrame();

        byte maxUses = CreatureUtils.GetAbilityModifierMin1(creature, Ability.Charisma);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FaveurDuMalin, maxUses);

        maxUses += occultiste.GetRemainingSpellSlots(2);

        creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante, maxUses);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeProvocatrice, maxUses);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeEvanescente, maxUses);
        creature.SetFeatRemainingUses((Feat)CustomSkill.FouleeRedoutable, maxUses);
      }
    }
  }
}
