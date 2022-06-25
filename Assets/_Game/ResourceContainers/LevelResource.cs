using System.Collections.Generic;

[System.Serializable]
public class LevelResource {

    [System.Serializable]
    public class Sequence {
        public float Time;
        public int EnemyType;
        public float Delay;
        public string MovementPatternName;
        public int SpawnCount;

        public float _cooldown;
        public int _spawned;

        public void Reset() {
            _cooldown = 0;
            _spawned = 0;
        }
    }

    public string Name;
    public List<Sequence> SequenceList;

}
