using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeGarde()
      {
        if (learnableSkills.ContainsKey(CustomSkill.PaladinAuraDeGarde)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeGardeEffectTag))
        {
          int paladinLevels = oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level;
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeGarde(oid.LoginCreature, paladinLevels));
          UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(paladinLevels < 18 ? 3 : 9);
        }
      }
    }
  }
}
