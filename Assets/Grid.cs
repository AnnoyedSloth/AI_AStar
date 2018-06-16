// Created by Soohwan Park
// sksk31052@hanmail.net
// Hongik University Artificial Intelligence Final Project
// A* Algorithm

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Grid : MonoBehaviour
{
    [SerializeField] GameObject selectedObj;

    [SerializeField] private int sizeOfField;
    [SerializeField] private int gridSize;
    private int numOfGrid;
    [SerializeField] private bool[] accessible;

    public Text gridText;
    public Text curNodeText;

    int departPoint;
    int destPoint;

    [SerializeField] private List<int> openList;
    [SerializeField] private List<int> closedList;

    private int[] gCost;
    private int[] hCost;
    private int[] fCost;

    // Use this for initialization
    void Start()
    {
        // Setting up initial field options
        numOfGrid = sizeOfField / gridSize;

        departPoint = -1;
        destPoint = -1;

        openList = new List<int>();
        closedList = new List<int>();


        // Initialize transform values
        this.transform.localScale = new Vector3(sizeOfField, 1, sizeOfField);
        this.transform.localPosition = new Vector3(sizeOfField / 2, -.5f, sizeOfField / 2);
        this.transform.localRotation = Quaternion.identity;

        // Drawing grids as much as I set above
        for (int a = 0; a < numOfGrid; a++)
        {
            Debug.DrawLine(new Vector3(a * gridSize, 0, 0), new Vector3(a * gridSize, 0, sizeOfField), Color.green, Mathf.Infinity);
            Debug.DrawLine(new Vector3(0, 0, a * gridSize), new Vector3(sizeOfField, 0, a * gridSize), Color.green, Mathf.Infinity);
        }

        gCost = new int[numOfGrid * numOfGrid];
        hCost = new int[numOfGrid * numOfGrid];
        fCost = new int[numOfGrid * numOfGrid];

        accessible = new bool[numOfGrid * numOfGrid];

        for (int a = 0; a < numOfGrid * numOfGrid; a++)
        {
            gCost[a] = -1;
            hCost[a] = -1;
            fCost[a] = -1;

            if (Grid2Vector(a).y > .5f) accessible[a] = false;
            else accessible[a] = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                departPoint = Vector2Grid(selectedObj.transform.position);
                destPoint = Vector2Grid(hit.point);

                DrawCheckLine(destPoint, Color.blue);
                FindPath(departPoint, destPoint);

                //GetNeighbor(Vector2Grid(hit.point));
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                gridText.text = Vector2Grid(hit.point).ToString();
                print("GridNum : " + Vector2Grid(hit.point));
                print("Gcost : " + gCost[Vector2Grid(hit.point)]);
                print("Hcost : " + hCost[Vector2Grid(hit.point)]);
                print("Fcost : " + fCost[Vector2Grid(hit.point)]);
            }

        }
    }

    List<int> GetNeighbor(int gridNum)
    {
        List<int> neighbors = new List<int>() { -1, 1, -numOfGrid, numOfGrid, -numOfGrid - 1, -numOfGrid + 1, numOfGrid - 1, numOfGrid + 1 };
        //string str = "";

        if (gridNum % numOfGrid == 0) neighbors.RemoveAll((num) => { return num == -1 || num == -1 - numOfGrid || num == -1 + numOfGrid; });
        if (gridNum % numOfGrid == numOfGrid - 1) neighbors.RemoveAll((num) => { return num == 1 || num == 1 - numOfGrid || num == 1 + numOfGrid; });
        if (gridNum / numOfGrid == 0) neighbors.RemoveAll((num) => { return num == -numOfGrid || num == -1 - numOfGrid || num == -numOfGrid + 1; });
        if (gridNum / numOfGrid == numOfGrid - 1) neighbors.RemoveAll((num) => { return num == numOfGrid || num == numOfGrid - 1 || num == numOfGrid + 1; });

        for (int a = 0; a < neighbors.Count; a++)
        {
            neighbors[a] += gridNum;
            //str = str + neighbors[a].ToString() + ", ";
        }
        //print(str);
        return neighbors;
    }

    // Find out distance between two grids
    int GridDistance(int gridA, int gridB)
    {
        // To find diagonal distance, Got to be compared which row or column is bigger
        int h = Mathf.Abs(gridB / numOfGrid - gridA / numOfGrid);
        int w = Mathf.Abs(gridB % numOfGrid - gridA % numOfGrid);

        if (h > w) return w * 14 + (h - w) * 10;
        else return h * 14 + (w - h) * 10;        
    }

    // Convert Vector3 position to Grid number
    int Vector2Grid(Vector3 vector)
    {
        return (((int)vector.x / gridSize) + numOfGrid * ((int)vector.z / gridSize));
    }

    // Convert Grid number to Vector3 position
    Vector3 Grid2Vector(int grid)
    {
        return new Vector3((grid % (numOfGrid)) * gridSize + gridSize / 2, .5f, (grid / (numOfGrid) * gridSize + gridSize / 2));
    }

    void FindCost(int gridNum)
    {
        gCost[gridNum] = GridDistance(departPoint, gridNum);
        hCost[gridNum] = GridDistance(gridNum, destPoint);
        fCost[gridNum] = gCost[gridNum] + hCost[gridNum];
    }

    void DrawCheckLine(int grid, Color color)
    {
        Debug.DrawLine(Grid2Vector(grid) + new Vector3(-gridSize/2, -.5f, -gridSize/2), Grid2Vector(grid) + new Vector3(gridSize / 2, -.5f, gridSize / 2), color, Mathf.Infinity);
    }

    int SmallestGridNode(List<int> list)
    {
        int smallest = fCost[list[0]];
        int index = 0;

        for(int a=0; a<list.Count; a++)
        {
            if (fCost[list[a]] < smallest)
            {
                //print("FCost" + fCost[list[a]]);
                smallest = fCost[list[a]];
                index = list[a];
            }
            //print("fCost[list[" + index + "]]");
        }
        openList.Remove(index);

        foreach(int neighbor in GetNeighbor(index))
        {
            //print("neighbor" + neighbor);
            //print("fCost[neighbor] : " + fCost[neighbor]);
            if (fCost[neighbor] == -1 && neighbor != departPoint)// && !closedList.Contains(neighbor))
            {
                openList.Add(neighbor);
                FindCost(neighbor);
                DrawCheckLine(neighbor, Color.red);
            }
        }
        return index;
    }

    void FindPath(int depart, int dest)
    {
        int curGrid = -1;

        openList.Clear();
        closedList.Clear();
        
        openList.AddRange(GetNeighbor(depart));
        foreach (int nList in openList)
        {
            if(fCost[nList] == -1) FindCost(nList);
            DrawCheckLine(nList, Color.red);
        }

        for (int a = 0; a < 10; a++)
        {
            //closedList.Add(openList.BinarySearch(openList.Min(x => fCost[x])));
            //openList.Remove(openList.Min());

            StartCoroutine(FindingPathCoroutine());
            //Also this one does remove the node which having smallest fCost from openList

        }
    }

    IEnumerator FindingPathCoroutine()
    {
        int curNode = 0;
        while (curNode != destPoint)
        {
            curNode = SmallestGridNode(openList);
            curNodeText.text = curNode.ToString();
            closedList.Add(curNode);
            yield return new WaitForSeconds(.01f);
        }
    }

}