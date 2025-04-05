using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialogUI;
    public TMP_Text dialogText;

    private string[] dialogLines;
    private int currentLine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (dialogUI.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void StartDialog(string[] lines)
    {
        dialogLines = lines;
        currentLine = 0;
        dialogUI.SetActive(true);
        ShowLine();
    }

    private void ShowLine()
    {
        dialogText.text = dialogLines[currentLine];
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine < dialogLines.Length)
        {
            ShowLine();
        }
        else
        {
            dialogUI.SetActive(false);
        }
    }
}
