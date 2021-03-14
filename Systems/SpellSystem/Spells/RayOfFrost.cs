using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System;
using System.Threading.Tasks;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    [ScriptHandler("NW_S0_RayFrost")]
    private void HandleRayOfFrost(CallInfo callInfo)
    {
      var oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwGameObject>();
      var oCaster = (NwGameObject)callInfo.ObjectSelf;
      int nCasterLevel = NWScript.GetCasterLevel(oCaster);
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
      MetaMagic nMetaMagic = (MetaMagic)NWScript.GetMetaMagicFeat();

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpFrostS);
      API.Effect eRay = API.Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand);

      //Make SR Check
      if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
      {
        int nDamage = SpellUtils.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, nMetaMagic);
        //Set damage effect
        API.Effect eDam = API.Effect.Damage(nDamage, DamageType.Cold);
        //Apply the VFX impact and damage effect
        oTarget.ApplyEffect(EffectDuration.Instant, eVis);
        oTarget.ApplyEffect(EffectDuration.Instant, eDam);
      }

      oTarget.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(1.7));

      Task waitSpellUsed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        RestoreSpell(oCaster, NWScript.GetSpellId());
      });
    }
  }
}
