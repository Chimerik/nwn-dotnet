using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyChanceDebordante()
      {
        if (learnableSkills.TryGetValue(CustomSkill.ChanceDebordante, out var protection) && protection.currentLevel > 0)
        {
          if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChanceDebordanteAuraEffectTag))
          {
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.chanceDebordanteAura(oid.LoginCreature));
            UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(10);
          }
        }
      }
    }
  }
}
