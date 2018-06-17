using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPlay : MonoBehaviour {

    NavMeshPath navPath;
    private float elapsed = 0.0f;

    // Use this for initialization
    void Start () {
        elapsed = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.CompareTag("NavMesh"))
            {
                navPath = new NavMeshPath();

                NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, navPath);

                for (int i = 0; i < navPath.corners.Length - 1; i++)
                    Debug.DrawLine(navPath.corners[i], navPath.corners[i + 1], Color.red, Mathf.Infinity);

            }
        }


    }
}
