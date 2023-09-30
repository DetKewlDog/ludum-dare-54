using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LevelManager.GetFarm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
