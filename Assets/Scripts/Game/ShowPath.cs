using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms;

public class ShowPath : MonoBehaviour {

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Map map;

    PathFinderFast pathFinder;
    List<Vector2i> mPath;
    Vector2i startPos;


    private void Start()
    {
        InitPathFinder();
        startPos = new Vector2i(17, 5);
        mPath = new List<Vector2i>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mousePosition.x);
            int y = Mathf.FloorToInt(mousePosition.y);
            TappedOnTile(new Vector2i(x, y));
        }
    }

    private void InitPathFinder() {
        pathFinder = new PathFinderFast(map.ByteGrid);
        pathFinder.Formula = HeuristicFormula.Manhattan;
        //if false then diagonal movement will be prohibited
        pathFinder.Diagonals = false;
        //if true then diagonal movement will have higher cost
        pathFinder.HeavyDiagonals = false;
        //estimate of path length
        pathFinder.HeuristicEstimate = 6;
        pathFinder.PunishChangeDirection = false;
        pathFinder.TieBreaker = false;
        pathFinder.SearchLimit = 10000;
        pathFinder.DebugProgress = false;
        pathFinder.DebugFoundPath = false;
    }

    public void TappedOnTile(Vector2i tilePosition) {
        OnFoundPath(pathFinder.FindPath(startPos, tilePosition, 1, 2, 5));
    }

    public void OnFoundPath(List<Vector2i> path)
    {       
        mPath.Clear();

        if (path != null && path.Count > 1)
        {
            for (var i = path.Count - 1; i >= 0; --i)
                mPath.Add(path[i]);           
        }       
    }

    void OnDrawGizmos()
    {       
        //draw the path

        if (mPath != null && mPath.Count > 0)
        {
            var start = mPath[0];

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(map.transform.position + new Vector3(start.x + 0.5f, start.y + 0.5f, -5.0f), 0.3f);

            for (var i = 1; i < mPath.Count; ++i)
            {
                var end = mPath[i];
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(map.transform.position + new Vector3(end.x + 0.5f, end.y + 0.5f, -5.0f), 0.3f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(map.transform.position + new Vector3(start.x + 0.5f, start.y + 0.5f, -5.0f),
                                map.transform.position + new Vector3(end.x + 0.5f, end.y + 0.5f, -5.0f));
                start = end;
            }
        }
    }
}
