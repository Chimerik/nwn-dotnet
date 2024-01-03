using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleChanceDebordante(CNWSCreature creature, int attackRoll)
    {
      if(attackRoll < 2 && !creature.m_ScriptVars.GetInt(EffectSystem.ChanceDebortanteCooldownExo).ToBool())
      {
        foreach(var eff in creature.m_appliedEffects)
          if(eff.m_sCustomTag.CompareNoCase(EffectSystem.ChanceDebortanteEffectExoTag).ToBool())
          {
            CNWSCreature effCreator = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(eff.m_oidCreator);

            if (effCreator is not null && !effCreator.m_ScriptVars.GetInt(EffectSystem.ChanceDebortanteCooldownExo).ToBool())
            {
              creature.m_ScriptVars.SetInt(EffectSystem.ChanceDebortanteCooldownExo, 1);
              effCreator.m_ScriptVars.SetInt(EffectSystem.ChanceDebortanteCooldownExo, 1);

              RemoveChanceDebordanteCooldown(creature);
              RemoveChanceDebordanteCooldown(effCreator);

              SendNativeServerMessage("Chance débordante".ColorString(StringUtils.gold), creature);
              SendNativeServerMessage("Chance débordante".ColorString(StringUtils.gold), effCreator);

              return NwRandom.Roll(Utils.random, 20);
            }
          }
      }

      return attackRoll;
    }
    private static async void RemoveChanceDebordanteCooldown(CNWSCreature creature)
    {
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      creature.m_ScriptVars.DestroyInt(EffectSystem.ChanceDebortanteCooldownExo);
    }
  }
}
