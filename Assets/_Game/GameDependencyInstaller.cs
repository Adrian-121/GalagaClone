using UnityEngine;
using Zenject;

public class GameDependencyInstaller : MonoInstaller {

    [SerializeField] PlayerMainController _playerPrefab;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] EnemyMainController _enemyPrefab;

    public override void InstallBindings() {

        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<PlayerFireSignal>();
        Container.DeclareSignal<StartGameSignal>();
        Container.DeclareSignal<HighscoresSignal>();
        Container.DeclareSignal<HighscoreBackSignal>();
        Container.DeclareSignal<EditorSignal>();


        Container.Bind<ProjectileManager>().FromInstance(FindObjectOfType<ProjectileManager>()).AsSingle().NonLazy();

        Container.Bind<ResourceLoader>().FromInstance(FindObjectOfType<ResourceLoader>()).AsSingle().NonLazy();

        Container.Bind<GameManager>().FromInstance(FindObjectOfType<GameManager>()).AsSingle().NonLazy();

        Container.Bind<UIManager>().FromInstance(FindObjectOfType<UIManager>()).AsSingle().NonLazy();

        Container.Bind<GameFSM>().FromInstance(FindObjectOfType<GameFSM>()).AsSingle().NonLazy();


        Container.Bind<PlayerSpawnPosition>().FromInstance(FindObjectOfType<PlayerSpawnPosition>()).AsSingle().NonLazy();


        //Container.Bind<EnemyManager>().FromInstance(FindObjectOfType<EnemyManager>()).AsSingle();


        //Container.Bind<PlayerFire>().AsTransient();
        Container.BindFactory<PlayerMainController, PlayerMainController.Factory>().FromComponentInNewPrefab(_playerPrefab).AsSingle();
        Container.BindFactory<EnemyMainController, EnemyMainController.Factory>().FromComponentInNewPrefab(_enemyPrefab).AsTransient();
        Container.BindFactory<Projectile, Projectile.Factory>().FromComponentInNewPrefab(_projectilePrefab).AsTransient();        
    }

}