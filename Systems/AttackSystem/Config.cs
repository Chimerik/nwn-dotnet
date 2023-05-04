
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class Config
  {
    public class Context
    {
      public OnCreatureAttack onAttack { get; set; }
      public OnCreatureDamage onDamage { get; set; }
      //public List<DamageType> weaponBaseDamageType { get; set; }
      public NwCreature oAttacker { get; }
      public PlayerSystem.Player attackingPlayer { get; }
      public NwCreature oTarget { get; }
      public PlayerSystem.Player targetPlayer { get; }
      public bool isUnarmedAttack { get; }
      public bool isRangedAttack { get; }
      public NwItem attackWeapon { get; set; }
      public NwItem targetArmor { get; set; }
      //public int maxBaseAC { get; set; }
      public int baseArmorPenetration { get; set; }
      public int bonusArmorPenetration { get; set; }
      public AttackPosition attackPosition { get; set; }
      public Dictionary<DamageType, int> targetAC { get; set; }
      public int physicalReduction { get; set; }


      public Context(OnCreatureAttack onAttack, NwCreature oTarget, OnCreatureDamage onDamage = null)
      {
        this.onAttack = onAttack;
        this.onDamage = onDamage;
        this.oAttacker = null;

        if (onAttack != null)
        {
          this.oAttacker = onAttack.Attacker;
          this.attackingPlayer = PlayerSystem.Players.TryGetValue(oAttacker, out PlayerSystem.Player attackerPlayer) ? attackerPlayer : null;
        }

        else if (onDamage != null && onDamage.DamagedBy is NwCreature oCreature)
        {
          this.oAttacker = oCreature;
          this.attackingPlayer = PlayerSystem.Players.TryGetValue(oAttacker, out PlayerSystem.Player attackerPlayer) ? attackerPlayer : null;
        }

        this.oTarget = oTarget;
        this.targetPlayer = PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player defendingPlayer) ? defendingPlayer : null;

        this.attackWeapon = null;
        this.targetArmor = null;
        //this.weaponBaseDamageType = new List<DamageType>(); // Slashing par défaut
        this.baseArmorPenetration = 0;
        this.bonusArmorPenetration = 0;
        //this.maxBaseAC = 0;
        this.attackPosition = AttackPosition.NormalOrRanged;
        this.isUnarmedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) == null;
        //PlayerSystem.Log.Info($"config - unarmed {isUnarmedAttack}");
        //PlayerSystem.Log.Info($"config - oAttacker {oAttacker}");
        //PlayerSystem.Log.Info($"config - right hand {oAttacker.GetItemInSlot(InventorySlot.RightHand)}");
        this.isRangedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) != null
        && ItemUtils.GetItemCategory(oAttacker.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType) == ItemUtils.ItemCategory.RangedWeapon;
        this.targetAC = new Dictionary<DamageType, int>();
        targetAC.Add(DamageType.BaseWeapon, 0);
        this.physicalReduction = 0;
      }
    }
    public enum AttackPosition
    {
      Low = 1,
      NormalOrRanged,
      High,
    }
    public static int GetIPSpecificAlignmentSubTypeAsInt(NwCreature oCreature)
    {
      return (oCreature.LawChaosAlignment + "_" + oCreature.GoodEvilAlignment) switch
      {
        "Lawful_Good" => 0,
        "Lawful_Neutral" => 1,
        "Lawful_Evil" => 2,
        "Neutral_Good" => 3,
        "Neutral_Neutral" => 4,
        "Neutral_Evil" => 5,
        "Chaotic_Good" => 6,
        "Chaotic_Neutral" => 7,
        "Chaotic_Evil" => 8,
        _ => 4,
      };
    }
    public static short RollDamage(int costValue)
    {
      return costValue switch
      {
        1 or 2 or 3 or 4 or 5 => (short)costValue,
        6 => (short)NwRandom.Roll(Utils.random, 4),
        7 => (short)NwRandom.Roll(Utils.random, 6),
        8 => (short)NwRandom.Roll(Utils.random, 8),
        9 => (short)NwRandom.Roll(Utils.random, 10),
        10 => (short)NwRandom.Roll(Utils.random, 6, 2),
        11 => (short)NwRandom.Roll(Utils.random, 8, 2),
        12 => (short)NwRandom.Roll(Utils.random, 4, 2),
        13 => (short)NwRandom.Roll(Utils.random, 10, 2),
        14 => (short)NwRandom.Roll(Utils.random, 12, 1),
        15 => (short)NwRandom.Roll(Utils.random, 12, 2),
        16 or 17 or 18 or 19 or 20 or 21 or 22 or 23 or 24 or 25 or 26 or 27 or 28 or 29 or 30 => (short)(costValue - 10),
        _ => 0,
      };
    }
    public static int GetContextDamage(Context ctx, DamageType damageType)
    {
      if (ctx.onAttack != null)
        return ctx.onAttack.DamageData.GetDamageByType(damageType);
      else if (ctx.onDamage != null)
        return ctx.onDamage.DamageData.GetDamageByType(damageType);

      LogUtils.LogMessage("ERROR : trying to get damage without any event context.", LogUtils.LogType.Combat);
      return -1;
    }
    public static void SetContextDamage(Context ctx, DamageType damageType, int value)
    {
      if (ctx.onAttack != null)
        SetDamage(ctx.onAttack.DamageData, damageType, (short)value);
      else if (ctx.onDamage != null)
        SetDamage(ctx.onDamage.DamageData, damageType, value);
      else
        LogUtils.LogMessage("ERROR : trying to set damage without any event context.", LogUtils.LogType.Combat);
    }
    public static void SetDamage<T>(DamageData<T> damageData, DamageType damageType, T value) where T : unmanaged
    {
      damageData.SetDamageByType(damageType, value);
    }
    public static void GetArmorValueFromArmorPiece(Context ctx, NwItem item, PlayerSystem.Player player, NwCreature attacker)
    {
      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
        ctx.targetAC[DamageType.BaseWeapon] = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

      if (!ctx.targetAC.TryAdd((DamageType)8192, item.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value))
        ctx.targetAC[(DamageType)8192] = item.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value;

      if (!ctx.targetAC.TryAdd((DamageType)16384, item.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value))
        ctx.targetAC[(DamageType)16384] = item.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value;


      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (Spells2da.spellTable.GetRow(item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value).inscriptionSkill)
        {
          case CustomInscription.Cuirassé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.Absorption:
            if (ctx.physicalReduction < 3)
              ctx.physicalReduction += 1;
            break;

          case CustomInscription.Prismatique:
            if (player.GetAirMagicSkillScore() > 14 && player.GetFireMagicSkillScore() > 14 && player.GetWaterMagicSkillScore() > 14 && player.GetAirMagicSkillScore() > 14)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Artisan:

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = player.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && SkillSystem.learnableDictionary.ContainsKey(slot.Param1) && ((LearnableSkill)SkillSystem.learnableDictionary[slot.Param1]).type == SkillSystem.Type.Signet)
                ctx.targetAC[DamageType.BaseWeapon] += 1;
            }
            break;

          case CustomInscription.GardeDragon:
            if (attacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeExtérieur:
            if (attacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAberration:
            if (attacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Inflexible: ctx.targetAC[(DamageType)8192] += 1; break;
          case CustomInscription.Redoutable: ctx.targetAC[(DamageType)16384] += 1; break;
          case CustomInscription.Marchevent:

            int positiveEffect = 0;
            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
            {
              if (eff.Tag.Contains("CUSTOM_POSITIVE_SPELL_"))
              {
                positiveEffect++;
                if (positiveEffect > 2)
                  ctx.targetAC[DamageType.BaseWeapon] += 1;

                if (positiveEffect > 5)
                  break;
              }
            }

            break;

          case CustomInscription.GardeGeant:
            if (attacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMagie:
            if (attacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeBon:
            if (attacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeChaos:
            if (attacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeMal:
            if (attacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeNeutre:
            if (attacker.GoodEvilAlignment == Alignment.Neutral && attacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeLoi:
            if (attacker.LawChaosAlignment == Alignment.Lawful)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Hivernal: ctx.targetAC[DamageType.Cold] += 2; break;
          case CustomInscription.Ignifugé: ctx.targetAC[DamageType.Fire] += 2; break;
          case CustomInscription.Paratonnerre: ctx.targetAC[DamageType.Electrical] += 2; break;
          case CustomInscription.Tectonique: ctx.targetAC[DamageType.Acid] += 2; break;
          case CustomInscription.Infiltrateur: ctx.targetAC[DamageType.Piercing] += 1; break;
          case CustomInscription.Saboteur: ctx.targetAC[DamageType.Slashing] += 1; break;
          case CustomInscription.AvantGarde: ctx.targetAC[DamageType.Bludgeoning] += 1; break;
          case CustomInscription.GardeHalfelin:
            if (attacker.Race.RacialType == RacialType.Halfling)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumain:
            if (attacker.Race.RacialType == RacialType.Human)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiElfe:
            if (attacker.Race.RacialType == RacialType.HalfElf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiOrc:
            if (attacker.Race.RacialType == RacialType.HalfOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeElfe:
            if (attacker.Race.RacialType == RacialType.Elf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGnome:
            if (attacker.Race.RacialType == RacialType.Gnome)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeNain:
            if (attacker.Race.RacialType == RacialType.Dwarf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Agitateur:
            if (player.oid.LoginCreature.AttackTarget is not null && (player.oid.LoginCreature.AnimationState != AnimationState.Walking
              || player.oid.LoginCreature.AnimationState != AnimationState.Running))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Sentinelle:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Belluaire:
            if (player.oid.LoginCreature.Faction.GetMembers().Any(a => a.Master == player.oid.LoginCreature && a.Tag.Contains("_ANIMAL_COMPANION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Eclaireur:
            if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_IS_USING_PREPARATION").HasValue)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Disciple:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_CONDITION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Virtuose:
            if (player.oid.LoginCreature.AnimationState == AnimationState.Cast1 || player.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast3 || player.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast5 || player.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || player.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || player.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Fossoyeur:
            double hpLeft = player.oid.LoginCreature.HP / player.MaxHP;

            if (hpLeft > 0.6)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            else if (hpLeft > 0.4)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            else if (hpLeft > 0.2)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            else
              ctx.targetAC[DamageType.BaseWeapon] += 4;

            break;

          case CustomInscription.Prodige:

            int nbSkillInCD = 0;

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = player.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && player.oid.LoginCreature.GetFeatRemainingUses(NwFeat.FromFeatId(slot.Param1)) < 1)
                nbSkillInCD += 1;
            }

            switch (nbSkillInCD)
            {
              case 2: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 4: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 6: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.Destructeur:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Bénédiction:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Centurion:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_SHOUT_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Oublié:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => !e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.GardeNonVie:
            if (attacker.Race.RacialType == RacialType.Undead)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeArtifice:
            if (attacker.Race.RacialType == RacialType.Construct)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeOrc:
            if (attacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Lieutenant:
            ctx.targetAC[DamageType.BaseWeapon] -= 4;
            break;

          case CustomInscription.MaîtreBlème:
            int controlledUndead = player.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == player.oid.LoginCreature && a.Race.RacialType == RacialType.Undead);

            switch (controlledUndead)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.Marionnettiste:
            int controlledSummons = player.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == player.oid.LoginCreature && a.Race.RacialType != RacialType.Undead && !a.Tag.Contains("_ANIMAL_COMPANION_"));

            switch (controlledSummons)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.GardeMonstre:
            if (attacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumanoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMétamorphe:
            if (attacker.Race.RacialType == RacialType.ShapeChanger)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGoblinoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAnimal:
            if (attacker.Race.RacialType == RacialType.Animal)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeReptilien:
            if (attacker.Race.RacialType == RacialType.HumanoidReptilian)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeVermine:
            if (attacker.Race.RacialType == RacialType.Vermin)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;
        }
      }
    }
    public static void GetArmorValueFromShield(Context ctx, NwItem item, PlayerSystem.Player player, NwCreature attacker)
    {
      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
        ctx.targetAC[DamageType.BaseWeapon] = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

      if (!ctx.targetAC.TryAdd(DamageType.Piercing, item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value))
        ctx.targetAC[DamageType.Piercing] = item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (Spells2da.spellTable.GetRow(item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value).inscriptionSkill)
        {
          case CustomInscription.Blindé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.RepousseDragon:
            if (attacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseExtérieur:
            if (attacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseAberration:
            if (attacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.LongueVieAuRoi:
            if (player.oid.LoginCreature.HP > player.MaxHP / 2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.LaFoiEstMonBouclier:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.LaSurvieDuMieuxEquipé:
            ctx.targetAC[(DamageType)8192] += 1;
            break;

          case CustomInscription.ParéEnTouteSaison:
            ctx.targetAC[(DamageType)16384] += 1;
            break;

          case CustomInscription.RepousseGéant:
            if (attacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseMagie:
            if (attacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseBon:
            if (attacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseChaos:
            if (attacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMal:
            if (attacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseNeutre:
            if (attacker.GoodEvilAlignment == Alignment.Neutral && attacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseLoi:
            if (attacker.LawChaosAlignment == Alignment.Lawful)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;
        }
      }
    }
  }
}
