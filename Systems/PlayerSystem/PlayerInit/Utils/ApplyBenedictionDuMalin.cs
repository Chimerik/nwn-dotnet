using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBenedictionDuMalin()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BenedictionDuMalin)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BenedictionDuMalinAuraEffectTag))
        {
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BenedictionDuMalinAura(oid.LoginCreature));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(4);

          oid.LoginCreature.OnCreatureDamage -= OccultisteUtils.OnDamageBenedictionDuMalin;
          oid.LoginCreature.OnCreatureDamage += OccultisteUtils.OnDamageBenedictionDuMalin;
        }
      }
    }
  }
}
