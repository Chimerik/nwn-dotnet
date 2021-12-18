using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static ScriptHandleResult ApplyFrogEffectToTarget(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (!(eventData.EffectTarget is NwCreature oTarget) || oTarget.GetObjectVariable<LocalVariableFloat>("CUSTOM_EFFECT_FROG").HasValue)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));
      oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").Value = (int)oTarget.CreatureAppearanceType;

      oTarget.CreatureAppearanceType = (AppearanceType)6396;
      oTarget.OnSpellCast -= FrogSpellMalus;
      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnCreatureDamage += FrogMalus;
      oTarget.OnSpellCastAt += FrogMalusCure;
      oTarget.OnSpellCast += FrogSpellMalus;
      
      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult RemoveFrogEffectFromTarget(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (!(eventData.EffectTarget is NwCreature oTarget))
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      if (oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").HasValue)
      {
        oTarget.CreatureAppearanceType = (AppearanceType)oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").Value;
        oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_FROG").Delete();
      }

      oTarget.OnCreatureDamage -= FrogMalus;
      oTarget.OnSpellCastAt -= FrogMalusCure;
      oTarget.OnSpellCast -= FrogSpellMalus;

      return ScriptHandleResult.Handled;
    }
    private static void FrogMalusCure(CreatureEvents.OnSpellCastAt onSpellCastAt)
    {
      switch (onSpellCastAt.Spell.SpellType)
      {
        case Spell.LesserRestoration:
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:

          foreach (Effect frogMalus in onSpellCastAt.Creature.ActiveEffects.Where(f => f.Tag == "CUSTOM_EFFECT_FROG"))
            onSpellCastAt.Creature.RemoveEffect(frogMalus);
          break;
      }
    }
    private static void FrogMalus(OnCreatureDamage onDamage)
    {
      int damage = onDamage.DamageData.Base / 4;
      if (damage < 1)
        damage = 1;
      onDamage.DamageData.Base = damage;
    }
    private static void FrogSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell.SpellType)
      {
        case Spell.Restoration:
        case Spell.GreaterRestoration:
        case Spell.RemoveCurse:
          return;
      }

      onSpellCast.PreventSpellCast = true;

      if(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
        oPC.ControllingPlayer.SendServerMessage("La métamorphose vous empêche de faire usage de magie !");
    }
  }
}
