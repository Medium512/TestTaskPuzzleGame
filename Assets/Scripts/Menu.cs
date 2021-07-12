using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestTaskPuzzleGame {
    public class Menu : MonoBehaviour {
        [SerializeField] private Button StartBtn;
        [SerializeField] private Button DeleteBtn;

        public Action onStartGame;
        public Action onDeleteSaves;

        private void Start() {
            StartBtn.onClick.AddListener(() => {
                onStartGame?.Invoke();
            });
            DeleteBtn.onClick.AddListener(() => {
                onDeleteSaves?.Invoke();
            });
        }
    }
}