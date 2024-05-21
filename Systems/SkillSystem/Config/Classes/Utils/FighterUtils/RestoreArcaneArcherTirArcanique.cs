using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static async void RestoreTirArcanique(NwCreature creature)
    {
      byte? level = GetFighterLevel(creature);

      if (!level.HasValue)
        return;

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirAffaiblissant, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirAgrippant, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirBannissement, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirChercheur, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirEnvoutant, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirExplosif, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirOmbres, 2);
      creature.SetFeatRemainingUses((Feat)CustomSkill.ArcaneArcherTirPerforant, 2);
    }
  }
}
