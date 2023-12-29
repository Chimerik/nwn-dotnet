using Anvil.API;

namespace NWN.Systems
{
  public partial class DialogSystem
  {
    public static async void OnConversationIntroCaptain(Anvil.API.Events.CreatureEvents.OnConversation onConv)
    {
      NwCreature pc = (NwCreature)onConv.LastSpeaker;
      bool go = true;

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"race".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"origine".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez choisir une {"classe".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (pc.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").HasValue)
      {
        go = false;
        pc.LoginPlayer.SendServerMessage($"Avant de poursuivre votre voyage, veuillez finaliser vos {"stats".ColorString(ColorConstants.White)} auprès du miroir !", ColorConstants.Orange);
      }

      if (go)
        pc.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;

      //onConv.Creature.OnConversation -= OnConversationIntroCaptain;
      await pc.LoginPlayer.ActionStartConversation(pc, onConv.Creature.DialogResRef);
    }
  }
}
