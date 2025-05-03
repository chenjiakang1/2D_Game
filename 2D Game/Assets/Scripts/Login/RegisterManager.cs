using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class UserRegisterDto
{
    public string username;
    public string password;
}

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public GameObject statusPanel;
    public TextMeshProUGUI statusText;

    private const string registerUrl = "http://localhost:8080/user/register";

    void Start()
    {
        statusPanel.SetActive(false);
    }

    public void OnClickRegister()
    {
        UserRegisterDto user = new UserRegisterDto
        {
            username = usernameInput.text,
            password = passwordInput.text
        };

        StartCoroutine(Register(user));
    }

    IEnumerator Register(UserRegisterDto user)
    {
        string jsonData = JsonUtility.ToJson(user);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(registerUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;

            if (result.Contains("success"))
            {
                ShowStatus(result, Color.green);
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
