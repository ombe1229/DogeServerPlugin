using Log = Exiled.API.Features.Log;
using ServerEvents = Exiled.Events.Handlers.Server;
using PlayerEvents = Exiled.Events.Handlers.Player;
using MapEvents = Exiled.Events.Handlers.Map;
using Features = Exiled.API.Features;

namespace DogeServerPlugin{
    public class dogeServerPlugin : Features.Plugin<Configs>{
        public static bool IsStarted { get; set; }
        public EventHandlers EventHandlers { get; private set; }
        public Database DatabasePlayerData { get; private set; }

        public void LoadEvents(){
            PlayerEvents.Joined += EventHandlers.OnJoin;
            PlayerEvents.Died += EventHandlers.OnPlayerDeath;
            PlayerEvents.Left += EventHandlers.OnPlayerLeft;
        }

        public void LoadCommands(){
            
        }

        public override void OnEnabled(){
            if (!Config.IsEnabled) return;
            EventHandlers = new EventHandlers(this);
            DatabasePlayerData = new Database(this);
            LoadEvents();
            LoadCommands();
            Log.Info("DogeServerPlugin enabled.");
        }

        public override void OnDisabled(){
            EventHandlers = null;
        }

        public override void OnReloaded(){
            
        }
    }
}