using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeProtection()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeProtection)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeProtectionEffectTag && e.Creator == oid.LoginCreature))
        {
          int paladinLevel = oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level;
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(oid.LoginCreature, paladinLevel));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(paladinLevel < 18 ? 3 : 9);
        }
      }
    }
  }
}
