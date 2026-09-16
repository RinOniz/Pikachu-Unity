using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintText;

    private int totalGetHintTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private void Start()
    //{
    //    hintButton.onClick.AddListener(GetHint);
    //}

    //private void GetHint()
    //{
    //    if (hintText.text == "HINT")
    //    {
    //        bool hasHint = GetComponent<GameController>().GetHint(totalGetHintTime);

    //        if (hasHint)
    //        {
    //            totalGetHintTime++;
    //        }
    //    }
    //}

    //public void ChangeText(string text)
    //{
    //    hintText.text = text;
    //}
}
