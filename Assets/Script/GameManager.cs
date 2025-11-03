using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//          Yuki Ono  KTOPSOFT  present                //

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text welcomeText;
    // Start is called before the first frame update
    void Start()
    {
#if mobile
        ShowWelcomeText();
#endif
    }

    private void ShowWelcomeText()
    {
        welcomeText.text = "Welcome "+Local_DataBase.userName+"to our game scene";
    }
}
