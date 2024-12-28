using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LavelData", order = 1)]
public class LevelDataScriptable : ScriptableObject
{
    public List<LevelData> levels;
}
