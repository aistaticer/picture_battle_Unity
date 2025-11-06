// using Zenject;
// using UnityEngine;

// public class GameInstaller : MonoInstaller
// {
// 	[SerializeField] private PlayerModelSet _playerModelSetAsset;

// 	public override void InstallBindings()
// 	{
// 		// Scene 上の Manager を取得して共有

// 		Container.Bind<PlayerManager>().FromComponentInHierarchy().AsSingle();

// 		Container.Bind<TileActionService>().AsSingle();
// 		Container.Bind<ICameraMover>().To<CameraMover>().AsSingle();
// 		Container.Bind<ClickState>().AsSingle();
// 		Container.Bind<Movement>().AsSingle();
// 		Container.Bind<PlayerRegistry>().AsSingle();


// 		// シーン内に CoroutineRunner を自動生成して登録
// 		Container.Bind<CoroutineRunner>()
// 			.FromNewComponentOnNewGameObject()
// 			.AsSingle()
// 			.NonLazy();

// 		// SignalBus 自体をシングルトンとして登録
// 		SignalBusInstaller.Install(Container);

// 		// Signal を登録（必要なシグナルごとに）
// 		Container.DeclareSignal<TileClickedSignal>();

// 		// シーン全体で1つのインスタンスを共有
// 		//Container.Bind<ClickEvent>().AsSingle();

// 		// Inspector で設定された ScriptableObject をそのまま DI へ
// 		Container.BindInstance(_playerModelSetAsset).AsSingle();
// 	}
// }
