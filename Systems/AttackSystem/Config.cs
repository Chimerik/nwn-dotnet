
using System.Collections.Generic;
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
      public NwCreature oTarget { get; }
      public bool isUnarmedAttack { get; }
      public bool isRangedAttack { get; }
      public NwItem attackWeapon { get; set; }
      public NwItem targetArmor { get; set; }
      public int maxBaseAC { get; set; }
      public int baseArmorPenetration { get; set; }
      public int bonusArmorPenetration { get; set; }
      public AttackPosition attackPosition { get; set; }
      public Dictionary<DamageType, int> targetAC { get; set; }


      public Context(OnCreatureAttack onAttack, NwCreature oTarget, OnCreatureDamage onDamage = null)
      {
        this.onAttack = onAttack;
        this.onDamage = onDamage;
        this.oAttacker = null;

        if (onAttack != null)
          this.oAttacker = onAttack.Attacker;
        else if (onDamage != null && onDamage.DamagedBy is NwCreature oCreature)
            this.oAttacker = oCreature;

        this.oTarget = oTarget;
        this.attackWeapon = null;
        this.targetArmor = null;
        //this.weaponBaseDamageType = new List<DamageType>(); // Slashing par défaut
        this.baseArmorPenetration = 0;
        this.bonusArmorPenetration = 0;
        this.maxBaseAC = 0;
        this.attackPosition = AttackPosition.NormalOrRanged;
        this.isUnarmedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) == null;
        this.isRangedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) != null
        && ItemUtils.GetItemCategory(oAttacker.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType) == ItemUtils.ItemCategory.RangedWeapon;
        this.targetAC = new Dictionary<DamageType, int>();
        targetAC.Add(DamageType.BaseWeapon, 0);
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
      switch(costValue)
      {
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
          return (short)costValue;
        case 6:
          return (short)NwRandom.Roll(Utils.random, 4);
        case 7:
          return (short)NwRandom.Roll(Utils.random, 6);
        case 8:
          return (short)NwRandom.Roll(Utils.random, 8);
        case 9:
          return (short)NwRandom.Roll(Utils.random, 10);
        case 10:
          return (short)NwRandom.Roll(Utils.random, 6, 2);
        case 11:
          return (short)NwRandom.Roll(Utils.random, 8, 2);
        case 12:
          return (short)NwRandom.Roll(Utils.random, 4, 2);
        case 13:
          return (short)NwRandom.Roll(Utils.random, 10, 2);
        case 14:
          return (short)NwRandom.Roll(Utils.random, 12, 1);
        case 15:
          return (short)NwRandom.Roll(Utils.random, 12, 2);
        case 16:
        case 17:
        case 18:
        case 19:
        case 20:
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
        case 26:
        case 27:
        case 28:
        case 29:
        case 30:
          return (short)(costValue - 10);
        default:
          return 0;
      }
    }
    public static int GetContextDamage(Context ctx, DamageType damageType)
    {
      if (ctx.onAttack != null)
        return ctx.onAttack.DamageData.GetDamageByType(damageType);
      else if (ctx.onDamage != null)
        return ctx.onDamage.DamageData.GetDamageByType(damageType);
      
      PlayerSystem.Log.Info("Error : trying to get damage without any event context.");
      return -1;
    }
    public static void SetContextDamage(Context ctx, DamageType damageType, int value)
    {
      if (ctx.onAttack != null)
        SetDamage(ctx.onAttack.DamageData, damageType, (short)value);
      else if (ctx.onDamage != null)
        SetDamage(ctx.onDamage.DamageData, damageType, value);
      else
        PlayerSystem.Log.Info("Error : trying to set damage without any event context.");
    }
    public static void SetDamage<T>(DamageData<T> damageData, DamageType damageType, T value) where T : unmanaged
    {
      damageData.SetDamageByType(damageType, value);
    }
    public static int GetWeaponMasteryLevel(BaseItemType baseItem, NwCreature playerCreature)
    {
      if (!PlayerSystem.Players.TryGetValue(playerCreature, out PlayerSystem.Player player))
        return 0;

      int masteryLevel = 0;

      switch (baseItem)
      {
        case BaseItemType.Shortsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortSwordMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortSwordMastery, player.learntCustomFeats[CustomFeats.ShortSwordMastery]);
          return masteryLevel;
        case BaseItemType.Battleaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.BattleAxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BattleAxeMastery, player.learntCustomFeats[CustomFeats.BattleAxeMastery]);
          return masteryLevel;
        case BaseItemType.Bastardsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.BastardsSwordMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BastardsSwordMastery, player.learntCustomFeats[CustomFeats.BastardsSwordMastery]);
          return masteryLevel;
        case BaseItemType.LightFlail:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightFlailMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightFlailMastery, player.learntCustomFeats[CustomFeats.LightFlailMastery]);
          return masteryLevel;
        case BaseItemType.Warhammer:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.WarhammerMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WarhammerMastery, player.learntCustomFeats[CustomFeats.WarhammerMastery]);
          return masteryLevel;
        case BaseItemType.HeavyCrossbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HeavyCrossbowMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HeavyCrossbowMastery, player.learntCustomFeats[CustomFeats.HeavyCrossbowMastery]);
          return masteryLevel;
        case BaseItemType.LightCrossbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightCrossbowMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightCrossbowMastery, player.learntCustomFeats[CustomFeats.LightCrossbowMastery]);
          return masteryLevel;
        case BaseItemType.Longbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LongbowMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LongbowMastery, player.learntCustomFeats[CustomFeats.LongbowMastery]);
          return masteryLevel;
        case BaseItemType.LightMace:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightMaceMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightMaceMastery, player.learntCustomFeats[CustomFeats.LightMaceMastery]);
          return masteryLevel;
        case BaseItemType.Halberd:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HalberdMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HalberdMastery, player.learntCustomFeats[CustomFeats.HalberdMastery]);
          return masteryLevel;
        case BaseItemType.TwoBladedSword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.TwoBladedSwordMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.TwoBladedSwordMastery, player.learntCustomFeats[CustomFeats.TwoBladedSwordMastery]);
          return masteryLevel;
        case BaseItemType.Shortbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortbowMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortbowMastery, player.learntCustomFeats[CustomFeats.ShortbowMastery]);
          return masteryLevel;
        case BaseItemType.Greatsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.GreatSwordMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.GreatSwordMastery, player.learntCustomFeats[CustomFeats.GreatSwordMastery]);
          return masteryLevel;
        case BaseItemType.Greataxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.GreatAxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.GreatAxeMastery, player.learntCustomFeats[CustomFeats.GreatAxeMastery]);
          return masteryLevel;
        case BaseItemType.Dagger:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DaggerMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DaggerMastery, player.learntCustomFeats[CustomFeats.DaggerMastery]);
          return masteryLevel;
        case BaseItemType.Club:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ClubMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ClubMastery, player.learntCustomFeats[CustomFeats.ClubMastery]);
          return masteryLevel;
        case BaseItemType.Dart:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DartMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DartMastery, player.learntCustomFeats[CustomFeats.DartMastery]);
          return masteryLevel;
        case BaseItemType.DireMace:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DireMaceMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DireMaceMastery, player.learntCustomFeats[CustomFeats.DireMaceMastery]);
          return masteryLevel;
        case BaseItemType.Doubleaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DoubleAxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DoubleAxeMastery, player.learntCustomFeats[CustomFeats.DoubleAxeMastery]);
          return masteryLevel;
        case BaseItemType.HeavyFlail:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HeavyFlailMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HeavyFlailMastery, player.learntCustomFeats[CustomFeats.HeavyFlailMastery]);
          return masteryLevel;
        case BaseItemType.LightHammer:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightHammerMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightHammerMastery, player.learntCustomFeats[CustomFeats.LightHammerMastery]);
          return masteryLevel;
        case BaseItemType.Handaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HandAxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HandAxeMastery, player.learntCustomFeats[CustomFeats.HandAxeMastery]);
          return masteryLevel;
        case BaseItemType.Kama:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KamaMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KamaMastery, player.learntCustomFeats[CustomFeats.KamaMastery]);
          return masteryLevel;
        case BaseItemType.Katana:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KatanaMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KatanaMastery, player.learntCustomFeats[CustomFeats.KatanaMastery]);
          return masteryLevel;
        case BaseItemType.Kukri:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KukriMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KukriMastery, player.learntCustomFeats[CustomFeats.KukriMastery]);
          return masteryLevel;
        case BaseItemType.MagicStaff:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.MagicStaffMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.MagicStaffMastery, player.learntCustomFeats[CustomFeats.MagicStaffMastery]);
          return masteryLevel;
        case BaseItemType.Morningstar:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.MorningStarMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.MorningStarMastery, player.learntCustomFeats[CustomFeats.MorningStarMastery]);
          return masteryLevel;
        case BaseItemType.Quarterstaff:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.QuarterStaffMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.QuarterStaffMastery, player.learntCustomFeats[CustomFeats.QuarterStaffMastery]);
          return masteryLevel;
        case BaseItemType.Rapier:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.RapierMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.RapierMastery, player.learntCustomFeats[CustomFeats.RapierMastery]);
          return masteryLevel;
        case BaseItemType.Scimitar:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ScimitarMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ScimitarMastery, player.learntCustomFeats[CustomFeats.ScimitarMastery]);
          return masteryLevel;
        case BaseItemType.Scythe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ScytheMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ScytheMastery, player.learntCustomFeats[CustomFeats.ScytheMastery]);
          return masteryLevel;
        case BaseItemType.ShortSpear:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortSpearMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortSpearMastery, player.learntCustomFeats[CustomFeats.ShortSpearMastery]);
          return masteryLevel;
        case BaseItemType.Shuriken:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShurikenMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShurikenMastery, player.learntCustomFeats[CustomFeats.ShurikenMastery]);
          return masteryLevel;
        case BaseItemType.Sickle:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.SickleMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SickleMastery, player.learntCustomFeats[CustomFeats.SickleMastery]);
          return masteryLevel;
        case BaseItemType.Sling:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.SlingMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SlingMastery, player.learntCustomFeats[CustomFeats.SlingMastery]);
          return masteryLevel;
        case BaseItemType.ThrowingAxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ThrowingAxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ThrowingAxeMastery, player.learntCustomFeats[CustomFeats.ThrowingAxeMastery]);
          return masteryLevel;
        case BaseItemType.Trident:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.TridentMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.TridentMastery, player.learntCustomFeats[CustomFeats.TridentMastery]);
          return masteryLevel;
        case BaseItemType.DwarvenWaraxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DwarvenWaraxeMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DwarvenWaraxeMastery, player.learntCustomFeats[CustomFeats.DwarvenWaraxeMastery]);
          return masteryLevel;
        case BaseItemType.Whip:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.WhipMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WhipMastery, player.learntCustomFeats[CustomFeats.WhipMastery]);
          return masteryLevel;
        case BaseItemType.Longsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LongSwordMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LongSwordMastery, player.learntCustomFeats[CustomFeats.LongSwordMastery]);
          return masteryLevel;
        default:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.FistMastery))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.FistMastery, player.learntCustomFeats[CustomFeats.FistMastery]);
          return masteryLevel;
      }
    }
    public static int GetWeaponCritScienceLevel(BaseItemType baseItem, NwCreature playerCreature)
    {
      if (!PlayerSystem.Players.TryGetValue(playerCreature, out PlayerSystem.Player player))
        return 0;

      int masteryLevel = 0;

      switch (baseItem)
      {
        case BaseItemType.Shortsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortSwordScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortSwordScience, player.learntCustomFeats[CustomFeats.ShortSwordScience]);
          return masteryLevel;
        case BaseItemType.Battleaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.BattleAxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BattleAxeScience, player.learntCustomFeats[CustomFeats.BattleAxeScience]);
          return masteryLevel;
        case BaseItemType.Bastardsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.BastardsSwordScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BastardsSwordScience, player.learntCustomFeats[CustomFeats.BastardsSwordScience]);
          return masteryLevel;
        case BaseItemType.LightFlail:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightFlailScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightFlailScience, player.learntCustomFeats[CustomFeats.LightFlailScience]);
          return masteryLevel;
        case BaseItemType.Warhammer:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.WarhammerScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WarhammerScience, player.learntCustomFeats[CustomFeats.WarhammerScience]);
          return masteryLevel;
        case BaseItemType.HeavyCrossbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HeavyCrossbowScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HeavyCrossbowScience, player.learntCustomFeats[CustomFeats.HeavyCrossbowScience]);
          return masteryLevel;
        case BaseItemType.LightCrossbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightCrossbowScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightCrossbowScience, player.learntCustomFeats[CustomFeats.LightCrossbowScience]);
          return masteryLevel;
        case BaseItemType.Longbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LongbowScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LongbowScience, player.learntCustomFeats[CustomFeats.LongbowScience]);
          return masteryLevel;
        case BaseItemType.LightMace:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightMaceScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightMaceScience, player.learntCustomFeats[CustomFeats.LightMaceScience]);
          return masteryLevel;
        case BaseItemType.Halberd:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HalberdScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HalberdScience, player.learntCustomFeats[CustomFeats.HalberdScience]);
          return masteryLevel;
        case BaseItemType.TwoBladedSword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.TwoBladedSwordScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.TwoBladedSwordScience, player.learntCustomFeats[CustomFeats.TwoBladedSwordScience]);
          return masteryLevel;
        case BaseItemType.Shortbow:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortbowScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortbowScience, player.learntCustomFeats[CustomFeats.ShortbowScience]);
          return masteryLevel;
        case BaseItemType.Greatsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.GreatSwordScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.GreatSwordScience, player.learntCustomFeats[CustomFeats.GreatSwordScience]);
          return masteryLevel;
        case BaseItemType.Greataxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.GreatAxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.GreatAxeScience, player.learntCustomFeats[CustomFeats.GreatAxeScience]);
          return masteryLevel;
        case BaseItemType.Dagger:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DaggerScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DaggerScience, player.learntCustomFeats[CustomFeats.DaggerScience]);
          return masteryLevel;
        case BaseItemType.Club:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ClubScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ClubScience, player.learntCustomFeats[CustomFeats.ClubScience]);
          return masteryLevel;
        case BaseItemType.Dart:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DartScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DartScience, player.learntCustomFeats[CustomFeats.DartScience]);
          return masteryLevel;
        case BaseItemType.DireMace:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DireMaceScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DireMaceScience, player.learntCustomFeats[CustomFeats.DireMaceScience]);
          return masteryLevel;
        case BaseItemType.Doubleaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DoubleAxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DoubleAxeScience, player.learntCustomFeats[CustomFeats.DoubleAxeScience]);
          return masteryLevel;
        case BaseItemType.HeavyFlail:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HeavyFlailScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HeavyFlailScience, player.learntCustomFeats[CustomFeats.HeavyFlailScience]);
          return masteryLevel;
        case BaseItemType.LightHammer:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LightHammerScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LightHammerScience, player.learntCustomFeats[CustomFeats.LightHammerScience]);
          return masteryLevel;
        case BaseItemType.Handaxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.HandAxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.HandAxeScience, player.learntCustomFeats[CustomFeats.HandAxeScience]);
          return masteryLevel;
        case BaseItemType.Kama:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KamaScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KamaScience, player.learntCustomFeats[CustomFeats.KamaScience]);
          return masteryLevel;
        case BaseItemType.Katana:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KatanaScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KatanaScience, player.learntCustomFeats[CustomFeats.KatanaScience]);
          return masteryLevel;
        case BaseItemType.Kukri:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.KukriScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.KukriScience, player.learntCustomFeats[CustomFeats.KukriScience]);
          return masteryLevel;
        case BaseItemType.MagicStaff:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.MagicStaffScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.MagicStaffScience, player.learntCustomFeats[CustomFeats.MagicStaffScience]);
          return masteryLevel;
        case BaseItemType.Morningstar:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.MorningStarScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.MorningStarScience, player.learntCustomFeats[CustomFeats.MorningStarScience]);
          return masteryLevel;
        case BaseItemType.Quarterstaff:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.QuarterStaffScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.QuarterStaffScience, player.learntCustomFeats[CustomFeats.QuarterStaffScience]);
          return masteryLevel;
        case BaseItemType.Rapier:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.RapierScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.RapierScience, player.learntCustomFeats[CustomFeats.RapierScience]);
          return masteryLevel;
        case BaseItemType.Scimitar:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ScimitarScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ScimitarScience, player.learntCustomFeats[CustomFeats.ScimitarScience]);
          return masteryLevel;
        case BaseItemType.Scythe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ScytheScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ScytheScience, player.learntCustomFeats[CustomFeats.ScytheScience]);
          return masteryLevel;
        case BaseItemType.ShortSpear:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShortSpearScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShortSpearScience, player.learntCustomFeats[CustomFeats.ShortSpearScience]);
          return masteryLevel;
        case BaseItemType.Shuriken:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ShurikenScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ShurikenScience, player.learntCustomFeats[CustomFeats.ShurikenScience]);
          return masteryLevel;
        case BaseItemType.Sickle:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.SickleScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SickleScience, player.learntCustomFeats[CustomFeats.SickleScience]);
          return masteryLevel;
        case BaseItemType.Sling:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.SlingScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SlingScience, player.learntCustomFeats[CustomFeats.SlingScience]);
          return masteryLevel;
        case BaseItemType.ThrowingAxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ThrowingAxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ThrowingAxeScience, player.learntCustomFeats[CustomFeats.ThrowingAxeScience]);
          return masteryLevel;
        case BaseItemType.Trident:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.TridentScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.TridentScience, player.learntCustomFeats[CustomFeats.TridentScience]);
          return masteryLevel;
        case BaseItemType.DwarvenWaraxe:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.DwarvenWaraxeScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.DwarvenWaraxeScience, player.learntCustomFeats[CustomFeats.DwarvenWaraxeScience]);
          return masteryLevel;
        case BaseItemType.Whip:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.WhipScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.WhipScience, player.learntCustomFeats[CustomFeats.WhipScience]);
          return masteryLevel;
        case BaseItemType.Longsword:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.LongbowScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.LongbowScience, player.learntCustomFeats[CustomFeats.LongbowScience]);
          return masteryLevel;
        default:
          if (player.learntCustomFeats.ContainsKey(CustomFeats.FistScience))
            masteryLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.FistScience, player.learntCustomFeats[CustomFeats.FistScience]);
          return masteryLevel;
      }
    }
  }
}
