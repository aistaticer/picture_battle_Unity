using System;
using Zenject;
using UnityEngine;

public class CameraSystemContext
{
    public CameraFactory _cameraFactory { get; set; }

    public CameraState mainCameraState { get; set; }

    public CameraState subCameraState { get; set; }



}
