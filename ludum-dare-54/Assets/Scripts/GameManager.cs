using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LevelManager.Instance.GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }
}