using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeDevotion()
      {
        if (learnableSkills.ContainsKey(CustomSkill.PaladinAuraDeDevotion)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeDevotionEffectTag))
        {
          int paladinLevel = oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level;
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(oid.LoginCreature, paladinLevel));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(paladinLevel < 18 ? 3 : 9);
        }
      }
    }
  }
}
