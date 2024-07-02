using System;
using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SpellSwitch(NwGameObject oCaster, NwSpell spell, NwFeat feat, SpellEntry spellEntry, NwGameObject target, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationTargets = new();

      switch (spell.SpellType)
      {
        case Spell.AcidSplash:
          SpellSystem.AcidSplash(oCaster, spell, spellEntry, target, targetLocation, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
        
        case Spell.ElectricJolt:
          SpellSystem.ElectricJolt(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Light:
          SpellSystem.Light(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RayOfFrost:
          SpellSystem.RayOfFrost(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.TrueStrike:
          concentrationTargets.AddRange(SpellSystem.TrueStrike(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          SpellSystem.SacredFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /* case Spell.Virtue:
           HealingBreeze(onSpellCast, durationModifier);
           oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
           break;*/

        /*case Spell.RaiseDead:
        case Spell.Resurrection:
          new RaiseDead(onSpellCast);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case Spell.Invisibility:
          concentrationTargets.AddRange(SpellSystem.Invisibility(oCaster, spell, spellEntry, target, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ImprovedInvisibility:
          SpellSystem.ImprovedInvisibility(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /*case Spell.FleshToStone:
          Petrify(onSpellCast);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case Spell.Darkness:
          concentrationTargets.AddRange(SpellSystem.Darkness(oCaster, spell, feat, spellEntry, target, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Silence:
          if(oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.MonkSilence)
          {
            caster.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(caster, 2);
          }
          break;

        case Spell.Darkvision:
          if (oCaster is NwCreature castCreature && feat is not null && feat.Id == CustomSkill.MonkDarkVision)
          {
            castCreature.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(castCreature, 2);
          }
          break;

        case Spell.BurningHands:
          SpellSystem.BurningHands(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.FlameStrike:
          SpellSystem.FlameStrike(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Stoneskin:
          concentrationTargets.AddRange(SpellSystem.PeauDePierre(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.IceStorm:
          SpellSystem.TempeteDeGrele(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Bane:
          concentrationTargets.AddRange(SpellSystem.Fleau(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.HoldPerson:
          concentrationTargets.AddRange(SpellSystem.ImmobilisationDePersonne(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.HoldMonster:
          concentrationTargets.AddRange(SpellSystem.ImmobilisationDeMonstre(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Haste:
          concentrationTargets.AddRange(SpellSystem.Hate(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.AbilityTurnUndead:
          SpellSystem.RenvoiDesMortsVivants(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CharmPerson:
          SpellSystem.CharmePersonne(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Fear:
          concentrationTargets.AddRange(SpellSystem.Terreur(oCaster, spell, spellEntry, target, castingClass, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      switch (spell.Id)
      {
        case CustomSpell.ProtectionContreLeMalEtLeBien:
          concentrationTargets.AddRange(SpellSystem.ProtectionContreLeMalEtLeBien(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BladeWard:
          SpellSystem.BladeWard(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FireBolt:
          SpellSystem.FireBolt(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Friends:
          concentrationTargets.AddRange(SpellSystem.Friends(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BoneChill:
          SpellSystem.BoneChill(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PoisonSpray:
          SpellSystem.PoisonSpray(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FaerieFire:
          concentrationTargets.AddRange(SpellSystem.FaerieFire(oCaster, spell, spellEntry, target, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Enlarge:
          concentrationTargets.AddRange(SpellSystem.Enlarge(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpeakAnimal:
          SpellSystem.SpeakAnimal(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProduceFlame:
          SpellSystem.ProduceFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MageHand:
          SpellSystem.MageHand(oCaster, spell, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Thaumaturgy:
          SpellSystem.Thaumaturgy(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Serenity:
          SpellSystem.Serenity(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Sprint:
          SpellSystem.Sprint(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Stealth:
          SpellSystem.Stealth(oCaster);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Disengage:
          SpellSystem.Disengage(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Dodge:
          SpellSystem.Dodge(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TirPerforant:
          SpellSystem.TirPerforant(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PresenceIntimidante:
          SpellSystem.PresenceIntimidante(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlameBlade:
          concentrationTargets.AddRange(SpellSystem.FlameBlade(oCaster, spell));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SearingSmite:
          SpellSystem.SearingSmite(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BrandingSmite:
          concentrationTargets.AddRange(SpellSystem.BrandingSmite(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SensAnimal:
          concentrationTargets.AddRange(SpellSystem.SensAnimal(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CommunionAvecLaNature:
          SpellSystem.CommunionAvecLaNature(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageParLesArbres:
          SpellSystem.PassageParLesArbres(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.HurlementGalvanisant:
          SpellSystem.HurlementGalvanisant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageSansTrace:
          concentrationTargets.AddRange(SpellSystem.PassageSansTrace(oCaster, spell, feat, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RegardHypnotique:
          concentrationTargets.AddRange(SpellSystem.RegardHypnotique(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.IllusionMineure:
          SpellSystem.IllusionMineure(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.InvocationPermutation:
          SpellSystem.InvocationPermutation(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Deguisement:
          SpellSystem.Deguisement(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Scrutation:
          SpellSystem.Scrutation(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CordeEnchantee:
          SpellSystem.CordeEnchantee(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ApparencesTrompeuses:
          SpellSystem.ApparencesTrompeuses(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MauvaisAugure:
          SpellSystem.MauvaisAugure(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpiderCocoon:
          SpellSystem.SpiderCocoon(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.WildMagicTeleportation:
          SpellSystem.WildMagicTeleportation(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DevotionArmeSacree:
          SpellSystem.ArmeSacree(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DevotionRenvoiDesImpies:
          SpellSystem.RenvoiDesImpies(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AnciensRenvoiDesInfideles:
          SpellSystem.RenvoiDesInfideles(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ConspuerLennemi:
          SpellSystem.ConspuerLennemi(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LueurDespoir:
          concentrationTargets.AddRange(SpellSystem.LueurDespoir(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AnciensCourrouxDeLaNature:
          SpellSystem.CourrouxDeLaNature(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FrappePiegeuse:
          concentrationTargets.AddRange(SpellSystem.FrappePiegeuse(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Communion:
          SpellSystem.Communion(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FouleeBrumeuse:
          SpellSystem.FouleeBrumeuse(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PorteDimensionnelle:
          SpellSystem.PorteDimensionnelle(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonDeLune:
          concentrationTargets.AddRange(SpellSystem.RayonDeLune(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProtectionContreLacide:
        case CustomSpell.ProtectionContreLeFroid:
        case CustomSpell.ProtectionContreLeFeu:
        case CustomSpell.ProtectionContreLelectricite:
        case CustomSpell.ProtectionContreLeTonnerre:
          concentrationTargets.AddRange(SpellSystem.ProtectionContreLenergie(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CroissanceVegetale:
          concentrationTargets.AddRange(SpellSystem.CroissanceVegetale(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MarqueDuChasseur:
          concentrationTargets.AddRange(SpellSystem.MarqueDuChasseur(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Bannissement:
          concentrationTargets.AddRange(SpellSystem.Bannissement(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BenedictionEscroc:
          concentrationTargets.AddRange(SpellSystem.BenedictionEscroc(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RepliqueInvoquee:
          concentrationTargets.AddRange(SpellSystem.RepliqueInvoquee(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImageMiroir:
          SpellSystem.ImageMiroir(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionAttaque:
          concentrationTargets.AddRange(SpellSystem.MaledictionAttaque(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionDegats:
          concentrationTargets.AddRange(SpellSystem.MaledictionDegats(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionEffroi:
          concentrationTargets.AddRange(SpellSystem.MaledictionEffroi(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionForce:
        case CustomSpell.MaledictionDexterite:
        case CustomSpell.MaledictionConstitution:
        case CustomSpell.MaledictionIntelligence:
        case CustomSpell.MaledictionSagesse:
          concentrationTargets.AddRange(SpellSystem.MaledictionCaracteristique(oCaster, spell, spellEntry, target, castingClass, spell.Id));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      if (oCaster is NwCreature castingCreature)
      {
        if (castingClass is not null)
        {
          SpellSystem.OnSpellCastAbjurationWard(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));
          SpellSystem.OnSpellCastDivinationExpert(castingCreature, spell, castingClass);
          SpellSystem.OnSpellCastInvocationPermutation(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));
          SpellSystem.OnSpellCastTransmutationStone(castingCreature, spell, spell.GetSpellLevelForClass(castingClass));

          if(castingClass.ClassType == ClassType.Paladin && Players.TryGetValue(castingCreature, out Player player))
          {
            byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow)
            ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);

            if(spell.GetSpellLevelForClass(castingClass) == chatimentLevel)
              player.oid.LoginCreature.DecrementRemainingFeatUses((Feat)CustomSkill.ChatimentDivin);
          }
        }

        if(spellEntry.requiresConcentration)
          EffectSystem.ApplyConcentrationEffect(castingCreature, spell.Id, concentrationTargets, spellEntry.duration);
      }

      oCaster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Delete();
      oCaster.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
    }
  }
}
