using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;  // �� ���� (�����л�����)

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public GameObject statusPanel;
    public TextMeshProUGUI statusText;

    private const string loginUrl = "http://localhost:8080/user/login";

    void Start()
    {
        statusPanel.SetActive(false);
    }

    public void OnClickLogin()
    {
        string url = $"{loginUrl}?username={UnityWebRequest.EscapeURL(usernameInput.text)}&password={UnityWebRequest.EscapeURL(passwordInput.text)}";
        StartCoroutine(Login(url));
    }

    IEnumerator Login(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;

            if (result.Contains("success"))
            {
                ShowStatus(result, Color.green);

                // ��¼�ɹ����ӳ� 0.5 ����� Sample Scene
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                ShowStatus(result, Color.red);
            }
        }
        else
        {
            ShowStatus(request.error, Color.red);
        }
    }

    void ShowStatus(string message, Color color)
    {
        statusPanel.SetActive(true);
        statusText.text = message;
        statusText.color = color;

        CancelInvoke("HideStatus");
        Invoke("HideStatus", 3f);
    }

    void HideStatus()
    {
        statusPanel.SetActive(false);
    }
}
