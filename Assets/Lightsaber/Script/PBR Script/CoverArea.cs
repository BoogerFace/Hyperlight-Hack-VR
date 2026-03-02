using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverArea : MonoBehaviour
{
    private Cover[] covers;

    private void Awake()
    {
        covers = GetComponentsInChildren<Cover>();
    }

    public Cover GetRandomCover()
    {
        return covers[Random.Range(0, covers.Length - 1)];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
