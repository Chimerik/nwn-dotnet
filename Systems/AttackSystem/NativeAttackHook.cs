﻿using Anvil.API;
using NWN.Native.API;
using Anvil.Services;
using System.Linq;
using NWN.Core;
using System.Numerics;

namespace NWN.Systems
{
  [ServiceBinding(typeof(NativeAttackHook))]
  public unsafe class NativeAttackHook
  {
    private readonly CExoString casterLevelVariable = "_CREATURE_CASTER_LEVEL".ToExoString();
    private readonly CExoString durabilityVariable = "_DURABILITY".ToExoString();
    private readonly CExoString maxDurabilityVariable = "_MAX_DURABILITY".ToExoString();

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

    [NativeFunction("_ZN17CNWSCreatureStats21GetSpellGainWithBonusEhh", null)]
    private delegate byte GetSpellGainWithBonusHook(byte nMultiClass, byte nSpellLevel);

    //[NativeFunction("_ZN17CNWSCreatureStats33GetSpellGainWithBonusAfterLevelUpEhhP13CNWLevelStatshi", null)]
    //private delegate byte GetSpellGainWithBonusAfterLevelUpHook(byte creatureClass, byte spellLevel, CNWLevelStats levelStats , byte school, int newClass = 0);

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
      CNWSCombatRound combatRound = creature.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      string targetName = $"{targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}";
      string attackerName = $"{creature.GetFirstName().GetSimple(0)} {creature.GetLastName().GetSimple(0)}";

      LogUtils.LogMessage($"----- {attackerName} attaque {targetName} - type {attackData.m_nAttackType} - nb {combatRound.m_nCurrentAttack} -----", LogUtils.LogType.Combat);
      
      CNWSItem targetArmor = targetCreature?.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Chest);

      if (targetCreature is not null && targetCreature.m_bPlayerCharacter > 0 && targetArmor is not null
        && targetArmor.m_ScriptVars.GetInt(maxDurabilityVariable) > 0 && targetArmor.m_ScriptVars.GetInt(durabilityVariable) < 1)
      {
        attackData.m_nAttackResult = 3;
        NativeUtils.SendNativeServerMessage("CRITIQUE AUTOMATIQUE - Armure en ruine ".ColorString(StringUtils.gold), targetCreature);
        LogUtils.LogMessage("Armure ruinée : critique automatique", LogUtils.LogType.Combat);
        return;
      }
      
      if (targetCreature is not null && attackData.m_bRangedAttack > 0 && creature.m_bPlayerCharacter > 0 
        && (creature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Blindness || (EffectTrueType)e.m_nType == EffectTrueType.Darkness)
        || targetCreature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Darkness))
        && Vector3.DistanceSquared(creature.m_vPosition.ToManagedVector(), targetCreature.m_vPosition.ToManagedVector()) > 9)
      {
        attackData.m_nAttackResult = 4;
        attackData.m_nMissedBy = 2;
        NativeUtils.SendNativeServerMessage($"ECHEC AUTOMATIQUE - Cible à plus de 3m".ColorString(ColorConstants.Red), creature);
        LogUtils.LogMessage($"Attaquant aveuglé et cible à plus de 3m : échec automatique", LogUtils.LogType.Combat);
        return;
      }

      // Si l'arme utilisée pour attaquer est une arme de finesse, et que la créature a une meilleur DEX, alors on utilise la DEX pour attaquer
      CNWSItem attackWeapon = combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);
      Anvil.API.Ability attackStat = Anvil.API.Ability.Strength;

      //*** CALCUL DU BONUS D'ATTAQUE ***//
      byte dexMod = creature.m_pStats.m_nDexterityModifier;
      byte strMod = creature.m_pStats.m_nStrengthModifier;
      int dexBonus = dexMod > 122 ? dexMod - 255 : dexMod;
      int strBonus = strMod > 122 ? strMod - 255 : strMod;

      // On prend le bonus d'attaque calculé automatiquement par le jeu en fonction de la cible qui peut être une créature ou un placeable
      int attackModifier = targetCreature is null ? creature.m_pStats.GetAttackModifierVersus() : NativeUtils.GetAttackBonus(creature, targetCreature, attackData, attackWeapon, strBonus, dexBonus);

      if (creature.m_pStats.HasFeat(CustomSkill.TotemLienTigre).ToBool() && attackWeapon is not null
        && ItemUtils.IsMeleeWeapon(NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem))
        && targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.saignementEffectExoTag).ToBool()
        || (EffectTrueType)e.m_nType == EffectTrueType.Poison))
        strBonus *= 2;

      if (creature.m_pStats.GetNumLevelsOfClass(CustomClass.Monk) > 0
        && (attackWeapon is null || NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).IsMonkWeapon))
        attackStat = dexBonus > strBonus ? Anvil.API.Ability.Dexterity : Anvil.API.Ability.Strength;
      else
        attackStat = attackWeapon is null || attackData.m_bRangedAttack < 1 ? Anvil.API.Ability.Strength : Anvil.API.Ability.Dexterity;

      if (attackWeapon != null
          && dexBonus > strBonus
          && attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable) != 0)
      {
        attackStat = Anvil.API.Ability.Dexterity;
      }

      // On ajoute le bonus de maîtrise de la créature
      int attackBonus = NativeUtils.GetCreatureWeaponProficiencyBonus(creature, attackWeapon);
      LogUtils.LogMessage($"Bonus de maîtrise {attackBonus} {(attackBonus < 1 ? "(Arme non maîtrisée)" : "")}", LogUtils.LogType.Combat);
      
      NativeUtils.HandleCrossbowMaster(creature, targetObject, combatRound, attackBonus, attackerName);

      // TODO : Dans certains cas, la STAT à utiliser pourra être INT, SAG ou CHA, à implémenter 
      switch (attackStat)
      {
        case Anvil.API.Ability.Strength:

          LogUtils.LogMessage($"Ajout modificateur de force : {strBonus}", LogUtils.LogType.Combat);
          attackBonus += strBonus;

          break;

        case Anvil.API.Ability.Dexterity:

          LogUtils.LogMessage($"Ajout modificateur de dextérité : {dexBonus}", LogUtils.LogType.Combat);
          attackBonus += dexBonus;

          break;
      }

      attackBonus += attackModifier;

      if (attackData.m_nWeaponAttackType == 2) // combat à deux armes
      {
        int bonusAction = creature.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable);

        if (bonusAction > 0) // L'attaque supplémentaire consomme l'action bonus du personnage
          creature.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, bonusAction - 1);
        else // Si pas d'action bonus dispo, auto miss
        {
          attackData.m_nAttackResult = 4;
          attackData.m_nMissedBy = 8;

          NativeUtils.SendNativeServerMessage($"Main secondaire - Echec automatique - Pas d'action bonus disponible".ColorString(ColorConstants.Red), creature);
        }
      }

      string opportunityString = "";

      if(attackData.m_nAttackType == 65002) // 65002 = attaque d'opportunité
      {
        opportunityString = $"Attaque d'opportunité {creature.m_ScriptVars.GetString(CreatureUtils.OpportunityAttackTypeVariableExo)}- ";
        creature.m_ScriptVars.DestroyString(CreatureUtils.OpportunityAttackTypeVariableExo);
      }

      if (targetCreature is not null)
      {
        if(NativeUtils.IsAttackRedirected(creature, targetCreature, combatRound, attackerName, targetName))
        {
          attackData.m_nMissedBy = 2;
          attackData.m_nAttackResult = 4;
          return;
        }

        int advantage = CreatureUtils.GetAdvantageAgainstTarget(creature, attackData, attackWeapon, attackStat, targetCreature);
        creature.m_ScriptVars.SetInt($"_ADVANTAGE_ATTACK_{combatRound.m_nCurrentAttack}".ToExoString(), advantage);

        int attackRoll = NativeUtils.GetAttackRoll(creature, advantage, attackStat);
        int targetAC  = NativeUtils.GetCreatureAC(targetCreature, creature);
        bool isCriticalHit = attackRoll >= NativeUtils.GetCriticalRange(creature, attackWeapon, attackData);

        if (isCriticalHit && targetCreature.m_ScriptVars.GetInt(CreatureUtils.SecondeChanceVariableExo).ToBool())
        {
          attackRoll = NativeUtils.GetAttackRoll(creature, advantage, attackStat);
          isCriticalHit = attackRoll >= NativeUtils.GetCriticalRange(creature, attackWeapon, attackData);
          NativeUtils.SendNativeServerMessage("Seconde chance".ColorString(StringUtils.gold), targetCreature);
        }

        string hitString = "touchez".ColorString(new Color(32, 255, 32));
        string rollString = $"{attackRoll} + {attackBonus} = {attackRoll + attackBonus}".ColorString(new Color(32, 255, 32));
        string criticalString = "";
        string advantageString = advantage == 0 ? "" : advantage > 0 ? "Avantage - ".ColorString(StringUtils.gold) : "Désavantage - ".ColorString(ColorConstants.Red);
        int totalAttack = attackRoll + attackBonus;
        int defensiveDuellistBonus = NativeUtils.GetDefensiveDuellistBonus(targetCreature, attackData.m_bRangedAttack);
        int superiorityDiceBonus = 0;
        int inspirationBardique = 0;
        string inspirationString = "";
        CGameEffect inspirationEffect = null;

        if (targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.ImageMiroirEffectExoTag).ToBool()))
        {
          int nbImages = targetObject.m_ScriptVars.GetInt(EffectSystem.ImageMiroirEffectExoTag);
          int dupliRoll = NwRandom.Roll(Utils.random, 20);

          bool dupliHit = nbImages switch
          {
            3 => dupliRoll > 5,
            2 => dupliRoll > 7,
            _ => dupliRoll > 10,
          };

          if (dupliHit)
          {
            hitString = "manquez".ColorString(ColorConstants.Red);

            byte dexModDupli = targetCreature.m_pStats.m_nDexterityModifier;
            int dexDupli = dexModDupli > 122 ? dexModDupli - 255 : dexModDupli;

            if (totalAttack >= 10 + dexDupli)
            {
              nbImages -= 1;

              if (nbImages < 1)
              {
                EffectUtils.RemoveTaggedEffect(targetCreature, EffectSystem.ImageMiroirEffectExoTag);
                targetObject.m_ScriptVars.DestroyInt(EffectSystem.ImageMiroirEffectExoTag);
              }
              else
                targetObject.m_ScriptVars.SetInt(EffectSystem.ImageMiroirEffectExoTag, nbImages);

              NativeUtils.SendNativeServerMessage($"{advantageString}{opportunityString}{criticalString}Vous {hitString} l'image miroir {nbImages} de {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature);
              NativeUtils.BroadcastNativeServerMessage($"{advantageString}{opportunityString}{criticalString}{attackerName.ColorString(ColorConstants.Cyan)} {hitString.Replace("z", "")} une image miroir de {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature, true);
            }
            else
            {
              NativeUtils.SendNativeServerMessage($"{advantageString}{opportunityString}{criticalString}Vous {hitString} l'image miroir {nbImages} de {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature);
              NativeUtils.BroadcastNativeServerMessage($"{advantageString}{opportunityString}{criticalString}{attackerName.ColorString(ColorConstants.Cyan)} {hitString.Replace("z", "")} une image miroir de {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature, true);
            }

            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = 8;
            return;
          }
        }

        if (creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterAttaquePrecise)
        {
          int superiorityDice = creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
          superiorityDiceBonus = NwRandom.Roll(Utils.random, superiorityDice);
          LogUtils.LogMessage($"Attaque précise : dé de supériorité 1d{superiorityDice} (+{superiorityDiceBonus})", LogUtils.LogType.Combat);
        }

        foreach (var eff in creature.m_appliedEffects)
          if (eff.m_sCustomTag.CompareNoCase(EffectSystem.inspirationBardiqueEffectExoTag).ToBool())
          {
            inspirationBardique = eff.m_nCasterLevel;
            inspirationEffect = eff;
            inspirationString = inspirationBardique > 0 ? "Inspiration Bardique +" : "Mots Cinglants ";
            break;
          }

        bool isAssassinate = NativeUtils.IsAssassinate(creature);

        if (isCriticalHit || isAssassinate 
          || (attackData.m_bRangedAttack < 1 && targetCreature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.SetState && e.GetInteger(0) == 8))) // Si la cible est paralysée, que l'attaque touche et est en mêlée, alors critique auto
        {
          attackData.m_nAttackResult = 3;
          criticalString = "CRITIQUE - ".ColorString(StringUtils.gold);
          
          if(isAssassinate)
            LogUtils.LogMessage("Coup critique - Assassinat", LogUtils.LogType.Combat);
          else
            LogUtils.LogMessage("Coup critique", LogUtils.LogType.Combat);
        }
        else if (attackRoll > 1 && totalAttack >= targetAC) // L'attaque touche
        {
          if (totalAttack + inspirationBardique + superiorityDiceBonus < targetAC + defensiveDuellistBonus) // Echoue alors qu'elle aurait du toucher
          {
            if (inspirationBardique < 0)
              NativeUtils.HandleInspirationBardiqueUsed(creature, inspirationBardique, inspirationEffect, inspirationString, attackerName);

            if (superiorityDiceBonus > 0)
              NativeUtils.HandleAttaquePreciseUsed(creature, superiorityDiceBonus);

            if (defensiveDuellistBonus > 0)
              NativeUtils.SendNativeServerMessage("Duelliste défensif activé !".ColorString(ColorConstants.Orange), targetCreature);

            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = (byte)(targetAC - attackRoll) > 8 ? (byte)Utils.random.Next(1, 9) : (byte)(targetAC - attackRoll);
            hitString = "manquez".ColorString(ColorConstants.Red);
            rollString = $"{attackRoll} + {attackBonus + inspirationBardique + superiorityDiceBonus} = {attackRoll + attackBonus + inspirationBardique + superiorityDiceBonus}".ColorString(new Color(32, 255, 32));
            rollString = rollString.StripColors().ColorString(ColorConstants.Red);

            LogUtils.LogMessage($"Manqué : {attackRoll} + {attackBonus + inspirationBardique + superiorityDiceBonus} = {attackRoll + attackBonus + inspirationBardique + superiorityDiceBonus} vs {targetAC + defensiveDuellistBonus}", LogUtils.LogType.Combat);
          }
          else // Touche : cas normal
          {
            attackData.m_nAttackResult = 1;
            LogUtils.LogMessage($"Touché : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
          }
        }
        else if (attackRoll > 1) // L'attaque échoue
        {
          if (totalAttack + inspirationBardique + superiorityDiceBonus >= targetAC + defensiveDuellistBonus) // Touche alors qu'elle aurait du échouer
          {
            if (inspirationBardique > 0)
              NativeUtils.HandleInspirationBardiqueUsed(creature, inspirationBardique, inspirationEffect, inspirationString, attackerName);

            if (superiorityDiceBonus > 0)
              NativeUtils.HandleAttaquePreciseUsed(creature, superiorityDiceBonus);

            if (defensiveDuellistBonus > 0)
              NativeUtils.SendNativeServerMessage("Duelliste défensif activé !".ColorString(ColorConstants.Orange), targetCreature);

            attackData.m_nAttackResult = 1;
            rollString = $"{attackRoll} + {attackBonus + inspirationBardique + superiorityDiceBonus} = {attackRoll + attackBonus + inspirationBardique + superiorityDiceBonus}".ColorString(new Color(32, 255, 32));
            LogUtils.LogMessage($"Touché : {attackRoll} + {attackBonus + inspirationBardique + superiorityDiceBonus} = {attackRoll + attackBonus + inspirationBardique + superiorityDiceBonus} vs {targetAC + defensiveDuellistBonus}", LogUtils.LogType.Combat);
          }
          else // Echoue : cas normal
          {
            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = (byte)(targetAC - attackRoll) > 8 ? (byte)Utils.random.Next(1, 9) : (byte)(targetAC - attackRoll);
            hitString = "manquez".ColorString(ColorConstants.Red);
            rollString = rollString.StripColors().ColorString(ColorConstants.Red);
            LogUtils.LogMessage($"Manqué : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
          }
        }
        else // Attack roll = 1 : échec auto
        {
          attackData.m_nAttackResult = 4;
          attackData.m_nMissedBy = (byte)(targetAC - attackRoll) > 8 ? (byte)Utils.random.Next(1, 9) : (byte)(targetAC - attackRoll);
          hitString = "manquez".ColorString(ColorConstants.Red);
          rollString = rollString.StripColors().ColorString(ColorConstants.Red);
          LogUtils.LogMessage($"Manqué : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
        }

        if (attackData.m_nAttackResult == 4)
        {
          NativeUtils.HandleRafaleDuTraqueur(creature, targetObject, combatRound, attackerName, targetName);
          NativeUtils.HandleRiposte(creature, targetCreature, attackData, attackerName);
          creature.m_ScriptVars.DestroyInt(FeatSystem.BotteDamageExoVariable);

          if (creature.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
            creature.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreRiposteVariableExo);
        }
 
        NativeUtils.SendNativeServerMessage($"{advantageString}{opportunityString}{criticalString}Vous {hitString} {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature);
        NativeUtils.BroadcastNativeServerMessage($"{advantageString}{opportunityString}{criticalString}{attackerName.ColorString(ColorConstants.Cyan)} {hitString.Replace("z", "")} {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), creature, true);

        NativeUtils.HandleSentinelleOpportunityTarget(creature, combatRound, attackerName);
        NativeUtils.HandleSentinelle(creature, targetCreature, combatRound);
        NativeUtils.HandleFureurOrc(creature, targetCreature, combatRound, attackerName);
        NativeUtils.HandleDiversion(creature, attackData, targetCreature);
        NativeUtils.HandleTueurDeGeants(creature, targetCreature, combatRound, attackerName, attackWeapon, attackData.m_bRangedAttack.ToBool());
        NativeUtils.HandleMonkOpportunist(creature, targetCreature, attackData, combatRound, attackerName, targetName);
      }
      else
        attackData.m_nAttackResult = 7;

      NativeUtils.HandleHastMaster(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleBalayage(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleRiposteBonusAttack(creature, combatRound, attackData, attackerName);
      NativeUtils.HandleFrappeFrenetiqueBonusAttack(creature, targetObject, combatRound, attackData, attackerName);
      NativeUtils.HandleBersekerRepresaillesBonusAttack(creature, combatRound, attackData, attackerName);
      NativeUtils.HandleTigreAspect(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleArcaneArcherTirIncurveBonusAttack(creature, attackData, combatRound, attackerName, attackWeapon, targetObject);
      NativeUtils.HandleMonkBonusAttack(creature, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleMonkDeluge(creature, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleThiefReflex(creature, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleBardeBotteTranchante(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleBriseurDeHordes(creature, targetObject, combatRound, attackerName, attackWeapon, attackData.m_bRangedAttack.ToBool());
      NativeUtils.HandleVolee(creature, targetObject, combatRound, attackData.m_bRangedAttack.ToBool(), attackerName);
      NativeUtils.HandleAttaqueCoordonnee(creature, targetObject, combatRound);
      NativeUtils.HandleFurieBestiale(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleVoeuHostile(creature, combatRound, attackData, attackerName);
      NativeUtils.HandleClercMartial(creature, targetObject, combatRound, attackerName);
      NativeUtils.HandleFureurOuraganFoudre(targetObject, attackData);
      NativeUtils.HandleFureurTonnerreFoudre(targetObject, attackData);
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

      if (attacker is null || targetObject is null || targetObject.m_bPlotObject == 1
        || attacker.m_ScriptVars.GetInt(CreatureUtils.CancelDamageDoublonVariableExo).ToBool())
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.CancelDamageDoublonVariableExo);
        return -1;
      }

      CNWSCombatRound combatRound = attacker.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      if (attackData.m_nAttackResult < 1)
        return -1;

      CNWSCreature targetCreature = targetObject.m_nObjectType == (int)ObjectType.Creature ? targetObject.AsNWSCreature() : null;
      string attackerName = $"{creatureStats.GetFullName().ToExoLocString().GetSimple(0)}";

      LogUtils.LogMessage($"----- Jet de dégâts : {creatureStats.GetFullName().ToExoLocString().GetSimple(0)} attaque {targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)} - type {attackData.m_nAttackType} - nb {combatRound.m_nCurrentAttack} -----", LogUtils.LogType.Combat);

      CNWSItem attackWeapon = combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);
      int baseDamage = 0;
      bool isDuelFightingStyle = false;
      int sneakAttack = 0;

      // Jet de dégâts de l'arme
      if (attackWeapon is not null)
      {
        if (attackWeapon.GetPropertyByTypeExists((ushort)Native.API.ItemProperty.NoDamage) > 0)
          return -1;

        if (!attacker.m_ScriptVars.GetInt(CreatureUtils.MonkUnarmedDamageVariableExo).ToBool())
        {
          NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);

          if (attackData.m_nAttackType == 65002 && attacker.m_ScriptVars.GetInt(CreatureUtils.HastMasterSpecialAttackExo).ToBool())
          {
            baseDamage = NwRandom.Roll(Utils.random, 4, 1);

            if (!bCritical.ToBool())
              attacker.m_ScriptVars.DestroyInt(CreatureUtils.HastMasterSpecialAttackExo);
          }
          else
          {
            baseDamage += NativeUtils.RollWeaponDamage(attacker, baseWeapon, attackData, targetCreature, attackWeapon);
            isDuelFightingStyle = NativeUtils.IsDuelFightingStyle(attacker, baseWeapon, attackData);
            baseDamage += isDuelFightingStyle ? 2 : 0;
          }

          sneakAttack = NativeUtils.GetSneakAttackDamage(attacker, targetCreature, attackWeapon, attackData, combatRound);
          baseDamage += sneakAttack;

          if (NativeUtils.IsCogneurLourd(attacker, attackWeapon))
          {
            baseDamage += 10;
            LogUtils.LogMessage($"Cogneur Lourd : +10 dégâts", LogUtils.LogType.Combat);
          }
          else if (NativeUtils.IsTireurDelite(attacker, attackData, attackWeapon))
          {
            baseDamage += 10;
            LogUtils.LogMessage($"Tireur d'élite : +10 dégâts", LogUtils.LogType.Combat);
          }
        }
        else
        {
          baseDamage += NativeUtils.GetUnarmedDamage(attacker);
        }
      }
      else
      {
        baseDamage += NativeUtils.GetUnarmedDamage(attacker);
      }

      if (bCritical > 0)
      {
        int critDamage = NativeUtils.GetCritDamage(attacker, attackWeapon, attackData, sneakAttack, isDuelFightingStyle, targetCreature);
        LogUtils.LogMessage($"Critique - Base {baseDamage} + crit {critDamage} = {baseDamage + critDamage}", LogUtils.LogType.Combat);
        baseDamage += critDamage;
      }

      int monkUnarmedDamage = attacker.m_ScriptVars.GetInt(CreatureUtils.MonkUnarmedDamageVariableExo);
      if (monkUnarmedDamage > 0) 
      {
        monkUnarmedDamage -= 1;

        if (monkUnarmedDamage < 1)
          attacker.m_ScriptVars.DestroyInt(CreatureUtils.MonkUnarmedDamageVariableExo);
        else
          attacker.m_ScriptVars.SetInt(CreatureUtils.MonkUnarmedDamageVariableExo, monkUnarmedDamage);
      }

      // Ajout du bonus de caractéristique
      byte dexMod = creatureStats.m_nDexterityModifier;
      byte strMod = creatureStats.m_nStrengthModifier;
      int dexBonus = dexMod > 122 ? dexMod - 255 : dexMod;
      int strBonus = strMod > 122 ? strMod - 255 : strMod;
      int damageBonus = 0;

      if (attackData.m_bRangedAttack > 0)
      {
        damageBonus += dexBonus;
        LogUtils.LogMessage($"Arme à distance - Ajout Dextérité ({dexBonus})", LogUtils.LogType.Combat);
      }
      else if (attacker.m_pStats.GetNumLevelsOfClass(CustomClass.Monk) > 0 // Les moins utilisent leur caract la plus élevée à mains nues ou avec arme de moine
        && (attackWeapon is null || NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).IsMonkWeapon))
      {
        if (dexBonus > strBonus)
        {
          damageBonus += dexBonus;
          LogUtils.LogMessage($"Moine - Ajout Dextérité ({dexBonus})", LogUtils.LogType.Combat);
        }
        else
        {
          damageBonus += strBonus;
          LogUtils.LogMessage($"Moine - Ajout Force ({strBonus})", LogUtils.LogType.Combat);
        }
      }// Si arme de finesse et dextérité plus élevée, alors on utilise la dextérité
      else if(attackWeapon is not null &&  attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable) != 0 
        && dexBonus > strBonus)
      {
        damageBonus +=  dexBonus;
        LogUtils.LogMessage($"Arme de finesse - Ajout Dextérité ({dexBonus})", LogUtils.LogType.Combat);
      }
      else
      {
        damageBonus += strBonus;
        LogUtils.LogMessage($"Ajout Force ({strBonus})", LogUtils.LogType.Combat);
      }
      // Pour l'attaque de la main secondaire, on n'ajoute le modificateur de caractéristique que s'il est négatif
      if (bOffHand < 1 || NativeUtils.HasTwoWeaponStyle(attacker) || (bOffHand > 0 && dexBonus < 0))
        baseDamage += damageBonus;
      /*else
        LogUtils.LogMessage($"Main secondaire - Bonus de caractéristique non appliqué aux dégâts", LogUtils.LogType.Combat);*/

      baseDamage += NativeUtils.HandleBagarreurDeTaverne(attacker, attackWeapon, strBonus);
      baseDamage += NativeUtils.HandleAnimalCompanionBonusDamage(attacker);

      if (targetCreature is not null)
      {
        baseDamage += NativeUtils.HandleMarqueDuChasseurBonusDamage(attacker, targetCreature, attackWeapon);
        baseDamage += NativeUtils.HandlePourfendeurDeColosse(attacker, targetCreature, attackWeapon);
        baseDamage -= NativeUtils.HandleMaitreArmureLourde(targetCreature);
        baseDamage -= NativeUtils.HandleParade(targetCreature);
        baseDamage -= NativeUtils.HandleParadeDeProjectile(targetCreature, attackData.m_bRangedAttack.ToBool());
        baseDamage /= NativeUtils.HandleEsquiveInstinctive(targetCreature);
      }

      if (attacker.m_ScriptVars.GetInt(CreatureUtils.TirAffaiblissantVariableExo).ToBool())
      {
        baseDamage /= 2;
        LogUtils.LogMessage($"Tir affaiblissant : Dégâts {baseDamage}", LogUtils.LogType.Combat);
      }
      
      LogUtils.LogMessage($"Dégâts : {baseDamage}", LogUtils.LogType.Combat);

      // Application des réductions du jeu de base
      
      baseDamage = targetObject.DoDamageImmunity(attacker, baseDamage, damageFlags, 0, 1);
      LogUtils.LogMessage($"Application des immunités de la cible - Dégats : {baseDamage}", LogUtils.LogType.Combat);
      baseDamage = targetObject.DoDamageResistance(attacker, baseDamage, damageFlags, 0, 1, 1, attackData.m_bRangedAttack);
      LogUtils.LogMessage($"Application des résistances de la cible - Dégâts : {baseDamage}", LogUtils.LogType.Combat);
      baseDamage = targetObject.DoDamageReduction(attacker, baseDamage, attacker.CalculateDamagePower(targetObject, bOffHand), 0, 1, attackData.m_bRangedAttack);
      LogUtils.LogMessage($"Application des réductions de la cible - Calcul Final - Dégâts : {baseDamage}", LogUtils.LogType.Combat);

      if(attacker.m_ScriptVars.GetInt(CreatureUtils.AspectTigreMalusVariableExo) > 1)
        baseDamage = baseDamage * 3 / 4;

      NativeUtils.HandleCogneurLourdBonusAttack(attacker, targetObject, combatRound, attackData, baseDamage, attackerName);
      NativeUtils.HandleFrappeDivineRemoval(attacker);
      
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) != CustomSkill.WarMasterAttaquePrecise)
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);
      }

      if (baseDamage < 1)
      {
        attacker.m_ScriptVars.SetInt(CreatureUtils.CancelDamageDoublonVariableExo, 1);

        if(targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.abjurationWardEffectExoTag).ToBool()))
        {
          targetObject.m_ScriptVars.SetInt(CreatureUtils.AbjurationWardForcedTriggerVariableExo, 1);
          EffectUtils.RemoveTaggedEffect(targetObject, EffectSystem.abjurationWardEffectExoTag);
        }
      }


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
