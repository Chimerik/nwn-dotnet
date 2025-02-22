using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaleficeTag = "_MALEFICE_EFFECT";

    private static ScriptCallbackHandle onRemoveMaleficeCallback;
    public static Effect Malefice(NwCreature caster, NwSpell spell)
    {
      caster.OnCreatureAttack -= OnAttackMalefice;
      caster.OnCreatureAttack += OnAttackMalefice;

      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveMaleficeCallback);
      eff.Tag = MaleficeTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnRemoveMalefice(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      ModuleSystem.Log.Info("Malefice ON REMOVED");

      if (eventData.EffectTarget is NwCreature creature)
        creature.OnDeath -= SpellSystem.OnDeathMalefice;

      if (eventData.Effect.Creator is NwCreature caster)
      {
        ModuleSystem.Log.Info($"Maléfice creator : {caster.Name}");
        caster.OnCreatureAttack -= OnAttackMalefice;
      }

      return ScriptHandleResult.Handled;
    }

    public const string FreeMaleficeTag = "_FREE_MALEFICE_EFFECT";

    public static Effect FreeMalefice
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SkillIncrease);
        eff.Tag = FreeMaleficeTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void OnAttackMalefice(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit: 
          
          if(target.ActiveEffects.Any(e => e.Tag == MaleficeTag && e.Creator == onAttack.Attacker))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, 
              Effect.Damage(Utils.Roll(6, onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1), CustomDamageType.Necrotic)));
          }
          
          break;
      }
    }
  }
}
