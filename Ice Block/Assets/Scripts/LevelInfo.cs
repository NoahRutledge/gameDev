using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelInfo : MonoBehaviour{

    public static LevelInfo S;

    public int maxLevelParse = 10;
    public TextAsset levelData;
    public List<Level> levels;

    public void Awake()
    {
        S = this;
    }

    public void Init()
    {
        StartCoroutine(ParseLevelData());
    }

    static public void INIT()
    {
        S.Init();
    }

    public IEnumerator ParseLevelData()
    {
        string[] lines = levelData.text.Split('\n');
        //string[] lines = File.ReadAllLines(Path.Combine(Application.persistentDataPath, "LevelData.txt"));
        int index = 1;
        int parsed = 0;

        for(int a = 0; a < int.Parse(lines[0]); a++)
        {
            if (a % maxLevelParse == 0)
            {
                yield return null;
            }
            Level l = new Level();
            l.level = int.Parse(lines[index]);
            index++;

            string line = lines[index];

            string[] row = line.Split(' ');

            l.size = row.Length;
            l.field = new int[l.size, l.size];
            for (int i = 0; i < l.size; i++)
            {
                for (int j = 0; j < l.size; j++)
                {
                    if (row[j] == "S")
                    {
                        l.start = new Vector2(j, i);
                        l.field[j, Mathf.Abs(l.size - 1 - i)] = 1;
                    }
                    else if (row[j] == "E")
                    {
                        l.end = new Vector2(j, i);
                        l.field[j, Mathf.Abs(l.size - 1 - i)] = 1;
                    }
                    else
                    {
                        l.field[j, Mathf.Abs(l.size - 1 - i)] = int.Parse(row[j]);
                    }
                }
                index++;
                if(index != lines.Length) row = lines[index].Split(' ');
            }
            index++;
            levels.Add(l);
            parsed++;
        }
        gameObject.SendMessage("InitDone");
    }

    public Level getLevel(int n)
    {
        return levels[n-1];
    }
}