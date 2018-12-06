using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UCharacterMgr  {

	private UCombineSkinnedMgr skinnedMgr = null;
    //本例中合成方法被当作单例使用 建议直接作为全局静态方法使用
	public UCombineSkinnedMgr CombineSkinnedMgr { get{ return skinnedMgr; } }

	private int characterIndex = 0;
	private Dictionary<int, UCharacterController> characterDic = new Dictionary<int, UCharacterController>();

    //自构时初始化一个单例类成员 建议直接作为全局静态方法使用
	public UCharacterMgr () 
    {
		skinnedMgr = new UCombineSkinnedMgr ();
	}

    //增操作
	public UCharacterController Generatecharacter (string skeleton, string weapon, 
        string head, string chest, string hand, string feet, bool combine = false)
	{
		UCharacterController instance = new UCharacterController (characterIndex,
            skeleton,weapon,head,chest,hand,feet,combine);
		characterDic.Add(characterIndex,instance);
		characterIndex++;

		return instance;
	}

	public void Removecharacter (CharacterController character)
	{
        //作为一个玩家管理组件 当然需要有增/删/查/改等一般功能
        //移除角色 这里写处理代码
	}

	public void Update () 
    {
        //没啥更新内容
		foreach(UCharacterController character in characterDic.Values)
		{
			character.Update();
		}
	}
}
