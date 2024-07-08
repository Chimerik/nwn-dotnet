using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAvatarDeBataille()
      {
        if (learnableSkills.ContainsKey(CustomSkill.ClercGuerreAvatarDeBataille)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AvatarDeBatailleEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AvatarDeBataille));
      }
    }
  }
}
