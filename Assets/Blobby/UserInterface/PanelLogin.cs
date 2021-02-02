using Blobby.UserInterface.Components;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Blobby.Game;

namespace Blobby.UserInterface
{
    public static class PanelLogin
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            LoginHelper.Logout += OnLogout;
            LoginHelper.Login += OnLogin;
            LoginHelper.RegisterSuccess += OnRegisterSuccess;
            LoginHelper.RegisterFailed += OnRegisterFailed;

            ButtonGlobe.Clicked += OnButtonGlobe;
            ButtonLogin.Clicked += OnButtonLogin;
            ButtonRegister.Clicked += OnButtonRegister;
            ButtonRegisterShow.Clicked += () => SetPanelRegister(true);
        }

        static void OnButtonGlobe()
        {
            if (ClientConnection.UserData != null) LoginHelper.LogoutUser();
            else
            {
                var animator = GameObject.Find("parent_login").GetComponent<Animator>();
                var show = animator.GetBool("show");
                if (!show)
                {
                    SetPanelRegister(false);
                }
                else
                {
                    InputHelper.SetTabCallback(null);
                    InputHelper.SetEnterCallback(null);
                }
                animator.SetBool("show", !show);
            }
        }

        static void OnButtonLogin()
        {
            var username = GameObject.Find("input_user").GetComponent<TMP_InputField>().text;
            var password = GameObject.Find("input_pass").GetComponent<TMP_InputField>().text;

            Task.Run(() => LoginHelper.LoginRequest(username, password));
        }

        static void OnButtonRegister()
        {
            var username = GameObject.Find("input_user").GetComponent<TMP_InputField>().text;
            var password = GameObject.Find("input_pass").GetComponent<TMP_InputField>().text;
            var email = GameObject.Find("input_mail").GetComponent<TMP_InputField>().text;

            Task.Run(() => LoginHelper.RegisterRequest(username, password, email));
        }

        static void OnLogin(UserData userData)
        {
            GameObject panelOffline = GameObject.Find("panel_offline");

            for (int i = 0; i < panelOffline.transform.childCount; i++)
                panelOffline.transform.GetChild(i).gameObject.SetActive(false);

            GameObject.Find("parent_login").GetComponent<Animator>().SetBool("show", false);
            InputHelper.SetTabCallback(null);
            InputHelper.SetEnterCallback(null);
        }

        static void OnLogout()
        {
            var panelOffline = GameObject.Find("panel_offline");
            for (int i = 0; i < panelOffline.transform.childCount; i++)
                panelOffline.transform.GetChild(i).gameObject.SetActive(true);
        }

        static void OnRegisterSuccess()
        {
            SetPanelRegister(false);
        }

        static void OnRegisterFailed()
        {
            //TODO Fehler melden
        }

        static void SetPanelRegister(bool visible)
        {
            var inputMail = GameObject.Find("input_mail");

            foreach (Image image in inputMail.GetComponentsInChildren<Image>())
                image.enabled = visible;
            foreach (TMP_InputField input in inputMail.GetComponentsInChildren<TMP_InputField>())
                input.enabled = visible;
            foreach (TextMeshProUGUI text in inputMail.GetComponentsInChildren<TextMeshProUGUI>())
                text.enabled = visible;

            var buttonRegister = GameObject.Find("button_register");

            foreach (Image image in buttonRegister.GetComponentsInChildren<Image>())
                image.enabled = visible;
            foreach (Button input in buttonRegister.GetComponentsInChildren<Button>())
                input.enabled = visible;
            foreach (TextMeshProUGUI text in buttonRegister.GetComponentsInChildren<TextMeshProUGUI>())
                text.enabled = visible;

            var buttonLogin = GameObject.Find("button_login");

            foreach (Image image in buttonLogin.GetComponentsInChildren<Image>())
                image.enabled = !visible;
            foreach (Button input in buttonLogin.GetComponentsInChildren<Button>())
                input.enabled = !visible;
            foreach (TextMeshProUGUI text in buttonLogin.GetComponentsInChildren<TextMeshProUGUI>())
                text.enabled = !visible;

            var buttonRegisterShow = GameObject.Find("button_register_show");

            foreach (Image image in buttonRegisterShow.GetComponentsInChildren<Image>())
                image.enabled = !visible;
            foreach (Button input in buttonRegisterShow.GetComponentsInChildren<Button>())
                input.enabled = !visible;
            foreach (TextMeshProUGUI text in buttonRegisterShow.GetComponentsInChildren<TextMeshProUGUI>())
                text.enabled = !visible;

            if (visible)
            {
                InputHelper.SetTabCallback(OnTabRegister);
                InputHelper.SetEnterCallback(OnButtonRegister);
            }
            else
            {
                InputHelper.SetTabCallback(OnTabLogin);
                InputHelper.SetEnterCallback(OnButtonLogin);
            }

            EventSystem.current.SetSelectedGameObject(GameObject.Find("input_user"));
        }

        #region Navigation

        static void OnTabLogin()
        {
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            var inputUser = GameObject.Find("input_user");
            var inputPass = GameObject.Find("input_pass");

            if (selectedObject == inputUser)
            {
                EventSystem.current.SetSelectedGameObject(inputPass);
            }
            else if (selectedObject == inputPass)
            {
                EventSystem.current.SetSelectedGameObject(inputUser);
            }
        }

        static void OnTabRegister()
        {
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            var inputUser = GameObject.Find("input_user");
            var inputPass = GameObject.Find("input_pass");
            var inputMail = GameObject.Find("input_mail");

            if (selectedObject == inputUser)
            {
                EventSystem.current.SetSelectedGameObject(inputPass);
            }
            else if (selectedObject == inputPass)
            {
                EventSystem.current.SetSelectedGameObject(inputMail);
            }
            else if (selectedObject == inputMail)
            {
                EventSystem.current.SetSelectedGameObject(inputUser);
            }
        }

        #endregion
    }
}
