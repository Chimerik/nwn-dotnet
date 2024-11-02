using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static int GetFaveurDuMalinBonusSave(NwCreature caster)
    {
      int bonusSave = 0;

      if (caster is not null)
      {
        var eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.FaveurDivineEffectTag && e.IntParams[5] == CustomSpell.FaveurDuMalinJDS);

        if (eff is not null)
        {
          bonusSave = NwRandom.Roll(Utils.random, 10);
          LogUtils.LogMessage($"Faveur du Malin JDS : +{bonusSave}", LogUtils.LogType.Combat);
        }
      }

      return bonusSave;
    }
  }
}
