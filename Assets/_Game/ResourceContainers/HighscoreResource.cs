using System.Collections.Generic;

[System.Serializable]
public class HighscoreResource {

    private const int MAX_HIGHSCORES = 3;

    [System.Serializable]
    public class Highscore {
        public int Score;
        public string Name;
    }

    private List<Highscore> _highscoreList = new List<Highscore>();
    public List<Highscore> HighscoreList {
        get {
            _highscoreList.Sort(CompareHighscores);
            return _highscoreList;
        }
    }

    public void AddHighscore(Highscore newScore) {
        int minHighscore = int.MaxValue;
        int minHighscoreIndex = -1;
        
        for (int i = 0; i < _highscoreList.Count; i++) {
            if (_highscoreList[i].Score < minHighscore) {
                minHighscore = _highscoreList[i].Score;
                minHighscoreIndex = i;
            }
        }

        if (newScore.Score > minHighscore && minHighscoreIndex >= 0) {
            _highscoreList[minHighscoreIndex].Score = newScore.Score;
            _highscoreList[minHighscoreIndex].Name = newScore.Name;
        }
    }

    private int CompareHighscores(Highscore x, Highscore y) {
        return x.Score - y.Score;
    }
}