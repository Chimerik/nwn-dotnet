using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AuraDeProtection(NwCreature caster)
    {
      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeProtectionEffectTag && e.Creator == caster))
      {
        EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.AuraDeProtectionEffectTag);
      }
      else
      {
        int paladinLevel = caster.GetClassInfo(ClassType.Paladin).Level;
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(caster, paladinLevel));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(paladinLevel < 18 ? 3 : 9);
      }
    }
  }
}
