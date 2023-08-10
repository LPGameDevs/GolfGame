using System;
using Godot;
using Newtonsoft.Json;

namespace GolfGame
{
    public enum WebSocketRequestType
    {
        HostNewGame = 1
    }

    public class WebSocket : Node
    {
        private const bool _secureConnection = false;

        public static event Action OnConnected;
        public static event Action OnDisconnected;
        public static event Action<WebSocketResponse> OnResponseReceived;

        private WebSocketClient _webSocketClient = null;
        private float _pollTime = 1f;
        private float _pollTimer = 0;

        private int _reconnectionAttempts = 5;
        private int _currentReconnectionAttempt = 0;
        private float _reconnectionTime = 3f;
        private float _reconnectionTimer = 0;

        // private const string WebSocketUrl = "wss://5kh2nhf5ql.execute-api.eu-north-1.amazonaws.com/production";
        private const string WebSocketUrl = "ws://localhost:1337";

        private void ConnectToServer()
        {
            // Connect signal events.
            _webSocketClient = new WebSocketClient();
            _webSocketClient.Connect("connection_established", this, nameof(OnConnectionEstablished));
            _webSocketClient.Connect("data_received", this, nameof(OnDataReceived));
            _webSocketClient.Connect("server_close_request", this, nameof(OnServerCloseRequest));
            _webSocketClient.Connect("connection_closed", this, nameof(OnConnectionClosed));
            _webSocketClient.Connect("connection_error", this, nameof(OnConnectionError));

            string[] supportedProtocols = null;
            if (_secureConnection)
            {
                // Additional security precautions.
                supportedProtocols = new string[1] {"some-protocol"};
            }

            Error error = _webSocketClient.ConnectToUrl(WebSocketUrl, supportedProtocols);

            if (error != Error.Ok)
            {
                _webSocketClient.GetPeer(1).Close();
                GD.Print("Error connect to " + WebSocketUrl);
            }
            else
            {
                // We cannot handle binary data exchange.
                _webSocketClient.GetPeer(1).SetWriteMode(WebSocketPeer.WriteMode.Text);
                GD.Print("Starting socket connetion to " + WebSocketUrl);
            }
        }

        private void HandlePolling()
        {
            if (_webSocketClient.GetConnectionStatus() == NetworkedMultiplayerPeer.ConnectionStatus.Disconnected)
            {
                return;
            }

            _webSocketClient.Poll();
        }

        private void HandleReconnection()
        {
            if (_webSocketClient.GetConnectionStatus() != NetworkedMultiplayerPeer.ConnectionStatus.Disconnected)
            {
                return;
            }

            if (_reconnectionTimer > _reconnectionTime)
            {
                _reconnectionTimer = 0;
                AttemptReconnection();
            }


            void AttemptReconnection()
            {
                _currentReconnectionAttempt++;
                if (_currentReconnectionAttempt > _reconnectionAttempts)
                {
                    GD.Print("Max reconnection attempts reached.");
                    return;
                }

                GD.Print("Attempting reconnection. Attempt: " + _currentReconnectionAttempt.ToString());
                ConnectToServer();
            }
        }

        public void SendMessage(RequestData requestData)
        {
            string json = JsonConvert.SerializeObject(requestData);
            Error error = _webSocketClient.GetPeer(1).PutPacket(json.ToUTF8());

            if (!error.Equals(Error.Ok))
            {
                GD.Print("Error sending message to server.");
            }
        }

        #region WebSocketEvents

        public void OnConnectionEstablished(string protocol)
        {
            GD.Print("Connection established.");
            _currentReconnectionAttempt = 0;
            OnConnected?.Invoke();
        }

        public void OnServerCloseRequest(int code, string reason)
        {
            GD.Print("Close request, reason: " + reason + ", code: " + code.ToString());
        }

        public void OnConnectionClosed(bool wasCleanClose)
        {
            GD.Print("Connection closed. was clean close." + wasCleanClose.ToString());
            OnDisconnected?.Invoke();
        }

        public void OnConnectionError()
        {
            GD.Print("Connection error.");
        }

        public void OnDataReceived()
        {
            var packetCount = _webSocketClient.GetPeer(1).GetAvailablePacketCount();
            GD.Print("data received. " + packetCount.ToString() + " packets.");

            // Example only: Handle reciving text from server.
            var packet = _webSocketClient.GetPeer(1).GetPacket();
            bool isString = _webSocketClient.GetPeer(1).WasStringPacket();

            if (isString)
            {
                string msg = packet.GetStringFromUTF8();
                WebSocketResponse webSocketResponse = JsonConvert.DeserializeObject<WebSocketResponse>(msg);
                if (webSocketResponse.message.Length == 0)
                {
                    return;
                }

                OnResponseReceived?.Invoke(webSocketResponse);
                GD.Print("Server sent you text: " + webSocketResponse.message);
            }
        }

        #endregion

        #region GodotHooks

        public override void _Ready()
        {
            ConnectToServer();
        }

        public override void _Process(float delta)
        {
            _pollTimer += delta;
            _reconnectionTimer += delta;

            if (_pollTimer > _pollTime)
            {
                _pollTimer = 0;
                HandlePolling();
                HandleReconnection();
            }
        }

        #endregion

        #region RequestTypes

        public void MakeRequest(WebSocketRequestType type)
        {
            RequestData requestData = new RequestData(type);
            SendMessage(requestData);
        }

        #endregion

        #region DTOs

        [Serializable]
        public class WebSocketResponse
        {
            public string message;
            public string user;
            public string data;
            public WebSocketRequestType RequestType;
        }

        [Serializable]
        public class RequestData
        {
            public string action = "notify";
            public string person;
            public WebSocketRequestType RequestType;

            public RequestData(WebSocketRequestType type, string person = "")
            {
                RequestType = type;
                setActionFromType(type);
                this.person = person;
            }

            private void setActionFromType(WebSocketRequestType type)
            {
                switch (type)
                {
                    case WebSocketRequestType.HostNewGame:
                        action = "host";
                        break;

                    default:
                        action = "notify";
                        break;
                }
            }
        }

        #endregion
    }
}
