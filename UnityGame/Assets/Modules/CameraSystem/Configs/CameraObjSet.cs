using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/CameraObjSet")] 
public class CameraObjSet : ScriptableObject 
{ 
	public GameObject _mainCamera;
	public GameObject _subCamera; 
	

    public GameObject GetCameraByType(CameraObjType type)
    {
			switch (type)
			{
				case CameraObjType.MainType:
					return _mainCamera.gameObject;

				case CameraObjType.SubType:
					return _subCamera.gameObject;

				default:
					return null;
			}
    }
}	