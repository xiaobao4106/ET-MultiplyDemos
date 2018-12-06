using UnityEngine;
using System.Collections;

//简单的游戏框架 没有啥用
public class App
{
	private static App app = new App();

	public static App Game { get { return app; } }

	private UCharacterMgr characterMgr = new UCharacterMgr ();
	public UCharacterMgr CharacterMgr { get { return characterMgr; } }

	public void Update ()
	{
		characterMgr.Update ();
	}
}

public class TestAvater1 : MonoBehaviour
{
    //部件状态默认配置 Prefab命名规则
	private readonly string[] index = new string[]{ "004", "006", "008" };
	private const int DEFAULT_WEAPON = 0;//默认武器
	private const int DEFAULT_HEAD = 0; //默认头部
	private const int DEFAULT_CHEST = 0;//默认胸部
	private const int DEFAULT_HAND = 0; //默认护手
	private const int DEFAULT_FEET = 0; //默认鞋子
	private const bool DEFAULT_COMBINEMATERIAL = true; //是否合并材质

    //UI参数容器
    private bool combine = DEFAULT_COMBINEMATERIAL;
	private bool[] weapon_list = new bool[3];
	private bool[] head_list = new bool[3];
	private bool[] chest_list = new bool[3];
	private bool[] hand_list = new bool[3];
	private bool[] feet_list = new bool[3];
    
    //当前场景中的合成角色对象
	private UCharacterController character = null;
    
	void Start () 
    {
        //设置默认参数
		weapon_list [DEFAULT_WEAPON] = true;
		head_list [DEFAULT_HEAD] = true;
		chest_list [DEFAULT_CHEST] = true;
		hand_list [DEFAULT_HAND] = true;
		feet_list [DEFAULT_FEET] = true;

        //创建合成角色 调整位置和角度
        //这里使用UCharacterMgr类用于管理所有合成角色 该类在本例中用处不大
        //核心过程就是实例化了一个UCharacterController（合成角色）对象
        //Prefab名需要遵守命名规则
        //如果需要使用这套系统直接实例化对象后添加到自己的玩家管理组件即可
        character = App.Game.CharacterMgr.Generatecharacter (
			"ch_pc_hou", 
			"ch_we_one_hou_" + index[DEFAULT_WEAPON],
			"ch_pc_hou_" + index[DEFAULT_HEAD] + "_tou", 
			"ch_pc_hou_" + index[DEFAULT_CHEST] + "_shen", 
			"ch_pc_hou_" + index[DEFAULT_HAND] + "_shou", 
			"ch_pc_hou_" + index[DEFAULT_FEET] + "_jiao",
			combine);
		character.Instance.transform.position = new Vector3 (0, -1, -5);
		character.Instance.transform.eulerAngles = new Vector3 (0, 180, 0);
	}
	
	//执行合成玩家管理器的更新 这里的管理器是一个单例 没啥好更新的
	void Update () 
    {
		App.Game.Update();
	}

	void OnGUI () 
    {
		GUI.Button (new Rect (0, 0, 100, 30),   "Euipments 1");
		GUI.Button (new Rect (100, 0, 100, 30), "Euipments 2");
		GUI.Button (new Rect (200, 0, 100, 30), "Euipments 3");

		for (int i = 0; i < weapon_list.Length; i++) 
        {
			if (GUI.Button (new Rect (i * 100, 30, 100, 50), "Weapon" + (weapon_list[i] ? "(√)" : ""))) 
            {
				
				if (!weapon_list [i]) 
                {
					for (int j = 0; j < weapon_list.Length; j++) 
                    {
						weapon_list [j] = false;
					}
					weapon_list [i] = true;
					
					character.ChangeWeapon ("ch_we_one_hou_" + index[i]);
				}
			}
		}

		for (int i = 0; i < head_list.Length; i++) 
        {

			if (GUI.Button (new Rect (i * 100, 80, 100, 50), "Head" + (head_list[i] ? "(√)" : ""))) 
            {

				if (!head_list [i]) 
                {
					for (int j = 0; j < head_list.Length; j++) 
                    {
						head_list [j] = false;
					}
					head_list [i] = true;

					character.ChangeHeadEquipment ("ch_pc_hou_" + index[i] + "_tou", combine);
				}
			}
		}

		for (int i = 0; i < chest_list.Length; i++) 
        {

			if (GUI.Button (new Rect (i * 100, 130, 100, 50), "Chest" + (chest_list[i] ? "(√)" : ""))) 
            {

				if (!chest_list [i]) 
                {
					for (int j = 0; j < chest_list.Length; j++) 
                    {
						chest_list [j] = false;
					}
					chest_list [i] = true;

					character.ChangeChestEquipment ("ch_pc_hou_" + index[i] + "_shen", combine);
				}
			}
		}

		for (int i = 0; i < hand_list.Length; i++) 
        {

			if (GUI.Button (new Rect (i * 100, 180, 100, 50), "Hand" + (hand_list[i] ? "(√)" : ""))) 
            {

				if (!hand_list [i]) 
                {
					for (int j = 0; j < hand_list.Length; j++) 
                    {
						hand_list [j] = false;
					}
					hand_list [i] = true;

					character.ChangeHandEquipment("ch_pc_hou_" + index[i] + "_shou", combine);
				}
			}
		}

		for (int i = 0; i < feet_list.Length; i++) 
        {

			if (GUI.Button (new Rect (i * 100, 230, 100, 50), "Feet" + (feet_list[i] ? "(√)" : ""))) 
            {

				if (!feet_list [i]) 
                {
					for (int j = 0; j < feet_list.Length; j++) 
                    {
						feet_list [j] = false;
					}
					feet_list [i] = true;

					character.ChangeFeetEquipment("ch_pc_hou_" + index[i] + "_jiao", combine);
				}
			}
		}

		if (GUI.Button(new Rect(Screen.width - 100,0,100,50),character.animationState == 0 ? "Attack" : "Stand"))
		{
			if (character.animationState == 0)
			{
				character.PlayAttack();
			}
            else
			{
				character.PlayStand();
			}
		}

		if (GUI.Button(new Rect(Screen.width - 100,50,100,50),character.rotate ? "Static" : "Rotate"))
		{
			if (character.rotate)
			{
				character.rotate = false;
			}
            else
			{
				character.rotate = true;
			}
		}

        if (GUI.Button(new Rect(Screen.width - 150, 100, 150, 50), combine ? "Merge materials(√)" : "Merge materials"))
        {
            combine = !combine;
        }
    }

}
