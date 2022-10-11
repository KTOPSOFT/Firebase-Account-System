using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Space]
    [Header("Login")]
    public InputField emailloginfield;
    public InputField passwordloginfield;

    [Space]
    [Header("Resiger")]
    public InputField nameregisterfield;
    public InputField emailregisterfield;
    public InputField password1registerfield;
    public InputField password2registerfield;

    private void Start()
    {
        StartCoroutine(CheckAndFixDependenciesAsync());
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(() => dependencyTask.IsCompleted);

        dependencyStatus = dependencyTask.Result;

        if(dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.Log("Could not resovle Firebase dependencies"+dependencyStatus);
        }
    }
    
    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckForAutoLogin()
    {
        if( user != null )
        {
            var reloadUser = user.ReloadAsync();

            yield return new WaitUntil(() => reloadUser.IsCompleted);

            AutoLogin();
        }
        else
        {
            UIManager.Instance.OpenLogin();
        }
    }

    private void AutoLogin()
    {
        if( user != null )
        {
            if(user.IsEmailVerified)
            {
                Local_DataBase.userName = user.DisplayName;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Firebase Main");
            }
            else
            {
                SendEmailForVerification();
            }
        }
        else
        {
            UIManager.Instance.OpenLogin();
        }
    }

    void AuthStateChanged(object sender , System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if(!signedIn && user != null)
            {
                Debug.Log("Signed Out" + user.UserId);
            }

            user = auth.CurrentUser;

            if(signedIn)
            {
                Debug.Log("Signed In" + user.UserId);
            }
        }
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailloginfield.text , passwordloginfield.text));
    }

    private IEnumerator LoginAsync(string email , string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email , password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed Because";

            switch(authError)
            {
                case AuthError.InvalidEmail:
                {
                    failedMessage += "Email is invalid";
                    break;
                }
                case AuthError.WrongPassword:
                {
                    failedMessage += "Password is wrong";
                    break;
                }
                case AuthError.MissingEmail:
                {
                    failedMessage += "Email is missing";
                    break;
                }
                case AuthError.MissingPassword:
                {
                    failedMessage += "Password is missing";
                    break;
                }
                default :
                {
                    failedMessage += "Login is failed";
                    break;
                }
            }

            Debug.Log(failedMessage);
        }
        else
        {
            user = loginTask.Result;

            Debug.Log("You are successfully log in" + user.DisplayName);

            if(user.IsEmailVerified)
            {
                Local_DataBase.userName = user.DisplayName;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Firebase Main");
            }
            else
            {
                SendEmailForVerification();
            }
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameregisterfield.text , emailregisterfield.text , password1registerfield.text , password2registerfield.text));
    }

    private IEnumerator RegisterAsync(string name , string email , string password , string confirmPassword)
    {
        if(name == "")
        {
            Debug.Log("Name is empty");
        }
        if(email == "")
        {
            Debug.Log("Email is empty");
        }
        if(password != confirmPassword)
        {
            Debug.Log("Password is not match");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email , password);

            yield return new WaitUntil(() => registerTask.IsCompleted);
            if(registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Register Failed Because";

                switch(authError)
                {
                    case AuthError.InvalidEmail:
                    {
                        failedMessage += "Email is invalid";
                        break;
                    }
                    case AuthError.WrongPassword:
                    {
                        failedMessage += "Password is wrong";
                        break;
                    }
                    case AuthError.MissingEmail:
                    {
                        failedMessage += "Email is missing";
                        break;
                    }
                    case AuthError.MissingPassword:
                    {
                        failedMessage += "Password is missing";
                        break;
                    }
                    default :
                    {
                        failedMessage += "Register is failed";
                        break;
                    }
                }

                Debug.Log(failedMessage);
            }
            else
            {
                user = registerTask.Result;

                UserProfile userProfile = new UserProfile{DisplayName = name};
                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil (() => updateProfileTask.IsCompleted);

                if(updateProfileTask.Exception != null)
                {
                    user.DeleteAsync();
                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "Profiel Update Failed Because";

                    switch(authError)
                    {
                        case AuthError.InvalidEmail:
                        {
                            failedMessage += "Email is invalid";
                            break;
                        }
                        case AuthError.WrongPassword:
                        {
                            failedMessage += "Password is wrong";
                            break;
                        }
                        case AuthError.MissingEmail:
                        {
                            failedMessage += "Email is missing";
                            break;
                        }
                        case AuthError.MissingPassword:
                        {
                            failedMessage += "Password is missing";
                            break;
                        }
                        default :
                        {
                            failedMessage += "Profile Update is failed";
                            break;
                        }
                    }

                    Debug.Log(failedMessage);
                }
                else
                {
                    Debug.Log("Registeration is successful. Welcome "+user.DisplayName);
                    if(user.IsEmailVerified)
                    {
                        UIManager.Instance.OpenLogin();
                    }
                    else
                    {
                        SendEmailForVerification();
                    }
                }
            }
        }
    }

    public void SendEmailForVerification()
    {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    private IEnumerator SendEmailForVerificationAsync()
    {
        if( user != null )
        {
            var sendEmailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if(sendEmailTask.Exception != null)
            {
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string errorMessage = "Unknown Error. Please try again later";

                switch(authError)
                {
                    case AuthError.Cancelled:
                    {
                        errorMessage = "Verification is cancelled";
                        break;
                    }
                    case AuthError.TooManyRequests:
                    {
                        errorMessage = "TooManyRequest";
                        break;
                    }
                    case AuthError.InvalidRecipientEmail:
                    {
                        errorMessage = "Invalid email you entered";
                        break;
                    }
                }

                UIManager.Instance.OpenVerify(false , user.Email , errorMessage);
            }
            else
            {
                Debug.Log("Email has successfully sent");
                UIManager.Instance.OpenVerify(true , user.Email , null);
            }
        }
    }
}
