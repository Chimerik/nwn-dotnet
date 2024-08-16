using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFureurTempete()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoFureurTempete)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FureurTempeteEffectTag))
          {
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurTempete(oid.LoginCreature.GetClassInfo(ClassType.Sorcerer).Level)));

          oid.LoginCreature.OnDamaged -= EnsoUtils.OnDamagedFureurTempete;
          oid.LoginCreature.OnDamaged += EnsoUtils.OnDamagedFureurTempete;
        } 
      }
    }
  }
}
