using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileSCRIPT : MonoBehaviour
{
    public TileManagerSCRIPT managerScript;

    public Sprite tile_sprite;
    public Sprite tileObstacle_sprite;

    public TileUnitSCRIPT unit;

    private TileSCRIPT topLeft, topRight, right, botRight, botLeft, left;

    public TileSCRIPT TopLeft
    {
        get { return topLeft;  }
        set { topLeft = value; }
    }

    public TileSCRIPT TopRight
    {
        get { return topRight; }
        set { topRight = value; }
    }

    public TileSCRIPT Right
    {
        get { return right; }
        set { right = value; }
    }

    public TileSCRIPT BotRight
    {
        get { return botRight; }
        set { botRight = value; }
    }

    public TileSCRIPT BotLeft
    {
        get { return botLeft; }
        set { botLeft = value; }
    }

    public TileSCRIPT Left
    {
        get { return left; }
        set { left = value; }
    }

    public bool isSelected = false;
    public bool isMarked = false;
    public bool isObstacle = false;
    public bool isBorder = false;
    public bool chainStarter = false;
    public bool isObstacleT = false;

    public bool isHighlighted = false;

    public int pathVal = -1;

    public int row, col;
    private static int lowestStepsFound = -1;
     

    public TileSCRIPT getSide(int dir)
    {
        switch(dir)
        {
            case 0:
                return topLeft;
            case 1:
                return topRight;
            case 2:
                return right;
            case 3:
                return botRight;
            case 4:
                return botLeft;
            case 5:
                return left;
        }
        return null;
    }

    public bool okForObstacle()
    {
        setAsObstacle();
        bool val = managerScript.checkObstacleOK(this);
        removeObstacle();
        return val;
    }

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        

    }


    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightClick();
            return;
        }    
        
    }

    public void OnMouseEnter()
    {
        if(managerScript.selectedUnit != null && this.unit != null && managerScript.selectedUnit.team != this.unit.team) // if hovering over enemy of selected unit
        {
            Highlight(Color.red);
            return;
        }

        if (isHighlighted == false && unit != null && isSelected == false)
        {
            Highlight(Color.yellow);
        }
    }

    public void Highlight(Color c)
    {
        isHighlighted = true;

        if(unit != null)
        {
            unit.GetComponent<SpriteRenderer>().color = c;
        }

        GetComponent<SpriteRenderer>().color = c;       
    }

    public void UnHighlight()
    {
        isHighlighted = false;
        if (unit != null)
        {
            unit.GetComponent<SpriteRenderer>().color = Color.white;
        }
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    void OnMouseExit()
    {
        if (isHighlighted)
        {
            UnHighlight();

            if (isMarked)
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }

            
        }
    }

    public void onSelected()
    {
        if (isHighlighted)
        {
            UnHighlight();
        }
        managerScript.selectedTile = this;
        isSelected = true;
        GetComponent<SpriteRenderer>().color = Color.cyan;
        distanceTest();
    }

    public void onUnselected()
    {
        managerScript.selectedTile = null;
        isSelected = false;
        isMarked = false;
        pathVal = -1;
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    public TileSCRIPT[] getSideTiles()
    {
        TileSCRIPT[] sides = new TileSCRIPT[6];
        sides[0] = topLeft;
        sides[1] = topRight;
        sides[2] = right;
        sides[3] = botRight;
        sides[4] = botLeft;
        sides[5] = left;
        return sides;
    }

    // return number of non-null and non-obstacle tiles surrounding this tile
    public int openSides()
    {
        int numOpenSides = 0;
        TileSCRIPT[] sides = getSideTiles();

        foreach(TileSCRIPT ts in sides)
        {
            if (ts != null && ts.isObstacle == false)
            {
                numOpenSides++;
            }
        }

        return numOpenSides;
    }

    public void OnLeftClick()
    {
        managerScript.UnselectAll();
        if(unit != null)
        {
            unit.OnSelected();
        }            
    }

    public void OnRightClick()
    {

        if(managerScript.selectedUnit == null)
        {
            return;
        }

        if(unit != null && managerScript.selectedUnit.team != unit.team)
        {
            managerScript.selectedUnit.Attack(this.unit);
        }

        if(isMarked == false)
        {
            return;
        }

        if(unit != null)
        {
            return;
        }

        managerScript.selectedUnit.MoveTo(this);
    }

    public void markTargets(int range, bool marking)
    {
        if(isObstacle)
        {
            return;
        }

        pathVal = range;

        if(!isSelected && marking)
        {
            onMarked();
        }

        if(range > 0)
        {
            range--;

            TileSCRIPT[] sides = getSideTiles();

            foreach(TileSCRIPT ts in sides)
            {
                if(ts != null && range > ts.pathVal)
                {
                    ts.markTargets(range, marking);
                }
            }

        }
    }

    public int ShortestPath(TileSCRIPT targetTile)
    {
        if(managerScript.checkPathExists(this, targetTile) == false)
        {
            return -1;
        }

        if(this == targetTile)
        {
            return 0;
        }

        List<TileSCRIPT> checkedTiles = new List<TileSCRIPT>();
        this.pathVal = 0;
        checkedTiles.Add(this);
        int val = searchForShortestPath(targetTile, checkedTiles, 0);
        managerScript.removePathMarks();
        return val;

        /*
        int val = searchForShortestPath(targetTile, 0);
        managerScript.removePathMarks();
        lowestStepsFound = -1;
        return val;
        */
    }

    public int searchForShortestPath(TileSCRIPT targetTile, List<TileSCRIPT> checkedTiles, int steps)
    {
        foreach(TileSCRIPT ts in checkedTiles)
        {
            if(ts == targetTile)
            {
                return steps;
            }
        }

        List<TileSCRIPT> newCheckedTiles = new List<TileSCRIPT>();
        foreach(TileSCRIPT ts in checkedTiles)
        {
            TileSCRIPT[] sides = ts.getSideTiles();
            foreach(TileSCRIPT side in sides)
            {
                if(side != null && side.isObstacle == false && side.pathVal == -1)
                {
                    side.pathVal = 0;
                    newCheckedTiles.Add(side);
                }
            } // end foreach
        } // end foreach

         return searchForShortestPath(targetTile, newCheckedTiles, steps + 1);


        /*
        if(lowestStepsFound != -1 && steps >= lowestStepsFound)
        {
            return 9001;
        }

        pathVal = steps;
        if(this == targetTile)
        {
            lowestStepsFound = steps;
            return steps;
        }

        TileSCRIPT[] sides = getSideTiles();
        int[] stepsToTarget = new int[6];

        for (int i = 0; i < 6; i++)
        {
            if(sides[i] != null && sides[i].isObstacle == false && (sides[i].pathVal == -1 || sides[i].pathVal > steps) )
            {
                stepsToTarget[i] = sides[i].searchForShortestPath(targetTile, steps + 1);
            }
            else
            {
                stepsToTarget[i] = 9001; // #epic
            }
        }

        return stepsToTarget.Min();
        */
    }

    public bool pathExists(TileSCRIPT targetTile, int range)
    {
        bool val = searchForPath(targetTile, range);
        managerScript.removePathMarks();
        return val;
    }

    public bool searchForPath(TileSCRIPT targetTile, int range)
    {
        if (managerScript.targetTileFound == true)
        {
            return true;
        }

        pathVal = range;

        if (this == targetTile)
        {
            managerScript.targetTileFound = true;
            return true;
        }

        if (range > 0)
        {
            range--;
            TileSCRIPT[] sides = getSideTiles();
            foreach (TileSCRIPT ts in sides)
            {
                if (ts != null && ts.isObstacle == false && range > ts.pathVal)
                {
                    ts.searchForPath(targetTile, range);
                }
            }
        }

        return managerScript.targetTileFound;
    }

    public bool canBeBorder()
    {
        if (left == null || topLeft == null || right == null || botRight == null)
            return true;
        return false;
    }

    public void setAsBorder()
    {
        isBorder = true;
        setAsObstacle();
    }

    public void setAsObstacle()
    {
        isObstacle = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = tileObstacle_sprite;
    }

    public void removeObstacle()
    {
        isObstacle = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = tile_sprite;
    }

    

    public void onMarked()
    {
        isMarked = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void distanceTest()
    {
        Debug.Log(ShortestPath(managerScript.tileArray[1, 1]));
    }

}
