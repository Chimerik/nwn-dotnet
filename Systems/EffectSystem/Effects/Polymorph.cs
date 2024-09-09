using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PolymorphEffectTag = "_POLYMORPH_EFFECT";
    public static Effect Polymorph(NwCreature creature, PolymorphType shapeType)
    {
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));

      Effect eff = Effect.Polymorph(shapeType);
      eff.Tag = PolymorphEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      int nLvl = 0;
      int shapeHP = shapeType switch
      {
        PolymorphType.GiantSpider => 18,
        PolymorphType.Wolf => 16,
        PolymorphType.Panther => 33,
        (PolymorphType)107 => 2,
        (PolymorphType)108 => 21,
        (PolymorphType)109 => 47,
        (PolymorphType)110 => 44,
        _ => 11,
      };

      if (creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").HasNothing)
        creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value = creature.HP;

      creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SHAPE").Value = (int)shapeType;

      foreach (var level in creature.LevelInfo)
      {
        nLvl += 1;
        creature.GetObjectVariable<PersistentVariableInt>($"_SHAPECHANGE_HITDIE_LEVEL_{nLvl}").Value = level.HitDie;

        if (level.ClassInfo.Class.Id == CustomClass.Adventurer)
          level.HitDie = (byte)(shapeHP - (creature.Level - 1) * ((NwGameTables.PolymorphTable[(int)shapeType].Constitution - 10) / 2));
        else
          level.HitDie = 0;
      }

      DelayDamageBonus(creature, shapeType);
      creature.HP = creature.MaxHP;
      creature.OnEffectRemove -= OnRemovePolymorph;
      creature.OnEffectRemove += OnRemovePolymorph;
      creature.OnDamaged -= OnDamagedPolymorph;
      creature.OnDamaged += OnDamagedPolymorph;

      return eff;
    }
    public static async void DelayDamageBonus(NwCreature creature, PolymorphType shapeType)
    {
      IPDamageBonus damageBonus = 0;
      IPDamageType damageType = IPDamageType.Slashing;
      int druidLevel = creature.GetClassInfo(ClassType.Druid).Level;

      switch (shapeType)
      {
        case PolymorphType.Badger:
        case PolymorphType.Wolf:
        case PolymorphType.Panther:
        case (PolymorphType)108:
        case (PolymorphType)109:
        case (PolymorphType)110:

          damageBonus = druidLevel > 15 ? IPDamageBonus.Plus1d12 : druidLevel > 11 ? IPDamageBonus.Plus1d10 : druidLevel > 7 ? IPDamageBonus.Plus1d8 : druidLevel > 3 ? IPDamageBonus.Plus1d6 : 0;
          damageType = IPDamageType.Piercing;

          break;

        case PolymorphType.GiantSpider:

          damageBonus = druidLevel > 15 ? IPDamageBonus.Plus1d12 : druidLevel > 11 ? IPDamageBonus.Plus1d10 : druidLevel > 7 ? IPDamageBonus.Plus1d8 : druidLevel > 3 ? IPDamageBonus.Plus1d6 : 0;
          damageType = IPDamageType.Piercing;

          creature.OnCreatureAttack -= RangerUtils.OnAttackSpiderPoisonBite;
          creature.OnCreatureAttack += RangerUtils.OnAttackSpiderPoisonBite;

          break;
      }

      if (damageBonus > 0)
      {
        await NwTask.NextFrame();
        creature.GetItemInSlot(InventorySlot.CreatureLeftWeapon)?.AddItemProperty(ItemProperty.DamageBonus(damageType, damageBonus), EffectDuration.Permanent);
      } 
    }
    public static void OnRemovePolymorph(OnEffectRemove onRemove)
    {
      if (onRemove.Effect.EffectType != EffectType.Polymorph || onRemove.Object is not NwCreature creature)
        return;

      int nLvl = 0;

      foreach (var level in creature.LevelInfo)
      {
        nLvl += 1;
        level.HitDie = (byte)creature.GetObjectVariable<PersistentVariableInt>($"_SHAPECHANGE_HITDIE_LEVEL_{nLvl}").Value;
        creature.GetObjectVariable<PersistentVariableInt>($"_SHAPECHANGE_HITDIE_LEVEL_{nLvl}").Delete();
      }

      DelayHPReset(creature);
      creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SHAPE").Delete();

      creature.OnEffectRemove -= OnRemovePolymorph;
      creature.OnDamaged -= OnDamagedPolymorph;
      creature.OnCreatureAttack -= RangerUtils.OnAttackSpiderPoisonBite;
    }
    public static PolymorphType GetPolymorphType(int featId)
    {
      return featId switch
      {
        CustomSkill.FormeSauvageChat => (PolymorphType)107,
        CustomSkill.FormeSauvageAraignee => PolymorphType.GiantSpider,
        CustomSkill.FormeSauvageLoup => PolymorphType.Wolf,
        CustomSkill.FormeSauvageRothe => (PolymorphType)108,
        CustomSkill.FormeSauvagePanthere => PolymorphType.Panther,
        CustomSkill.FormeSauvageOursHibou => (PolymorphType)109,
        CustomSkill.FormeSauvageDilophosaure => (PolymorphType)110,
        _ => PolymorphType.Badger,
      };
    }
    public static async void DelayHPReset(NwCreature creature)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      if (creature is null || !creature.IsValid)
        return;

      creature.HP = creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value;
      creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Delete();

      if (creature.HP < 1)
        creature.ApplyEffect(EffectDuration.Instant, Effect.Death());

      creature.LoginPlayer?.ExportCharacter();
    }
    public static void OnDamagedPolymorph(CreatureEvents.OnDamaged onDamaged)
    {
      NwCreature creature = onDamaged.Creature;

      if (creature.HP < 1)
      {
        creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value += creature.HP;

        creature.HP = 1;
        EffectUtils.RemoveTaggedEffect(creature, creature, PolymorphEffectTag);
      }
    }
  }
}
