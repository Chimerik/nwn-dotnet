using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static void HandlePlayerAttackedEvent(CreatureEvents.OnPhysicalAttacked onAttacked)
    {
      if (PlayerSystem.Players.TryGetValue(onAttacked.Creature, out PlayerSystem.Player player))
      {
        // La cible de l'attaque est un joueur, on fait diminuer la durabilité

        int dexBonus = onAttacked.Creature.GetAbilityModifier(Ability.Dexterity) - (CreaturePlugin.GetArmorCheckPenalty(onAttacked.Creature) + CreaturePlugin.GetShieldCheckPenalty(onAttacked.Creature));
        if (dexBonus < 0)
          dexBonus = 0;

        int safetyLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
          safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, player.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

        int durabilityRate = 30 - dexBonus - safetyLevel;
        if (durabilityRate < 1)
          durabilityRate = 1;

        if (NwRandom.Roll(Utils.random, 100, 1) > 1 && NwRandom.Roll(Utils.random, 100, 1) > durabilityRate)
          return;

        int random = NwRandom.Roll(Utils.random, 11, 1) - 1;
        int loop = random + 1;
        NwItem item = onAttacked.Creature.GetItemInSlot((InventorySlot)random);

        while (item == null && loop != random)
        {
          if (loop > 10)
            loop = 0;

          item = onAttacked.Creature.GetItemInSlot((InventorySlot)loop);
          loop++;
        }

        if (item == null || item.Tag == "amulettorillink")
          return;

        item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
        if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
        {
          if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
          {
            item.Destroy();
            player.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
          }
          else
            HandleItemRuined(onAttacked.Creature, item);
        }

        /*switch (attackData.iAttackResult)
         {
           // Attaque parée, c'est donc l'arme, ou le gantelet qui prend
           case 2:
             item = player.oid.GetItemInSlot(InventorySlot.RightHand);
             if (item == null)
               item = player.oid.GetItemInSlot(InventorySlot.LeftHand);
             if (item == null)
               item = player.oid.GetItemInSlot(InventorySlot.Arms);
             if (item == null)
               return;

             item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
             if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
             {
               item.Destroy();
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécupérables de votre {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
             }

             break;
           // Attaque résistée, c'est donc l'armure, le casque ou le bouclier qui prennent
           case 5:

             InventorySlot[] localizedDamaged = new InventorySlot[] { InventorySlot.Head, InventorySlot.Chest, InventorySlot.LeftHand };

             item = player.oid.GetItemInSlot(localizedDamaged[NwRandom.Roll(Utils.random, 3, 1)]);
             if (item == null)
               item = player.oid.GetItemInSlot(localizedDamaged[NwRandom.Roll(Utils.random, 3, 1)]);
             if (item == null)
               item = player.oid.GetItemInSlot(localizedDamaged[NwRandom.Roll(Utils.random, 3, 1)]);
             if (item == null)
               return;

             item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
             if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
             {
               item.Destroy();
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécuprables de votre {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
             }

             break;
           // Attaque échouée classique, donc on détermine aléatoirement la cible qui perd de la durabilité
           // La CA d'esquive (dextérité) permet de diminuer le risque d'usure
           case 4:

             int random = NwRandom.Roll(Utils.random, 11, 1) - 1;
             int loop = random + 1;
             item = player.oid.GetItemInSlot((InventorySlot)random);

             while (item == null && loop != random)
             {
               if (loop > 10)
                 loop = 0;

               item = player.oid.GetItemInSlot((InventorySlot)loop);
               loop++;
             }

             if (item == null || item.Tag == "amulettorillink")
               return;

             item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
             if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
             {
               item.Destroy();
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécupérables de votre {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
             }

             break;
         }*/
      }
    }
    public static async void HandleAttackEvent(OnCreatureAttack onAttack)
    {
      PlayerSystem.Log.Info("Base : " + onAttack.DamageData.Base);
      PlayerSystem.Log.Info("Blud : " + onAttack.DamageData.Bludgeoning);
      PlayerSystem.Log.Info("Pierce : " + onAttack.DamageData.Pierce);
      PlayerSystem.Log.Info("Slash : " + onAttack.DamageData.Slash);

      if (!(onAttack.Target is NwCreature oTarget))
        return;

      await NwModule.Instance.WaitForObjectContext();

      if (NwRandom.Roll(Utils.random, 100) <= oTarget.GetAbilityModifier(Ability.Dexterity) - oTarget.ArmorCheckPenalty - oTarget.ShieldCheckPenalty)
      {
        onAttack.AttackResult = AttackResult.Miss;
        if (onAttack.Attacker.IsPlayerControlled)
          onAttack.Attacker.ControllingPlayer.SendServerMessage($"{oTarget.Name} a esquivé votre attaque.");

        if (oTarget.IsPlayerControlled)
          oTarget.ControllingPlayer.SendServerMessage($"Attaque de {onAttack.Attacker.Name} esquivée.");

        return;
      }

      if (onAttack.AttackResult == AttackResult.Miss)
        onAttack.AttackResult = AttackResult.AutomaticHit;

      if (oTarget.GetLocalVariable<int>("_IS_GNOME_MECH").HasValue && onAttack.DamageData.Electrical > 0)
      {
        oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(onAttack.DamageData.Electrical));
        oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        oTarget.GetLocalVariable<int>("_SPARK_LEVEL").Value += 5;

        foreach (NwCreature oPC in oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.ControllingPlayer != null))
          oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature(oTarget, onAttack.DamageData.Electrical.ToString().ColorString(new Color(32, 255, 32)));

        if (oTarget.Tag == "dog_meca_defect" && CreaturePlugin.GetAttacksPerRound(oTarget) < 6
          && NwRandom.Roll(Utils.random, 100) <= oTarget.GetLocalVariable<int>("_SPARK_LEVEL").Value)
        {
          foreach (NwCreature oPC in oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.ControllingPlayer != null))
            oPC.ControllingPlayer.SendServerMessage("Attention, la décharge électrique surcharge le chien mécanisé défectueux, le rendant plus dangereux !");

          oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
          oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(oTarget.MaxHP / 10, DamageType.Magical));
          oTarget.BaseAttackCount = CreaturePlugin.GetAttacksPerRound(oTarget) + 1;
        }

        onAttack.DamageData.Electrical = 0;
      }

      bool isUnarmedAttack = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) == null;
      bool isRangedAttack = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) != null
        && ItemUtils.GetItemCategory(onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand).BaseItemType) == ItemUtils.ItemCategory.RangedWeapon;
      bool isOffHandAttack = onAttack.WeaponAttackType == WeaponAttackType.Offhand;

      int baseArmorPenetration = 0;
      int bonusArmorPenetration = 0;

      if (isOffHandAttack)
        baseArmorPenetration = CreaturePlugin.GetAttackBonus(onAttack.Attacker, -1, 1);
      else
        baseArmorPenetration = CreaturePlugin.GetAttackBonus(onAttack.Attacker);


      NwItem attackWeapon = null;
      
      if (isUnarmedAttack && onAttack.Attacker.GetItemInSlot(InventorySlot.Arms) != null) // la fonction CreaturePlugin.GetAttackBonus ne prend pas en compte le + AB des gants, donc je le rajoute
      {
        attackWeapon = onAttack.Attacker.GetItemInSlot(InventorySlot.Arms);
        int glovesAttackBonus = 0;

        foreach (ItemProperty ip in attackWeapon.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AttackBonus || i.PropertyType == ItemPropertyType.EnhancementBonus))
          glovesAttackBonus += ip.CostTableValue;

        baseArmorPenetration += glovesAttackBonus;
      }

      // Déterminer quelle partie du corps est touchée par l'attaque
      // Déterminer s'il s'agit d'une attaque low, normal/ranged, ou high

      Config.AttackPosition attackPosition = Config.AttackPosition.NormalOrRanged;

      if ((isUnarmedAttack || !isRangedAttack)
        && onAttack.Attacker.Size != oTarget.Size
        && !((onAttack.Attacker.Size == CreatureSize.Small && oTarget.Size == CreatureSize.Medium) || (onAttack.Attacker.Size == CreatureSize.Medium && oTarget.Size == CreatureSize.Small)))
      {
        if (onAttack.Attacker.Size > oTarget.Size)
          attackPosition = Config.AttackPosition.High;
        else
          attackPosition = Config.AttackPosition.Low;
      }

      InventorySlot hitSlot;
      int randLocation = NwRandom.Roll(Utils.random, 100);

      switch (attackPosition)
      {
        case Config.AttackPosition.NormalOrRanged:

          if (randLocation < 13)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 38)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 51)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 88)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        case Config.AttackPosition.Low:

          if (randLocation < 28)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 69)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 82)
            hitSlot = InventorySlot.Arms;
          else
            hitSlot = InventorySlot.Chest;

          break;

        case Config.AttackPosition.High:

          if (randLocation < 19)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 33)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 74)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        default:
          return;
      }

      NwItem targetArmor = oTarget.GetItemInSlot(hitSlot);
      NwItem targetShield = oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (ItemUtils.GetItemCategory(targetShield.BaseItemType) != ItemUtils.ItemCategory.Shield)
        targetShield = null;

      Dictionary<DamageType, int> targetAC = new Dictionary<DamageType, int>();

      if (targetArmor != null)
      {
        foreach (ItemProperty ip in targetArmor.ItemProperties.Where(i
         => i.PropertyType == ItemPropertyType.AcBonus
         || i.PropertyType == ItemPropertyType.AcBonusVsDamageType
         || (i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)onAttack.Attacker.RacialType)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)onAttack.Attacker.GoodEvilAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)onAttack.Attacker.LawChaosAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(onAttack.Attacker))))
        {
          switch (ip.PropertyType)
          {
            case ItemPropertyType.AcBonusVsDamageType:

              switch (ip.SubType)
              {
                case 0:

                  if (targetAC.ContainsKey(DamageType.Bludgeoning))
                    targetAC[DamageType.Bludgeoning] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Bludgeoning, ip.CostTableValue);

                  break;

                case 1:

                  if (targetAC.ContainsKey(DamageType.Piercing))
                    targetAC[DamageType.Piercing] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Piercing, ip.CostTableValue);

                  break;

                case 2:

                  if (targetAC.ContainsKey(DamageType.Slashing))
                    targetAC[DamageType.Slashing] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Slashing, ip.CostTableValue);

                  break;

                case 4: // All physical damage

                  if (targetAC.ContainsKey((DamageType)4))
                    targetAC[(DamageType)4] += ip.CostTableValue;
                  else
                    targetAC.Add((DamageType)4, ip.CostTableValue);

                  break;

                case 5:

                  if (targetAC.ContainsKey(DamageType.Magical))
                    targetAC[DamageType.Magical] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Magical, ip.CostTableValue);

                  break;

                case 6:

                  if (targetAC.ContainsKey(DamageType.Acid))
                    targetAC[DamageType.Acid] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Acid, ip.CostTableValue);

                  break;

                case 7:

                  if (targetAC.ContainsKey(DamageType.Cold))
                    targetAC[DamageType.Cold] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Cold, ip.CostTableValue);

                  break;

                case 9:

                  if (targetAC.ContainsKey(DamageType.Electrical))
                    targetAC[DamageType.Electrical] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Electrical, ip.CostTableValue);

                  break;

                case 10:

                  if (targetAC.ContainsKey(DamageType.Fire))
                    targetAC[DamageType.Fire] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Fire, ip.CostTableValue);

                  break;

                case 13:

                  if (targetAC.ContainsKey(DamageType.Sonic))
                    targetAC[DamageType.Sonic] += ip.CostTableValue;
                  else
                    targetAC.Add(DamageType.Sonic, ip.CostTableValue);

                  break;

                case 14: // All elemental damage

                  if (targetAC.ContainsKey((DamageType)14))
                    targetAC[(DamageType)14] += ip.CostTableValue;
                  else
                    targetAC.Add((DamageType)14, ip.CostTableValue);

                  break;
              }

              break;

            default:

              if (targetAC.ContainsKey(DamageType.BaseWeapon))
                targetAC[DamageType.BaseWeapon] += ip.CostTableValue;
              else
                targetAC.Add(DamageType.BaseWeapon, ip.CostTableValue);

              break;
          }
        }
      }

      if (attackWeapon != null)
      {
        foreach (ItemProperty ip in attackWeapon.ItemProperties.Where(i => (i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)oTarget.RacialType)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)oTarget.GoodEvilAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)oTarget.LawChaosAlignment)
         || (i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(oTarget))
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsRacialGroup && i.SubType == (int)oTarget.RacialType)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)oTarget.GoodEvilAlignment)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)oTarget.LawChaosAlignment)
         || (i.PropertyType == ItemPropertyType.EnhancementBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(oTarget))))
          bonusArmorPenetration += ip.CostTableValue;
      }

      if (targetAC.ContainsKey(DamageType.BaseWeapon))
        targetAC[DamageType.BaseWeapon] = targetAC[DamageType.BaseWeapon] * (100 - baseArmorPenetration - bonusArmorPenetration) / 100;
      else
        targetAC.Add(DamageType.BaseWeapon, 0);

      if (targetShield != null)
      {
        foreach (ItemProperty ip in targetShield.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.AcBonus))
          targetAC[DamageType.BaseWeapon] += ip.CostTableValue;

        foreach (ItemProperty ip in targetShield.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.AcBonusVsDamageType && ip.SubType == 1)) // 1 = piercing
          if (targetAC.ContainsKey(DamageType.Piercing))
            targetAC[DamageType.Piercing] += ip.CostTableValue;
          else targetAC.Add(DamageType.Piercing, ip.CostTableValue);
      }

      if (onAttack.DamageData.Base > 0)
      {
        int damageType = 3; // par défaut : Slashing

        switch(onAttack.WeaponAttackType)
        {
          case WeaponAttackType.MainHand:
            if (onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) != null)
            {
              attackWeapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);
              damageType = ItemUtils.GetItemDamageType(attackWeapon);
            }
            break;
          case WeaponAttackType.Offhand:
            if (onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand) != null)
            {
              attackWeapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand);
              damageType = ItemUtils.GetItemDamageType(attackWeapon);
            }
            break;
        }

        if (isUnarmedAttack)
          damageType = 2; // Bludgeoning

        int bonusAC = 0;

        switch (damageType)
        {
          case 1: // Piercing
            onAttack.DamageData.Base = (short)(onAttack.DamageData.Base * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));
            break;
          case 2: // Bludgeoning
            onAttack.DamageData.Base = (short)(onAttack.DamageData.Base * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Bludgeoning) - 60) / 40));
            break;
          case 3: // Slashing
            onAttack.DamageData.Base = (short)(onAttack.DamageData.Base * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Slashing) - 60) / 40));
            break;
          case 4: // Slashing and Piercing

            if (targetAC.GetValueOrDefault(DamageType.Slashing) > targetAC.GetValueOrDefault(DamageType.Piercing))
              bonusAC = targetAC.GetValueOrDefault(DamageType.Piercing);
            else
              bonusAC = targetAC.GetValueOrDefault(DamageType.Slashing);

            onAttack.DamageData.Base = (short)(onAttack.DamageData.Base * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + bonusAC - 60) / 40));

            break;
          case 5: //Piercing and bludgeoning

            if (targetAC.GetValueOrDefault(DamageType.Bludgeoning) > targetAC.GetValueOrDefault(DamageType.Piercing))
              bonusAC = targetAC.GetValueOrDefault(DamageType.Piercing);
            else
              bonusAC = targetAC.GetValueOrDefault(DamageType.Bludgeoning);

            onAttack.DamageData.Base = (short)(onAttack.DamageData.Base * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + bonusAC - 60) / 40));

            break;
        }
      }

      if (onAttack.DamageData.Bludgeoning > 0)
        onAttack.DamageData.Bludgeoning = (short)(onAttack.DamageData.Bludgeoning * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Bludgeoning) - 60) / 40));

      if (onAttack.DamageData.Pierce > 0)
        onAttack.DamageData.Pierce = (short)(onAttack.DamageData.Pierce * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Piercing) - 60) / 40));

      if (onAttack.DamageData.Slash > 0)
        onAttack.DamageData.Slash = (short)(onAttack.DamageData.Slash * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)4) + targetAC.GetValueOrDefault(DamageType.Slashing) - 60) / 40));

      if (onAttack.DamageData.Electrical > 0)
        onAttack.DamageData.Electrical = (short)(onAttack.DamageData.Electrical * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Electrical) - 60) / 40));

      if (onAttack.DamageData.Acid > 0)
        onAttack.DamageData.Acid = (short)(onAttack.DamageData.Acid * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Acid) - 60) / 40));

      if (onAttack.DamageData.Cold > 0)
        onAttack.DamageData.Cold = (short)(onAttack.DamageData.Cold * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Cold) - 60) / 40));

      if (onAttack.DamageData.Fire > 0)
        onAttack.DamageData.Fire = (short)(onAttack.DamageData.Fire * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Fire) - 60) / 40));

      if (onAttack.DamageData.Magical > 0)
        onAttack.DamageData.Magical = (short)(onAttack.DamageData.Magical * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Magical) - 60) / 40));

      if (onAttack.DamageData.Sonic > 0)
        onAttack.DamageData.Sonic = (short)(onAttack.DamageData.Sonic * Math.Pow(0.5, (targetAC[DamageType.BaseWeapon] + targetAC.GetValueOrDefault((DamageType)14) + targetAC.GetValueOrDefault(DamageType.Sonic) - 60) / 40));
        
      if (PlayerSystem.Players.TryGetValue(onAttack.Attacker, out PlayerSystem.Player attacker))
      {
        // L'attaquant est un joueur. On diminue la durabilité de son arme
        NwItem weapon;
        
        switch (onAttack.WeaponAttackType)
        {
          case WeaponAttackType.MainHand:
          case WeaponAttackType.HastedAttack:
            weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);
            break;
          case WeaponAttackType.Offhand:
            weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.LeftHand);
            break;
          case WeaponAttackType.Unarmed:
          case WeaponAttackType.UnarmedExtra:
            weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.Arms);
            break;
          default:
            return;
        }

        if (weapon == null)
          return;

        int durabilityChance;

        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.Parried:
          case AttackResult.Resisted:
            durabilityChance = 30;
            break;
          case AttackResult.Miss:
            durabilityChance = 20;
            break;
          default:
            return;
        }

        int dexBonus = onAttack.Attacker.GetAbilityModifier(Ability.Dexterity) - (CreaturePlugin.GetArmorCheckPenalty(onAttack.Attacker) + CreaturePlugin.GetShieldCheckPenalty(onAttack.Attacker));
        if (dexBonus < 0)
          dexBonus = 0;

        int safetyLevel = 0;
        if (attacker.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
          safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, attacker.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

        durabilityChance -= dexBonus + safetyLevel;

        if (NwRandom.Roll(Utils.random, 100, 1) < 2 && NwRandom.Roll(Utils.random, 100, 1) < durabilityChance)
        {
          weapon.GetLocalVariable<int>("_DURABILITY").Value -= 1;
          if (weapon.GetLocalVariable<int>("_DURABILITY").Value <= 0)
          {
            if (weapon.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
            {
              weapon.Destroy();
              attacker.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {weapon.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
            }
            else
              HandleItemRuined(onAttack.Attacker, weapon);
          }
        }
      }
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      CreaturePlugin.RunUnequip(oPC, oItem);
      oItem.GetLocalVariable<int>("_DURABILITY").Value = -1;
      foreach (ItemProperty ip in oItem.ItemProperties.Where(ip => ip.Tag.StartsWith("ENCHANTEMENT")))
      {
        Task waitLoopEnd = NwTask.Run(async () =>
        {
          ItemProperty deactivatedIP = ip;
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          oItem.RemoveItemProperty(deactivatedIP);
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          deactivatedIP.Tag += "_INACTIVE";
          oItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
        });
      }

      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
    }
  }
}
