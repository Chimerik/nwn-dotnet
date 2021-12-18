using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static void Invisibility(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      int nDuration = nCasterLevel;
      Effect eInvis = Effect.Invisibility(InvisibilityType.Normal);
      Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
      Effect eLink = Effect.LinkEffects(eInvis, eDur);

      eLink = Effect.LinkEffects(eLink, Effect.AreaOfEffect((PersistentVfxType)193, null, scriptHandleFactory.CreateUniqueHandler(HandleInvisibiltyHeartBeat)));  // 193 = AoE 20 m

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eInvis, NwTimeSpan.FromRounds(nDuration));
    }
  }
}
