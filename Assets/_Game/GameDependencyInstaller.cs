using UnityEngine;
using Zenject;

public class GameDependencyInstaller : MonoInstaller {

    [SerializeField] PlayerMainController _playerPrefab;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] EnemyMainController _enemyPrefab;
    [SerializeField] VFXObject _explosionPrefab;

    public override void InstallBindings() {
        SignalBusInstaller.Install(Container);

        // Game Events.
        Container.DeclareSignal<PlayerFireSignal>();
        Container.DeclareSignal<PlayerKilledSignal>();
        Container.DeclareSignal<PlayerObjectKilledSignal>();

        Container.DeclareSignal<GameTimeTickSignal>();
        Container.DeclareSignal<StartGameSignal>();
        Container.DeclareSignal<GameOverSignal>();
        Container.DeclareSignal<GameOverBackButtonSignal>();

        Container.DeclareSignal<EnemyKilledSignal>();
        
        Container.DeclareSignal<SoundMuteButtonSignal>();
        Container.DeclareSignal<SoundMuteStatusChangedSignal>();

        Container.DeclareSignal<LevelChangedSignal>();
        Container.DeclareSignal<LevelClearedSignal>();

        Container.DeclareSignal<CurrentScoreSignal>();
        Container.DeclareSignal<HighscoresProcessedSignal>();
        Container.DeclareSignal<HighscoreInGameChangeSignal>();

        Container.DeclareSignal<UIButtonPressedSignal>();

        // Game Systems.
        Container.Bind<GameFSM>().FromInstance(FindObjectOfType<GameFSM>()).AsSingle().NonLazy();
        Container.Bind<GameManager>().FromInstance(FindObjectOfType<GameManager>()).AsSingle().NonLazy();
        Container.Bind<ResourceLoader>().FromInstance(FindObjectOfType<ResourceLoader>()).AsSingle().NonLazy();
        Container.Bind<ProjectileManager>().FromInstance(FindObjectOfType<ProjectileManager>()).AsSingle().NonLazy();
        Container.Bind<UIManager>().FromInstance(FindObjectOfType<UIManager>()).AsSingle().NonLazy();
        Container.Bind<VFXManager>().FromInstance(FindObjectOfType<VFXManager>()).AsSingle().NonLazy();
        Container.Bind<SoundManager>().FromInstance(FindObjectOfType<SoundManager>()).AsSingle().NonLazy();
        Container.Bind<PlayerSpawnPosition>().FromInstance(FindObjectOfType<PlayerSpawnPosition>()).AsSingle().NonLazy();

        // Object Factories.
        Container.BindFactory<PlayerMainController, PlayerMainController.Factory>().FromComponentInNewPrefab(_playerPrefab).AsSingle();
        Container.BindFactory<EnemyMainController, EnemyMainController.Factory>().FromComponentInNewPrefab(_enemyPrefab).AsTransient();
        Container.BindFactory<Projectile, Projectile.Factory>().FromComponentInNewPrefab(_projectilePrefab).AsTransient();
        Container.BindFactory<VFXObject, VFXObject.Factory>().FromComponentInNewPrefab(_explosionPrefab).AsTransient();
    }
}