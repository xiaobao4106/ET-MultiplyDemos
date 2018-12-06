using UnityEngine;
using System.Collections.Generic;

public class UCombineSkinnedMgr 
{
	private const int COMBINE_TEXTURE_MAX = 512;
	private const string COMBINE_DIFFUSE_TEXTURE = "_MainTex";
    
    //将复数的mesh合并为一个新mesh 将新mesh赋值给骨架
	public void CombineObject (GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
    {
        //收集骨架上的全部骨骼(包括Prefab上的全部Transform)
		List<Transform> transforms = new List<Transform>();
		transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));
        //所有部件的所有材质
		List<Material> materials = new List<Material>();

        //所有部件的所有mesh
		List<CombineInstance> combineInstances = new List<CombineInstance>();

        //部件与骨架之间重复的骨骼
        List<Transform> bones = new List<Transform>();

		//合并材质所需的临时参数
		List<Vector2[]> oldUV = null;
		Material newMaterial = null;
		Texture2D newDiffuseTex = null;
        
        //从复数的mesh中收集Material/Mesh/Transform(骨骼)
		for (int i = 0; i < meshes.Length; i ++)
		{
			SkinnedMeshRenderer smr = meshes[i];
            //添加材质
			materials.AddRange(smr.materials); 

			//添加子mesh 每个子网格对应一个材质
            //这里可能指的是多材质mesh的子mesh数不为1 未验证
			for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
            }

			//添加部件与骨架之间重复的骨骼
			for (int j = 0 ; j < smr.bones.Length; j ++)
			{
				for (int tBase = 0; tBase < transforms.Count; tBase++)
				{
					if (smr.bones[j].name.Equals(transforms[tBase].name))
					{
						bones.Add(transforms[tBase]);
						break;
					}
				}
			}
		}

        //合并材质 编辑Shader
        //不建议合并材质 优秀的美术总是为不同的物体（头发/皮肤）指定材质
		if (combine)
		{
			newMaterial = new Material (Shader.Find ("Mobile/Diffuse"));
			oldUV = new List<Vector2[]>();
			
            //收集材质上的贴图 贴图名在Shader中被指定
			List<Texture2D> Textures = new List<Texture2D>();
			for (int i = 0; i < materials.Count; i++)
			{
				Textures.Add(materials[i].GetTexture(COMBINE_DIFFUSE_TEXTURE) as Texture2D);
			}

            //贴图合成
			newDiffuseTex = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
			Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0); //合并UV贴图
			newMaterial.mainTexture = newDiffuseTex; //为新材质赋值纹理贴图

            //UV合成 纹理贴图合并后mesh上的顶点需要寻找新的UV坐标
            //因为在Unity里面无法像3D软件一样直观看到UV贴图 这里是黑箱操作
			Vector2[] uva, uvb;
			for (int j = 0; j < combineInstances.Count; j++)
			{
                uva = combineInstances[j].mesh.uv; //获取部件合并前的UV贴图
				uvb = new Vector2[uva.Length];
				for (int k = 0; k < uva.Length; k++) //设置每个顶点在新UV贴图中的位置 具体意义不明
				{
					uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
				}
                oldUV.Add(uva); //保存部件的旧UV贴图
				combineInstances[j].mesh.uv = uvb; //使用新UV贴图
            }
		}

        //添加新的SkinnedMeshRenderer组件 并为其赋值mesh/材质/骨骼
        SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
		if (oldSKinned != null) 
        {
        	GameObject.DestroyImmediate (oldSKinned);
		}
		SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);
        r.bones = bones.ToArray();
		if (combine)
		{
			r.material = newMaterial;
            for (int i = 0; i < combineInstances.Count; i++)
            {
                //恢复部件mesh的UV贴图到合并前
                //Prefab只被加载一次 修改里面的mesh的贴图后要复用需要还原
                combineInstances[i].mesh.uv = oldUV[i];
            }
		}
        else
		{
			r.materials = materials.ToArray();
		}
	}
}
