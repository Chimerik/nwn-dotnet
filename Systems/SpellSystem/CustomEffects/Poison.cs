using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class Poison
  {
    public Poison(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonL));

      int arenaAddedDD = 0;
      if (PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
        arenaAddedDD += (int)player.pveArena.currentDifficulty * 2;

        if (oTarget.RollSavingThrow(SavingThrow.Fortitude, 15 + arenaAddedDD, SavingThrowType.Poison) != SavingThrowResult.Failure)
        return;

      Effect poison = Effect.VisualEffect(VfxType.DurGlowGreen);
      poison.SubType = EffectSubType.Supernatural;
      poison.Tag = "CUSTOM_EFFECT_POISON";
      oTarget.ApplyEffect(EffectDuration.Permanent, poison);
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(oTarget.MaxHP / 4, DamageType.Acid));

      oTarget.OnHeartbeat -= PoisonMalus;
      oTarget.OnSpellCastAt -= PoisonMalusCure;
      oTarget.OnHeartbeat += PoisonMalus;
      oTarget.OnSpellCastAt += PoisonMalusCure;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));
      oTarget.OnHeartbeat -= PoisonMalus;
      oTarget.OnSpellCastAt -= PoisonMalusCure;
    }
    private void PoisonMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.NeutralizePoison:
          foreach (Effect poisonMalus in onSpellCastAt.Creature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_POISON"))
            onSpellCastAt.Creature.RemoveEffect(poisonMalus);
          onSpellCastAt.Creature.OnHeartbeat -= PoisonMalus;
          onSpellCastAt.Creature.OnSpellCastAt -= PoisonMalusCure;
          break;
      }
    }
    private void PoisonMalus(CreatureEvents.OnHeartbeat onHearbeat)
    {
      int hpLost = onHearbeat.Creature.MaxHP / 32;
      if (hpLost < 1)
        hpLost = 1;

      onHearbeat.Creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(hpLost, DamageType.Acid));
    }
  }
}
