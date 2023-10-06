using Anvil.API;
using NWN.Native.API;
using Anvil.Services;
using System.Linq;
using NWN.Core;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(NativeAttackHook))]
  public unsafe class NativeAttackHook
  {
    private readonly CExoString casterLevelVariable = "_CREATURE_CASTER_LEVEL".ToExoString();
    private readonly CExoString isFinesseWeaponVariable = "_IS_FINESSE_WEAPON".ToExoString();
    private readonly CExoString currentDualAttacksVariable = "_CURRENT_DUAL_ATTACK".ToExoString();
    private readonly CExoString currentUnarmedExtraAttacksVariable = "_CURRENT_UNARMED_EXTRA_ATTACK".ToExoString();
    private readonly CExoString isBonusActionAvailableVariable = "_BONUS_ACTION".ToExoString();
    //private readonly CExoString minWeaponDamageVariable = "_MIN_WEAPON_DAMAGE".ToExoString();
    //private readonly CExoString maxWeaponDamageVariable = "_MAX_WEAPON_DAMAGE".ToExoString();
    //private readonly CExoString minCreatureDamageVariable = "_MIN_CREATURE_DAMAGE".ToExoString();
    //private readonly CExoString maxCreatureDamageVariable = "_MAX_CREATURE_DAMAGE".ToExoString();
    private readonly CExoString critChanceVariable = "_ADD_CRIT_CHANCE".ToExoString();
    //private readonly CExoString itemGradeVariable = "_ITEM_GRADE".ToExoString();
    private readonly CExoString durabilityVariable = "_DURABILITY".ToExoString();
    private readonly CExoString maxDurabilityVariable = "_MAX_DURABILITY".ToExoString();
    //private readonly CExoString blindEffectString = "CUSTOM_CONDITION_BLIND".ToExoString();
    //private readonly CExoString spellIdVariable = "_CURRENT_SPELL".ToExoString();

    //_ZN12CNWSCreature15SavingThrowRollEhthjiti
    // _ZN8CNWRules8RollDiceEhh

    [NativeFunction("_ZN17CNWSCreatureStats13GetDamageRollEP10CNWSObjectiiiii", null)]
    private delegate int GetDamageRollHook(void* thisPtr, void* pTarget, int bOffHand, int bCritical, int bSneakAttack, int bDeathAttack, int bForceMax);
    
    [NativeFunction("_ZN12CNWSCreature17ResolveAttackRollEP10CNWSObject", null)]
    private delegate void ResolveAttackRollHook(void* pCreature, void* pTarget);
    
    [NativeFunction("_ZN17CNWSCreatureStats30GetSpellLikeAbilityCasterLevelEj", null)]
    private delegate byte GetSpellLikeAbilityCasterLevelHook(void* pCreatureStats, int nSpellId);
    //private delegate byte GetCasterLevelHook(void* pCreatureStats, byte nMultiClass);
    
    [NativeFunction("_ZN12CNWSCreature27AddUseTalentOnObjectActionsEiijhjihh", null)]
    protected delegate int AddUseTalentOnObjectHook(void* pCreature, int talentType, int talentId, uint oidTarget, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType);

    [NativeFunction("_ZN12CNWSCreature29AddUseTalentAtLocationActionsEii6Vectorhjihh", null)]
    private delegate int AddUseTalentAtLocationHook(void* pCreature, int talentType, int talentId, Vector3 vTargetLocation, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType);

    private readonly FunctionHook<GetDamageRollHook> getDamageRollHook;
    private readonly FunctionHook<AddUseTalentOnObjectHook> addUseTalentOnObjectHook;
    private readonly FunctionHook<AddUseTalentAtLocationHook> addUseTalentAtLocationHook;

    public NativeAttackHook(HookService hookService)
    {
      getDamageRollHook = hookService.RequestHook<GetDamageRollHook>(OnGetDamageRoll, HookOrder.Early);
      hookService.RequestHook<ResolveAttackRollHook>(OnResolveAttackRoll, HookOrder.Early);
      hookService.RequestHook<GetSpellLikeAbilityCasterLevelHook>(OnGetSpellLikeAbilityCasterLevel, HookOrder.Early);
      //hookService.RequestHook<GetCasterLevelHook>(OnGetCasterLevel, HookOrder.Early); // Malheureusement ce hook est inutile => La fonction n'est jamais appelée en jeu
      addUseTalentOnObjectHook = hookService.RequestHook<AddUseTalentOnObjectHook>(OnAddUseTalentOnObjectHook, HookOrder.Early);
      addUseTalentAtLocationHook = hookService.RequestHook<AddUseTalentAtLocationHook>(OnAddUseTalentAtLocationHook, HookOrder.Early);
    }
    private void OnResolveAttackRoll(void* pCreature, void* pTarget)
    {
      CNWSCreature creature = CNWSCreature.FromPointer(pCreature);
      CNWSObject targetObject = CNWSObject.FromPointer(pTarget);
      CNWSCreature targetCreature = targetObject.m_nObjectType == (int)ObjectType.Creature ? targetObject.AsNWSCreature() : null;

      //LogUtils.LogMessage("--------------------------------------------------------------------", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"{creature.m_pStats.GetFullName().ToExoLocString().GetSimple(0)} attacking {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", LogUtils.LogType.Combat);

      CNWSCombatRound combatRound = creature.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);
      CNWSItem targetArmor = targetCreature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Chest);

      if (targetCreature is not null && targetCreature.m_bPlayerCharacter > 0 && targetArmor is not null
        && targetArmor.m_ScriptVars.GetInt(maxDurabilityVariable) > 0 && targetArmor.m_ScriptVars.GetInt(durabilityVariable) < 1)
      {
        attackData.m_nAttackResult = 3;
        NativeUtils.SendNativeServerMessage($"CRITIQUE AUTOMATIQUE - Armure en ruine ".ColorString(new Color(255, 215, 0)), targetCreature);
        return;
      }

  //*** CALCUL DU BONUS D'ATTAQUE ***//
      // On prend le bonus d'attaque calculé automatiquement par le jeu en fonction de la cible qui peut être une créature ou un placeable
      int attackModifier = targetCreature is null 
        ? creature.m_pStats.GetAttackModifierVersus() 
        : creature.m_pStats.GetAttackModifierVersus(targetObject.AsNWSCreature());

      //LogUtils.LogMessage($"Attack Modifier versus : {attackModifier}", LogUtils.LogType.Combat);

      // Si l'arme utilisée pour attaquer est une arme de finesse, et que la créature a une meilleur DEX, alors on utilise la DEX pour attaquer
      CNWSItem attackWeapon = combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);
      Anvil.API.Ability attackStat = Anvil.API.Ability.Strength;

      byte dexMod = creature.m_pStats.m_nDexterityModifier;
      byte strMod = creature.m_pStats.m_nStrengthModifier;
      int dexBonus = dexMod > 122 ? dexMod - 255 : dexMod;
      int strBonus = strMod > 122 ? strMod - 255 : strMod;

      if (creature.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Monk) > 0
        && (attackWeapon is null || NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).IsMonkWeapon))
        attackStat = dexBonus > strBonus ? Anvil.API.Ability.Dexterity : Anvil.API.Ability.Strength;
      else
        attackStat = attackWeapon is null || ItemUtils.GetItemCategory(NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType) != ItemUtils.ItemCategory.RangedWeapon 
          ? Anvil.API.Ability.Strength : Anvil.API.Ability.Dexterity;

      if (attackWeapon != null
          && dexBonus > strBonus
          && attackWeapon.m_ScriptVars.GetInt(isFinesseWeaponVariable) != 0)
      {
        attackModifier += -strBonus + dexBonus;
        attackStat = Anvil.API.Ability.Dexterity;
      }
      
      //LogUtils.LogMessage($"Attack Modifier finesse : {attackModifier}", LogUtils.LogType.Combat);

      // TODO : Dans certains cas, la STAT à utiliser pourra être INT, SAG ou CHA, à implémenter 

      // On ajoute le bonus de maîtrise de la créature et on se débarrasse de la pénalité de 5 pour les attaques supplémentaires du round
      int attackBonus = NativeUtils.GetWeaponProficiencyBonus(creature, attackWeapon) + attackModifier;

      //LogUtils.LogMessage($"+ Proficiency Bonus : {attackBonus}", LogUtils.LogType.Combat);

      if (attackData.m_nWeaponAttackType != 6 && NativeUtils.IsDualWieldingLightWeapon(attackWeapon, creature.m_nCreatureSize, creature.m_pInventory.m_pEquipSlot[5].ToNwObject<NwItem>()))
        attackBonus += 2;

      if (combatRound.m_nCurrentAttack == 0)
      {
        creature.m_ScriptVars.SetInt(currentDualAttacksVariable, 0);
        creature.m_ScriptVars.SetInt(currentUnarmedExtraAttacksVariable, 0);
      }

      switch (attackData.m_nWeaponAttackType)
      {
        case 1:
        case 3:
        case 4:
        case 5:
        case 7:
          attackBonus += 5 * combatRound.m_nCurrentAttack;
            break;
        case 2:
          int currentDualAttack = creature.m_ScriptVars.GetInt(currentDualAttacksVariable);
          attackBonus += 5 * currentDualAttack;
          creature.m_ScriptVars.SetInt(currentDualAttacksVariable, currentDualAttack + 1);

          int bonusAction = creature.m_ScriptVars.GetInt(isBonusActionAvailableVariable);

          if (bonusAction > 0)
            creature.m_ScriptVars.SetInt(isBonusActionAvailableVariable, bonusAction - 1);
          else
          {
            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = 8;

            NativeUtils.SendNativeServerMessage($"Main secondaire - Echec automatique - Pas d'action bonus disponible".ColorString(ColorConstants.Red), creature);
            return;
          }
          break;
        case 8:
          int currentUnarmedExtraAttack = creature.m_ScriptVars.GetInt(currentUnarmedExtraAttacksVariable);
          attackBonus += 5 * currentUnarmedExtraAttack;
          creature.m_ScriptVars.SetInt(currentUnarmedExtraAttacksVariable, currentUnarmedExtraAttack + 1);
          break;
      }

      /*LogUtils.LogMessage($"+ Multi-attack bonus : {attackBonus}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"m_nWeaponAttackType : {attackData.m_nWeaponAttackType}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"m_nCurrentAttack : {combatRound.m_nCurrentAttack}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"Final Attack Bonus : {attackBonus}", LogUtils.LogType.Combat);*/

      if (targetCreature is not null)
      {
        int advantage = CreatureUtils.HasAdvantageAgainstTarget(creature, attackStat, targetCreature);
        int attackRoll = Utils.RollAdvantage(advantage);
        short targetAC = targetCreature.m_pStats.GetArmorClassVersus(creature);

        string hitString = "touchez".ColorString(new Color(32, 255, 32));
        string rollString = $"{attackRoll} + {attackBonus} = {attackRoll + attackBonus}".ColorString(new Color(32, 255, 32));
        string criticalString = "";
        string advantageString = advantage == 0 ? "" : advantage > 0 ? "Avantage - ".ColorString(new Color(255, 215, 0)) : "Désavantage - ".ColorString(ColorConstants.Red);

        if(attackRoll == 20) // TODO : certains items permettront d'augmenter la plage des critiques dans certaines conditions
        {
          attackData.m_nAttackResult = 3;
          criticalString = "CRITIQUE - ".ColorString(new Color(255, 215, 0));
        }
        else if (attackRoll > 1 && attackRoll + attackBonus > targetAC) 
          attackData.m_nAttackResult = 1;
        else
        {
          attackData.m_nAttackResult = 4;
          attackData.m_nMissedBy = (byte)(targetAC - attackRoll) > 8 ? (byte)Utils.random.Next(9) : (byte)(targetAC - attackRoll);
          hitString = "manquez".ColorString(ColorConstants.Red);
          rollString = rollString.StripColors().ColorString(ColorConstants.Red);
        }
        
        string targetName = $"{targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        NativeUtils.SendNativeServerMessage($"{advantageString}{criticalString}Vous {hitString} {targetName} {rollString}".ColorString(ColorConstants.Cyan), creature);
      }
      else
        attackData.m_nAttackResult = 7;
    }
    private int OnAddUseTalentOnObjectHook(void* pCreature, int talentType, int talentId, uint oidTarget, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType)
    {
      if (talentType == (int)TalentType.Spell)
      {
        if (CNWSCreature.FromPointer(pCreature).m_pStats.m_pSpellLikeAbilityList.Count(s => s.m_nSpellId == talentId) > 9)
        {
          NWScript.ActionCastSpellAtObject(talentId, oidTarget, nMetaType, 1);
          return 0;
        }
      }

      return addUseTalentOnObjectHook.CallOriginal(pCreature, talentType, talentId, oidTarget, nMultiClass, oidItem, nItemPropertyIndex, nCasterLevel, nMetaType);
    }
    private int OnAddUseTalentAtLocationHook(void* pCreature, int talentType, int talentId, Vector3 vTargetLocation, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType)
    {
      if (talentType == (int)TalentType.Spell)
      {
        var creature = CNWSCreature.FromPointer(pCreature);

        if (creature.m_pStats.m_pSpellLikeAbilityList.Count(s => s.m_nSpellId == talentId) > 9)
        {
          NWScript.ActionCastSpellAtLocation(talentId, NWScript.Location(creature.GetArea().m_idSelf, vTargetLocation, NWScript.GetFacing(creature.m_idSelf)), nMetaType, 1);
          return 0;
        }
      }

      return addUseTalentAtLocationHook.CallOriginal(pCreature, talentType, talentId, vTargetLocation, nMultiClass, oidItem, nItemPropertyIndex, nCasterLevel, nMetaType);
    }
    private byte OnGetSpellLikeAbilityCasterLevel(void* pCreatureStats, int nSpellId)
    {
      //Log.Info($"----------------------get spellLikeAbility caster level called : spell {nSpellId} !---------------------");
      CNWSCreatureStats creatureStats = CNWSCreatureStats.FromPointer(pCreatureStats);
      int casterLevel = creatureStats.m_pBaseCreature.m_ScriptVars.GetInt(casterLevelVariable);

      if (casterLevel > 0)
        return (byte)casterLevel;
      else
        return creatureStats.m_pSpellLikeAbilityList.FirstOrDefault(s => s.m_nSpellId == nSpellId).m_nCasterLevel;
    }
    /*private byte OnGetCasterLevel(void* pCreatureStats, byte nMultiClass)
    {
      CNWSCreatureStats creatureStats = CNWSCreatureStats.FromPointer(pCreatureStats);
      int casterLevel = 1;

      Log.Info("ENTERING NATIVE CASTER LEVEL HOOK");

      if (PlayerSystem.Players.TryGetValue(creatureStats.m_pBaseCreature.m_idSelf, out PlayerSystem.Player player))
      {
        player.oid.SendServerMessage("ENTERING NATIVE CASTER LEVEL HOOK");
        if (player.oid.ControlledCreature != player.oid.LoginCreature)
          casterLevel = creatureStats.m_pBaseCreature.m_ScriptVars.GetInt(casterLevelVariable);
        else
          casterLevel = !player.oid.IsDM ? player.learnableSpells[creatureStats.m_pBaseCreature.m_ScriptVars.GetInt(spellIdVariable)].currentLevel : 15;
      }

      Log.Info(casterLevel);

      return (byte)casterLevel;
    }*/

    private int OnGetDamageRoll(void* thisPtr, void* pTarget, int bOffHand, int bCritical, int bSneakAttack, int bDeathAttack, int bForceMax)
    {
      var creatureStats = CNWSCreatureStats.FromPointer(thisPtr);
      var attacker = CNWSCreature.FromPointer(creatureStats.m_pBaseCreature);
      var targetObject = CNWSObject.FromPointer(pTarget);
      var damageFlags = creatureStats.m_pBaseCreature.GetDamageFlags();

      if (attacker is null || targetObject is null || targetObject.m_bPlotObject == 1)
        return -1;

      LogUtils.LogMessage($"Jet de dégâts : {creatureStats.GetFullName().ToExoLocString().GetSimple(0)} attaque {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", LogUtils.LogType.Combat);

      CNWSCombatRound combatRound = attacker.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);
      CNWSItem attackWeapon = combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);
      int baseDamage = 0;

      // Jet de dégâts de l'arme
      if (attackWeapon is not null)
      {
        if (attackWeapon.GetPropertyByTypeExists((ushort)Native.API.ItemProperty.NoDamage) > 0)
          return -1;

        NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);
        baseDamage += NwRandom.Roll(Utils.random, baseWeapon.DieToRoll, baseWeapon.NumDamageDice);

        // On ne peut faire des attaques sournoises qu'avec une arme
        if (bSneakAttack > 0)
          baseDamage += NwRandom.Roll(Utils.random, 6, (int)Math.Ceiling((double)attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) / 2));
      }
      else
        baseDamage += CreatureUtils.GetUnarmedDamage(attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Monk));

      if(bCritical >  0)
        baseDamage += NativeUtils.GetCritDamage(attacker, attackWeapon, bSneakAttack);
        
      // Ajout du bonus de caractéristique
      byte dexMod = creatureStats.m_nDexterityModifier;
      byte strMod = creatureStats.m_nStrengthModifier;
      int dexBonus = dexMod > 122 ? dexMod - 255 : dexMod;
      int strBonus = strMod > 122 ? strMod - 255 : strMod;
      int damageBonus = 0;

      if (ItemUtils.GetItemCategory(NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType) == ItemUtils.ItemCategory.RangedWeapon)
        damageBonus += dexBonus;
      else if (attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Monk) > 0 // Les moins utilisent leur caract la plus élevée à mains nues ou avec arme de moine
        && (attackWeapon is null || NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).IsMonkWeapon))
        damageBonus += dexBonus > strBonus ? dexBonus : strBonus;
      else // Si arme de finesse et dextérité plus élevée, alors on utilise la dextérité
        damageBonus += attackWeapon?.m_ScriptVars.GetInt(isFinesseWeaponVariable) != 0 && dexBonus > strBonus ? dexBonus : strBonus;

      // Pour l'attaque de la main secondaire, on n'ajoute le modificateur de caractéristique que s'il est négatif
      if (bOffHand < 1 || (bOffHand > 0 && dexBonus < 0)) 
        baseDamage += damageBonus;

      // Application des réductions du jeu de base
      baseDamage = targetObject.DoDamageImmunity(attacker, baseDamage, damageFlags, 0, 1);
      baseDamage = targetObject.DoDamageResistance(attacker, baseDamage, damageFlags, 0, 1, 1);
      baseDamage = targetObject.DoDamageReduction(attacker, baseDamage, attacker.CalculateDamagePower(targetObject, bOffHand), 0, 1);

      return baseDamage;

      // ANCIEN SYSTEME guild wars
      /*int minDamage = 0;
      int maxDamage = 0;
      int damage = 0;

      // Get attacker weapon
      var weapon = GetAttackWeapon(attacker, bOffHand);

      if (weapon is not null)
      {
        if(weapon.m_ScriptVars.GetInt(durabilityVariable) < 0)
          return -1;

        minDamage = weapon.m_ScriptVars.GetInt(minWeaponDamageVariable);
        maxDamage = weapon.m_ScriptVars.GetInt(maxWeaponDamageVariable);

        if (minDamage < 1) // S'il ne s'agit pas d'un joueur, avec les dégâts de base sont déterminés par des variables sur la créature
        {
          minDamage = attacker.m_ScriptVars.GetInt(minCreatureDamageVariable);
          maxDamage = attacker.m_ScriptVars.GetInt(maxCreatureDamageVariable);

          if(minDamage < 1 && ItemUtils.itemDamageDictionary.ContainsKey((BaseItemType)weapon.m_nBaseItem)) // S'il la créature ne dispose pas de variable pour ses dégâts, alors on va chercher les dégâts correspondant à son arme
          {
            int weaponGrade = weapon.m_ScriptVars.GetInt(itemGradeVariable);
            minDamage = ItemUtils.itemDamageDictionary[(BaseItemType)weapon.m_nBaseItem][weaponGrade, 0];
            maxDamage = ItemUtils.itemDamageDictionary[(BaseItemType)weapon.m_nBaseItem][weaponGrade, 1];
            weapon.m_ScriptVars.SetInt(minWeaponDamageVariable, minDamage);
            weapon.m_ScriptVars.SetInt(maxWeaponDamageVariable, maxDamage);
          }
        }
      }

      if (minDamage < 1)
        minDamage = 1;

      if (maxDamage < 1)
        maxDamage = 3;

      damage += bCritical > 0 ? maxDamage : Utils.random.Next(minDamage, maxDamage + 1);

      if (damage < 1)
        damage = 1;

      LogUtils.LogMessage($"Dégâts de base de l'arme : {damage} (min {minDamage}, max {maxDamage})", LogUtils.LogType.Combat);

      // Calculate total defense on the target.
      /*if (targetObject != null && targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        var target = CNWSCreature.FromPointer(pTarget);

        foreach (var slotItemId in target.m_pInventory.m_pEquipSlot)
        {
          if (slotItemId != NWNXLib.OBJECT_INVALID)
          {
            var item = NWNXLib.AppManager().m_pServerExoApp.GetItemByGameObjectID(slotItemId);
            for (var index = 0; index < item.m_lstPassiveProperties.array_size; index++)
            {
              var ip = item.GetPassiveProperty(index);
              if (ip != null && ip.m_nPropertyName == (ushort)ItemPropertyType.Defense)
              {
                defense += ip.m_nCostTableValue;
              }
            }
          }
        }
      }*/
    }
    /*private bool IsHitCritical(CNWSCreature attacker, CNWSCreature target, CNWSItem weapon)
    {
      if (target.m_pStats.m_nRace == (ushort)Anvil.API.RacialType.Construct || target.m_pStats.m_nRace == (ushort)Anvil.API.RacialType.Undead)
      {
        LogUtils.LogMessage("Cible immunisée aux coups critiques", LogUtils.LogType.Combat);
        return false;
      }

      Player attackerPlayer = Players.GetValueOrDefault(attacker.m_idSelf);
      Player defender = Players.GetValueOrDefault(target.m_idSelf);

      // Si la cible est en mouvement et est frappée en mêlée par une créature qu'elle ne voit pas, alors crit auto
      if ((weapon is null || ItemUtils.GetItemCategory((BaseItemType)weapon.m_nBaseItem) != ItemUtils.ItemCategory.RangedWeapon) && CreaturePlugin.GetMovementType(target.m_idSelf) > 0)
      {
        var visionNode = target.GetVisibleListElement(attacker.m_idSelf);

        if (visionNode is null || visionNode.m_bSeen < 1 || target.GetFlatFooted() > 0)
        {
          if (defender is not null && defender.learnableSkills.ContainsKey(CustomSkill.UncannyDodge))
          {
            int survivalSkill = defender.learnableSkills.ContainsKey(CustomSkill.WildernessSurvival) ? defender.learnableSkills[CustomSkill.WildernessSurvival].totalPoints : 0;
            survivalSkill += defender.learnableSkills.ContainsKey(CustomSkill.WildernessSurvivalExpert) ? defender.learnableSkills[CustomSkill.WildernessSurvivalExpert].totalPoints : 0;
            survivalSkill += defender.learnableSkills.ContainsKey(CustomSkill.WaterMagicScience) ? defender.learnableSkills[CustomSkill.WaterMagicScience].totalPoints : 0;
            survivalSkill += defender.learnableSkills.ContainsKey(CustomSkill.WaterMagicMaster) ? defender.learnableSkills[CustomSkill.WaterMagicMaster].totalPoints : 0;

            if (attackerPlayer is not null)
            {
              int deceptionSkill = attackerPlayer.learnableSkills.ContainsKey(CustomSkill.Deception) ? attackerPlayer.learnableSkills[CustomSkill.Deception].totalPoints : 0;
              deceptionSkill += attackerPlayer.learnableSkills.ContainsKey(CustomSkill.DeceptionExpert) ? attackerPlayer.learnableSkills[CustomSkill.DeceptionExpert].totalPoints : 0;
              deceptionSkill += attackerPlayer.learnableSkills.ContainsKey(CustomSkill.DeceptionScience) ? attackerPlayer.learnableSkills[CustomSkill.DeceptionScience].totalPoints : 0;
              deceptionSkill += attackerPlayer.learnableSkills.ContainsKey(CustomSkill.DeceptionMaster) ? attackerPlayer.learnableSkills[CustomSkill.DeceptionMaster].totalPoints : 0;

              if (NwRandom.Roll(Utils.random, 20) + deceptionSkill
                > NwRandom.Roll(Utils.random, 20) + survivalSkill + defender.learnableSkills[CustomSkill.UncannyDodge].totalPoints)
                return true;
            }
            else
            {
              if (NwRandom.Roll(Utils.random, 20) + attacker.m_pStats.m_fChallengeRating
                > NwRandom.Roll(Utils.random, 20) + survivalSkill + defender.learnableSkills[CustomSkill.UncannyDodge].totalPoints)
                return true;
            }
          }
          else
          {
            LogUtils.LogMessage("Frappe de mêlée dans le dos sur cible en mouvement : Critique automatique (+20 AP)", LogUtils.LogType.Combat);
            return true;
          }
        }
      }

      int critChance = weapon is not null ? weapon.m_ScriptVars.GetInt(critChanceVariable) : 0 ; // TODO : Gérer les chances de crit pour chaque type d'arme de base
      string critLog = $"{critChance} (arme) ";

      if (!Players.TryGetValue(attacker.m_idSelf, out Player player)) // Si l'attaquant n'est pas un joueur, le crit est déterminé par le FP
      {
        int creatureCrit = attacker.m_ScriptVars.GetInt(critChanceVariable);
        critLog += $"+ {creatureCrit} (créature) ";
        critChance += creatureCrit; 
      }
      else
      {
        // Pour un joueur , la chance de crit dépend de sa maîtrise de l'arme (max + 20 %)

        int playerCrit = weapon is not null ? player.GetWeaponMasteryLevel(weapon.m_idSelf.ToNwObject<NwItem>()) : 0;
        int criticalMastery = player.GetAttributeLevel(SkillSystem.Attribut.CriticalStrikes);
        playerCrit += criticalMastery > attacker.m_pStats.GetDEXStat() ? attacker.m_pStats.GetDEXStat() : criticalMastery;
        critLog += $"+ {playerCrit} (entrainement) ";
        critChance += playerCrit;
      }

      int critRoll = NwRandom.Roll(Utils.random, 100);
      LogUtils.LogMessage($"{critLog} = {critChance} VS {critRoll}", LogUtils.LogType.Combat);

      if (critRoll <= critChance)
      {
        LogUtils.LogMessage("Coup critique ! (+20 AP)", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }*/
  } 
}
