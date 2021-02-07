using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game.Entities;
using Blobby.Game.States;
using Blobby.Models;
using Blobby.Networking;
using Blobby.UserInterface;
using Blobby.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Blobby.Game
{
    public static class MatchHandler
    {
        public static IMatch Match { get; set; }
        public static MatchData MatchData { get; set; }
        public static MatchData LocalMatchData { get; private set; }

        public static ServerData ServerData { get; set; }

        public static bool IsAi { get; set; }

        public static SlideEffect SlideEffect { get; private set; }
        public static BlackoutEffect BlackoutEffect { get; private set; }
        public static ZoomEffect ZoomEffect { get; private set; }

        public static IClientState ClientMenuState;
        public static IClientState ClientGameOnlineState;
        public static IClientState ClientGameLocalState;

        static IClientState _clientState;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            SlideEffect = new SlideEffect(null, OnSlideOver);
            BlackoutEffect = new BlackoutEffect(OnBlackoutOver, OnWhiteoutOver);
            ZoomEffect = new ZoomEffect(null, OnZoomOutOver);

            ClientMenuState = new ClientMenuState();
            ClientGameOnlineState = new ClientGameOnlineState();
            ClientGameLocalState = new ClientGameLocalState();

            _clientState = ClientMenuState;

            SubscribeEventHandler();

            InputHelper.SetEscapeCallback(OnEscapeDown);

            LocalMatchData = new MatchData();
            
            //TODO: Load and save matchData
        }

        public static void SetState(IClientState state)
        {
            if (state == null) return;

            _clientState.ExitState();
            _clientState = state;
            _clientState.EnterState();
        }

        static void SubscribeEventHandler()
        {
            ButtonPauseResume.Clicked += OnButtonResume;
            ButtonPauseSurrender.Clicked += OnButtonSurrender;

            ButtonOverMenu.Clicked += OnButtonMenu;
            ButtonOverRevanche.Clicked += OnButtonRevanche;

            ButtonLocalPlay.Clicked += OnButtonLocalPlay;
            ButtonLocalPlayAi.Clicked += OnButtonLocalPlayAi;
            ButtonRanked.Clicked += OnButtonRanked;
            ButtonMapLeft.Clicked += () => ChangeMap((Map)MathHelper.Mod((int)LocalMatchData.Map - 1, Enum.GetNames(typeof(Map)).Length - 1));
            ButtonMapRight.Clicked += () => ChangeMap((Map)MathHelper.Mod((int)LocalMatchData.Map + 1, Enum.GetNames(typeof(Map)).Length - 1));
            ToggleJumpOverNet.Toggled += ChangeJumpOverNet;
            SliderSpeed.Changed += ChangeSpeed;
            ButtonPlayerModeSingle.Clicked += () => ChangePlayerMode(PlayerMode.Single);
            ButtonPlayerModeDouble.Clicked += () => ChangePlayerMode(PlayerMode.Double);
            ButtonPlayerModeDoubleFixed.Clicked += () => ChangePlayerMode(PlayerMode.DoubleFixed);
            ButtonPlayerModeGhost.Clicked += () => ChangePlayerMode(PlayerMode.Ghost);
            ButtonGameModeStandard.Clicked += () => ChangeGameMode(GameMode.Standard);
            ButtonGameModeBomb.Clicked += () => ChangeGameMode(GameMode.Bomb);
            ButtonGameModeTennis.Clicked += () => ChangeGameMode(GameMode.Tennis);
            ButtonGameModeBlitz.Clicked += () => ChangeGameMode(GameMode.Blitz);
            ButtonJumpModeStandard.Clicked += () => ChangeJumpMode(JumpMode.Standard);
            ButtonJumpModeNoJump.Clicked += () => ChangeJumpMode(JumpMode.NoJump);
            ButtonJumpModePogo.Clicked += () => ChangeJumpMode(JumpMode.Pogo);
            ButtonJumpModeSpring.Clicked += () => ChangeJumpMode(JumpMode.Spring);

            ClientConnection.Connecting -= OnConnecting;
            ClientConnection.Disconnected -= OnDisconnected;
            ClientConnection.OverReceived -= OnMatchOver;
            ClientConnection.RematchReceived -= OnRematchReceived;
            ClientConnection.Connected -= OnConnected;

            ClientConnection.Connecting += OnConnecting;
            ClientConnection.Disconnected += OnDisconnected;
            ClientConnection.OverReceived += OnMatchOver;
            ClientConnection.RematchReceived += OnRematchReceived;
            ClientConnection.Connected += OnConnected;
        }

        static void OnButtonRanked()
        {
            _clientState.OnButtonRanked();
        }

        static void OnButtonLocalPlay()
        {
            _clientState.OnButtonLocalPlay();
        }

        static void OnButtonLocalPlayAi()
        {
            _clientState.OnButtonLocalPlayAi();
        }

        static void OnButtonResume()
        {
            _clientState.OnButtonResume();
        }

        static void OnButtonRevanche()
        {
            _clientState.OnButtonRevanche();
        }

        static void OnButtonSurrender()
        {
            _clientState.OnButtonSurrender();
        }

        static void OnButtonMenu()
        {
            _clientState.OnButtonMenu();
        }

        public static void OnEscapeDown()
        {
            _clientState.OnEscapeDown();
        }

        static void OnConnecting(ServerData serverData)
        {
            _clientState.OnConnecting(serverData);
        }

        static void OnConnected()
        {
            _clientState.OnConnected();
        }

        static void OnDisconnected()
        {
            _clientState.OnDisconnected();
        }

        public static void OnMatchOver(Side winner, int scoreLeft, int scoreRight, int time)
        {
            _clientState.OnMatchOver(winner, scoreLeft, scoreRight, time);
        }

        static void OnRematchReceived()
        {
            _clientState.OnRematchReceived();
        }

        public static void OnMatchStopped()
        {
            _clientState.OnMatchStopped();
        }

        public static void OnSlideOver()
        {
            _clientState.OnSlideOver();
        }

        public static void OnBlackoutOver()
        {
            _clientState.OnBlackoutOver();
        }

        public static void OnWhiteoutOver()
        {
            _clientState.OnWhiteoutOver();
        }

        public static void OnZoomOutOver()
        {
            _clientState.OnZoomOutOver();
        }

        #region LocalMatchData

        static void ChangeMap(Map map) => LocalMatchData.Map = map;

        static void ChangeJumpOverNet(bool value) => LocalMatchData.JumpOverNet = value;

        static void ChangeSpeed(int value) => LocalMatchData.TimeScale = (float)value / 100;

        static void ChangePlayerMode(PlayerMode playerMode) => LocalMatchData.PlayerMode = playerMode;

        static void ChangeGameMode(GameMode gameMode) => LocalMatchData.GameMode = gameMode;

        static void ChangeJumpMode(JumpMode jumpMode) => LocalMatchData.JumpMode = jumpMode;

        #endregion

    }
}
