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
    [ScriptHandler("X0_S0_ElecJolt")]
    private void HandleElectricJolt(CallInfo callInfo)
    {
      var oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwGameObject>();
      var oCaster = (NwGameObject)callInfo.ObjectSelf;
      int nCasterLevel = NWScript.GetCasterLevel(oCaster);
      int spellId = NWScript.GetSpellId();
      NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, spellId));
      MetaMagic nMetaMagic = (MetaMagic)NWScript.GetMetaMagicFeat();

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpLightningS);
      //Make SR Check
      if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
      {
        //Set damage effect
        int iDamage = 3;
        API.Effect eBad = API.Effect.Damage(SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic), DamageType.Electrical);
        //Apply the VFX impact and damage effect
        oTarget.ApplyEffect(EffectDuration.Instant, eVis);
        oTarget.ApplyEffect(EffectDuration.Instant, eBad);
      }

      if (oCaster is NwPlayer && nMetaMagic == MetaMagic.None)
      {
        oCaster.GetLocalVariable<int>("_AUTO_SPELL").Value = spellId;
        oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value = oTarget;
        ((NwPlayer)oCaster).OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        Task waitMovement = NwTask.Run(async () =>
        {
          float posX = oCaster.Position.X;
          float posY = oCaster.Position.Y;
          await NwTask.WaitUntil(() => oCaster.Position.X != posX || oCaster.Position.Y != posY);

          oCaster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
          oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
          ((NwPlayer)oCaster).OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        });

        Task waitSpellUsed = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          RestoreSpell(oCaster, spellId);
        });
      }
    }
  }
}
