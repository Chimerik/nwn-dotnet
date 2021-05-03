using NWN.Core;
using NWN.API;
using NWN.API.Constants;
using System.Threading.Tasks;
using System;
using NWN.API.Events;

namespace NWN.Systems
{
  class EletricJolt
  {
    public EletricJolt(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;
      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpLightningS);
      //Make SR Check
      if (SpellUtils.MyResistSpell(oCaster, onSpellCast.TargetObject) == 0)
      {
        //Set damage effect
        int iDamage = 3;
        API.Effect eBad = API.Effect.Damage(SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat), DamageType.Electrical);
        //Apply the VFX impact and damage effect
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eBad);
      }

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        oCaster.GetLocalVariable<int>("_AUTO_SPELL").Value = (int)onSpellCast.Spell;
        oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        oCaster.OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        Task waitMovement = NwTask.Run(async () =>
        {
          float posX = oCaster.Position.X;
          float posY = oCaster.Position.Y;
          await NwTask.WaitUntil(() => oCaster.Position.X != posX || oCaster.Position.Y != posY);

          oCaster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
          oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
          oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        });

        Task waitSpellUsed = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          SpellSystem.RestoreSpell(oCaster, onSpellCast.Spell);
        });
      }
    }
  }
}
