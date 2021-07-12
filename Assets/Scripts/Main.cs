using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization.Json;
using UnityEngine.Networking;


namespace TestTaskPuzzleGame {
    enum GameState {
        Menu,
        Game,
        Win
    } 
    
    public class Main : MonoBehaviour {
        [SerializeField] private Menu menu;
        [SerializeField] private Game game;
        [SerializeField] private WinPopup winPopup;
        
        private int _level;
        
        private void Start() {
            menu.onStartGame = StartGame;
            menu.onDeleteSaves = DeleteSaves;
            game.onWin = OnWin;
            game.onBackBtnClicked = ShowMenu;
            winPopup.onStartClicked = OnStartClicked;

            LoadGame();
        }

        private void LoadGame() {
            using (FileStream fs = new FileStream (Application.persistentDataPath + "/save.sav", FileMode.OpenOrCreate)) {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                _level = array.Length == 0 ? 1 : Int32.Parse(Encoding.Default.GetString(array));
                
                ShowMenu();
            }
        }

        private void SaveGame(int level) {
            using (FileStream fs = new FileStream (Application.persistentDataPath + "/save.sav", FileMode.OpenOrCreate)) {
                byte[] array = Encoding.Default.GetBytes(level.ToString());
                fs.Write(array, 0, array.Length);
            }
        }
        
        private void DeleteSaves() {
            SaveGame(1);
            _level = 1;
        }

        private void ShowMenu() {
            SetGameState(GameState.Menu);
        }

        private void SetGameState(GameState state) {
            switch (state) {
                case GameState.Menu:
                    menu.gameObject.SetActive(true);
                    game.gameObject.SetActive(false);
                    winPopup.gameObject.SetActive(false);
                    break;
                case GameState.Game:
                    game.gameObject.SetActive(true);
                    game.SetTopPanelTitle("Уровень " + _level);
                    menu.gameObject.SetActive(false);
                    winPopup.gameObject.SetActive(false);
                    break;
                case GameState.Win:
                    winPopup.gameObject.SetActive(true);
                    winPopup.Init(_level);
                    menu.gameObject.SetActive(false);
                    game.gameObject.SetActive(false);
                    break;
            }
        }

        private void StartGame() {
            SetGameState(GameState.Game);
            StartCoroutine(LoadJson(_level));
        }
        
        private IEnumerator LoadJson (int level) {
            string jsonString;
            string filePath = Path.Combine (Application.streamingAssetsPath, "level" + level + ".json");
            if (Application.platform == RuntimePlatform.Android) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                if (!www.isNetworkError && !www.isHttpError) {
                    jsonString = www.downloadHandler.text;
                    jsonString = jsonString.Remove(0, 1);
                    ParseJson(jsonString);
                }
            }
            else {
                jsonString = File.ReadAllText (filePath);
                ParseJson(jsonString);
            }
        }
        
        private void ParseJson(string jsonString) {
            using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(jsonString))) {
                memoryStream.Position = 0;
                var serializer = new DataContractJsonSerializer(typeof(Level));
                game.InitLevel((Level)serializer.ReadObject(memoryStream));
            }
        }

        private void OnStartClicked() {
            if (_level == GameConsts.TOTAL_LEVELS) {
                SetGameState(GameState.Menu);
            }
            else {
                _level++;
                StartGame();
            }
        }

        private void OnWin() {
            SetGameState(GameState.Win);
            SaveGame(Math.Min(_level + 1, GameConsts.TOTAL_LEVELS));
        }
    }
}