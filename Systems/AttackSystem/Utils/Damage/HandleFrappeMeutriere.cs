using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleFrappeMeurtriere(CNWSCreature attacker, CNWSCreature target, int damage)
    {
      if (target is null || !attacker.m_pStats.HasFeat(CustomSkill.AssassinAssassinate).ToBool()
        || attacker.m_pStats.GetClassLevel(8, 1) < 17 || !IsAssassinate(attacker))
        return 1;

      int advantage = GetTargetConstitutionDisadvantage(target) ? -1 : 0;
      int DC = 8 + attacker.m_pStats.m_nDexterityModifier + GetCreatureProficiencyBonus(attacker);

      int saveBonus = SpellUtils.GetSavingThrowProficiencyBonus(target, Anvil.API.Ability.Constitution);
      byte conMod = target.m_pStats.GetAbilityMod((byte)Native.API.Ability.Constitution);
      saveBonus += conMod < 250 ? conMod : conMod - 256;

      int saveRoll = Utils.RollAdvantage(advantage);
      saveRoll = HandleChanceDebordante(target, saveRoll);
      saveRoll = HandleHalflingLuck(target, saveRoll);

      if (saveRoll + saveBonus < DC)
      {
        LogUtils.LogMessage($"Frappe meurtrière : dégâts x2 ({damage * 2})", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage("Frappe meutrière".ColorString(StringUtils.gold), attacker);
        return 2;
      }

      return 1;
    }
    public static bool GetTargetConstitutionDisadvantage(CNWSCreature creature)
    {
      foreach (var eff in creature.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.saignementEffectExoTag).ToBool())
          return true;

      return false;
    }
  }
}
