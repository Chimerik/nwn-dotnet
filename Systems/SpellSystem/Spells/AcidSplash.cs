using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    [ScriptHandler("X0_S0_AcidSplash")]
    private void HandleAcidSplash(CallInfo callInfo)
    {
      var oTarget = (NWScript.GetSpellTargetObject()).ToNwObject<NwGameObject>();
      var oCaster = callInfo.ObjectSelf;
      int nCasterLevel = NWScript.GetCasterLevel(oCaster);
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
      MetaMagic nMetaMagic = (MetaMagic)NWScript.GetMetaMagicFeat();

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpAcidS);
      
      //Make SR Check
      if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
      {
        //Set damage effect
        int iDamage = 3;
        int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic);
        oTarget.ApplyEffect(EffectDuration.Instant, NWScript.EffectLinkEffects(eVis, API.Effect.Damage(nDamage, DamageType.Acid)));
      }

      Task waitSpellUsed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        RestoreSpell(oCaster, NWScript.GetSpellId());
      });
    }
  }
}
