using System.Collections.Generic;

[System.Serializable]
public class LevelResource {

    [System.Serializable]
    public class Sequence {
        public float TimeFromLast;
        public int EnemyType;
        public float Delay;
        public string MovementPatternName;
        public int SpawnCount;
    }

    public string Name;
    public List<Sequence> SequenceList;

}
