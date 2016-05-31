using UnityEngine;

public class TileUnitSCRIPT : MonoBehaviour
{
    public TileSCRIPT tile;
    public bool isSelected = false;
    public TileManagerSCRIPT managerScript;

    public int range = 3;
    public int team = -1;


    public void OnSelected()
    {
        isSelected = true;  

        if (managerScript.selectedTile != null)
            managerScript.selectedTile.onUnselected();

        managerScript.unMarkTargetTiles();
        managerScript.selectedUnit = this;
        tile.onSelected();
        managerScript.selectedTile = tile;
        managerScript.markTargetTiles(range);
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    public void OnUnselected()
    {
        isSelected = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        managerScript.selectedUnit = null;
    }

    public void Attack(TileUnitSCRIPT otherUnit)
    {
        Debug.Log("SMAAASH");
    }

    public void MoveTo(TileSCRIPT ts)
    {
        if(ts.unit != null)
        {
            return;
        }

        tile.unit = null;
        tile = ts;
        ts.unit = this;
        Vector3 tileCord = ts.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(tileCord.x, tileCord.y, -10);

        managerScript.UnselectAll();                  
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void OnMouseOver()
    {
        if(tile.isHighlighted == false && isSelected == false)
        {
            tile.Highlight();
        }           
    }
    */

    

}
