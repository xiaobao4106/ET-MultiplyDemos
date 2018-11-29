using FairyGUI;

namespace ETModel
{
    public static class NoneFGUIFactory
    {
        public static void Create()
        {
            FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();
            FUI fui = ComponentFactory.Create<FUI, GObject>(UIPackage.CreateObject(FUIType.Sekia, FUIType.NoneFGUI));
            fui.Name = FUIType.NoneFGUI;
            fui.AddComponent<SelectCharacterComponent>();
            fuiComponent.Add(fui);
        }
    }

    [Event(EventIdType.NoneFGUIFinish)]
    public class NoneFGUIFinish : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.NoneFGUI);
        }
    }
}