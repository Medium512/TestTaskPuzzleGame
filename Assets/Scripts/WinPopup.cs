using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTaskPuzzleGame {
    public class WinPopup : MonoBehaviour {
        [SerializeField] private Button StartBtn;
        [SerializeField] private TextMeshProUGUI TittleTxt;

        public Action onStartClicked;
        private void Start() {
            StartBtn.onClick.AddListener(() => {
                onStartClicked?.Invoke();
            });
        }

        public void Init(int level) {
            if (level == GameConsts.TOTAL_LEVELS) {
                TittleTxt.text = GameConsts.WinGameCongratulation;
            }
            else {
                TittleTxt.text = GameConsts.WinLevelCongratulation.Replace("{0}", level.ToString());
            }
        }
    }
}