using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRow : MonoBehaviour {

    public int[] type = new int[5];
    public GameObject[] gem = new GameObject[5];
    [SerializeField]
    GameObject[] gemPrefab = new GameObject[4];
    public Vector3[] position = new Vector3[5];

    public void FillRow()
    {
        for (int i = 0; i < position.Length; i++)
        {
            int rand = Random.Range(0, 4);
            GameObject child = Instantiate(gemPrefab[rand], transform) as GameObject;
            child.transform.SetParent(transform);
            child.transform.localPosition = position[i];
            type[i] = rand;
            gem[i] = child;
        }
    }

    public void EmptyRow()
    {
        for(int i=0; i<type.Length; i++)
        {
            type[i] = -1;
        }
    }

    public Transform GetGemAt(int index)
    {
        Transform temp = gem[index].transform;
        gem[index] = null;
        return temp;
    }

    public int GetTypeAt(int index)
    {
        int temp = type[index];
        type[index] = -1;
        return temp;
    }

	void Start () {

	}
}
