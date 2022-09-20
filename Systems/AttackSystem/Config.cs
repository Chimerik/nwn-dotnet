
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
      //public int maxBaseAC { get; set; }
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
  }
}
