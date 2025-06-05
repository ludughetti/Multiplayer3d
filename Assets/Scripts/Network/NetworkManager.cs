using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Network
{
    public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, INetworkRunnerCallbacks
    {
        [Header("Player Settings")]
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private List<Transform> playerSpawnPositions;
        
        [Header("Player Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        
        private readonly Dictionary<PlayerRef, NetworkObject> _activePlayers = new ();
        private NetworkRunner _runner;
        
        public PlayerController LocalPlayer { get; set; }
        
        // ---------------------- Start game ---------------------- //
        private async void Start()
        {
            var sessionStarted = await StartGameSession();

            if (!sessionStarted)
                Debug.LogError("Could not start game session!");
        }
        
        private async Task<bool> StartGameSession()
        {
            Debug.Log("Starting game session...");
            var networkRunnerObject = new GameObject(typeof(NetworkRunner).Name, typeof(NetworkRunner));

            Debug.Log("Getting network runner...");
            _runner = networkRunnerObject.GetComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
            
            Debug.Log("Setting up game arguments...");
            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "MultiPicoPark-Test",
                SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                PlayerCount = playerSpawnPositions.Count,
                Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
            };

            Debug.Log("Starting game...");
            var startTask = _runner.StartGame(startGameArgs);
            await startTask;

            Debug.Log("Game started!");
            return startTask.Result.Ok;
        }
        
        // ---------------------- Stop game ---------------------- //
        
        private void OnApplicationQuit()
        {
            Shutdown();
        }
        
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            if (_runner.IsClient)
                Shutdown();
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                DespawnPlayer(runner, player);

                if (_activePlayers.Count == 0)
                    Shutdown();
            }
        }
        
        private void Shutdown()
        {
            if (_runner)
                _runner.Shutdown();
        }
        
        private void DespawnPlayer(NetworkRunner runner, PlayerRef player)
        {
            if (!_activePlayers.TryGetValue(player, out var activePlayer)) return;
            
            runner.Despawn(activePlayer);
            _activePlayers.Remove(player);
        }
        
        // ---------------------- On new connection ---------------------- //
        
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("Connected to server!");
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("New player joined: " + player.PlayerId);
            if (runner.IsServer)
            {
                SpawnNewPlayer(runner, player);
            }
        }
        
        private void SpawnNewPlayer(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Spawning new player...");
            var spawnPosition = GetRandomSpawnPosition();
            var networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            _activePlayers[player] = networkPlayerObject;
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            if (playerSpawnPositions == null || playerSpawnPositions.Count == 0)
                return Vector3.zero; // fallback or handle no positions available

            var randomIndex = UnityEngine.Random.Range(0, playerSpawnPositions.Count);
            return playerSpawnPositions[randomIndex].position;
        }
        
        // ---------------------- On player input ---------------------- //
        
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!LocalPlayer) return;
            
            var move = moveAction.action.ReadValue<Vector2>().normalized;
            input.Set(new NetworkInputData { Move = move });
        }
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}
