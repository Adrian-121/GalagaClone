using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour {

    private EnemyMainController.Factory _enemyFactory;

    private EnemyFormation _formation;

    private List<MovementPatternResource> _movementPatterns;

    [Inject]
    public void Construct(EnemyMainController.Factory enemyFactory, ResourceLoader resourceLoader) {
        _enemyFactory = enemyFactory;

        _formation = GetComponentInChildren<EnemyFormation>();
        FormationResource formationResource = resourceLoader.GetFormation("default_formation");
        _formation.Initialize(formationResource);

        _movementPatterns = resourceLoader.GetMovementPatterns();
    }
    
    public void StartGame() {
        EnemyMainController newEnemy = _enemyFactory.Create();
        newEnemy.Construct(_formation);


        newEnemy.Initialize(Vector3.zero, _movementPatterns[0]);

        //for (int i = 0; i < formationResource.UsedSlotsNumber; i++) {
        //    EnemyMainController en = Instantiate(_TEMPenemyPrefab);
        //    en.Construct(_formation);
        //}
    }


}