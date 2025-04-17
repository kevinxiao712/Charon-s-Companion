using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    public float typingSpeed;

    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textDisplay.text = string.Empty;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && gameObject.activeSelf)
        {
            if (textDisplay.text == sentences[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textDisplay.text = sentences[index];
            }
        }

    }

    public void StartDialogue()
    {
        Debug.Log("Dialogue started!");
        gameObject.SetActive(true);
        index = 0;
        textDisplay.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void NextLine()
    {
        if (index < sentences.Length -1)
        {
            index++;
            textDisplay.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }

}
