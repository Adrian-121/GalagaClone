using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    public enum TypeEnum {
        TOP_CENTER = 1,
        TOP_LEFT = 2,
        TOP_RIGHT = 3,

        LEFT_TOP = 10,
        LEFT_CENTER = 11,
        LEFT_BOTTOM = 12,

        RIGHT_TOP = 20,
        RIGHT_CENTER = 21,
        RIGHT_BOTTOM = 22
    }

    [SerializeField] private TypeEnum _type;
    public TypeEnum Type => _type;

}