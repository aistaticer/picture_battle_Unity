using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CameraFactory
{
    public GameObject Create(GameObject cameraPrefab, Boolean enabledFlg)
    {
        var go = GameObject.Instantiate(cameraPrefab);
        var CameraCom = go.GetComponent<Camera>();
        CameraCom.enabled = enabledFlg; 

        return go;
    }
}
