using UnityEngine;
using System.Collections;

public class UCharacterController 
{
    //合成角色的模型
	public GameObject Instance = null;
	public GameObject WeaponInstance = null;

    //合成角色的部件名
	public string skeleton;
	public string equipment_head;
	public string equipment_chest;
	public string equipment_hand;
	public string equipment_feet;

    //当前角色在玩家管理器中的编号
	public int index;

    //其他编号 用于在UI界面中Update
	public bool rotate = false;
	public int animationState = 0;

	private Animation animationController = null;

    //合成角色的自构方法
    //参数：序列号/骨架名/武器名/头部名/胸部名/手部名/脚步名/是否合成材质
    //这里的部件名使用的是Prefab文件全名 调用时需在外部根据命名规则处理Prefab文件全名
	public UCharacterController (int index,string skeleton, string weapon, 
        string head, string chest, string hand, string feet, bool combine = false)
    {
        //设置合成角色参数
        this.index = index;
        this.skeleton = skeleton;
        this.equipment_head = head;
        this.equipment_chest = chest;
        this.equipment_hand = hand;
        this.equipment_feet = feet;

        //实例化骨架
        Object res = Resources.Load ("Sekia/Prefab/" + skeleton);
		this.Instance = GameObject.Instantiate (res) as GameObject;

		//实例化所有部件 收集网格和材质
		string[] equipments = new string[4];
		equipments [0] = head;
		equipments [1] = chest;
		equipments [2] = hand;
		equipments [3] = feet;
		SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
		GameObject[] objects = new GameObject[4];
		for (int i = 0; i < equipments.Length; i++) 
        {
			res = Resources.Load ("Sekia/Prefab/" + equipments [i]);
			objects[i] = GameObject.Instantiate (res) as GameObject;
			meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer> ();
		}

        //合成所有网格和材质到骨架 建议直接作为全局静态方法使用
        App.Game.CharacterMgr.CombineSkinnedMgr.CombineObject(Instance, meshes, combine);

        //删除临时实例化的GameObject
        for (int i = 0; i < objects.Length; i++) 
        {
			GameObject.DestroyImmediate (objects [i].gameObject);
		}
		
		//实例化武器
		res = Resources.Load ("Sekia/Prefab/" + weapon);
		WeaponInstance = GameObject.Instantiate (res) as GameObject;
		
        //将武器挂载到武器根目录 根据骨骼命名规则找武器根目录
		Transform[] transforms = Instance.GetComponentsInChildren<Transform>();
		foreach (Transform joint in transforms) 
        {
			if (joint.name == "weapon_hand_r") 
            {
				WeaponInstance.transform.parent = joint.gameObject.transform;
				break;
			}	
		}

        //初始化武器实例状态
		WeaponInstance.transform.localScale = Vector3.one;
		WeaponInstance.transform.localPosition = Vector3.zero;
		WeaponInstance.transform.localRotation = Quaternion.identity;

        //播放动画 这里播放老版本Legacy动画才使用的Animation组件
        //使用Mecanim动画系统的应使用Animator组件和动画控制器
        animationController = Instance.GetComponent<Animation>();
		PlayStand();
	}

	public void ChangeHeadEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (0, equipment, combine);
	}
	
	public void ChangeChestEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (1, equipment, combine);
	}
	
	public void ChangeHandEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (2, equipment, combine);
	}
	
	public void ChangeFeetEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (3, equipment, combine);
	}
	
    //更换武器 替换到旧武器的位置
	public void ChangeWeapon (string weapon)
	{
		Object res = Resources.Load ("Sekia/Prefab/" + weapon);
		GameObject oldWeapon = WeaponInstance;
		WeaponInstance = GameObject.Instantiate (res) as GameObject;
		WeaponInstance.transform.parent = oldWeapon.transform.parent;
		WeaponInstance.transform.localPosition = Vector3.zero;
		WeaponInstance.transform.localScale = Vector3.one;
		WeaponInstance.transform.localRotation = Quaternion.identity;
		
		GameObject.Destroy(oldWeapon);
	}
	
    //更换装备
    //因为武器是挂在骨架上的 武器的父级没有发生变化
    //发生变化的内容为Skinned Mesh Renderer组件的Mesh/Materials赋值
	public void ChangeEquipment (int index, string equipment,bool combine = false)
	{
        //更新合成角色参数
		switch (index) 
        {
			
		case 0:
			equipment_head = equipment;
			break;
		case 1:
			equipment_chest = equipment;
			break;
		case 2:
			equipment_hand = equipment;
			break;
		case 3:
			equipment_feet = equipment;
			break;
		}

        //实例化所有部件 收集网格和材质
        string[] equipments = new string[4];
		equipments [0] = equipment_head;
		equipments [1] = equipment_chest;
		equipments [2] = equipment_hand;
		equipments [3] = equipment_feet;
		Object res = null;
		SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
		GameObject[] objects = new GameObject[4];
		for (int i = 0; i < equipments.Length; i++) 
        {
			
			res = Resources.Load ("Sekia/Prefab/" + equipments [i]);
			objects[i] = GameObject.Instantiate (res) as GameObject;
			meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer> ();
		}

        //合成所有网格和材质到骨架 建议直接作为全局静态方法使用
        App.Game.CharacterMgr.CombineSkinnedMgr.CombineObject(
            Instance, meshes, combine);

        //删除临时实例化的GameObject
        for (int i = 0; i < objects.Length; i++) 
        {
			GameObject.DestroyImmediate(objects[i].gameObject);
		}
	}

	public void PlayStand () 
    {

		animationController.wrapMode = WrapMode.Loop;
		animationController.Play("legacy_breath");
		animationState = 0;
	}
	
	public void PlayAttack () 
    {
		
		animationController.wrapMode = WrapMode.Once;
		animationController.PlayQueued("legacy_attack1");
		animationController.PlayQueued("legacy_attack2");
		animationController.PlayQueued("legacy_attack3");
		animationController.PlayQueued("legacy_attack4");
		animationState = 1;
	}

    //这个Update作用为保持持续播放Lagacy动画
    public void Update () 
    {
		if (animationState == 1)
		{
			if (! animationController.isPlaying)
			{
				PlayAttack();
			}
		}
		if (rotate)
		{
			Instance.transform.Rotate(new Vector3(0,90 * Time.deltaTime,0));
		}
	}
}
