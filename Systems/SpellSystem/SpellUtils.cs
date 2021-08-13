using Anvil.API;
using System.Linq;
using System;
using NWN.Core.NWNX;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static class SpellUtils
  {
    public static int MaximizeOrEmpower(int nDice, int nNumberOfDice, MetaMagic nMeta, int nBonus = 0)
    {
      int i = 0;
      int nDamage = 0;
      for (i = 1; i <= nNumberOfDice; i++)
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
        nDamage = nDamage + nDamage / 2;
      }
      return nDamage + nBonus;
    }
    public static void RemoveAnySpellEffects(Spell spell, NwCreature oTarget)
    {
      foreach (var eff in oTarget.ActiveEffects.Where(e => e.Spell == spell))
        oTarget.RemoveEffect(eff);
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
    public static Spell GetSpellIDFromScroll(NwItem oScroll)
    {
        ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell);

        if (ip != null)
            return ItemPropertySpells2da.spellsTable.GetSpellDataEntry(ip.SubType).spell;

      return (Spell)(0);
    }
    public static byte GetSpellLevelFromScroll(NwItem oScroll)
    {
        ItemProperty ip = oScroll.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell);

        if (ip != null)
            return ItemPropertySpells2da.spellsTable.GetSpellDataEntry(ip.SubType).innateLevel;

      return 255;
    }
    public static int GetSpellSchoolFromString(string school)
    {
      return "GACDEVINT".IndexOf(school);
    }
    public static void ApplyCustomEffectToTarget(NwGameObject target, string effectTag, string onApply, string onRemoved, int iconId = -1, string onInterval = "", float interval = 0, string effectData = "", int effectDuration = 0)
    {
      Effect eff = Effect.RunScript(onApply, onRemoved, onInterval, interval, effectData);
      eff.Tag = effectTag;
      eff.SubType = EffectSubType.Supernatural;

      if(iconId > -1)
        eff = Effect.LinkEffects(eff, Effect.Icon(iconId));

      if (effectDuration > 0)
        target.ApplyEffect(EffectDuration.Temporary, eff, TimeSpan.FromSeconds(effectDuration));
      else
        target.ApplyEffect(EffectDuration.Permanent, eff);
    }
    public static async void RestoreSpell(NwCreature caster, Spell spell)
    {
      if (caster == null)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      foreach (MemorizedSpellSlot spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(0).Where(s => s.Spell == spell && !s.IsReady))
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
  }
}
