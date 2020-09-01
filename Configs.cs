using Exiled.API.Interfaces;
using System.ComponentModel;
using Log = Exiled.API.Features.Log;

namespace DogeServerPlugin{
    public class Configs : IConfig{
        public bool IsEnabled { get; set; } = true;
        
        [Description("SCP 중도 퇴장 메세지 표시 여부")]
        public bool ScpLeaveMessageEnable { get; private set; } = true;

        [Description("저위험군 격리 메세지 표시 여부")]
        public bool DecontaminationMessageEnable { get; private set; } = true;

        [Description("NTF 지원 메세지 표시 여부")]
        public bool NtfMassageEnable { get; private set; } = true;

        [Description("혼돈의 반란 지원 메세지 표시 여부")]
        public bool ChaosMessageEnable { get; private set; } = true;

        [Description("SCP 격리 메세지 표시 여부")]
        public bool ScpTerminatedMessageEnable { get; private set; } = true;

        [Description("핵 작동/중지 메세지 표시 여부 (개발중)")]
        public bool NukeMessageEnable { get; private set; } = false;

        [Description("사망 시 메세지 표시 여부")]
        public bool DeathMessageEnable { get; private set; } = true;

        [Description("106희생대 메세지 표시 여부")]
        public bool FemurBreakerMessageEnable { get; private set; } = true;

        [Description("닉네임 필터링 사용 여부")]
        public bool NickNameFilteringEnable { get; private set; } = true;

        [Description("SCP 체력회복 사용 여부")]
        public bool ScpHealingEnable { get; private set; } = true;

        [Description("173 치유량")]
        public int Healing173 { get; private set; } = 150;

        [Description("096 h치유량")]
        public int Healing096 { get; private set; } = 0;
        
        [Description("049 치유량")]
        public int Healing049 { get; private set; } = 25;
        
        [Description("049-2 치유량")]
        public int Healing0492 { get; private set; } = 25;
        
        [Description("106 치유량")]
        public int Healing106 { get; private set; } = 75;
        
        [Description("939 치유량")]
        public int Healing939 { get; private set; } = 125;

        [Description("데이터베이스 이름")]
        public string DatabaseName { get; private set; } = "DogeServerPlugin";

        [Description("데이터베이스 위치")]
        public string DatabaseFolder { get; private set; } = "EXILED";
        
        
        public void ConfigValidator(){
            if (!IsEnabled){
                Log.Warn("You disabled the IRCPlugin in server configs!");
            }
        }
    }
}