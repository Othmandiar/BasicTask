using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

    public class ConnectionUI : MonoBehaviour
    {

        //----------------------------------------------------------
        // UI elements
        //----------------------------------------------------------
        public Button loginButton;
        public Text errorText;

        //----------------------------------------------------------
        // Private properties
        //----------------------------------------------------------

        private SmartFox sfs;

        //----------------------------------------------------------
        // Unity calback methods
        //----------------------------------------------------------

        void Start()
        {
            // Initialize UI
            errorText.text = "";
        }

        void Update()
        {
            if (sfs != null)
                sfs.ProcessEvents();
        }

        void OnApplicationQuit()
        {
            // Always disconnect before quitting
            if (sfs != null && sfs.IsConnected)
                sfs.Disconnect();
        }

        // Disconnect from the socket when ordered by the main Panel scene
        public void Disconnect()
        {
            OnApplicationQuit();
        }

        //----------------------------------------------------------
        // Public interface methods for UI
        //----------------------------------------------------------

        public void OnLoginButtonClick()
        {

            // Set connection parameters
            ConfigData cfg = new ConfigData();
            cfg.Host = "127.0.0.1";
            cfg.Port = 9933;
            cfg.Zone = "BasicExamples";

            // Initialize SFS2X client and add listeners
#if !UNITY_WEBGL
            sfs = new SmartFox();
#else
			sfs = new SmartFox(UseWebSocket.WS_BIN);
#endif

            sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
            sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
            sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);

            // Connect to SFS2X
            sfs.Connect(cfg);
        }

        private void reset()
        {
            // Remove SFS2X listeners
            sfs.RemoveAllEventListeners();

            // Enable interface
        }

        //----------------------------------------------------------
        // SmartFoxServer event listeners
        //----------------------------------------------------------

        private void OnConnection(BaseEvent evt)
        {
            if ((bool)evt.Params["success"])
            {
                // Save reference to the SmartFox instance in a static field, to share it among different scenes
                SmartFoxConnection.Connection = sfs;

                Debug.Log("SFS2X API version: " + sfs.Version);
                Debug.Log("Connection mode is: " + sfs.ConnectionMode);

                // Login
                sfs.Send(new Sfs2X.Requests.LoginRequest(""));
            }
            else
            {
                // Remove SFS2X listeners and re-enable interface
                reset();

                // Show error message
                errorText.text = "Connection failed; is the server running at all?";
            }
        }

        private void OnConnectionLost(BaseEvent evt)
        {
            // Remove SFS2X listeners and re-enable interface
            reset();

            string reason = (string)evt.Params["reason"];

            if (reason != ClientDisconnectionReason.MANUAL)
            {
                // Show error message
                errorText.text = "Connection was lost; reason is: " + reason;
            }
        }

        private void OnLogin(BaseEvent evt)
        {
            string roomName = "Game Room";

            // We either create the Game Room or join it if it exists already
            if (sfs.RoomManager.ContainsRoom(roomName))
            {
                sfs.Send(new JoinRoomRequest(roomName));
            }
            else
            {
                RoomSettings settings = new RoomSettings(roomName);
                settings.MaxUsers = 40;
                sfs.Send(new CreateRoomRequest(settings, true));
            }
        }

        private void OnLoginError(BaseEvent evt)
        {
            // Disconnect
            sfs.Disconnect();

            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
            errorText.text = "Login failed: " + (string)evt.Params["errorMessage"];
        }

        private void OnRoomJoin(BaseEvent evt)
        {
            // Remove SFS2X listeners and re-enable interface before moving to the main game scene
            reset();

        // Go to main game scene
        SceneManager.LoadScene(0);
        }

        private void OnRoomJoinError(BaseEvent evt)
        {
            // Show error message
            errorText.text = "Room join failed: " + (string)evt.Params["errorMessage"];
        }
    }

