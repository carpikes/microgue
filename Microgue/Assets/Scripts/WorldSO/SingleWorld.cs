using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "World", menuName ="Assets/Create/SingleWorld", order = 1)]
public class SingleWorld : ScriptableObject {
    [Header("Levels")]
    public string mWorldName = "";
    public string mWorldAssetPrefix = "debug";
    public string mBossRoomName = "debug_boss";
    public string mStartRoomName = "debug_start";

    [Header("Music & SFX")]
    public string mMusicSnapshotPath;
    public string mAmbiencePath;

    [Header("World layout")]
    public int mMinRooms = 3;
    public int mMaxRooms = 7;
    public int mTimeInSeconds = 60 * 5;
}
