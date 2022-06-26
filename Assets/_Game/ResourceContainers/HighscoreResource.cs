using System.Collections.Generic;

[System.Serializable]
public class HighscoreResource {

    private const int MAX_HIGHSCORES = 3;

    [System.Serializable]
    public class Highscore {
        public int Score;
        public string Name;
    }

    public List<Highscore> HighscoreList = new List<Highscore>();

    public void AddHighscore(Highscore newScore) {
        // If the top 3 is not filled, fill here.
        if (HighscoreList.Count < MAX_HIGHSCORES) {
            HighscoreList.Add(newScore);
            return;
        }

        // If it's filled, check to replace the lowest score, if any.
        int minHighscore = int.MaxValue;
        int minHighscoreIndex = -1;
        
        for (int i = 0; i < HighscoreList.Count; i++) {
            if (HighscoreList[i].Score < minHighscore) {
                minHighscore = HighscoreList[i].Score;
                minHighscoreIndex = i;
            }
        }

        // Replace the minimum score with this one
        if (newScore.Score > minHighscore && minHighscoreIndex >= 0) {
            HighscoreList[minHighscoreIndex].Score = newScore.Score;
            HighscoreList[minHighscoreIndex].Name = newScore.Name;
        }
    }

    public int GetTop() {
        int max = 0;

        for (int i = 0; i < HighscoreList.Count; i++) {
            if (HighscoreList[i].Score > max) {
                max = HighscoreList[i].Score;
            }
        }

        return max;
    }

    public List<Highscore> GetHighscoreList() {
        HighscoreList.Sort(CompareHighscores);
        return HighscoreList;
    }

    private int CompareHighscores(Highscore x, Highscore y) {
        return y.Score - x.Score;
    }
}