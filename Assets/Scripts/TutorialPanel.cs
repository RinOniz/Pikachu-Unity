using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialCase
{
    public GameObject wholeSetup;
    public GameObject lineAndResult;
}


public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private Button closePanelButton;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject closeMenuButton;

    [SerializeField] private TutorialCase[] validCases;
    [SerializeField] private TutorialCase[] invalidCases;

    private Coroutine tutorialRoutine;

    private void Start()
    {
        closePanelButton.onClick.AddListener(Hide);
    }
        
    public void Show()
    {
        panel.SetActive(true);

        title.SetActive(false);

        closeMenuButton.SetActive(false);

        tutorialRoutine = StartCoroutine(PlayTutorialAnimation());
    }

    public void Hide()
    {
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
        }

        panel.SetActive(false);

        title.SetActive(true);

        closeMenuButton.SetActive(true);
    }

    private IEnumerator PlayTutorialAnimation()
    {
        int step = 0;

        while (true)
        {
            int vIndex = step % validCases.Length;
            int iIndex = step % invalidCases.Length;

            TutorialCase currentValid = validCases[vIndex];
            TutorialCase currentInvalid = invalidCases[iIndex];

            foreach (var c in validCases)
            {
                c.wholeSetup.SetActive(false);
            }

            foreach (var c in invalidCases)
            {
                c.wholeSetup.SetActive(false);
            }

            currentValid.wholeSetup.SetActive(true);
            currentValid.lineAndResult.SetActive(false);

            currentInvalid.wholeSetup.SetActive(true);
            currentInvalid.lineAndResult.SetActive(false);

            yield return new WaitForSeconds(1.0f);

            currentValid.lineAndResult.SetActive(true);
            currentInvalid.lineAndResult.SetActive(true);

            yield return new WaitForSeconds(2.0f);

            step++;
        }
    }
}
