using NWN.Core;
using NWN.API;
using NWN.API.Constants;
using System;
using System.Threading.Tasks;
using NWN.API.Events;

namespace NWN.Systems
{
  class RayOfFrost
  {
    public RayOfFrost(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      API.Effect eVis = API.Effect.VisualEffect(VfxType.ImpFrostS);
      API.Effect eRay = API.Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand);

      //Make SR Check
      if (SpellUtils.MyResistSpell(oCaster, onSpellCast.TargetObject) == 0)
      {
        int nDamage = SpellUtils.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
        //Set damage effect
        API.Effect eDam = API.Effect.Damage(nDamage, DamageType.Cold);
        //Apply the VFX impact and damage effect
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eDam);
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eRay, TimeSpan.FromSeconds(1.7));

      if (oCaster is NwPlayer && onSpellCast.MetaMagicFeat == MetaMagic.None)
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
