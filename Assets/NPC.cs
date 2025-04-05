using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;

    private bool isTyping = false;

    void Update()
    {
        // แสดงปุ่มถ้าแสดงข้อความครบแล้ว
        if (dialogueText.text == dialogue[index] && !isTyping)
        {
            contButton.SetActive(true);
        }
    }

    public void zeroText()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        isTyping = false;
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        isTyping = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialoguePanel.SetActive(true);
            index = 0;
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            zeroText();
        }
    }
}
