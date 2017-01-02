using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "World", menuName ="Assets/Create/SingleWorld", order = 1)]
public class SingleWorld : ScriptableObject {
    public string mWorldName = "";
    public string mWorldAssetPrefix = "debug";
    public string mBossRoomName = "debug_boss";
    public string mStartRoomName = "debug_start";
    public int mMinRooms = 3;
    public int mMaxRooms = 7;
    public int mTimeInSeconds = 60 * 5;
}
