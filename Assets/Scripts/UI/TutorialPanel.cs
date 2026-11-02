using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// lop chua du lieu cho mot truong hop minh hoa huong dan
[System.Serializable]
public class TutorialCase
{
    public GameObject wholeSetup;
    public GameObject lineAndResult;
}

public class TutorialPanel : MonoBehaviour
{
    // button dong bang huong dan
    [SerializeField] private Button closePanelButton;

    // cac gameobject thanh phan tren giao dien
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject closeMenuButton;

    // danh sach cac truong hop an hop le va khong hop le
    [SerializeField] private TutorialCase[] validCases;
    [SerializeField] private TutorialCase[] invalidCases;

    // coroutine chay vong lap hieu ung minh hoa
    private Coroutine tutorialRoutine;

    // ham khoi tao va gan su kien click nut close
    private void Start()
    {
        closePanelButton.onClick.AddListener(Hide);
    }
        
    // ham hien thi bang huong dan va bat dau hoat anh
    public void Show()
    {
        panel.SetActive(true);

        title.SetActive(false);

        closeMenuButton.SetActive(false);

        // bat dau coroutine vong lap dien hoat huong dan
        tutorialRoutine = StartCoroutine(PlayTutorialAnimation());
    }

    // ham an bang huong dan va dung hoat anh
    public void Hide()
    {
        // dung coroutine neu dang chay de tranh ton bo nho
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
        }

        panel.SetActive(false);

        title.SetActive(true);

        closeMenuButton.SetActive(true);
    }

    // coroutine chay hieu ung trinh chieu cac truong hop an co
    private IEnumerator PlayTutorialAnimation()
    {
        int step = 0;

        // vong lap lien tuc de trinh chieu cac vi du
        while (true)
        {
            int vIndex = step % validCases.Length;
            int iIndex = step % invalidCases.Length;

            TutorialCase currentValid = validCases[vIndex];
            TutorialCase currentInvalid = invalidCases[iIndex];

            // tat het tat ca cac thiet lap cu
            foreach (var c in validCases)
            {
                c.wholeSetup.SetActive(false);
            }

            foreach (var c in invalidCases)
            {
                c.wholeSetup.SetActive(false);
            }

            // bat truong hop hien tai len nhung chua ve duong noi
            currentValid.wholeSetup.SetActive(true);
            currentValid.lineAndResult.SetActive(false);

            currentInvalid.wholeSetup.SetActive(true);
            currentInvalid.lineAndResult.SetActive(false);

            // cho 1 giay truoc khi hien thi ket qua noi duong
            yield return new WaitForSeconds(1.0f);

            // bat tia noi va ket qua dung sai
            currentValid.lineAndResult.SetActive(true);
            currentInvalid.lineAndResult.SetActive(true);

            // giu trang thai ket qua trong 2 giay
            yield return new WaitForSeconds(2.0f);

            step++;
        }
    }
}
