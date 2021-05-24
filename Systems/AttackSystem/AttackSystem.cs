using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;
using NLog;

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
          if (item.GetLocalVariable<int>("_ORIGINAL_CRAFTER_NAME").HasNothing)
          {
            item.Destroy();
            player.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(Color.WHITE)}. Ces débris ne sont même pas réparables !", Color.RED);
          }
          else
          {
            item.GetLocalVariable<int>("_DURABILITY").Value = -1;
            HandleItemRuined(onAttacked.Creature, item);
            player.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(Color.WHITE)}. Des réparations s'imposent !", Color.RED);
          }
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
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécupérables de votre {item.Name.ColorString(Color.WHITE)}.", Color.RED);
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
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécuprables de votre {item.Name.ColorString(Color.WHITE)}.", Color.RED);
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
               player.oid.SendServerMessage($"Il ne reste plus que des ruines irrécupérables de votre {item.Name.ColorString(Color.WHITE)}.", Color.RED);
             }

             break;
         }*/
      }
    }
    public static void HandleAttackEvent(OnCreatureAttack onAttack)
    {
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
            if (weapon.GetLocalVariable<int>("_ORIGINAL_CRAFTER_NAME").HasNothing)
            {
              weapon.Destroy();
              attacker.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {weapon.Name.ColorString(Color.WHITE)}. Ces débris ne sont même pas réparables !", Color.RED);
            }
            else
            {
              weapon.GetLocalVariable<int>("_DURABILITY").Value = -1;
              HandleItemRuined(onAttack.Attacker, weapon);
              attacker.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {weapon.Name.ColorString(Color.WHITE)}. Des réparations s'imposent !", Color.RED);
            }
          }
        }
      }
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      CreaturePlugin.RunUnequip(oPC, oItem);

    }
  }
}
