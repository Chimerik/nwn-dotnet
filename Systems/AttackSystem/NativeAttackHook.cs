using Anvil.API;
using NWN.Native.API;
using Anvil.Services;
using System.Linq;
using NWN.Core;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(NativeAttackHook))]
  public unsafe class NativeAttackHook
  {
    private readonly CExoString casterLevelVariable = "_CREATURE_CASTER_LEVEL".ToExoString();
    private readonly CExoString minWeaponDamageVariable = "_MIN_WEAPON_DAMAGE".ToExoString();
    private readonly CExoString maxWeaponDamageVariable = "_MAX_WEAPON_DAMAGE".ToExoString();
    private readonly CExoString minCreatureDamageVariable = "_MIN_CREATURE_DAMAGE".ToExoString();
    private readonly CExoString maxCreatureDamageVariable = "_MAX_CREATURE_DAMAGE".ToExoString();
    private readonly CExoString critChanceVariable = "_ADD_CRIT_CHANCE".ToExoString();
    private readonly CExoString itemGradeVariable = "_ITEM_GRADE".ToExoString();
    private readonly CExoString durabilityVariable = "_DURABILITY".ToExoString();
    private readonly CExoString blindEffectString = "CUSTOM_CONDITION_BLIND".ToExoString();
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

      LogUtils.LogMessage($"{creature.m_pStats.GetFullName().ToExoLocString().GetSimple(0)} attacking {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", LogUtils.LogType.Combat);

      CNWSCombatRound combatRound = creature.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      //attack = creature.m_pStats.GetAttackModifierVersus(targetCreature) - 1 + proficiency bonus - STR MOD si mêlée - DEX MOD si ranged + STAT MOD de l'arme + 5 * combatRound.m_nCurrentAttack 
      // Cas spécifique combat à deux armes : Donner à tous les joueurs les dons two-weapon fighting et ambidextry. Si l'arme est light, donner + 2 à toutes, sinon donner +4
      // Light => l'arme est inférieure d'au moins une catégorie de taille à celle du personnage

      LogUtils.LogMessage($"m_nAttackType : {attackData.m_nAttackType}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"m_nCurrentAttack : {combatRound.m_nCurrentAttack}", LogUtils.LogType.Combat);

      if (targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        CNWSCreature targetCreature = targetObject.AsNWSCreature();
        LogUtils.LogMessage($"targetCreature AC {targetCreature.m_pStats.GetArmorClassVersus(creature)}", LogUtils.LogType.Combat);
        LogUtils.LogMessage($"Attack : {creature.m_pStats.GetAttackModifierVersus(targetCreature)}", LogUtils.LogType.Combat);
      }

      //LogUtils.LogMessage($"melee attack bonus : {creature.m_pStats.GetMeleeAttackBonus()}", LogUtils.LogType.Combat);



      if (attackData.m_nAttackResult == 0 || attackData.m_nAttackResult == 4 || attackData.m_nAttackResult == 3)
        attackData.m_nAttackResult = 1;

      if (targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        CNWSCreature targetCreature = targetObject.AsNWSCreature();
        int skillBonusDodge = 0;
        string logString = "";

        if (targetCreature.m_nCreatureSize < creature.m_nCreatureSize)
        {
          skillBonusDodge += 5;
          logString += "+ 5 (Taille créature) ";
        }

        foreach(var eff in creature.m_appliedEffects)
          if(eff.GetCustomTag() == blindEffectString) // 90 % miss chance if blinded
          {
            if (NwRandom.Roll(Utils.random, 100) < 11)
            {
              attackData.m_nAttackResult = 4;
              attackData.m_nMissedBy = 8;
              LogUtils.LogMessage("Attaque échouée - aveuglement", LogUtils.LogType.Combat);
              return;
            }

            break;
          }

        int armorPenalty = unchecked((sbyte)targetCreature.m_pStats.m_nArmorCheckPenalty) < 0 ? 256 - targetCreature.m_pStats.m_nArmorCheckPenalty : targetCreature.m_pStats.m_nArmorCheckPenalty;
        int shieldPenalty = unchecked((sbyte)targetCreature.m_pStats.m_nShieldCheckPenalty) < 0 ? 256 - targetCreature.m_pStats.m_nShieldCheckPenalty : targetCreature.m_pStats.m_nShieldCheckPenalty;
        int dexScore = unchecked((sbyte)targetCreature.m_pStats.GetAbilityMod(1)) < 0 ? 256 - targetCreature.m_pStats.GetAbilityMod(1) : targetCreature.m_pStats.GetAbilityMod(1);
        int dodgeRoll = NwRandom.Roll(Utils.random, 100);
        int dodgeCalculations = dexScore + skillBonusDodge - armorPenalty - shieldPenalty;

        LogUtils.LogMessage($"{logString} + {dexScore} (DEX) - {armorPenalty + shieldPenalty} (Pénalité d'armure) = {dodgeCalculations} VS {dodgeRoll}", LogUtils.LogType.Combat);
        
        if (dodgeRoll <= dodgeCalculations) // TODO : supprimer l'esquive passive pour la remplacer par une esquive active
        {
          attackData.m_nAttackResult = 4;
          attackData.m_nMissedBy = 8;
          LogUtils.LogMessage("Esquive réussie", LogUtils.LogType.Combat);
        }
        else if (IsHitCritical(creature, targetCreature, combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType)))
          attackData.m_nAttackResult = 3;
      }
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
      //var damageFlags = creatureStats.m_pBaseCreature.GetDamageFlags();

      if (attacker is null || targetObject is null || targetObject.m_bPlotObject == 1)
        return -1;

      LogUtils.LogMessage($"Jet de dégâts : {creatureStats.GetFullName().ToExoLocString().GetSimple(0)} attaque {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", LogUtils.LogType.Combat);

      int minDamage = 0;
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

      // On n'applique pas les calculs de réduction du jeu de base, car ces propriétés n'existent pas. Chez nous, les réductions sont calculées par rapport à l'armure
      // TODO : il faudra donc modifier tous les sorts qui donnnent de la résistance ou de la réduction pour qu'ils donnent de l'armure spécifique
      //damage = target.DoDamageImmunity(creature, damage, damageFlags, 0, 1);
      //damage = target.DoDamageResistance(creature, damage, damageFlags, 0, 1, 1);
      //damage = target.DoDamageReduction(creature, damage, damagePower, 0, 1);

      return damage;
    }
    private static CNWSItem GetAttackWeapon(CNWSCreature attacker, int bOffHand)
    {
      var weapon = ((bOffHand == 1
            ? attacker.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand)
            : attacker.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand)) 
            ?? attacker.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Arms)) // Si pas d'arme, on check les gants
            ?? (bOffHand == 1 // si toujours pas d'arme, on check les griffes
              ? attacker.m_pInventory.GetItemInSlot((uint)EquipmentSlot.CreatureWeaponLeft)
              : attacker.m_pInventory.GetItemInSlot((uint)EquipmentSlot.CreatureWeaponRight)); 

      return weapon;
    }
    private bool IsHitCritical(CNWSCreature attacker, CNWSCreature target, CNWSItem weapon)
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
    }
  } 
}
