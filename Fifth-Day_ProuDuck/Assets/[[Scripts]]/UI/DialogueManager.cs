// This implementation of the Dialogue Manager is based on a tutorial by Brackeys
// https://www.youtube.com/watch?v=_nRzoTzeyxU&ab_channel=Brackeys
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;


    private Queue<string> _sentences = new Queue<string>();

    public void StartDialogue(Dialogue dialogue)
    {
        nameText.text = dialogue.NPCName;
        _sentences.Clear();

        foreach (string sentence in dialogue.Sentences)
        {
            if (!dialogue.NPCName.Equals(nameText.text))
            {
                continue;
            }
            else
            {
                _sentences.Enqueue(sentence);
            }
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    private void EndDialogue()
    {
        Debug.Log("End of conversation.");
    }
}
