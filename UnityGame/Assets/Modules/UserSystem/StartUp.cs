using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Zenject;
using UnityGame.Assets.Modules.UserSystem.Domain;


namespace UnityGame.Assets.Modules.UserSystem
{

    public class StartUp
    {
        private DiContainer _container;

        [Inject] // ← コンストラクタインジェクション
        public void Construct(DiContainer diContainer)
		{
            _container = diContainer;
            var userState = new Domain.UserState(userName: "test", teamName:"testTeam", userId: "testId");
            _container.BindInstance(userState);
		}
    }
}
