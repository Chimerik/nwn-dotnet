using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeNewCharacter()
      {
        LogUtils.LogMessage($"{oid.PlayerName} vient de créer un nouveau personnage : {oid.LoginCreature.Name}", LogUtils.LogType.PlayerConnections);

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_NAME").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_GENDER").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").Value = 1;
        
        Utils.DestroyInventory(oid.LoginCreature);
        ClearKnownSpells();
        CreateCharacterSkin();     
        CreateCharacterDB(CreateIntroScene(oid, areaSystem));
        InitializeStartingLearnables();
      }
    }
  }
}
