using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Text.RegularExpressions;

public class Authentication : MonoBehaviour
{
    #region Variables
    private string email;
    private string username;
    private string password;
    private string confirmPassword;
    private string playFabPlayerIdCache;

    [SerializeField]
    GameObject messagePanel; // in the future, instead of logging the message, it will display the message pop-up 
    #endregion

    #region PlayFab Authentication
    public void PlayFabLogin()
    {
        if (CheckLoginRequest(username, password))
        {
            var request = new LoginWithPlayFabRequest { Username = username, Password = password };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnPlayFabError);
        }
    }

    public bool CheckLoginRequest(string username, string password)
    {
        if (username.Length < 4)
        {
            Debug.LogWarning("Username should contains at least 4 characters.");
            return false;
        }
        if (password.Length < 6)
        {
            Debug.LogWarning("Password should contains at least 6 characters.");
            return false;
        }
        return true;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        playFabPlayerIdCache = result.PlayFabId;
        ConnectToPhoton();
    }

    public void PlayFabSignUp()
    {
        if (CheckSignUpRequest(email, username, password, confirmPassword))
        {
            var request = new RegisterPlayFabUserRequest { Username = username, Email = email, Password = password };
            PlayFabClientAPI.RegisterPlayFabUser(request, OnSignUpSuccess, OnPlayFabError);
            email = "";
            username = "";
            password = "";
            confirmPassword = "";
        }
    }

    public bool CheckSignUpRequest(string email, string username, string password, string confirmPassword)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(email);
        if (match.Success)
            Debug.Log("Email format is correct.");
        else
        {
            Debug.LogWarning("Email format is not corerct.");
            return false;
        }
        if (username.Length < 4)
        {
            Debug.LogWarning("Username should contains at least 4 characters.");
            return false;
        }
        if (password.Length < 6)
        {
            Debug.LogWarning("Password should contains at least 6 characters.");
            return false;
        }
        if (confirmPassword.Length < 6)
        {
            Debug.LogWarning("Confirm Password should contains at least 6 characters.");
            return false;
        }
        if (!password.Equals(confirmPassword))
        {
            Debug.LogWarning("Password and Confirm Password should be the same.");
            return false;
        }
        return true;
    }

    private void OnSignUpSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Sign-up Success. Your ID is " + result.PlayFabId);
    }

    public void SetUsername(string username)
    {
        this.username = username;
    }

    public void SetEmail(string email)
    {
        this.email = email;
    }

    public void SetPassword(string password)
    {
        this.password = password;
    }

    public void SetConfirmPassword(string confirmPassword)
    {
        this.confirmPassword = confirmPassword;
    }

    #endregion

    #region Get Photon token
    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("PlayFab authenticated. Requesting photon token...");

            GetPhotonAuthenticationTokenRequest photonRequest = new GetPhotonAuthenticationTokenRequest();
            photonRequest.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
            try
            {
                PlayFabClientAPI.GetPhotonAuthenticationToken(photonRequest, AuthWithPhoton, OnPlayFabError);
            }
            catch
            {
                Debug.Log("Could not get authentication token, please log in!");
            }
        }
    }

    private void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        Debug.Log("Photon token acquired: " + result.PhotonCustomAuthenticationToken + "  Authentication complete.");
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        customAuth.AddAuthParameter("username", playFabPlayerIdCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = customAuth;
        PhotonNetwork.NickName = username;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong");
        Debug.LogError(error.GenerateErrorReport());
    }

}

