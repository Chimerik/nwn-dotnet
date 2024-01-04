using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static async void RestoreTirArcanique(NwCreature creature)
    {
      byte? level = creature.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.ArcaneArcher)?.Level;

      if (!level.HasValue)
        return;

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAffaiblissant), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAgrippant), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirBannissement), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirChercheur), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirEnvoutant), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirExplosif), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirOmbres), 2);
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirPerforant), 2);
    }
  }
}
