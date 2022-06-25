using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "SO/GameConfig", order = 1)]

public class GameConfig : ScriptableObject {

    public EnemyConfig DefaultEnemyConfig;
    public List<EnemyConfig> EnemyConfigList;

    public EnemyConfig GetEnemyConfigByType(EnemyMainController.TypeEnum type) {
        foreach (EnemyConfig config in EnemyConfigList) {
            if (config.type == type) {
                return config;
            }
        }

        return DefaultEnemyConfig;
    }

    [System.Serializable]
    public class EnemyConfig {
        public EnemyMainController.TypeEnum type;
        public int hp;
        public float speed;
        public Animator sprite1;
    }

}