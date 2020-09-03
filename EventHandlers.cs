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

        internal void OnRoundStart(){
            dogeServerPlugin.IsStarted = true;
            foreach(var player in Exiled.API.Features.Player.List){
                player.GetDatabasePlayer().TotalGamesPlayed++;
                AddExp(player, 3);
            }
        }


        internal void OnPlayerDeath(DiedEventArgs ev){
            AddExp(ev.Target, 5);
            ev.Target.GetDatabasePlayer().TotalDeath++;
            ev.Killer.GetDatabasePlayer().TotalKilled++;

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
                    AddExp(ev.Killer, 5);
                }
        }

        internal void OnScpTerminated(AnnouncingScpTerminationEventArgs ev){
            ev.Killer.GetDatabasePlayer().TotalScpKilled++;
            AddExp(ev.Killer, 20);

            if (_pluginInstance.Config.ScpTerminatedMessageEnable){
                string teamName = "";
                switch(ev.Killer.Team){
                    case Team.CDP:
                        teamName = "<color=orange>D계급 인원</color>";
                        break;
                    case Team.CHI:
                        teamName = "<color=green>혼돈의 반란</color>";
                        break;
                    case Team.MTF:
                        teamName = "<color=blue>기동특무부대</color>";
                        break;
                    case Team.RSC:
                        teamName = "<color=yellow>과학자</color>";
                        break;
                    default:
                        teamName = "<color=red>알 수 없음</color>";
                        break;
                }

                Map.Broadcast(10, $"<color=red>{ev.Role.fullName}</color>(이)가 {teamName}에 의해 격리되었습니다.\n격리 사유:<color=red>{ev.TerminationCause}</color>");
            }
        }




        private string FilteringNickname(Exiled.API.Features.Player player){
            return player.Nickname.Replace("Youtube", "").Replace("Twitch","").Replace("유튜브","").Replace("트위치","");
        }

        private static void AddExp(Exiled.API.Features.Player player, int exp)
        {
            int nowExp = player.GetDatabasePlayer().Exp;
            int nowLevel = player.GetDatabasePlayer().Level;
            if (nowExp + exp >= (nowLevel + 2) * 2)
            {
                player.GetDatabasePlayer().Level++;
                player.GetDatabasePlayer().Exp = 0;
                player.Broadcast(5,$"레벨업!\n당신의 레벨이 <color=green>{player.GetDatabasePlayer().Level}</color>레벨으로 올랐습니다.\n`를 눌러 콘솔창을 연 뒤 <color=green>.irc_stats</color> 명령어로 확인이 가능합니다!");
            }
            else
            {
                player.GetDatabasePlayer().Exp += exp;
            }
            return;
        }
    }
}