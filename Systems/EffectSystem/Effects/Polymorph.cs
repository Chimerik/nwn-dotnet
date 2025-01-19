using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PolymorphEffectTag = "_POLYMORPH_EFFECT";
    public const string PolymorphTempHPEffectTag = "_POLYMORPH_EFFECT";
    public static Effect Polymorph(NwCreature creature, PolymorphType shapeType)
    {
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPolymorph));
  
      Effect eff = Effect.Polymorph(shapeType);
      eff.Tag = PolymorphEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      int nLvl = 0;
      int shapeHP = shapeType switch
      {
        CustomPolymorph.Araignee => 18,
        CustomPolymorph.Loup => 16,
        CustomPolymorph.Panthere => 33,
        CustomPolymorph.Chat => 2,
        CustomPolymorph.Rothe => 21,
        CustomPolymorph.OursHibou => 47,
        CustomPolymorph.Dilophosaure => 44,
        CustomPolymorph.OursBlanc => 28,
        CustomPolymorph.Corbeau => 11,
        CustomPolymorph.Tigre => 32,
        CustomPolymorph.Air => 72,
        CustomPolymorph.Feu => 72,
        CustomPolymorph.Eau => 72,
        CustomPolymorph.Terre => 85,
        _ => 11,
      };

      if (creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").HasNothing)
        creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value = creature.HP > creature.MaxHP ? creature.MaxHP : creature.HP;

      //ModuleSystem.Log.Info($"creature HP BEFORE : {creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value}");

      creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SHAPE").Value = (int)shapeType;

      foreach (var level in creature.LevelInfo)
      {
        nLvl += 1;
        creature.GetObjectVariable<PersistentVariableInt>($"_SHAPECHANGE_HITDIE_LEVEL_{nLvl}").Value = level.HitDie;
        //ModuleSystem.Log.Info($"hit die before : {level.HitDie}");

        if (level.ClassInfo.Class.Id == CustomClass.Adventurer)
        {
          level.HitDie = (byte)(shapeHP/* - (creature.Level - 1) * ((NwGameTables.PolymorphTable[(int)shapeType].Constitution - 10) / 2)*/);
        }
        else
          level.HitDie = 0;

        //ModuleSystem.Log.Info($"hit die after : {level.HitDie}");
      }

      DelayDamageBonus(creature, shapeType);
      creature.HP = creature.MaxHP;
      creature.OnEffectRemove -= OnRemovePolymorph;
      creature.OnEffectRemove += OnRemovePolymorph;
      creature.OnDamaged -= OnDamagedPolymorph;
      creature.OnDamaged += OnDamagedPolymorph;

      if (creature.KnowsFeat((Feat)CustomSkill.DruideFormeDeLune))
      {
        creature.ApplyEffect(EffectDuration.Permanent, FormeDeLune(creature));
        creature.SetFeatRemainingUses((Feat)CustomSkill.DruideLuneRadieuse, 1);
      }
      else
      {
        var tempHP = Effect.TemporaryHitpoints(creature.GetClassInfo(ClassType.Druid).Level);
        tempHP.Tag = PolymorphTempHPEffectTag;
        tempHP.SubType = EffectSubType.Supernatural;
        creature.ApplyEffect(EffectDuration.Permanent, tempHP);
      }

      return eff;
    }
    public static async void DelayDamageBonus(NwCreature creature, PolymorphType shapeType)
    {
      IPDamageBonus damageBonus = 0;
      IPDamageType damageType = IPDamageType.Slashing;
      int druidLevel = creature.GetClassInfo(ClassType.Druid).Level;

      switch (shapeType)
      {
        case CustomPolymorph.Blaireau:
        case CustomPolymorph.Loup:
        case CustomPolymorph.Panthere:
        case CustomPolymorph.Rothe:
        case CustomPolymorph.OursHibou:
        case CustomPolymorph.Dilophosaure:
        case CustomPolymorph.OursBlanc:
        case CustomPolymorph.Corbeau:

          damageBonus = druidLevel > 15 ? IPDamageBonus.Plus1d12 : druidLevel > 11 ? IPDamageBonus.Plus1d10 : druidLevel > 7 ? IPDamageBonus.Plus1d8 : druidLevel > 3 ? IPDamageBonus.Plus1d6 : 0;
          damageType = IPDamageType.Piercing;

          break;

        case CustomPolymorph.Araignee:

          damageBonus = druidLevel > 15 ? IPDamageBonus.Plus1d12 : druidLevel > 11 ? IPDamageBonus.Plus1d10 : druidLevel > 7 ? IPDamageBonus.Plus1d8 : druidLevel > 3 ? IPDamageBonus.Plus1d6 : 0;
          damageType = IPDamageType.Piercing;

          creature.OnCreatureAttack -= RangerUtils.OnAttackSpiderPoisonBite;
          creature.OnCreatureAttack += RangerUtils.OnAttackSpiderPoisonBite;

          break;

        case CustomPolymorph.Tigre:

          damageBonus = druidLevel > 15 ? IPDamageBonus.Plus1d12 : druidLevel > 11 ? IPDamageBonus.Plus1d10 : druidLevel > 7 ? IPDamageBonus.Plus1d8 : druidLevel > 3 ? IPDamageBonus.Plus1d6 : 0;
          damageType = IPDamageType.Piercing;

          creature.OnCreatureAttack -= DruideUtils.OnAttackBriseArmure;
          creature.OnCreatureAttack += DruideUtils.OnAttackBriseArmure;

          creature.OnHeartbeat -= DruideUtils.OnHeartbeatVitaliteAnimale;
          creature.OnHeartbeat += DruideUtils.OnHeartbeatVitaliteAnimale;

          break;

        case CustomPolymorph.Air:

          creature.OnCreatureAttack -= DruideUtils.OnAttackElemAirStun;
          creature.OnCreatureAttack += DruideUtils.OnAttackElemAirStun;

          break;

        case CustomPolymorph.Terre:

          creature.OnCreatureAttack -= DruideUtils.OnAttackElemTerreKnockdown;
          creature.OnCreatureAttack += DruideUtils.OnAttackElemTerreKnockdown;

          break;

        case CustomPolymorph.Feu:

          creature.OnCreatureAttack -= DruideUtils.OnAttackElemFeuBrulure;
          creature.OnCreatureAttack += DruideUtils.OnAttackElemFeuBrulure;

          break;

        case CustomPolymorph.Eau:

          creature.OnCreatureAttack -= DruideUtils.OnAttackElemEauChill;
          creature.OnCreatureAttack += DruideUtils.OnAttackElemEauChill;

          break;
      }

      if (damageBonus > 0)
      {
        await NwTask.NextFrame();
        creature.GetItemInSlot(InventorySlot.CreatureLeftWeapon)?.AddItemProperty(ItemProperty.DamageBonus(damageType, damageBonus), EffectDuration.Permanent);
      }

      if (creature.KnowsFeat((Feat)CustomSkill.DruideLuneRadieuse))
      {
        var druidClass = creature.GetClassInfo(ClassType.Druid);

        if (druidClass is not null && druidClass.Level > 13)
        { 
          await NwTask.NextFrame();
          creature.GetItemInSlot(InventorySlot.CreatureLeftWeapon)?.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Divine, IPDamageBonus.Plus1d10), EffectDuration.Permanent);
        }
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
      creature.OnCreatureAttack -= DruideUtils.OnAttackBriseArmure;
      creature.OnCreatureAttack -= DruideUtils.OnAttackElemAirStun;
      creature.OnCreatureAttack -= DruideUtils.OnAttackElemTerreKnockdown;
      creature.OnCreatureAttack -= DruideUtils.OnAttackElemFeuBrulure;
      creature.OnCreatureAttack -= DruideUtils.OnAttackElemEauChill;
      creature.OnHeartbeat -= DruideUtils.OnHeartbeatVitaliteAnimale;

      EffectUtils.RemoveTaggedEffect(creature, PolymorphTempHPEffectTag, FormeDeLuneEffectTag);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DruideLuneRadieuse, 0);
    }
    public static PolymorphType GetPolymorphType(int featId)
    {
      return featId switch
      {
        CustomSpell.FormeSauvageChat => CustomPolymorph.Chat,
        CustomSpell.FormeSauvageAraignee => CustomPolymorph.Araignee,
        CustomSpell.FormeSauvageLoup => CustomPolymorph.Loup,
        CustomSpell.FormeSauvageRothe => CustomPolymorph.Rothe,
        CustomSpell.FormeSauvagePanthere => CustomPolymorph.Panthere,
        CustomSpell.FormeSauvageOursHibou => CustomPolymorph.OursHibou,
        CustomSpell.FormeSauvageDilophosaure => CustomPolymorph.Dilophosaure,
        CustomSkill.FormeSauvageOurs => CustomPolymorph.OursBlanc,
        CustomSkill.FormeSauvageTigre => CustomPolymorph.Tigre,
        CustomSkill.FormeSauvageAir => CustomPolymorph.Air,
        CustomSkill.FormeSauvageFeu => CustomPolymorph.Feu,
        CustomSkill.FormeSauvageEau => CustomPolymorph.Eau,
        CustomSkill.FormeSauvageTerre => CustomPolymorph.Terre,
        CustomSkill.FormeSauvageCorbeau => CustomPolymorph.Corbeau,
        _ => CustomPolymorph.Blaireau,
      };
    }
    public static async void DelayHPReset(NwCreature creature)
    {
      //ModuleSystem.Log.Info($"REMOVE : creature HP BEFORE DELAY : {creature.HP}");
      //ModuleSystem.Log.Info($"REMOVE : creature MAX HP BEFORE value change : {creature.MaxHP}");
      await NwTask.Delay(TimeSpan.FromSeconds(0.6));

      if (creature is null || !creature.IsValid)
        return;

      //ModuleSystem.Log.Info($"REMOVE : creature MAX HP AFTER value change : {creature.MaxHP}");

      //ModuleSystem.Log.Info($"REMOVE : creature HP : {creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value}");
      //ModuleSystem.Log.Info($"REMOVE : creature HP AFTER DELAY : {creature.HP}");

      creature.HP = creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value;
      //ModuleSystem.Log.Info($"REMOVE : creature HP AFTER RESET : {creature.HP}");
      creature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Delete();
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
