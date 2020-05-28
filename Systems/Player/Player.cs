using NWN.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWN.Systems
{
    public partial class Player : NWPlayer
    {
        #region Properties

        public readonly int PCID;
        public readonly bool IsDeveloper;
        public readonly DateTime ConnectionTime = DateTime.Now;
        public int HeartbeatTicks;

        public int Kills;
        public int KillsAnimal;
        public int KillsPvP;
        public int Deaths;
        public int NearDeaths;
        public int Piety;
        public int Miracles;
        public int ResidenceID;
        public int Playtime;
        public int Privileges;

        //public Faction Faction;
        public string DebugName => Name + "(" + PlayerName + "/" + CDKey + ")";

        //public RingBuffer<string> ChatMessages = new RingBuffer<string>(10);
        public bool CanSpeak = true;
        public bool IsAFK { get => _isAFK; set { _isAFK = value; UpdateAFK(); } }

        private Boolean _IsSelecting;
        public virtual Boolean IsSelecting
        {
            get => _IsSelecting;
            set => _IsSelecting = value;
        }

        private List<uint> _SelectedObjectsList = new List<uint>();
        public virtual List<uint> SelectedObjectsList
        {
            get => _SelectedObjectsList;
           // set => _SelectedObjectsList.Add(value);
        }

        #endregion

        #region Initialization

        public Player(uint nwobj) : base(nwobj)
        {
            /*           IsDeveloper = IsDM; // fixme
                       Debug.Trace($"Player constructor: {Name}");

                       if (!int.TryParse(Tag, out PCID))
                       {
                              var q = MySql.PrepareQuery(
                                   "INSERT INTO characters(CDKey, Name, BicFile) VALUES(@cdkey, @name, @bicfile);");
                           q.Parameters.AddWithValue("@cdkey", CDKey);
                           q.Parameters.AddWithValue("@name", Name);
                           q.Parameters.AddWithValue("@bicfile", BicFile);
                           q.ExecuteNonQuery(); // NOTASYNC - need to get pcid back.

                           PCID = (int)SQL.PrepareQuery("SELECT MAX(PCID) FROM characters;").ExecuteScalar();

                           Debug.Notice($"New PC registered: PCID:{PCID}, Name:{Name}, CDKey:{CDKey}, IP:{IPAddress}, BIC:{Name}");

                           q = SQL.PrepareQueryAsync("INSERT INTO charstats(PCID) VALUES(@pcid);");
                           q.Parameters.AddWithValue("@pcid", PCID);
                           q.ExecuteNonQueryAsync();

                           Tag = PCID.ToString();
                           NWScript.ExportSingleCharacter(self);
                       }
                       else
                       {
                           var q = SQL.PrepareQuery("SELECT * FROM characters WHERE PCID=@pcid;");
                           q.Parameters.AddWithValue("@pcid", PCID);
                           using (var reader = q.ExecuteReader())
                           {
                               if (reader.Read())
                               {
                                   string areatag = reader.GetString("AreaTag");
                                   float x = reader.GetFloat("PosX");
                                   float y = reader.GetFloat("PosY");
                                   float f = reader.GetFloat("Facing");
                                   var loc = new NWLocation(NWN.Core.Area.Areas[areatag].self, new Vector(x, y, 0.0f), f);
                                   AssignCommand(() => { NWScript.JumpToLocation(loc); });

                                   // TODO: move to faction.cs
                                   var fctn = Faction.GetById(reader.GetInt32("Faction"));
                                   if (fctn != null)
                                       fctn.AddPC(this);

                                   Kills = reader.GetInt32("Kills");
                                   KillsAnimal = reader.GetInt32("KillsAnimal");
                                   KillsPvP = reader.GetInt32("KillsPvP");
                                   Deaths = reader.GetInt32("Deaths");
                                   NearDeaths = reader.GetInt32("NearDeaths");
                                   Piety = reader.GetInt32("Piety");
                                   Miracles = reader.GetInt32("Miracles");
                                   ResidenceID = reader.GetInt32("ResidenceID");
                                   Playtime = reader.GetInt32("Playtime");
                                   Privileges = reader.GetInt32("Privileges");

                                   Debug.Assert(BicFile == reader.GetString("BicFile"));
                                   Debug.Assert(Name == reader.GetString("Name"));
                                   Debug.Assert(CDKey == reader.GetString("CDKey"));

                                   Debug.Info($"Loaded PC with PCID:{PCID} from database");
                               }
                               else
                               {
                                   Debug.Error($"No PC with PCID:{PCID} found in database ({DebugName})");
                               }
                           }
                       }
           */

            Players[nwobj.AsPlayer().uuid] = this;
 //           Debug.Info($"Player join: PCID:{PCID}, Name:{Name}, CDKey:{CDKey}, IP:{IPAddress}, BIC:{BicFile}");
        }

        #endregion

        #region Events
        public class Events
        {
            // Todo: damaged
            public delegate void PlayerJoinDelegate(Player player);
            public delegate void PlayerLeaveDelegate(Player player);
            public delegate void PlayerHeartbeatDelegate(Player player, int tick);
            public delegate void PlayerDyingDelegate(Player player);
            public delegate void PlayerDeathDelegate(Player player, NWObject killer);
            public delegate void PlayerCutsceneAbortDelegate(Player player);
            public delegate void PlayerLevelUpDelegate(Player player);
            public delegate void PlayerRespawnDelegate(Player player);
            public delegate void PlayerRestDelegate(Player player, int restEventId);
            public delegate void PlayerChatDelegate(Player player, ChatChannel channel, string text, Player target);
            public delegate void PlayerCombatRoundStartDelegate(Player player);
            public delegate void PlayerCombatRoundEndDelegate(Player player);
            public delegate void PlayerSpellCastAtDelegate(Player player, NWObject caster, int spellId, int metamagic, bool harmful);
            public delegate void PlayerPerceptionDelegate(Player player, NWCreature target, int perceptionType);
            public delegate void PlayerStealthDelegate(Player player);
            public delegate void PlayerExamineDelegate(Player player, NWObject examinee, bool before);
            public delegate void PlayerJoinPartyDelegate(Player player, Player leader);
            public delegate void PlayerUserDefinedDelegate(Player player, int eventId);
            //public delegate void PlayerDamagedDelegate(Player player, NWObject damager, NWNX.Damage.DamageEventData data);
            //public delegate void PlayerAttackDelegate(Player player, NWObject target, NWNX.Damage.AttackEventData data);

            public PlayerJoinDelegate OnJoin = delegate { };
            public PlayerLeaveDelegate OnLeave = delegate { };
            public PlayerHeartbeatDelegate OnHeartbeat = delegate { };
            public PlayerDyingDelegate OnDying = delegate { };
            public PlayerDeathDelegate OnDeath = delegate { };
            public PlayerCutsceneAbortDelegate OnCutsceneAbort = delegate { };
            public PlayerLevelUpDelegate OnLevelUp = delegate { };
            public PlayerRespawnDelegate OnRespawn = delegate { };
            public PlayerRestDelegate OnRest = delegate { };
            public PlayerChatDelegate OnChat = delegate { };
            public PlayerCombatRoundStartDelegate OnCombatRoundStart = delegate { };
            public PlayerCombatRoundEndDelegate OnCombatRoundEnd = delegate { };
            public PlayerSpellCastAtDelegate OnSpellCastAt = delegate { };
            public PlayerPerceptionDelegate OnPerception = delegate { };
            public PlayerStealthDelegate OnStealth = delegate { };
            public PlayerExamineDelegate OnExamine = delegate { };
            public PlayerJoinPartyDelegate OnJoinParty = delegate { };
            public PlayerUserDefinedDelegate OnUserDefined = delegate { };
            //public PlayerDamagedDelegate OnDamaged = delegate { };
            //public PlayerAttackDelegate OnAttack = delegate { };

            public static Events All = new Events();
            public static Events DMs = new Events();
            public static Events PCs = new Events();
            public static Events Possessed = new Events();
        }

        public Events CustomEvents = new Events();
        public static IEnumerable<Events> AllEventGroups(Player pc)
        {
            yield return pc.CustomEvents;
            yield return Events.All;
            yield return pc.IsDM ? Events.DMs : Events.PCs;
            if (pc.IsPossessedFamiliar)
                yield return Events.Possessed;
        }

        #endregion

        #region Static

        public static Dictionary<string, Player> Players = new Dictionary<string, Player>();

        static Player()
        {
            /*          Player.Events.All.OnJoin += (pc) => { pc.LogEntry(); };
                      Player.Events.All.OnLeave += (pc) => { pc.LogExit(); };
                      //Player.Events.All.OnChat += (pc, ch, msg, t) => { pc.LogChat(ch, msg, t); };

                      //NWN.Core.Area.Events.All.OnPCEnter += (area, pc) => { pc.LogAreaEnter(area); };
                      //NWN.Core.Area.Events.All.OnPCExit += (area, pc) => { pc.LogAreaExit(area); };

                      Server.Events.OnLoad += () =>
                      {
                          Debug.Trace($"Player static OnModuleLoad started. Registering events..", +2);

                          Server.Module.Scripts[EventScript.Module_OnClientEnter] = nameof(NWN.Scripts.pc_join);
                          Debug.Trace($"OnClientEnter = {nameof(NWN.Scripts.pc_join)}");

                          // Use NWNX event as the object is still entirely valid then.
                          Server.Module.Scripts[EventScript.Module_OnClientExit] = "";
                          NWNX.Events.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", nameof(NWN.Scripts.pc_leave));
                          Debug.Trace($"OnClientExit (NWNX) = {nameof(NWN.Scripts.pc_leave)}");

                          // Use NWNX event as it has better capabilities
                          Server.Module.Scripts[EventScript.Module_OnPlayerChat] = "";
                          NWNX.Chat.RegisterChatScript(nameof(NWN.Scripts.pc_chat));
                          Debug.Trace($"OnPlayerChat (NWNX) = {nameof(NWN.Scripts.pc_chat)}");

                          NWNX.Events.SubscribeEvent("NWNX_ON_ENTER_STEALTH_AFTER", nameof(NWN.Scripts.pc_stealth));
                          Debug.Trace($"OnPlayerStealth (NWNX) = {nameof(NWN.Scripts.pc_stealth)}");


                          Debug.Trace($"Player static OnModuleLoad done.", -2);
                      };
                      Player.Events.All.OnJoin += (pc) =>
                      {
                          pc.Scripts[EventScript.Creature_OnHeartbeat] = nameof(NWN.Scripts.pc_heartbeat);
                      };
          */
        }

        /*       public static Player Get(NWObjectBase nw)
               {
                   if (NWScript.GetIsPC(nw.self) == 0)
                       return null;

                   int pcid;
                   return int.TryParse(nw.Tag, out pcid) ? Players[pcid] : null;
               }
          */
        public static void SendMessageToAllPCs(string msg)
        {
            foreach (var pc in Player.Players)
            {
                pc.Value.SendMessage(msg);
            }
        }
        public static Player FindByCDKey(string cdkey)
        {
            foreach (var pc in Player.Players.Values)
            {
                if (pc.CDKey == cdkey)
                    return pc;
            }
            return null;
        }
        #endregion

        #region Utility

 /*       public void PopUp(string title, string message, string icon)
        {
            NWPlaceable plc = NWScript.GetObjectByTag("PLC_POPUP");
            plc.Name = title;
            plc.Description = message;
            plc.Portrait = icon;
            NWNX.Player.ForcePlaceableExamineWindow(this, plc);
        }
*/
        #endregion

        #region Backend property implementation

        private bool _isAFK = false;
        private void UpdateAFK()
        {
            // TODO: AFK VFX
        }

        #endregion

        #region Logging

   /*     private void LogChat(NWNX.Chat.Channel channel, string message, Player target)
        {
            var q = SQL.PrepareQueryAsync(
                "INSERT INTO chat(SenderPCID,SenderName,SenderAreaTag,SenderPosX,SenderPosY,ChannelID,MessageText) " +
                "VALUES(@senderpcid,@sendername,@senderareatag,@senderposx,@senderposy,@channelid,@messagetext);");

            q.Parameters.AddWithValue("@senderpcid", PCID);
            q.Parameters.AddWithValue("@sendername", Name);
            q.Parameters.AddWithValue("@senderareatag", Area.Tag);
            q.Parameters.AddWithValue("@senderposx", Position.x);
            q.Parameters.AddWithValue("@senderposy", Position.y);
            q.Parameters.AddWithValue("@channelid", channel.ToString());
            q.Parameters.AddWithValue("@messagetext", message);
            q.ExecuteNonQueryAsync();
        }

        private void LogEntry()
        {
            var q = SQL.PrepareQueryAsync(
                    "INSERT INTO playerconnections (RestartID,PCID,IP,PlayerName,EnterAreaTag,EnterPosX,EnterPosY,GoldEnter)" +
                    " VALUES(@restartid,@pcid,@ip,@playername,@areatag,@posx,@posy,@gold);");

            q.Parameters.AddWithValue("@restartid", Server.RestartID);
            q.Parameters.AddWithValue("@pcid", PCID);
            q.Parameters.AddWithValue("@ip", IPAddress);
            q.Parameters.AddWithValue("@playername", PlayerName);
            q.Parameters.AddWithValue("@areatag", Area.Tag);
            q.Parameters.AddWithValue("@posx", Position.x);
            q.Parameters.AddWithValue("@posy", Position.y);
            q.Parameters.AddWithValue("@gold", Gold);
            q.ExecuteNonQueryAsync();

            q = SQL.PrepareQueryAsync(
                "UPDATE characters SET IsOnline=1, LastIP=@ipaddress WHERE PCID=@pcid;");
            q.Parameters.AddWithValue("@pcid", PCID);
            q.Parameters.AddWithValue("@ipaddress", IPAddress);
            q.ExecuteNonQueryAsync();
        }

        private void LogExit()
        {
            int playtime = (int)(DateTime.UtcNow.Subtract(ConnectionTime).TotalSeconds);

            var q = SQL.PrepareQueryAsync(
                "UPDATE playerconnections SET " +
                "ExitAreaTag=@areatag, ExitPosX=@posx, ExitPosY=@posy, GoldExit=@gold, Playtime=@playtime " +
                "WHERE ConnectionID = (SELECT MAX(ConnectionID) WHERE PCID=@pcid);");
            q.Parameters.AddWithValue("@areatag", Area.Tag);
            q.Parameters.AddWithValue("@posx", Position.x);
            q.Parameters.AddWithValue("@posy", Position.y);
            q.Parameters.AddWithValue("@gold", Gold);
            q.Parameters.AddWithValue("@playtime", playtime);
            q.ExecuteNonQueryAsync();

            q = SQL.PrepareQueryAsync(
                "UPDATE characters SET IsOnline=0, AreaTag=@areatag, PosX=@posx, PosY=@posy, Facing=@facing, Playtime=Playtime+@playtime WHERE PCID=@pcid;");
            q.Parameters.AddWithValue("@areatag", Area.Tag);
            q.Parameters.AddWithValue("@posx", Position.x);
            q.Parameters.AddWithValue("@posy", Position.y);
            q.Parameters.AddWithValue("@facing", Facing);
            q.Parameters.AddWithValue("@playtime", playtime);
            q.Parameters.AddWithValue("@pcid", PCID);
            q.ExecuteNonQueryAsync();
        }

        private void LogAreaEnter(Area area)
        {
            var q = SQL.PrepareQueryAsync(
                "INSERT INTO areatransitions(RestartID,PCID,EnterAreaTag,EnterPosX,EnterPosY) " +
                "VALUES (@restartid,@pcid,@enterareatag,@enterposx,@enterposy);");
            q.Parameters.AddWithValue("@restartid", Server.RestartID);
            q.Parameters.AddWithValue("@pcid", PCID);
            q.Parameters.AddWithValue("@enterareatag", area.Tag);
            q.Parameters.AddWithValue("@enterposx", Position.x);
            q.Parameters.AddWithValue("@enterposy", Position.y);
            q.ExecuteNonQueryAsync();
        }

        private void LogAreaExit(Area area)
        {
            var q = SQL.PrepareQueryAsync(
                "UPDATE areatransitions SET ExitAreaTag=@exitareatag, ExitPosX=@exitposx, ExitPosY=@exitposy " +
                "WHERE TransitionID=(SELECT MAX(TransitionID) WHERE PCID=@pcid);");
            q.Parameters.AddWithValue("@pcid", PCID);
            q.Parameters.AddWithValue("@exitareatag", area.Tag);
            q.Parameters.AddWithValue("@exitposx", Position.x);
            q.Parameters.AddWithValue("@exitposy", Position.y);
            q.ExecuteNonQueryAsync();
        }

        public void LogKill(NWCreature killed)
        {
            int killedId = (killed.IsPC ? Player.Get(killed.self).PCID : -1);

            var q = SQL.PrepareQueryAsync(
                "INSERT INTO kills(KillerID,KilledID,KilledName,KillAreaTag,KillPosX,KillPosY) " +
                "VALUES(@killerid,@killedid,@killedname,@killareatag,@killposx,@killposy);");

            q.Parameters.AddWithValue("@killerid", PCID);
            q.Parameters.AddWithValue("@killedid", killedId);
            q.Parameters.AddWithValue("@killedname", killed.Name);
            q.Parameters.AddWithValue("@killareatag", Area.Tag);
            q.Parameters.AddWithValue("@killposx", Position.x);
            q.Parameters.AddWithValue("@killposy", Position.y);
            q.ExecuteNonQueryAsync();
        }
        #endregion


        #region Chat commands

        [Chat.ChatCommand("pcinfo", "display basic PC info")]
        public static void CmdPlayerInfo(Player player, string param)
        {
            player.SendMessage($"Name: {player.Name}\nPlayerName: {player.PlayerName}\nCDKey: {player.CDKey}\nPCID: {player.PCID}\n");
            player.SendMessage($"ConnectionTime: {player.ConnectionTime}\nPlaytime: {player.Playtime}\n");
        }

        [Chat.ChatCommand("afk", "toggles AFK state")]
        public static void CmdPlayerAFK(Player player, string param)
        {
            player.IsAFK = !player.IsAFK;
            player.SendMessage($"AFK: {player.IsAFK}");
        }*/
        #endregion


    }
}
