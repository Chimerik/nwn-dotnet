using Anvil.API;
using NWN.Native.API;
using Anvil.Services;
using System.Linq;
using NWN.Core;
using System.Numerics;
using System.Collections.Generic;

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

    //[NativeFunction("_ZN17CNWSCreatureStats21GetSpellGainWithBonusEhh", null)]
    //private delegate byte GetSpellGainWithBonusHook(byte nMultiClass, byte nSpellLevel);

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
      CNWSCreature attacker = CNWSCreature.FromPointer(pCreature);
      CNWSObject targetObject = CNWSObject.FromPointer(pTarget);
      CNWSCreature targetCreature = targetObject.m_nObjectType == (int)ObjectType.Creature ? targetObject.AsNWSCreature() : null;
      CNWSCombatRound combatRound = attacker.m_pcCombatRound;
      CNWSCombatAttackData attackData = combatRound.GetAttack(combatRound.m_nCurrentAttack);

      string targetName = $"{targetObject.GetFirstName().GetSimple(0)} {targetObject.GetLastName().GetSimple(0)}";
      string attackerName = $"{attacker.GetFirstName().GetSimple(0)} {attacker.GetLastName().GetSimple(0)}";

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
      
      if (targetCreature is not null && attackData.m_bRangedAttack > 0 && attacker.m_bPlayerCharacter > 0 
        && (attacker.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Blindness || (EffectTrueType)e.m_nType == EffectTrueType.Darkness)
        || targetCreature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Darkness))
        && Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), targetCreature.m_vPosition.ToManagedVector()) > 9)
      {
        attackData.m_nAttackResult = 4;
        attackData.m_nMissedBy = 2;
        NativeUtils.SendNativeServerMessage($"ECHEC AUTOMATIQUE - Cible à plus de 3m".ColorString(ColorConstants.Red), attacker);
        LogUtils.LogMessage($"Attaquant aveuglé et cible à plus de 3m : échec automatique", LogUtils.LogType.Combat);
        return;
      }

      // Si l'arme utilisée pour attaquer est une arme de finesse, et que la créature a une meilleur DEX, alors on utilise la DEX pour attaquer
      CNWSItem attackWeapon = combatRound.GetCurrentAttackWeapon(attackData.m_nWeaponAttackType);

      //*** CALCUL DU BONUS D'ATTAQUE ***//
      // On prend le bonus d'attaque calculé automatiquement par le jeu en fonction de la cible qui peut être une créature ou un placeable
      Anvil.API.Ability attackAbility = NativeUtils.GetAttackAbility(attacker, attackData.m_bRangedAttack.ToBool(), attackWeapon);
      attackAbility = NativeUtils.HandleCoupAuBut(attacker, attackWeapon, attackAbility, EffectSystem.CoupAuButAttackEffectTag);
      attackAbility = NativeUtils.HandleShillelagh(attacker, attackWeapon, attackAbility);
      int attackModifier = targetCreature is null ? attacker.m_pStats.GetAttackModifierVersus() : NativeUtils.GetAttackBonus(attacker, targetCreature, attackData, attackWeapon, attackAbility);

      // On ajoute le bonus de maîtrise de la créature
      int attackBonus = NativeUtils.GetCreatureWeaponProficiencyBonus(attacker, attackWeapon);
      LogUtils.LogMessage($"Bonus de maîtrise {attackBonus} {(attackBonus < 1 ? "(Arme non maîtrisée)" : "")}", LogUtils.LogType.Combat);

      NativeUtils.HandleCrossbowMaster(attacker, targetObject, combatRound, attackBonus, attackerName);

      string opportunityString = "";

      if(attackData.m_nAttackType == 65002) // 65002 = attaque d'opportunité
      {
        opportunityString = $"Attaque d'opportunité {attacker.m_ScriptVars.GetString(CreatureUtils.OpportunityAttackTypeVariableExo)}- ";
        attacker.m_ScriptVars.DestroyString(CreatureUtils.OpportunityAttackTypeVariableExo);
      }

      if (targetCreature is not null)
      {
        if(NativeUtils.IsAttackRedirected(attacker, targetCreature, combatRound, attackerName, targetName))
        {
          attackData.m_nMissedBy = 2;
          attackData.m_nAttackResult = 4;
          return;
        }

        int advantage = CreatureUtils.GetAdvantageAgainstTarget(attacker, attackData, attackWeapon, attackAbility, targetCreature);
        attacker.m_ScriptVars.SetInt($"_ADVANTAGE_ATTACK_{combatRound.m_nCurrentAttack}".ToExoString(), advantage);

        int attackRoll = NativeUtils.GetAttackRoll(attacker, advantage, attackAbility);
        int targetAC  = NativeUtils.GetCreatureAC(targetCreature, attacker);
        bool isCriticalHit = attackRoll >= NativeUtils.GetCriticalRange(attacker, attackWeapon, attackData);

        if (isCriticalHit && targetCreature.m_ScriptVars.GetInt(CreatureUtils.SecondeChanceVariableExo).ToBool())
        {
          attackRoll = NativeUtils.GetAttackRoll(attacker, advantage, attackAbility);
          isCriticalHit = attackRoll >= NativeUtils.GetCriticalRange(attacker, attackWeapon, attackData);
          NativeUtils.SendNativeServerMessage("Seconde chance".ColorString(StringUtils.gold), targetCreature);
        }

        attackBonus += attackModifier;
        string hitString = "touchez".ColorString(new Color(32, 255, 32));
        string rollString = $"{attackRoll} + {attackBonus} = {attackRoll + attackBonus}".ColorString(new Color(32, 255, 32));
        string criticalString = "";
        string advantageString = advantage == 0 ? "" : advantage > 0 ? "Avantage - ".ColorString(StringUtils.gold) : "Désavantage - ".ColorString(ColorConstants.Red);
        int totalAttack = attackRoll + attackBonus;
        int defensiveDuellistBonus = NativeUtils.GetDefensiveDuellistBonus(targetCreature, attackData.m_bRangedAttack);
        int superiorityDiceBonus = 0;

        CGameEffect inspirationEffect = attacker.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.CompareNoCase(EffectSystem.inspirationBardiqueEffectExoTag).ToBool());
        int inspirationBardique = inspirationEffect is not null ? inspirationEffect.m_nCasterLevel : 0;
        string inspirationString = inspirationBardique != 0 ? inspirationBardique > 0 ? "Inspiration Bardique +" : "Mots Cinglants " : "";

        CGameEffect shieldEffect = targetObject.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.BouclierEffectTag);
        int shieldBonus = shieldEffect is not null ? 5 : 0;

        CGameEffect frappeGuideeEffect = targetObject.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.FrappeGuideeEffectTag);
        int frappeGuideeBonus = frappeGuideeEffect is not null ? 10 : 0;

        if (targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.ImageMiroirEffectExoTag).ToBool()))
        {
          int nbImages = targetObject.m_ScriptVars.GetInt(EffectSystem.ImageMiroirEffectExoTag);
          List<int> imagesRoll = new();

          for(int i = 0; i < nbImages; i++)
            imagesRoll.Add(Utils.Roll(6));

          if (imagesRoll.Any(r => r > 2))
          {
            nbImages -= 1;

            if (nbImages < 1)
            {
              EffectUtils.RemoveTaggedNativeEffect(targetCreature, EffectSystem.ImageMiroirEffectTag);
              targetObject.m_ScriptVars.DestroyInt(EffectSystem.ImageMiroirEffectExoTag);
            }
            else
              targetObject.m_ScriptVars.SetInt(EffectSystem.ImageMiroirEffectExoTag, nbImages);

            NativeUtils.SendNativeServerMessage($"Vous touchez l'image miroir de {targetName.ColorString(ColorConstants.Cyan)}".ColorString(ColorConstants.Cyan), attacker);
            NativeUtils.BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} touche une image miroir de {targetName.ColorString(ColorConstants.Cyan)}".ColorString(ColorConstants.Cyan), attacker, true);

            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = 8;
            return;
          }
        }

        if (attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterAttaquePrecise)
        {
          int superiorityDice = attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreDiceVariableExo);
          superiorityDiceBonus = NwRandom.Roll(Utils.random, superiorityDice);
          LogUtils.LogMessage($"Attaque précise : dé de supériorité 1d{superiorityDice} (+{superiorityDiceBonus})", LogUtils.LogType.Combat);
        }

        if (isCriticalHit 
          || (attackData.m_bRangedAttack < 1 && targetCreature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.SetState && e.GetInteger(0) == EffectState.Paralyse))) // Si la cible est paralysée, que l'attaque touche et est en mêlée, alors critique auto
        {
          attackData.m_nAttackResult = 3;
          criticalString = "CRITIQUE - ".ColorString(StringUtils.gold);
          
          LogUtils.LogMessage("Coup critique", LogUtils.LogType.Combat);
        }
        else if (attackRoll > 1 && totalAttack >= targetAC) // L'attaque touche
        {
          if (totalAttack + inspirationBardique + superiorityDiceBonus + frappeGuideeBonus < targetAC + defensiveDuellistBonus + shieldBonus) // Echoue alors qu'elle aurait du toucher
          {
            if (inspirationBardique < 0)
              attackBonus += NativeUtils.HandleInspirationBardiqueUsed(attacker, inspirationBardique, inspirationEffect, inspirationString, attackerName);

            if (superiorityDiceBonus > 0)
              attackBonus += NativeUtils.HandleAttaquePreciseUsed(attacker, superiorityDiceBonus);

            if (shieldBonus > 0)
            {
              targetAC += shieldBonus;
              EffectUtils.RemoveTaggedNativeEffect(targetObject, EffectSystem.BouclierEffectTag);
              LogUtils.LogMessage($"Bouclier Activé : +{shieldBonus} CA", LogUtils.LogType.Combat);
            }

            if (frappeGuideeBonus > 0)
            {
              attackBonus += frappeGuideeBonus;
              EffectUtils.RemoveTaggedNativeEffect(targetObject, EffectSystem.FrappeGuideeEffectTag);
              LogUtils.LogMessage($"Activation Frappe Guidée : +{frappeGuideeBonus} BA", LogUtils.LogType.Combat);
            }

            if (defensiveDuellistBonus > 0)
            {
              targetAC += defensiveDuellistBonus;
              NativeUtils.SendNativeServerMessage("Duelliste défensif !".ColorString(ColorConstants.Orange), targetCreature);
            }

            attackData.m_nAttackResult = 4;
            attackData.m_nMissedBy = (byte)(targetAC - attackRoll) > 8 ? (byte)Utils.random.Next(1, 9) : (byte)(targetAC - attackRoll);
            hitString = "manquez".ColorString(ColorConstants.Red);
            rollString = $"{attackRoll} + {attackBonus} = {attackRoll + attackBonus}".ColorString(new Color(32, 255, 32));
            rollString = rollString.StripColors().ColorString(ColorConstants.Red);

            LogUtils.LogMessage($"Manqué : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
          }
          else // Touche : cas normal
          {
            attackData.m_nAttackResult = 1;
            LogUtils.LogMessage($"Touché : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
          }
        }
        else if (attackRoll > 1) // L'attaque échoue
        {
          if (totalAttack + inspirationBardique + superiorityDiceBonus + frappeGuideeBonus >= targetAC + defensiveDuellistBonus + shieldBonus) // Touche alors qu'elle aurait du échouer
          {
            if (inspirationBardique > 0)
              attackBonus += NativeUtils.HandleInspirationBardiqueUsed(attacker, inspirationBardique, inspirationEffect, inspirationString, attackerName);

            if (superiorityDiceBonus > 0)
              attackBonus += NativeUtils.HandleAttaquePreciseUsed(attacker, superiorityDiceBonus);

            if (frappeGuideeBonus > 0)
            {
              attackBonus += frappeGuideeBonus;
              EffectUtils.RemoveTaggedNativeEffect(targetObject, EffectSystem.FrappeGuideeEffectTag);
              LogUtils.LogMessage($"Activation Frappe Guidée : +{frappeGuideeBonus} BA", LogUtils.LogType.Combat);
            }

            if (shieldBonus > 0)
            {
              targetAC += shieldBonus;
              EffectUtils.RemoveTaggedNativeEffect(targetObject, EffectSystem.BouclierEffectTag);
            }

            if (defensiveDuellistBonus > 0)
            {
              targetAC += defensiveDuellistBonus;
              NativeUtils.SendNativeServerMessage("Duelliste défensif !".ColorString(ColorConstants.Orange), targetCreature);
            }

            attackData.m_nAttackResult = 1;
            rollString = $"{attackRoll} + {attackBonus} = {attackRoll + attackBonus}".ColorString(new Color(32, 255, 32));
            LogUtils.LogMessage($"Touché : {attackRoll} + {attackBonus} = {attackRoll + attackBonus} vs {targetAC}", LogUtils.LogType.Combat);
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
          NativeUtils.HandleRafaleDuTraqueur(attacker, targetObject, combatRound, attackerName, targetName);
          NativeUtils.HandleRiposte(attacker, targetCreature, attackData, attackerName);

          if (attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreRiposteVariableExo).ToBool())
            attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreRiposteVariableExo);
        }
 
        NativeUtils.SendNativeServerMessage($"{advantageString}{opportunityString}{criticalString}Vous {hitString} {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), attacker);
        NativeUtils.BroadcastNativeServerMessage($"{advantageString}{opportunityString}{criticalString}{attackerName.ColorString(ColorConstants.Cyan)} {hitString.Replace("z", "")} {targetName.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan), attacker, true);

        NativeUtils.HandleSentinelleOpportunityTarget(attacker, combatRound, attackerName);
        NativeUtils.HandleSentinelle(attacker, targetCreature, combatRound);
        NativeUtils.HandleFureurOrc(attacker, targetCreature, combatRound, attackerName);
        NativeUtils.HandleDiversion(attacker, attackData, targetCreature);
        NativeUtils.HandleMonkOpportunist(attacker, targetCreature, attackData, combatRound, attackerName, targetName);
      }
      else
        attackData.m_nAttackResult = 7;

      NativeUtils.HandleHastMaster(attacker, targetObject, combatRound, attackerName);
      NativeUtils.HandleBalayage(attacker, targetObject, combatRound, attackerName);
      NativeUtils.HandleEntaille(attacker, targetObject, combatRound, attackData);
      NativeUtils.HandleFendre(attacker, targetObject, combatRound, attackData.m_nAttackResult);
      NativeUtils.HandleRiposteBonusAttack(attacker, combatRound, attackData, attackerName);
      NativeUtils.HandleBersekerRepresaillesBonusAttack(attacker, combatRound, attackData, attackerName);
      NativeUtils.HandleArcaneArcherTirIncurveBonusAttack(attacker, attackData, combatRound, attackerName, attackWeapon, targetObject);
      NativeUtils.HandleMonkBonusAttack(attacker, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleMonkDeluge(attacker, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleThiefReflex(attacker, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleBardeBotteTranchante(attacker, targetObject, combatRound, attackerName);
      NativeUtils.HandleBriseurDeHordes(attacker, targetObject, combatRound, attackerName, attackWeapon, attackData.m_bRangedAttack.ToBool());
      NativeUtils.HandleAttaqueCoordonnee(attacker, targetObject, combatRound);
      NativeUtils.HandleFurieBestiale(attacker, targetObject, combatRound, attackerName);
      NativeUtils.HandleVoeuHostile(attacker, combatRound, attackData, attackerName);
      NativeUtils.HandleFougueMartiale(attacker, targetObject, combatRound, attackerName, targetName);
      NativeUtils.HandleClercMartial(attacker, targetObject, combatRound, attackerName);
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
      Anvil.API.Ability damageAbility = NativeUtils.GetAttackAbility(attacker, attackData.m_bRangedAttack.ToBool(), attackWeapon);
      damageAbility = NativeUtils.HandleCoupAuBut(attacker, attackWeapon, damageAbility, EffectSystem.CoupAuButDamageEffectTag);
      damageAbility = NativeUtils.HandleShillelagh(attacker, attackWeapon, damageAbility);
      int baseDamage = 0;
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
            baseDamage += NativeUtils.RollWeaponDamage(attacker, baseWeapon, attackData, targetCreature, attackWeapon, damageAbility);
            baseDamage += NativeUtils.IsDuelFightingStyle(attacker, baseWeapon, attackData);
          }

          sneakAttack = NativeUtils.GetSneakAttackDamage(attacker, targetCreature, attackWeapon, attackData, combatRound);
          baseDamage += sneakAttack;
        }
        else
        {
          baseDamage += NativeUtils.GetUnarmedDamage(attacker, targetCreature, damageAbility, bCritical.ToBool());
        }
      }
      else
      {
        baseDamage += NativeUtils.GetUnarmedDamage(attacker, targetCreature, damageAbility, bCritical.ToBool());
      }

      if (bCritical > 0)
      {
        int critDamage = NativeUtils.GetCritDamage(attacker, attackWeapon, attackData, sneakAttack, targetCreature, damageAbility);
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
      int damageBonus = NativeUtils.GetAbilityModifier(attacker, damageAbility);

      // Pour l'attaque de la main secondaire, on n'ajoute le modificateur de caractéristique que s'il est négatif
      if (!bOffHand.ToBool() || attacker.m_pStats.HasFeat(CustomSkill.FighterCombatStyleDualWield).ToBool() || (bOffHand > 0 && damageBonus < 0))
      {
        baseDamage += damageBonus;
        LogUtils.LogMessage($"Adding {damageAbility} modifier (+{damageBonus})", LogUtils.LogType.Combat);
      }

      if (targetCreature is not null)
      {
        baseDamage /= NativeUtils.HandleEsquiveInstinctive(targetCreature);
        baseDamage = NativeUtils.HandleFendre(attacker, targetCreature, baseDamage);
      }

      if (attacker.m_ScriptVars.GetInt(CreatureUtils.TirAffaiblissantVariableExo).ToBool())
      {
        baseDamage /= 2;
        LogUtils.LogMessage($"Tir affaiblissant : Dégâts {baseDamage}", LogUtils.LogType.Combat);
      }
      
      LogUtils.LogMessage($"Dégâts : {baseDamage}", LogUtils.LogType.Combat);

      // Application des réductions du jeu de base

      if (attackWeapon is null || attackWeapon.m_ScriptVars.GetObject(CreatureUtils.PacteDeLaLameVariableExo) != attacker.m_idSelf)
      {
        LogUtils.LogMessage($"Application des immunités de la cible - Dégats : {baseDamage}", LogUtils.LogType.Combat);
        baseDamage = targetObject.DoDamageImmunity(attacker, baseDamage, damageFlags, 0, 1);
      }
      else
      {
        LogUtils.LogMessage($"Occultiste - Arme de Pacte - Application des immunités de la cible - Dégats : {baseDamage}", LogUtils.LogType.Combat);
        int damageAfterImmunity = targetObject.DoDamageImmunity(attacker, baseDamage, damageFlags, 0, 1);

        if (damageAfterImmunity < baseDamage)
          baseDamage -= (baseDamage - damageAfterImmunity) / 2;
        else
          baseDamage = damageAfterImmunity;
      }

      LogUtils.LogMessage($"Application des résistances de la cible - Dégâts : {baseDamage}", LogUtils.LogType.Combat);
      baseDamage = targetObject.DoDamageReduction(attacker, baseDamage, attacker.CalculateDamagePower(targetObject, bOffHand), 0, 1, attackData.m_bRangedAttack);   
      LogUtils.LogMessage($"Application des réductions de la cible - Calcul Final - Dégâts : {baseDamage}", LogUtils.LogType.Combat);

      NativeUtils.HandleCogneurLourdBonusAttack(attacker, targetObject, combatRound, attackData, baseDamage, attackerName);
      
      if (attacker.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) != CustomSkill.WarMasterAttaquePrecise)
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreTypeVariableExo);
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.ManoeuvreDiceVariableExo);
      }

      if (baseDamage < 1)
      {
        attacker.m_ScriptVars.SetInt(CreatureUtils.CancelDamageDoublonVariableExo, 1);

        if(targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.AbjurationWardEffectTag))
        {
          targetObject.m_ScriptVars.SetInt(CreatureUtils.AbjurationWardForcedTriggerVariableExo, 1);
          EffectUtils.RemoveTaggedNativeEffect(targetObject, EffectSystem.AbjurationWardEffectTag);
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
