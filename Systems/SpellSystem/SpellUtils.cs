using Anvil.API;
using System.Linq;
using System;
using Anvil.API.Events;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static class SpellUtils
  {
    public static int MaximizeOrEmpower(int nDice, int nNumberOfDice, MetaMagic nMeta, int nBonus = 0)
    {
      int nDamage = 0;
      for (int i = 1; i <= nNumberOfDice; i++)
      {
        nDamage = nDamage + Utils.random.Next(nDice) + 1;
      }
      //Resolve metamagic
      if (nMeta == MetaMagic.Maximize)
      {
        nDamage = nDice * nNumberOfDice;
      }
      else if (nMeta == MetaMagic.Empower)
      {
        nDamage += nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static void RemoveAnySpellEffects(Spell spell, NwCreature oTarget)
    {
      foreach (var eff in oTarget.ActiveEffects.Where(e => e.Spell.SpellType == spell))
        oTarget.RemoveEffect(eff);
    }
    public static ClassType GetCastingClass(NwSpell Spell)
    {
      byte clericCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Cleric));
      byte druidCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Druid));
      byte paladinCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Paladin));
      byte rangerCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Ranger));
      byte bardCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Bard));

      Dictionary<ClassType, byte> classSorter = new Dictionary<ClassType, byte>()
      {
        { ClassType.Cleric, clericCastLevel },
        { ClassType.Druid, druidCastLevel },
        { ClassType.Paladin, paladinCastLevel },
        { ClassType.Ranger, rangerCastLevel },
        { ClassType.Bard, bardCastLevel },
      };

      try
      {
        ClassType castingClass = classSorter.Where(c => c.Value < 255).Max().Key;
        return castingClass;
      }
      catch (Exception)
      {
        return (ClassType)43;
      }
    }
    public static void SignalEventSpellCast(NwGameObject target, NwCreature caster, Spell spell, bool harmful = true)
    {
      if (target is NwCreature oTargetCreature)
        CreatureEvents.OnSpellCastAt.Signal(caster, oTargetCreature, spell, harmful);
      else if (target is NwPlaceable oTargetPlc)
        PlaceableEvents.OnSpellCastAt.Signal(caster, oTargetPlc, spell, harmful);
      else if (target is NwDoor oTargetDoor)
        DoorEvents.OnSpellCastAt.Signal(caster, oTargetDoor, spell, harmful);
    }
    public static int GetSpellIDFromScroll(NwItem oScroll)
    {
        ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell);

        if (ip != null)
            return (int)ItemPropertySpells2da.ipSpellTable[ip.SubType].spell;

      return 0;
    }
    public static byte GetSpellLevelFromScroll(NwItem oScroll)
    {
        ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell);

        if (ip != null)
            return ItemPropertySpells2da.ipSpellTable[ip.SubType].innateLevel;

      return 255;
    }
    public static int GetSpellSchoolFromString(string school)
    {
      return "GACDEVINT".IndexOf(school);
    }
    
    public static async void RestoreSpell(NwCreature caster, Spell spell)
    {
      if (caster == null)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      foreach (MemorizedSpellSlot spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(0).Where(s => s.Spell.SpellType == spell && !s.IsReady))
        spellSlot.IsReady = true;
    }
    public static async void CancelCastOnMovement(NwCreature caster)
    {
      float posX = caster.Position.X;
      float posY = caster.Position.Y;
      await NwTask.WaitUntil(() => caster.Position.X != posX || caster.Position.Y != posY);

      caster.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
      caster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
      caster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
    }

    public static bool IsSpellBuff(NwSpell spell)
    {
      switch(spell.SpellType)
      {
        case Spell.Aid:
        case Spell.Amplify:
        case Spell.Auraofglory:
        case Spell.AuraOfVitality:
        case Spell.Awaken:
        case Spell.Barkskin:
        case Spell.Battletide:
        case Spell.BladeThirst:
        case Spell.Bless:
        case Spell.BlessWeapon:
        case Spell.BloodFrenzy:
        case Spell.BullsStrength:
        case Spell.Camoflage:
        case Spell.CatsGrace:
        case Spell.Charger:
        case Spell.ClairaudienceAndClairvoyance:
        case Spell.Clarity:
        case Spell.CloakOfChaos:
        case Spell.ContinualFlame:
        case Spell.Darkfire:
        case Spell.Darkvision:
        case Spell.DeathArmor:
        case Spell.DeathWard:
        case Spell.Displacement:
        case Spell.DivineFavor:
        case Spell.DivineMight:
        case Spell.DivinePower:
        case Spell.DivineShield:
        case Spell.EagleSpledor:
        case Spell.ElementalShield:
        case Spell.Endurance:
        case Spell.EndureElements:
        case Spell.EnergyBuffer:
        case Spell.EntropicShield:
        case Spell.EpicMageArmor:
        case Spell.Etherealness:
        case Spell.EtherealVisage:
        case Spell.ExpeditiousRetreat:
        case Spell.FlameWeapon:
        case Spell.FoxsCunning:
        case Spell.FreedomOfMovement:
        case Spell.GhostlyVisage:
        case Spell.GlobeOfInvulnerability:
        case Spell.GlyphOfWarding:
        case Spell.GreaterBullsStrength:
        case Spell.GreaterCatsGrace:
        case Spell.GreaterEagleSplendor:
        case Spell.GreaterEndurance:
        case Spell.GreaterFoxsCunning:
        case Spell.GreaterMagicFang:
        case Spell.GreaterMagicWeapon:
        case Spell.GreaterOwlsWisdom:
        case Spell.GreaterShadowConjurationMinorGlobe:
        case Spell.GreaterShadowConjurationMirrorImage:
        case Spell.GreaterSpellMantle:
        case Spell.GreaterStoneskin:
        case Spell.Stoneskin:
        case Spell.Haste:
        case Spell.HolyAura:
        case Spell.HolySword:
        case Spell.ImprovedInvisibility:
        case Spell.Invisibility:
        case Spell.InvisibilitySphere:
        case Spell.Ironguts:
        case Spell.KeenEdge:
        case Spell.LesserMindBlank:
        case Spell.LesserSpellMantle:
        case Spell.Light:
        case Spell.MageArmor:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstEvil:
        case Spell.MagicCircleAgainstGood:
        case Spell.MagicCircleAgainstLaw:
        case Spell.MagicFang:
        case Spell.MagicVestment:
        case Spell.MagicWeapon:
        case Spell.MassCamoflage:
        case Spell.MassHaste:
        case Spell.MestilsAcidSheath:
        case Spell.MindBlank:
        case Spell.MinorGlobeOfInvulnerability:
        case Spell.MonstrousRegeneration:
        case Spell.NegativeEnergyProtection:
        case Spell.OneWithTheLand:
        case Spell.OwlsInsight:
        case Spell.OwlsWisdom:
        case Spell.PolymorphSelf:
        case Spell.Prayer:
        case Spell.Premonition:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromElements:
        case Spell.ProtectionFromEvil:
        case Spell.ProtectionFromGood:
        case Spell.ProtectionFromLaw:
        case Spell.ProtectionFromSpells:
        case Spell.Regenerate:
        case Spell.Resistance:
        case Spell.ResistElements:
        case Spell.Sanctuary:
        case Spell.SeeInvisibility:
        case Spell.InvisibilityPurge:
        case Spell.ShadesStoneskin:
        case Spell.ShadowConjurationInivsibility:
        case Spell.ShadowConjurationMageArmor:
        case Spell.ShadowShield:
        case Spell.Shapechange:
        case Spell.Shield:
        case Spell.ShieldOfFaith:
        case Spell.ShieldOfLaw:
        case Spell.Silence:
        case Spell.SpellMantle:
        case Spell.SpellResistance:
        case Spell.StoneBones:
        case Spell.StoneToFlesh:
        case Spell.TensersTransformation:
        case Spell.TimeStop:
        case Spell.TrueSeeing:
        case Spell.TrueStrike:
        case Spell.TymorasSmile:
        case Spell.Virtue:
        case Spell.WarCry:
        case Spell.WoundingWhispers:
        case Spell.UnholyAura:
        case Spell.AbilityAuraBlinding:
        case Spell.AbilityAuraCold:
        case Spell.AbilityAuraElectricity:
        case Spell.AbilityAuraFear:
        case Spell.AbilityAuraFire:
        case Spell.AbilityAuraHorrificappearance:
        case Spell.AbilityAuraMenace:
        case Spell.AbilityAuraOfCourage:
        case Spell.AbilityAuraProtection:
        case Spell.AbilityAuraStun:
        case Spell.AbilityAuraUnearthlyVisage:
        case Spell.AbilityAuraUnnatural:
        case Spell.AbilityBarbarianRage:
        case Spell.AbilityFerocity1:
        case Spell.AbilityFerocity2:
        case Spell.AbilityFerocity3:
        case Spell.AbilityIntensity1:
        case Spell.AbilityIntensity2:
        case Spell.AbilityIntensity3:
        case Spell.AbilityHowlConfuse:
        case Spell.AbilityHowlDaze:
        case Spell.AbilityHowlDeath:
        case Spell.AbilityHowlDoom:
        case Spell.AbilityHowlFear:
        case Spell.AbilityHowlParalysis:
        case Spell.AbilityHowlSonic:
        case Spell.AbilityHowlStun:
        case Spell.AbilityDragonWingBuffet:
        case Spell.AbilityMummyBolsterUndead:
        case Spell.AbilityEpicMightyRage:
        case Spell.AbilityRage3:
        case Spell.AbilityRage4:
        case Spell.AbilityRage5:
        case Spell.AbilityTyrantFogMist:
        case Spell.Darkness:
        case Spell.ShadowConjurationDarkness:
        case Spell.LegendLore:
        case Spell.AbilityBattleMastery:
        case Spell.AbilityDivineStrength:
        case Spell.AbilityDivineProtection:
        case Spell.AbilityDivineTrickery:
        case Spell.AbilityRoguesCunning:
        case Spell.AbilityDragonFear:
        case Spell.Dirge:
        case Spell.ShadowEvade:
        case Spell.VineMineCamouflage:
        case Spell.DeafeningClang:
        case Spell.Blackstaff:
        case Spell.IounStoneBlue:
        case Spell.IounStoneDeepRed:
        case Spell.IounStoneDustyRose:
        case Spell.IounStonePaleBlue:
        case Spell.IounStonePink:
        case Spell.IounStonePinkGreen:
        case Spell.IounStoneScarletBlue:
        case Spell.AbilityTroglodyteStench:
          return true;
        default: return false;
      }
    }
    public static bool IgnoredSpellList(Spell spell)
    {
      switch (spell)
      {
        case Spell.AllSpells:
        case Spell.CloakOfChaos:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstLaw:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromLaw:
        case Spell.ShieldOfLaw:
        case Spell.SphereOfChaos:
        case Spell.AbilityDragonWingBuffet:
        case Spell.AbilitySmokeClaw:
        case Spell.AbilityTrumpetBlast:
        case Spell.AbilityQuiveringPalm:
        case Spell.AbilityEmptyBody:
        case Spell.AbilityDetectEvil:
        case Spell.AbilityAuraOfCourage:
        case Spell.AbilitySmiteEvil:
        case Spell.AbilityElementalShape:
        case Spell.AbilityWildShape:
        case Spell.ActivateItemPortal:
        case Spell.ActivateItemSelf2:
        case Spell.AbilityActivateItem:
        case Spell.CraftDyeMetalcolor1:
        case Spell.CraftDyeMetalcolor2:
        case Spell.CraftHarperItem:
        case Spell.CraftPoisonWeaponOrAmmo:
        case Spell.CraftAddItemProperty:
        case Spell.CraftCraftArmorSkill:
        case Spell.CraftCraftWeaponSkill:
        case Spell.CraftDyeClothcolor1:
        case Spell.CraftDyeClothcolor2:
        case Spell.CraftDyeLeathercolor1:
        case Spell.CraftDyeLeathercolor2:
        case Spell.GrenadeAcid:
        case Spell.GrenadeCaltrops:
        case Spell.GrenadeChicken:
        case Spell.GrenadeChoking:
        case Spell.GrenadeFire:
        case Spell.GrenadeHoly:
        case Spell.GrenadeTangle:
        case Spell.GrenadeThunderstone:
        case Spell.DivineMight:
        case Spell.DivineShield:
        case Spell.ShadowDaze:
        case Spell.ShadowEvade:
        case Spell.TymorasSmile:
        case Spell.TrapArrow:
        case Spell.TrapBolt:
        case Spell.TrapDart:
        case Spell.TrapShuriken:
        case Spell.RodOfWonder:
        case Spell.DeckOfManyThings:
        case Spell.ElementalSummoningItem:
        case Spell.DeckAvatar:
        case Spell.DeckButterflyspray:
        case Spell.DeckGemspray:
        case Spell.Powerstone:
        case Spell.KoboldJump:
        case Spell.Healingkit:
        case Spell.Spellstaff:
        case Spell.Decharger:
        case Spell.AbilityWhirlwind:
        case Spell.AbilityCommandTheHorde:
        case Spell.AbilityAaArrowOfDeath:
        case Spell.AbilityAaHailOfArrows:
        case Spell.AbilityAaImbueArrow:
        case Spell.AbilityAaSeekerArrow1:
        case Spell.AbilityAaSeekerArrow2:
        case Spell.AbilityAsDarkness:
        case Spell.AbilityAsGhostlyVisage:
        case Spell.AbilityAsImprovedInvisiblity:
        case Spell.AbilityAsInvisibility:
        case Spell.HorseAssignMount:
        case Spell.AbilityBgCreatedead:
        case Spell.AbilityBgFiendishServant:
        case Spell.AbilityBgInflictSeriousWounds:
        case Spell.AbilityBgInflictCriticalWounds:
        case Spell.AbilityBgContagion:
        case Spell.AbilityBgBullsStrength:
        case Spell.FlyingDebris:
        case Spell.AbilityDcDivineWrath:
        case Spell.AbilityPmAnimateDead:
        case Spell.AbilityPmSummonUndead:
        case Spell.AbilityPmUndeadGraft1:
        case Spell.AbilityPmUndeadGraft2:
        case Spell.AbilityPmSummonGreaterUndead:
        case Spell.AbilityPmDeathlessMasterTouch:
        case Spell.EpicHellball:
        case Spell.EpicMummyDust:
        case Spell.EpicDragonKnight:
        case Spell.EpicMageArmor:
        case Spell.EpicRuin:
        case Spell.AbilityDwDefensiveStance:
        case Spell.AbilityEpicMightyRage:
        case Spell.AbilityEpicCurseSong:
        case Spell.AbilityEpicImprovedWhirlwind:
        case Spell.AbilityEpicShapeDragonkin:
        case Spell.AbilityEpicShapeDragon:
        case Spell.HorseMenu:
        case Spell.HorseDismount:
        case Spell.HorseMount:
        case Spell.HorsePartyDismount:
        case Spell.HorsePartyMount:
        case Spell.PaladinSummonMount:
          return true;
        default: return false;
      }
    }
  }
}
