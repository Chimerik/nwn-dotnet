using NWN.Core;
using NWN.API;
using NWN.API.Constants;
using System.Threading.Tasks;
using System;
using NWN.API.Events;

namespace NWN.Systems
{
  class AcidSplash
  {
    public AcidSplash(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;
      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpAcidS);

      //Make SR Check
      if (SpellUtils.MyResistSpell(onSpellCast.Caster, onSpellCast.TargetObject) == 0)
      {
        //Set damage effect
        int iDamage = 3;
        int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, NWScript.EffectLinkEffects(eVis, API.Effect.Damage(nDamage, DamageType.Acid)));
      }

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        oCaster.GetLocalVariable<int>("_AUTO_SPELL").Value = (int)onSpellCast.Spell;
        oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        oCaster.OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        Task waitMovement = NwTask.Run(async () =>
        {
          float posX = onSpellCast.Caster.Position.X;
          float posY = onSpellCast.Caster.Position.Y;
          await NwTask.WaitUntil(() => onSpellCast.Caster.Position.X != posX || onSpellCast.Caster.Position.Y != posY);

          oCaster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
          oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
          oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        });

        Task waitSpellUsed = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          SpellSystem.RestoreSpell(onSpellCast.Caster, (int)onSpellCast.Spell);
        });
      }
    }
  }
}
