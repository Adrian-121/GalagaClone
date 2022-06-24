using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour {

    private const string MOVEMENT_PATTERNS = "movement_patterns";
    private const string FORMATIONS_FOLDER = "Formations/";
    private const string LEVELS_FOLDER = "Levels/";

    public FormationResource GetFormation(string name) {
        TextAsset rawResource = Resources.Load(FORMATIONS_FOLDER + name) as TextAsset;
        return ParseFormationResource(rawResource);
    }

    public List<MovementPatternResource> GetMovementPatterns() {
        TextAsset rawResource = Resources.Load(MOVEMENT_PATTERNS) as TextAsset;

        MovementPatternResource.MovementPatternContainer resource = 
            JsonUtility.FromJson<MovementPatternResource.MovementPatternContainer>(rawResource.text);

        return resource.Patterns;
    }

    public LevelResource GetLevel(string levelName) {
        TextAsset rawResource = Resources.Load(LEVELS_FOLDER + levelName) as TextAsset;
        LevelResource resource = JsonUtility.FromJson<LevelResource>(rawResource.text);

        return resource;
    }

    private FormationResource ParseFormationResource(TextAsset textFile) {
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

        return new FormationResource(maxRows, maxColumns, usedSlotNumbers, resultValues);
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