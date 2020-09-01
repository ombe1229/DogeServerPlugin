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




        private string FilteringNickname(Exiled.API.Features.Player player){
            return player.Nickname.Replace("Youtube", "").Replace("Twitch","").Replace("유튜브","").Replace("트위치","");
        }
    }
}