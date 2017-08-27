using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStage : MonoBehaviour {

    public float TOP_MARGIN = -0.3f;
    public int INITIAL_ROW_COUNT = 4;
    public float DEADLINE_POS = -7f;

    public enum MoveType
    {
        Step,
        Smooth
    }
    public MoveType moveType;

    float rowOffset;
    bool isAttached;

    public float gameSpeed;
    public float rowSpacing;
    public bool isGameOver;

    public GameObject rowPrefab;
    public Transform player;
    public Transform holdingGem;
    public int holdingType;
    public LinkedList<Transform> stoneRow; 

    public void PlayerAction(int index)
    {
        if (!isAttached)
        {
            //Get the gem
            LinkedListNode<Transform> row = stoneRow.Last;
            do
            {
                if (row.Value.GetComponent<StoneRow>().type[index] > -1)
                {
                    holdingType = row.Value.GetComponent<StoneRow>().GetTypeAt(index);
                    holdingGem = row.Value.GetComponent<StoneRow>().GetGemAt(index);
                    holdingGem.SetParent(player);
                    holdingGem.localPosition = Vector3.zero;
                    break;
                }

                if (row.Previous==null) //get the row that doesn't exist
                {
                    return; //no action
                }
                row = row.Previous;
            } while (row != null);
            
            isAttached = true;
        }
        else
        {
            //Release the gem
            ReleaseGemAt(index);
          
        }
    }

    void ReleaseGemAt(int index)
    {
        LinkedListNode<Transform> row = stoneRow.Last;
        do
        {
            if (row.Value.GetComponent<StoneRow>().type[index] > -1)
            {
                if(row.Value.GetComponent<StoneRow>().type[index] == holdingType)
                {
                    //recursive check
                    Destroy(holdingGem.gameObject);
                    ExplodeGem(index, row);
                    holdingType = -1;
                }
                else
                {
                    if (row.Next == null)
                    {
                        AddLast();
                    }
                    holdingGem.SetParent(row.Next.Value);
                    row.Next.Value.GetComponent<StoneRow>().type[index] = holdingType;
                    row.Next.Value.GetComponent<StoneRow>().gem[index] = holdingGem.gameObject;
                    holdingGem.localPosition = row.Next.Value.GetComponent<StoneRow>().position[index];
                    holdingType = -1;
                }
                break;
            }
            if(row.Previous == null) //upper row doesn't exist
            {
                holdingGem.SetParent(row.Value);
                row.Value.GetComponent<StoneRow>().type[index] = holdingType;
                row.Value.GetComponent<StoneRow>().gem[index] = holdingGem.gameObject;
                holdingGem.localPosition = row.Value.GetComponent<StoneRow>().position[index];
                holdingType = -1;
            }
            row = row.Previous;
        } while (row != null);
        isAttached = false;
    }

    void ExplodeGem(int index, LinkedListNode<Transform> row)
    {
        row.Value.GetComponent<StoneRow>().type[index] = -1;
        Destroy(row.Value.GetComponent<StoneRow>().GetGemAt(index).gameObject);
        row = row.Previous;
        if (row == null) return;
        if (row.Value.GetComponent<StoneRow>().type[index] == holdingType)
        {
            //recursive check
            ExplodeGem(index, row);
        }
    }

    void Deadline(Transform row)
    {
        for (int i = 0; i < row.GetComponent<StoneRow>().type.Length; i++)
        {
            if (row.GetComponent<StoneRow>().type[i] > -1)
            {
                isGameOver = true;
                Debug.Log("Game Over");
                return;
            }
        }
        Destroy(stoneRow.Last.Value.gameObject);
        stoneRow.RemoveLast();
    }

    void AddFirst()
    {
        GameObject row = Instantiate(rowPrefab, transform.position, Quaternion.identity) as GameObject;
        row.transform.SetParent(transform);
        stoneRow.AddFirst(row.transform);
        row.GetComponent<StoneRow>().FillRow();
    }

    void AddLast()
    {
        GameObject row = Instantiate(rowPrefab, transform.position, Quaternion.identity) as GameObject;
        row.transform.SetParent(transform);
        stoneRow.AddLast(row.transform);
        row.GetComponent<StoneRow>().EmptyRow();
    }

    void Initialize()
    {
        isGameOver = false;
        holdingType = -1;
        stoneRow = new LinkedList<Transform>();
        for (int i = 0; i < INITIAL_ROW_COUNT; i++)
        {
            AddFirst();
        }
    }

    void MoveRowDown()
    {
        rowOffset += gameSpeed * Time.deltaTime;
        if (rowOffset > TOP_MARGIN + rowSpacing)
        {
            rowOffset = TOP_MARGIN;
            AddFirst();
        }

        //Adjust Spacing
        LinkedListNode<Transform> row = stoneRow.First;
        int count = 0;
        do
        {
            //TOP_MARGIN = step move down
            //rowOffset = smooth move down
            float moveValue = 0;
            switch (moveType) {
                case MoveType.Step :
                    moveValue = TOP_MARGIN;
                    break;
                case MoveType.Smooth:
                    moveValue = rowOffset - (rowSpacing * 1);
                    break;
                default:
                    break;
            }

            row.Value.localPosition = Vector3.down * (moveValue + (rowSpacing * count));
            if (row.Value.position.y < DEADLINE_POS)
            {
                Deadline(row.Value);
                break;
            }
            row = row.Next;
            count++;
        } while (row != null);
        count = 0;
    }

    void Start()
    {
        Initialize();
        rowOffset = TOP_MARGIN;
    }

	void Update () {
        if(!isGameOver) MoveRowDown();
	}
}
