using UnityEngine;

public class ResourceLoader : MonoBehaviour {

    [SerializeField] private GameConfig _gameConfig;
    public GameConfig GameConfig => _gameConfig;

    public FormationPatternResource GetFormation(string name) {
        TextAsset rawResource = Resources.Load(Constants.FORMATIONS_FOLDER + name) as TextAsset;
        return ParseFormationResource(rawResource);
    }

    public MovementPatternResource.MovementPatternContainer GetMovementPatterns() {
        TextAsset rawResource = Resources.Load(Constants.MOVEMENT_PATTERNS) as TextAsset;

        MovementPatternResource.MovementPatternContainer resource = 
            JsonUtility.FromJson<MovementPatternResource.MovementPatternContainer>(rawResource.text);

        return resource;
    }

    public LevelResource GetLevel(string levelName) {
        TextAsset rawResource = Resources.Load(Constants.LEVELS_FOLDER + levelName) as TextAsset;
        if (rawResource == null) { return null; }

        LevelResource resource = JsonUtility.FromJson<LevelResource>(rawResource.text);
        return resource;
    }

    public HighscoreResource GetHighscores() {
        string rawHighscores = PlayerPrefs.GetString(Constants.HIGHSCORES_PREFS, JsonUtility.ToJson(new HighscoreResource()));
        HighscoreResource highscores = JsonUtility.FromJson<HighscoreResource>(rawHighscores);
        return highscores;
    }

    public void SetHighscores(HighscoreResource highscores) {
        PlayerPrefs.SetString(Constants.HIGHSCORES_PREFS, JsonUtility.ToJson(highscores));
    }

    private FormationPatternResource ParseFormationResource(TextAsset textFile) {
        int maxRows = int.MinValue;
        int maxColumns = int.MinValue;
        int usedSlotNumbers = 0;

        string[] lines = textFile.text.Split("\n");

        // Determine the maxRows / maxColumns of the CSV file.
        ComputeCSVSize(lines, out maxRows, out maxColumns);

        // Parse the CSV file and store the values.
        int[][] resultValues = new int[maxRows][];

        for (int i = 0; i < lines.Length; i++) {
            string[] values = lines[i].Split(",");
            resultValues[i] = new int[maxColumns];
            
            for (int j = 0; j < values.Length; j++){
                int parsedInt;

                resultValues[i][j] = 0;
                if (int.TryParse(values[j], out parsedInt)) {
                    resultValues[i][j] = parsedInt;
                    usedSlotNumbers++;
                }
            }
        }

        return new FormationPatternResource(maxRows, maxColumns, usedSlotNumbers, resultValues);
    }

    private void ComputeCSVSize(string[] lines, out int maxRows, out int maxColumns) {
        maxColumns = int.MinValue;
        maxRows = lines.Length;

        foreach (string line in lines) {
            string[] values = line.Split(",");
            if (values.Length > maxColumns) {
                maxColumns = values.Length;
            }
        }
    }
}