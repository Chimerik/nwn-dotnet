using Anvil.API;
using NWN.Native.API;
using Anvil.Services;
using System.Linq;
using NWN.Core;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core.NWNX;

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
    //private readonly CExoString spellIdVariable = "_CURRENT_SPELL".ToExoString();
    
    private delegate int GetDamageRollHook(void* thisPtr, void* pTarget, int bOffHand, int bCritical, int bSneakAttack, int bDeathAttack, int bForceMax);
    private delegate void ResolveAttackRollHook(void* pCreature, void* pTarget);
    private delegate byte GetSpellLikeAbilityCasterLevelHook(void* pCreatureStats, int nSpellId);
    //private delegate byte GetCasterLevelHook(void* pCreatureStats, byte nMultiClass);
    private delegate int AddUseTalentOnObjectHook(void* pCreature, int talentType, int talentId, uint oidTarget, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType);
    private delegate int AddUseTalentAtLocationHook(void* pCreature, int talentType, int talentId, Vector3 vTargetLocation, byte nMultiClass, uint oidItem, int nItemPropertyIndex, byte nCasterLevel, int nMetaType);

    private readonly FunctionHook<GetDamageRollHook> getDamageRollHook;
    private readonly FunctionHook<AddUseTalentOnObjectHook> addUseTalentOnObjectHook;
    private readonly FunctionHook<AddUseTalentAtLocationHook> addUseTalentAtLocationHook;

    public NativeAttackHook(HookService hookService)
    {
      getDamageRollHook = hookService.RequestHook<GetDamageRollHook>(OnGetDamageRoll, FunctionsLinux._ZN17CNWSCreatureStats13GetDamageRollEP10CNWSObjectiiiii, HookOrder.Early);
      hookService.RequestHook<ResolveAttackRollHook>(OnResolveAttackRoll, FunctionsLinux._ZN12CNWSCreature17ResolveAttackRollEP10CNWSObject, HookOrder.Early);
      hookService.RequestHook<GetSpellLikeAbilityCasterLevelHook>(OnGetSpellLikeAbilityCasterLevel, FunctionsLinux._ZN17CNWSCreatureStats30GetSpellLikeAbilityCasterLevelEj, HookOrder.Early);
      //hookService.RequestHook<GetCasterLevelHook>(OnGetCasterLevel, FunctionsLinux.damage, HookOrder.Early); // Malheureusement ce hook est inutile => La fonction n'est jamais appelée en jeu
      addUseTalentOnObjectHook = hookService.RequestHook<AddUseTalentOnObjectHook>(OnAddUseTalentOnObjectHook, FunctionsLinux._ZN12CNWSCreature27AddUseTalentOnObjectActionsEiijhjihh, HookOrder.Early);
      addUseTalentAtLocationHook = hookService.RequestHook<AddUseTalentAtLocationHook>(OnAddUseTalentAtLocationHook, FunctionsLinux._ZN12CNWSCreature29AddUseTalentAtLocationActionsEii6Vectorhjihh, HookOrder.Early);
    }
    private void OnResolveAttackRoll(void* pCreature, void* pTarget)
    {
      CNWSCreature creature = CNWSCreature.FromPointer(pCreature);
      CNWSObject targetObject = CNWSObject.FromPointer(pTarget);

      Utils.LogMessageToConsole($"{creature.m_pStats.GetFullName().ToExoLocString().GetSimple(0)} attacking {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", Config.Env.Chim);

      CNWSCombatRound combatRound = creature.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      if (attackData.m_nAttackResult == 0 || attackData.m_nAttackResult == 4 || attackData.m_nAttackResult == 3)
        attackData.m_nAttackResult = 1;

      if (targetObject.m_nObjectType == (int)ObjectType.Creature)
      {
        CNWSCreature targetCreature = targetObject.AsNWSCreature();
        int skillBonusDodge = PlayerSystem.Players.TryGetValue(targetObject.m_idSelf, out PlayerSystem.Player player) && player.learnableSkills.ContainsKey(CustomSkill.ImprovedDodge) ? 2 * player.learnableSkills[CustomSkill.ImprovedDodge].totalPoints : 0;

        if (targetCreature.m_pStats.HasFeat((ushort)Anvil.API.Feat.Dodge).ToBool())
          skillBonusDodge += 2;

        if (targetCreature.m_nCreatureSize < creature.m_nCreatureSize)
          skillBonusDodge += 5;

        int dodgeRoll = NwRandom.Roll(Utils.random, 100);
        if (dodgeRoll <= unchecked((sbyte)targetCreature.m_pStats.GetAbilityMod(1)) + skillBonusDodge - targetCreature.m_pStats.m_nArmorCheckPenalty - targetCreature.m_pStats.m_nShieldCheckPenalty)
        {
          attackData.m_nAttackResult = 4;
          attackData.m_nMissedBy = 8;
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

      Utils.LogMessageToConsole($"Entering GetDamageRoll Hook : {creatureStats.GetFullName().ToExoLocString().GetSimple(0)} attacking {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}", Config.Env.Chim);

      int minDamage = 0;
      int maxDamage = 0;
      int damage = 0;

      // Get attacker weapon
      if (attacker is not null)
      {
        var weapon = GetAttackWeapon(attacker, bOffHand);

        if (weapon is not null)
        {
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
        }

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
        return false;

      PlayerSystem.Player attackerPlayer = PlayerSystem.Players.GetValueOrDefault(attacker.m_idSelf);
      PlayerSystem.Player defender = PlayerSystem.Players.GetValueOrDefault(target.m_idSelf);

      // Si la cible est en mouvement et est frappée en mêlée par une créature qu'elle ne voit pas, alors crit auto
      if ((weapon is null || ItemUtils.GetItemCategory((BaseItemType)weapon.m_nBaseItem) != ItemUtils.ItemCategory.RangedWeapon) && CreaturePlugin.GetMovementType(target.m_idSelf) > 0)
      {
        var visionNode = target.GetVisibleListElement(attacker.m_idSelf);

        if (visionNode is null || visionNode.m_bSeen < 1 || target.GetFlatFooted() > 0)
        {
          if (defender is not null && defender.learnableSkills.ContainsKey(CustomSkill.UncannyDodge))
          {
            int survivalSkill = defender.learnableSkills.ContainsKey(CustomSkill.Survival) ? defender.learnableSkills[CustomSkill.Survival].totalPoints : 0;

            if (attackerPlayer is not null)
            {
              int deceptionSkill = attackerPlayer.learnableSkills.ContainsKey(CustomSkill.Deception) ? attackerPlayer.learnableSkills[CustomSkill.Deception].totalPoints : 0;

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
            return true;
        }
      }

      int critChance = weapon is not null ? weapon.m_ScriptVars.GetInt(critChanceVariable) : 0 ; // TODO : Gérer les chances de crit pour chaque type d'arme de base

      if (!PlayerSystem.Players.TryGetValue(attacker.m_idSelf, out PlayerSystem.Player player)) // Si l'attaquant n'est pas un joueur, le crit est déterminé par le FP
        critChance += attacker.m_ScriptVars.GetInt(critChanceVariable);
      else
      {
        // Pour un joueur , la chance de crit dépend de sa maîtrise de l'arme (max + 20 %)
        // TODO : Prévoir un skill spécial pour les roubs qui remplace attaque sournoise et augmente les chances de crit (Critical Strikes dans Guild Wars)
        // TODO : Prévoir un enchantement qui permet d'ajouter des chances de crit à une arme (max + 15 %)
        // TODO : Prévoir des capacités qui donnent des chances de crit temporaire (cf page Critical Hit du Guild Wars wiki)
        // TODO : chaque attaque spéciale d'une arme a des effets supplémentaires dont la puissance dépend du niveau de maîtrise et d'expertise dans l'arme

        critChance += weapon is not null ? player.GetWeaponCritScienceLevel((BaseItemType)weapon.m_nBaseItem) : 0;
      }

      if (NwRandom.Roll(Utils.random, 100) < critChance)
        return true;
      else
        return false;
    }
  } 
}
