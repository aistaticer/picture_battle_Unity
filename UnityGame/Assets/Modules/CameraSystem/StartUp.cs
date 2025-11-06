using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


namespace CameraSystem
{
    public class StartUp
    {

        private DiContainer _container;
        private CameraFactory _cameraFactory;

        [Inject] public CameraObjSet cameraObjSet;


        private GameObject mainCamera;

        private GameObject subCamera;

        CameraSystemContext _cameraSystemContext;
        
        [Inject]
        public void Construct(DiContainer diContainer)
        {
            // DI用Containar注入
            _container = diContainer;

            // Scene内にカメラ生成。subCameraは非アクティブにして描画はmain優先。
            _cameraFactory = new CameraFactory();
            mainCamera = _cameraFactory.Create(cameraObjSet._mainCamera, true);
            subCamera = _cameraFactory.Create(cameraObjSet._subCamera, false);

            // CameraStateをcameraの種類によってインスタンス化
            var mainCameraState = new CameraState(mainCamera);
            var subCameraState = new CameraState(subCamera);

            // Context作成。必要なプロパティを設定。DI登録。
            _cameraSystemContext = new CameraSystemContext();
            _cameraSystemContext.mainCameraState = mainCameraState;
            _cameraSystemContext.subCameraState = subCameraState;
            _cameraSystemContext._cameraFactory = _cameraFactory;

            _container.BindInstance(_cameraSystemContext).AsSingle();

            // CameraController をシーン上に生成
            var go = new GameObject("CameraController");
            var controller = go.AddComponent<CameraController>();

            var cameraManeger = new CameraManeger();
            _container.BindInstance(cameraManeger).AsSingle();

            // DI 注入
            _container.Inject(controller);

        }

    };
}

