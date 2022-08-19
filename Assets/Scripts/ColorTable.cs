using System.Collections.Generic;
using UnityEngine;

public class ColorTable : MonoBehaviour {
    [field: SerializeField] public Color[] Colors { get; private set; }

    public static ColorTable Instance { get; private set; }

    private void Awake() {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<int> GetColorKeys() {
        List<int> list = new();
        for(int i = 0; i < Colors.Length; i++) {
            list.Add(i);
        }
        return list;
    }
}