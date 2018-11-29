using ETModel;

namespace ETHotfix
{
    public static partial class UIType
    {
        public const string Moba5V5UI = "Moba5V5UI";
        public const string SekiaLobby = "SekiaLobby";
        public const string LandlordsRoom = "LandlordsRoom";
        public const string SekiaLogin = "SekiaLogin";
        public const string LandlordsEnd = "LandlordsEnd";
        public const string LandlordsInteraction = "LandlordsInteraction";
    }

    [Event(ETModel.EventIdType.SekiaInitLobby)]
    public class SekiaInitLobby_CreateLobbyUI : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.SekiaLobby);
        }
    }

    //Model层已经创建了Session 通知Hotfix层绑定
    [Event(ETModel.EventIdType.SetHotfixSession)]
    public class SetHotfixSession : AEvent
    {
        public override void Run()
        {
            //获取Model层的Session
            ETModel.Session session = ETModel.SessionComponent.Instance.Session;
            //创建Hotfix层的Session
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(session);
        }
    }
}