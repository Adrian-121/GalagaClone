using System.Collections.Generic;

[System.Serializable]
public class MovementPatternResource {

    [System.Serializable]
    public class MovementPatternContainer {
        public List<MovementPatternResource> Patterns;
    }

    [System.Serializable]
    public class Sequence {
        public float SpeedMultiplier;
        public float TimeInSeconds;
        public float AngleInDegrees;
    }

    public string Name;
    public int Spawner;
    public float InitialRotation;
    public List<Sequence> SequenceList = new List<Sequence>();

}