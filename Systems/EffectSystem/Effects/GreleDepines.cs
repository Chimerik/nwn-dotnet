using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GreleDepinesEffectTag = "_GRELE_DEPINES_EFFECT";
    private static ScriptCallbackHandle onRemoveGreleDepinesCallback;
    public static Effect GreleDepines(NwCreature caster, NwSpell spell, Ability dcAbility)
    {
      caster.OnCreatureAttack -= OnAttackGreleDepines;
      caster.OnCreatureAttack += OnAttackGreleDepines;

      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveGreleDepinesCallback);
      eff.Tag = GreleDepinesEffectTag;
      eff.IntParams[5] = (int)dcAbility;
      eff.SubType = EffectSubType.Supernatural;
      eff.Spell = spell;

      return eff;
    }
    private static ScriptHandleResult OnRemoveGreleDepines(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is  NwCreature creature)
        creature.OnCreatureAttack -= OnAttackGreleDepines;

      return ScriptHandleResult.Handled;
    }
    private static void OnAttackGreleDepines(OnCreatureAttack onAttack)
    {
      if (onAttack.IsRangedAttack && onAttack.Target is NwCreature target)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

            SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.GreleDepines];
            NwSpell spell = NwSpell.FromSpellId(CustomSpell.GreleDepines);
            int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, (Ability)onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == GreleDepinesEffectTag).IntParams[5]);

            foreach (NwCreature spellTarget in target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
            {
              SavingThrowResult result = CreatureUtils.GetSavingThrow(onAttack.Attacker, target, spellEntry.savingThrowAbility, spellDC);
              
              if(result == SavingThrowResult.Failure)
              {
                SpellUtils.DealSpellDamage(spellTarget, 1, spellEntry, SpellUtils.GetSpellDamageDiceNumber(onAttack.Attacker, spell), onAttack.Attacker, 1, result);
                spellTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
              }
            }

            EffectUtils.RemoveTaggedEffect(onAttack.Attacker, GreleDepinesEffectTag);
            

            break;
        }
      }
    }
  }
}

