using UnityEngine;
using System.Collections;
using TMPro;

public class JoinMsgUi : MonoBehaviour
{
    public TextMeshProUGUI joinText;
    public GameObject lan;
    public GameObject lanDropDown;
    public float removeSpeed = 5f;
    public float typingSpeed = 0.08f;
    private Coroutine typingCoroutine;

    public void Show(string Ip)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Text(Ip + " joined the game"));

        lanDropDown.SetActive(false);
        lan.gameObject.SetActive(false);
    }

    public IEnumerator Text(string message)
    {
        joinText.text = "";
        foreach (char letter in message)
        {
            joinText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(removeSpeed);

        for (int i = message.Length - 1; i >= 0; i--)
        {
            joinText.text = message.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }
}
