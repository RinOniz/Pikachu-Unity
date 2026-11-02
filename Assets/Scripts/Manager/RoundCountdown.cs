using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundCountdown : MonoBehaviour
{
    // text hien thi dong ho dem nguoc thoi gian
    [SerializeField] private TextMeshProUGUI roundCountdownText;

    // tong thoi gian quy dinh cho van dau tinh theo giay
    [SerializeField] private int roundTime;

    // thoi gian con lai thuc te
    private float roundTimeLeft;

    // ham khoi tao thoi gian cho man choi
    private void Start()
    {
        roundTimeLeft = (float)roundTime;
    }

    // ham update cap nhat dong ho dem nguoc moi frame
    private void Update()
    {
        // dung dem gio khi game dang bi pause hoac da ket thuc
        if (GetComponent<GameController>().IsPause() || GetComponent<GameController>().IsGameOver())
        {
            return;
        }

        float currentTime = 0.0f;

        // xu ly giam thoi gian dem nguoc
        if (roundTimeLeft > 0)
        {
            roundTimeLeft -= Time.deltaTime;

            currentTime = roundTimeLeft > 0 ? roundTimeLeft : 0.0f;

            // neu het gio thi thong bao thua cuoc game over
            if (currentTime == 0.0f)
            {
                gameObject.GetComponent<GameController>().SetGameOver();
            }
        }

        // tinh toan so phut va so giay de hien thi
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        // doi mau chu theo muc do khan cap cua thoi gian
        if (currentTime <= 30f)
        {
            // mau do canh bao khi sap het gio duoi 30 giay
            roundCountdownText.color = Color.red;
        }
        else if (currentTime > (float)roundTime * 0.6f)
        {
            // mau xanh la khi thoi gian con nhieu
            roundCountdownText.color = new Color(47f / 255f, 154f / 255f, 8f / 255f, 1f);
        }
        else if (currentTime <= (float)roundTime * 0.6f)
        {
            // mau vang cam khi thoi gian con mot nua
            roundCountdownText.color = new Color(217f / 255f, 152f / 255f, 0f, 1f);
        }

        // cap nhat chuoi thoi gian len giao dien
        roundCountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // ham tra ve tong thoi gian nguoi choi da choi trong van
    public float GetRemainTime()
    {
        return (float)roundTime - roundTimeLeft;
    }
}
