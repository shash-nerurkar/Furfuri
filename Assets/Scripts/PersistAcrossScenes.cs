using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistAcrossScenes : MonoBehaviour
{
    public World world;
    public string transitionSceneToLoad;
    public string transitionSceneToDelete;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
