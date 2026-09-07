using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject board;

    private GameObject firstTile;
    private GameObject secondTile;

    private bool isSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public GameObject GetFirstTile()
    {
        return firstTile;
    }

    public void SelectFirstTile(GameObject tile)
    {
        firstTile = tile;
        isSelected = true;
    }

    public void SelectSecondTile(GameObject tile)
    {
        secondTile = tile;

        ResetTile();
    }

    private void ResetTile()
    {
        firstTile = null;
        secondTile = null;

        isSelected = false;
    }
}
