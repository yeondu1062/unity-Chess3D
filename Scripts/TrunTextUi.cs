using UnityEngine;
using System.Collections;
using TMPro;

public class TrunTextUi : MonoBehaviour
{
    public TextMeshProUGUI trunText;
    public float typingSpeed = 0.1f;
    private Coroutine typingCoroutine;

    public void Next(int trun)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Text("TRUN " + trun));
    }

    public IEnumerator Text(string message)
    {
        trunText.text = "";
        foreach (char letter in message)
        {
            trunText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    private void Start()
    {
        Next(0);
    }
}
