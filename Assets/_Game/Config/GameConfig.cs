using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "SO/GameConfig", order = 1)]

public class GameConfig : ScriptableObject {

    public int PlayerLives = 3;

    public int StartLevel = 1;

    public EnemyConfig DefaultEnemyConfig;
    public List<EnemyConfig> EnemyConfigList;

    public List<FormationMovementConfig> FormationConfigList;

    public EnemyConfig GetEnemyConfigByType(Enemy.TypeEnum type) {
        foreach (EnemyConfig config in EnemyConfigList) {
            if (config.type == type) {
                return config;
            }
        }

        return DefaultEnemyConfig;
    }

    public FormationMovementConfig GetFormationConfig(EnemyFormationMovement.MovementTypeEnum movementType) {
        foreach (FormationMovementConfig config in FormationConfigList) {
            if (config.MovementType == movementType) {
                return config;
            }
        }

        return null;
    }

    [System.Serializable]
    public class EnemyConfig {
        public Enemy.TypeEnum type;

        [Header("Basic")]
        public int hp;
        public float speed;
        public int points;

        [Header("Movement Patterns")]
        public float LungeChange;
        public float LungeCheckTime;

        [Header("Projectile")]
        public float ProjectileChance;
        public float ProjectileCheckTimeout;
    }

    [System.Serializable]
    public class FormationMovementConfig {
        public EnemyFormationMovement.MovementTypeEnum MovementType;
        public float Time;
        public float Speed;
    }

}