using UnityEngine;
using FairyGUI;
using System;

namespace ETModel
{
    [ObjectSystem]
    public class NoneFGUIComponentAwakeSystem : AwakeSystem<NoneFGUIComponent>
    {
        public override void Awake(NoneFGUIComponent self)
        {
            self.Awake();
        }
    }

    public class NoneFGUIComponent : Component
    {
        

        public void Awake()
        {
            Log.Debug("加载空界面");
        }
        
        //退出界面需要重置所有元件属性
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}
