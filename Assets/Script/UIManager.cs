using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject login_panel;

    [SerializeField]
    private GameObject register_panel;

    [SerializeField]
    private GameObject verify_panel;

    [SerializeField]
    private Text notification_message;
    
    void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void ClearUI()
    {
        login_panel.SetActive(false);
        register_panel.SetActive(false);
        verify_panel.SetActive(false);
    }

    public void OpenLogin()
    {
        ClearUI();
        login_panel.SetActive(true);
    }

    public void OpenRegister()
    {
        ClearUI();
        register_panel.SetActive(true);
    }

    public void OpenVerify(bool isEmailSent , string emailId , string errorMessage)
    {
        ClearUI();
        verify_panel.SetActive(true);

        if(isEmailSent)
        {
            notification_message.text = "Please verify your email.  Message has been sent to"+emailId;
        }
        else
        {
            notification_message.text = "Couldn't sent message."+errorMessage;
        }
    }
}
