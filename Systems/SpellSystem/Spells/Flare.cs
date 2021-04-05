using NWN.Core;
using NWN.API;
using NWN.API.Events;
using System.Threading.Tasks;
using System;
using NWN.API.Constants;

namespace NWN.Systems
{
  class Flare
  {
    public Flare(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;
      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FLAME_S);

      // * Apply the hit effect so player knows something happened
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, onSpellCast.TargetObject);

      //Make SR Check
      if ((SpellUtils.MyResistSpell(oCaster, onSpellCast.TargetObject)) == 0 && SpellUtils.MySavingThrow(NWScript.SAVING_THROW_FORT, onSpellCast.TargetObject, onSpellCast.SaveDC) == 0) // 0 = failed
      {
        //Set damage effect
        Core.Effect eBad = NWScript.EffectAttackDecrease(1 + nCasterLevel / 6);
        //Apply the VFX impact and damage effect
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eBad, onSpellCast.TargetObject, NWScript.RoundsToSeconds(10 + 10 * nCasterLevel / 6));
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
