using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Respawning;

namespace DogeServerPlugin{
    public class EventHandlers{
        private readonly dogeServerPlugin _pluginInstance;
        public EventHandlers(dogeServerPlugin pluginInstance) => this._pluginInstance = pluginInstance;


        internal void OnJoin(JoinedEventArgs ev)
        {
            if (!Database.LiteDatabase.GetCollection<Player>().Exists(player => player.Id == ev.Player.GetRawUserId()))
            {
                Log.Info(ev.Player.Nickname + " is not present on DB!");
                _pluginInstance.DatabasePlayerData.AddPlayer(ev.Player);
            }

            var databasePlayer = ev.Player.GetDatabasePlayer();
            if (Database.PlayerData.ContainsKey(ev.Player))
            {
                Database.PlayerData.Add(ev.Player, databasePlayer);
                databasePlayer.LastSeen = DateTime.Now;
                databasePlayer.Name = ev.Player.Nickname;
                if (databasePlayer.FirstJoin == DateTime.MinValue) databasePlayer.FirstJoin = DateTime.Now;
            }

            if (!_pluginInstance.Config.NickNameFilteringEnable)
            {
                var nickname = FilteringNickname(ev.Player);
                if (ev.Player.Nickname != nickname) ev.Player.DisplayNickname = nickname;
            }
        }

        internal void OnPlayerLeft(LeftEventArgs ev)
        {
            if (ev.Player.Nickname != "Dedicated Server" && ev.Player != null && Database.PlayerData.ContainsKey(ev.Player))
            {
                ev.Player.GetDatabasePlayer().SetCurrentDayPlayTime();
                Database.LiteDatabase.GetCollection<Player>().Update(Database.PlayerData[ev.Player]);
                Database.PlayerData.Remove(ev.Player);
            }

            if (_pluginInstance.Config.ScpLeaveMessageEnable && ev.Player.Team == Team.SCP)
            {
                Map.Broadcast(10,$"<color=red>{ev.Player.Nickname}</color>(이)가 <color=red>SCP</color>진영에서 게임을 중도 퇴장하였습니다.");
            }
        }


        internal void OnPlayerDeath(DiedEventArgs ev){
            if (_pluginInstance.Config.ScpHealingEnable && ev.Killer.Team == Team.SCP && ev.Killer != ev.Target){
                    int heal = 0;
                    switch (ev.Killer.Role)
                    {
                        case RoleType.Scp173 :
                            heal = _pluginInstance.Config.Healing173;
                            break;
                        case RoleType.Scp096 :
                            heal = _pluginInstance.Config.Healing096;
                            break;
                        case RoleType.Scp049 :
                            heal = _pluginInstance.Config.Healing049;
                            break;
                        case RoleType.Scp0492 :
                            heal = _pluginInstance.Config.Healing0492;
                            break;
                        case RoleType.Scp106 :
                            heal = _pluginInstance.Config.Healing106;
                            break;
                        case RoleType.Scp93953 :
                            heal = _pluginInstance.Config.Healing939;
                            break;
                        case RoleType.Scp93989 :
                            heal = _pluginInstance.Config.Healing939;
                            break;
                    }

                    if (ev.Killer.MaxHealth <= ev.Killer.Health + heal) ev.Killer.Health = ev.Killer.MaxHealth;
                    else ev.Killer.Health += heal;
                    ev.Killer.Broadcast(10,$"당신은 인간을 처치하여 체력 <color=red>${heal}</color>만큼 회복했습니다.\n현재 HP:<color=red>{ev.Killer.Health}</color>");
                }
        }




        private string FilteringNickname(Exiled.API.Features.Player player){
            return player.Nickname.Replace("Youtube", "").Replace("Twitch","").Replace("유튜브","").Replace("트위치","");
        }
    }
}