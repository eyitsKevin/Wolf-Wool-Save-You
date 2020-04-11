using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    public Tilemap walkableTilemap;
    public Vector3Int[,] tilemapsPosition;
    BoundsInt bounds;
    Camera camera;

    private static GridManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }

    private void Start()
    {
        walkableTilemap.CompressBounds();
        bounds = walkableTilemap.cellBounds;
        camera = Camera.main;
        PopulateTile();
        print(walkableTilemap);
    }


    private void PopulateTile()
    {
        tilemapsPosition = new Vector3Int[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (walkableTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    tilemapsPosition[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    tilemapsPosition[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 world = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridpos = walkableTilemap.WorldToCell(world);
            print(walkableTilemap.GetTile(gridpos));
        }
    }

    public bool isWalkableTile(Vector2 coords)
    {
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (coords.x == x && coords.y == y && walkableTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static GridManager Instance
    {
        get { return instance; }
    }

    public TileBase GetTile(Vector3Int gridPos)
    {
        return walkableTilemap.GetTile(gridPos);
    }
}
