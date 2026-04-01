using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FlagDatabase", menuName = "Memory Game/Flag Database")]
public class FlagDatabase : ScriptableObject
{
    public List<FlagData> easyFlags;
    public List<FlagData> mediumFlags;
    public List<FlagData> hardFlags;
}