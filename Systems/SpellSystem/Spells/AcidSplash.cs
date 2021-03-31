using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System.Threading.Tasks;
using System;
using NWN.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    [ScriptHandler("X0_S0_AcidSplash")]
    private void HandleAcidSplash(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();

      int nCasterLevel = 15;

      if (onSpellCast.Caster is NwCreature)
        nCasterLevel = ((NwCreature)onSpellCast.Caster).LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(onSpellCast.Caster, (int)onSpellCast.Spell));
      MetaMagic nMetaMagic = (MetaMagic)NWScript.GetMetaMagicFeat();

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpAcidS);
      
      //Make SR Check
      if (SpellUtils.MyResistSpell(onSpellCast.Caster, onSpellCast.TargetObject) == 0)
      {
        //Set damage effect
        int iDamage = 3;
        int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, NWScript.EffectLinkEffects(eVis, API.Effect.Damage(nDamage, DamageType.Acid)));
      }

      if (onSpellCast.Caster is NwPlayer && nMetaMagic == MetaMagic.None)
      {
        onSpellCast.Caster.GetLocalVariable<int>("_AUTO_SPELL").Value = (int)onSpellCast.Spell;
        onSpellCast.Caster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        ((NwPlayer)onSpellCast.Caster).OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        Task waitMovement = NwTask.Run(async () =>
        {
          float posX = onSpellCast.Caster.Position.X;
          float posY = onSpellCast.Caster.Position.Y;
          await NwTask.WaitUntil(() => onSpellCast.Caster.Position.X != posX || onSpellCast.Caster.Position.Y != posY);

          onSpellCast.Caster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
          onSpellCast.Caster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
          ((NwPlayer)onSpellCast.Caster).OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        });

        Task waitSpellUsed = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          RestoreSpell(onSpellCast.Caster, (int)onSpellCast.Spell);
        });
      }
    }
  }
}
