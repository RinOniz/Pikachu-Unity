using UnityEngine;

public class Position : MonoBehaviour
{
    public int row, col;

    public Position(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public string GetMove(Position previousPos)
    {
        int deltaRow = row - previousPos.row;
        int deltaCol = col - previousPos.col;

        if (deltaRow == 0)
        {
            if (deltaCol == 1)
            {
                return "R";
            }
            else if (deltaCol == -1)
            {
                return "L";
            }
        }
        else if (deltaCol == 0)
        {
            if (deltaRow == 1)
            {
                return "D";
            }
            else if (deltaRow == -1)
            {
                return "U";
            }
        }

        return "";
    }
}
