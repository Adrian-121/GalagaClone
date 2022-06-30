using UnityEngine;
using Zenject;

public class GameDependencyInstaller : MonoInstaller {

    [SerializeField] Player _playerPrefab;
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] Enemy _enemyPrefab;
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
        Container.Bind<MainGameSystem>().FromInstance(FindObjectOfType<MainGameSystem>()).AsSingle().NonLazy();
        Container.Bind<ResourceLoader>().FromInstance(FindObjectOfType<ResourceLoader>()).AsSingle().NonLazy();
        Container.Bind<ProjectileSystem>().FromInstance(FindObjectOfType<ProjectileSystem>()).AsSingle().NonLazy();
        Container.Bind<UISystem>().FromInstance(FindObjectOfType<UISystem>()).AsSingle().NonLazy();
        Container.Bind<VFXSystem>().FromInstance(FindObjectOfType<VFXSystem>()).AsSingle().NonLazy();
        Container.Bind<SoundSystem>().FromInstance(FindObjectOfType<SoundSystem>()).AsSingle().NonLazy();
        Container.Bind<PlayerSpawnPosition>().FromInstance(FindObjectOfType<PlayerSpawnPosition>()).AsSingle().NonLazy();

        // Object Factories.
        Container.BindFactory<Player, Player.Factory>().FromComponentInNewPrefab(_playerPrefab).AsSingle();
        Container.BindFactory<Enemy, Enemy.Factory>().FromComponentInNewPrefab(_enemyPrefab).AsTransient();
        Container.BindFactory<Projectile, Projectile.Factory>().FromComponentInNewPrefab(_projectilePrefab).AsTransient();
        Container.BindFactory<VFXObject, VFXObject.Factory>().FromComponentInNewPrefab(_explosionPrefab).AsTransient();
    }
}