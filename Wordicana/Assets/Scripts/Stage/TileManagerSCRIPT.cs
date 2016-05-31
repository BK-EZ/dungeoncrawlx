
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManagerSCRIPT : MonoBehaviour
{
    private List<TileSCRIPT> obstacleTiles = new List<TileSCRIPT>();

    public Transform TilePrefab; // Prefab that the script spawns- this variable is set in the Unity inspector
    private Transform tile_TF; 
    private float tileWidth = 3.15f;
    private float tileHeight = 2.425f;
    public float tileScale = .20f;

    public int originX = 1;
    public int originY = 1;

    public bool giveBorder = true;

    public bool chainLabelsOn = false;
    public bool coordinateLabelsOn = false;

    public bool targetTileFound = false;

    public int testRange = 2;
    public int fontSize = 1;
    
    public int rows = 30;
    public int cols = 30;

    public int removeLoneObsChance = 75;
    public int removeDoubleObsChance = 50;
    public int endChainChance = 10;
    public int skipChainChance = 10;
    public int startChainChance = 10;
    public int changeChainDirChance = 10;
    public int sideCheckChance = 50;
    public int sidesToCheck = 3;
    public bool straightUpWalls = true;

    public int numPathTries1 = 10;
    public int numPathTries2 = 50;
    private int numPathTries3;

    public bool useOldAlg = false;

    public int numPurges = 1;
    public Transform purgePrefab; // Prefab that the script spawns- this variable is set in the Unity inspector
    public Transform evilPurgePrefab; // Prefab that the script spawns- this variable is set in the Unity inspector

    private List<TileUnitSCRIPT> units = new List<TileUnitSCRIPT>();
    Camera camScript;


    public TileSCRIPT[,] tileArray;

    public TileSCRIPT selectedTile;
    public TileUnitSCRIPT selectedUnit;

    void Start() // Use this for initialization
    {
        numPathTries3 = rows * cols;
        camScript = gameObject.GetComponent<Camera>();
        tileArray = new TileSCRIPT[cols+1, rows];

        float tileW = tileWidth * tileScale;
        float tileH = tileHeight * tileScale;

        int colsNeeded;

        float xPosDef = -12f;
        float yPosDef = 4.85f;

        float xPos;
        float yPos;

        bool inInnerRow = true;

        yPos = yPosDef;

        for(int i = 0; i < rows; i++)
        {
            if(inInnerRow) 
            {
                xPos = xPosDef;
                colsNeeded = cols;
            }
            else // non-inner-rows have one extra col and start more to the left
            {
                xPos = xPosDef - (tileW / 2f);
                colsNeeded = cols + 1;
            }

            for(int j = 0; j < colsNeeded; j++)
            {
                // make and place tile
                tile_TF = Instantiate(TilePrefab) as Transform;
                tile_TF.position = new Vector3(xPos, yPos, 0f);

                TileSCRIPT ts = tile_TF.GetComponent<TileSCRIPT>();
                tileArray[j, i] = ts; //add tile to 2d array

                // set properties of ts
                ts.managerScript = this;               
                ts.row = i;
                ts.col = j;

                xPos += tileW;
            } // end cols forloop

            yPos -= tileH;
            inInnerRow = !inInnerRow;
        } // end rows forloop

        linkTiles();
        createBoard();
        spawnPurges();
    }

    private void spawnPurges()
    {
        for(int i = 0; i < 2; i++)
        {
            var purge_TF = Instantiate(purgePrefab) as Transform; // create the enemy

            Vector3 tileCord = tileArray[i+1, 1].gameObject.transform.position;
            purge_TF.position = new Vector3(tileCord.x, tileCord.y, -10f);

            TileUnitSCRIPT tus = purge_TF.GetComponent<TileUnitSCRIPT>();
            tus.tile = tileArray[i+1, 1];
            tus.managerScript = this;
            tus.tile.unit = tus;
            units.Add(tus);
        }

        for (int i = 2; i < 4; i++)
        {
            var evil_purge_TF = Instantiate(evilPurgePrefab) as Transform; // create the enemy

            Vector3 tileCord = tileArray[i + 1, 1].gameObject.transform.position;
            evil_purge_TF.position = new Vector3(tileCord.x, tileCord.y, -10f);

            TileUnitSCRIPT tus = evil_purge_TF.GetComponent<TileUnitSCRIPT>();
            tus.tile = tileArray[i + 1, 1];
            tus.managerScript = this;
            tus.tile.unit = tus;
            units.Add(tus);
        }


    }

    private void createBoard()
    {
        setBorders();
        setObstacles();
        removeLoneObstacles();
        removeDoubleObstacles();
    }

    private void setBorders()
    {        
        if(giveBorder == false)
        {
            return;
        }

        foreach(TileSCRIPT ts in tileArray)
        {
            if(ts != null && ts.canBeBorder())
            {
                ts.setAsBorder();
            }
        }
    }

    private void setObstacles()
    {
        List<TileSCRIPT> list = generateRandomTileList();
        foreach(TileSCRIPT ts in list)
        {
            if(ts == null)
            {
                continue;
            }

            int rnd = Random.Range(1, 101);
            if (rnd <= startChainChance) // start new obstacle chain
            {
                beginObstacleChain(ts);
            }
        }
    }

    private void beginObstacleChain(TileSCRIPT ts)
    {
        int direction = Random.Range(0, 6); // random direction
        ts.chainStarter = true;

        while (ts != null)
        {
            // try to change chain direction
            int rnd = Random.Range(1, 101);
            if (rnd <= changeChainDirChance)
            {
                direction = Random.Range(0, 6);
            }

            rnd = Random.Range(0, 101);
            if(rnd <= 25 && straightUpWalls)
            {
                if (direction == 0)
                    direction = 1;
                else if (direction == 1)
                    direction = 0;
                else if (direction == 3)
                    direction = 4;
                else if (direction == 4)
                    direction = 3;
            }

            // place obstacle- if return val is 2, then chain ends
            int returnVal = placeObstacle(ts);
            if (returnVal == 2)
            {
                return;
            }

            // repeat loop with new tile
            ts = ts.getSide(direction);

        } // end while(ts != null)       
    }

    private void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private List<TileSCRIPT> generateRandomTileList()
    {
        List<TileSCRIPT> list = new List<TileSCRIPT>();

        foreach(TileSCRIPT ts in tileArray)
        {
            if(ts != null)
            {
                list.Add(ts);
            }
        }

        Shuffle(list);

        return list;
    }

    private int placeObstacle(TileSCRIPT ts)
    {

        if (ts.row == originX && ts.col == originY) // dont block first tile
        {
            return -1;
        }
        else if (ts.isObstacle)
        {
            return -2;
        }
        else if (ts.isBorder)
        {
            return -3;
        }


        int rnd = Random.Range(1, 101);

        if (rnd <= skipChainChance) // skip chain
        {
            return 1;
        }
        else if (rnd <= endChainChance + skipChainChance) // end chain
        {
            return 2;
        }
        else // continue chain
        {
            if (ts.okForObstacle() == true)
            {
                ts.setAsObstacle();
                obstacleTiles.Add(ts);
                return 3;
            }
            return 4;
        }
    }

    void removeLoneObstacles()
    {
        foreach (TileSCRIPT ts in tileArray)
        {
            int rnd = Random.Range(1, 101);
            if(rnd <= removeLoneObsChance)
            {
                if(ts != null && ts.isBorder == false && ts.isObstacle == true && ts.openSides() == 6)
                {
                    ts.removeObstacle();
                }
            }
        }
    }

    void removeDoubleObstacles()
    {
        foreach (TileSCRIPT ts in tileArray)
        {
            int rnd = Random.Range(1, 101);
            if (rnd <= removeDoubleObsChance)
            {
                if (ts != null && ts.isBorder == false && ts.isObstacle == true && ts.openSides() == 5) // if an obstacle is connected to only one obstacle...
                {
                    for(int dir = 0; dir < 6; dir++)
                    {
                        TileSCRIPT tsConnected = ts.getSide(dir);
                        // if tsConnected is the one connected obstacle AND it also only is connected to one obstacle ...
                        if (tsConnected != null && tsConnected.isObstacle == true && tsConnected.isBorder == false && tsConnected.openSides() == 5) 
                        {
                            //remove both obstacles
                            ts.removeObstacle();
                            tsConnected.removeObstacle();
                        }
                    }
                }
            }
        }
    }

    

    // check if new obstacle tile lacks the number of open adjacent spaces it needs
    private bool minSidesTrigger(TileSCRIPT ts)
    {
        TileSCRIPT[] sides = ts.getSideTiles();
        foreach (TileSCRIPT side in sides)
        {
            if (side != null && side.isObstacle == false && side.openSides() < sidesToCheck)
            {
                return true;
            }
        }
        return false;
    }


    public bool checkObstacleOK(TileSCRIPT ts) // ts is a tile that we just set as an obstacle, and we're checking that its ok like that
    {
        // part 1- check minSidesTrigger
        int rnd = Random.Range(1, 101);
        if (rnd <= sideCheckChance)
        {
            if(minSidesTrigger(ts) == true)
            {
                return false;
            }
        }

        // part 2- use trick to see if things are block
        if (ts.openSides() >= 5 || ts.openSides() == 1) // if there are 1, 5, or 6 open sides, it cant be blocking anything
        {
            return true;
        }

        // old part 3
        if (useOldAlg)
        {
            return checkBlockingOLD(ts);
        }

        // new part 3- make sure no tiles are blocked off
        bool nothingIsBlocked = checkBlocking(ts);

        return nothingIsBlocked;      
    }

    private bool checkBlockingOLD(TileSCRIPT ts) // old search alg- will probably delete soon
    {
        bool iupleftOK = checkPathExists(ts.TopLeft, tileArray[originX, originY]);
        bool iuprightOK = checkPathExists(ts.TopRight, tileArray[originX, originY]);
        bool irightOK = checkPathExists(ts.Right, tileArray[originX, originY]);
        bool ibotrightOK = checkPathExists(ts.BotRight, tileArray[originX, originY]);
        bool ibotleftOK = checkPathExists(ts.BotLeft, tileArray[originX, originY]);
        bool ileftOK = checkPathExists(ts.Left, tileArray[originX, originY]);

        return (iupleftOK && iuprightOK && irightOK && ibotrightOK && ibotleftOK && ileftOK);
    }

    // see if tile is blocking anything, by checking if all adjacent tiles can reach one another
    private bool checkBlocking(TileSCRIPT ts)
    {
        // pick any side to be the test point
        TileSCRIPT testPoint = null;
        for (int dir = 0; dir < 6; dir++)
        {
            TileSCRIPT side = ts.getSide(dir);
            if (side != null && side.isObstacle == false)
            {
                testPoint = side;
                break;
            }
        }
        
        bool upleftOK = checkPathExists(ts.TopLeft, testPoint);
        bool uprightOK = checkPathExists(ts.TopRight, testPoint);
        bool rightOK = checkPathExists(ts.Right, testPoint);
        bool botrightOK = checkPathExists(ts.BotRight, testPoint);
        bool botleftOK = checkPathExists(ts.BotLeft, testPoint);
        bool leftOK = checkPathExists(ts.Left, testPoint);

        return (upleftOK && uprightOK && rightOK && botrightOK && botleftOK && leftOK);
    }

    // my pathfinding is shitty but it "works" and I don't really care about imporiving it myself any more right now
    // returns if a path exists between two tiles or not
    public bool checkPathExists(TileSCRIPT ts1, TileSCRIPT ts2) // ts1 istile to check, ts2 is origin
    {
        if (ts1 == null || ts2 == null || ts1.isObstacle || ts2.isObstacle || (ts1 == ts2))
            return true;

        bool targetFound;

        targetFound = ts1.pathExists(ts2, numPathTries1); // see if a path exists within a range of numPathTries1

        if(targetFound == false) // try again with a bigger range
        {
            targetFound = ts1.pathExists(ts2, numPathTries2);
        }

        if (targetFound == false) // try again with an even bigger range
        {
            targetFound = ts1.pathExists(ts2, numPathTries3);
        }

        targetTileFound = false;
        return targetFound;

    }

    private void linkTiles()
    {
        int rowsToCheck = rows;
        int colsToCheck;
        bool inInnerRow;
        int x, y;

        foreach(TileSCRIPT ts in tileArray)
        {
            if(ts == null)
            {
                continue;
            }

            x = ts.col;
            y = ts.row;

            inInnerRow = false;
            if(ts.row % 2 == 0) // in inner row
            {
                inInnerRow = true;
                colsToCheck = cols;
            }
            else // not in inner row
            {
                colsToCheck = cols + 1;
            }

            // left tile
            if (ts.col != 0) 
            {
                tileArray[x, y].Left = tileArray[x - 1, y];
            }

            //upper-left tile
            if ((y != 0) && (x != 0 || inInnerRow))
            {
                if (inInnerRow)
                {
                    tileArray[x, y].TopLeft = tileArray[x, y - 1];
                }
                else
                {
                    tileArray[x, y].TopLeft = tileArray[x - 1, y - 1];
                }
            }

            // upper-right tile
            if ((y != 0) && (x != colsToCheck - 1 || inInnerRow))
            {
                if (inInnerRow)
                {
                    tileArray[x, y].TopRight = tileArray[x + 1, y - 1];
                }
                else
                {
                    tileArray[x, y].TopRight = tileArray[x, y - 1];
                }
            }

            // right tile
            if (x != colsToCheck - 1)
            {
                tileArray[x, y].Right = tileArray[x + 1, y];
            }

            // down-right tile
            if ((y != rowsToCheck - 1) && (x != colsToCheck - 1 || inInnerRow))
            {
                if (inInnerRow)
                {
                    tileArray[x, y].BotRight = tileArray[x + 1, y + 1];
                }
                else
                {
                    tileArray[x, y].BotRight = tileArray[x, y + 1];
                }
            }

            // down-left tile
            if ((y != rowsToCheck - 1) && (x != 0 || inInnerRow))
            {
                if (inInnerRow)
                {
                    tileArray[x, y].BotLeft = tileArray[x, y + 1];
                }
                else
                {
                    tileArray[x, y].BotLeft = tileArray[x - 1, y + 1];
                }
            }
        }    
    }

    public void onTileSelected(TileSCRIPT tileScript)
    {
        if (selectedTile != null)
        {
            selectedTile.onUnselected();
            unMarkTargetTiles();
        }

        selectedTile = tileScript;
        selectedTile.onSelected();
        markTargetTiles(testRange);
    }

    public void unMarkTargetTiles()
    {
        foreach(TileSCRIPT ts in tileArray)
        {
            if(ts != null && (ts.isMarked))
            {
                ts.onUnselected();
            }
        }
    }

    public void UnselectAll()
    {
        if (selectedTile != null)
        {
            selectedTile.onUnselected();
        }

        if (selectedUnit != null)
        {
            selectedUnit.OnUnselected();
        }

        unMarkTargetTiles();

    }

    public void removePathMarks()
    {
        foreach (TileSCRIPT ts in tileArray)
        {
            if (ts != null)
            {
                ts.pathVal = -1;
            }
        }
    }

    public void markTargetTiles(int range)
    {
        selectedTile.markTargets(range, true);
    }

    public void onDelete()
    {
        foreach(TileSCRIPT ts in tileArray)
        {
            if(ts != null)
            {
                Destroy(ts.gameObject);
            }
        }

        foreach(TileUnitSCRIPT tus in units)
        {
            Destroy(tus.gameObject);
        }
        obstacleTiles = new List<TileSCRIPT>();
        units = new List<TileUnitSCRIPT>();
    }

    public void restart()
    {
        onDelete();
        Start();
    }

    void OnGUI()
    {
        if (chainLabelsOn == true)
        {
            GUI_labelChains();
        }

        if (coordinateLabelsOn == true)
        {
            GUI_labelCoordinates();
        }
    }

    private void GUI_labelChains()
    {
        for (int i = 0; i < obstacleTiles.Count; i++)
        {
            if (obstacleTiles[i].chainStarter)
            {
                GUI.contentColor = Color.green;
            }
            else
            {
                GUI.contentColor = Color.red;
            }

            Vector3 tileCord = obstacleTiles[i].gameObject.transform.position;
            Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(tileCord);
            Rect textArea = new Rect(screenPos.x, Screen.height - screenPos.y, Screen.width, Screen.height);
            GUI.Label(textArea, (i + 1).ToString());
        }
    }

    private void GUI_labelCoordinates()
    {
        foreach (TileSCRIPT ts in tileArray)
        {
            GUI.skin.label.fontSize = fontSize;

            if (ts == null)
            {
                continue;
            }

            if (ts.isBorder)
                GUI.contentColor = Color.cyan;
            else if (ts.isObstacle)
                GUI.contentColor = Color.magenta;
            else
                GUI.contentColor = Color.blue;

            Vector3 tileCord = ts.gameObject.transform.position;
            Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(tileCord);
            Rect textArea = new Rect(screenPos.x - (float)16, Screen.height - screenPos.y - 10, Screen.width, Screen.height);
            GUI.Label(textArea, ts.col.ToString() + ", " + ts.row.ToString());
        }
    }


    void Update() // Update is called once per frame
    {
        if (Input.GetKeyDown("r"))
            restart();

        moveCamera();
        zoomCamera();
    }

    void moveCamera()
    {

        int speed = 5;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey("d"))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey("a"))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey("s"))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey("w"))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }

    void zoomCamera()
    {
        if(Input.GetAxis("Mouse ScrollWheel") == 0)
        {
            return;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && camScript.orthographicSize > 0.5f) // zoom out
        {
            camScript.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
            if (camScript.orthographicSize > 5.0f)
            {
                camScript.orthographicSize = 5.0f;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && camScript.orthographicSize < 5f) // zoom in
        {
            camScript.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
            if (camScript.orthographicSize < 0.5f)
            {
                camScript.orthographicSize = 0.5f;
            }
        }       
    }
}
