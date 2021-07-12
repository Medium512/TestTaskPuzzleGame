using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TestTaskPuzzleGame {
    public class Game : MonoBehaviour {
        [SerializeField] private GameObject Template;
        [SerializeField] private GameObject Parts;
        [SerializeField] private GameObject TopPanel;
        
        [Header("References")]
        [SerializeField] private Cell _cell;
        
        private Level _currentLevel;

        public Action onWin;
        public Action onBackBtnClicked;

        private void Start() {
            TopPanel.transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(() => {
                ResetGame();
                onBackBtnClicked?.Invoke();
            });
        }

        public void SetTopPanelTitle(string text) {
            TopPanel.transform.Find("TitleTxt").GetComponent<TextMeshProUGUI>().text = text;
        }

        public void InitLevel(Level level) {
            _currentLevel = level;
            InitTemplate();
            OptimizeParts();
            InitParts();
        }

        private void InitTemplate() {
            Template.GetComponent<GridLayoutGroup>().constraintCount = _currentLevel.template[0].Count;
            for (var i = 0; i < _currentLevel.template.Count; i++) {
                for (var j = 0; j < _currentLevel.template[i].Count; j++) {
                    var item = Instantiate(_cell, Template.transform);
                    item.Init(_currentLevel.template[i][j] == 1);
                    item.name = "CellTemplate";
                }
            }
        }

        private void InitParts() {
            for (var i = 0; i < GameConsts.MAX_PARTS; i++) {
                var item = Parts.transform.Find("Part" + i);
                item.GetComponent<Part>()._startPosition = item.position;
                
                if(i >= _currentLevel.parts.Count) return;
                
                var part = item.GetComponent<Part>();
                part.Init(_currentLevel.parts[i], _cell);
                part.SetPartColor(new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), 1));
                part.checkWin = CheckWin;
                part.swapPart = SwapPart;
            }
        }

        private void SwapPart(Transform obj) {
            obj.SetSiblingIndex(GameConsts.MAX_PARTS - 1);
        }

        private void CheckWin() {
            var isWin = true;
            foreach (Transform cell in Template.transform) {
                var cellc = cell.GetComponent<Cell>();
                if (!cellc._isCellSetted && cellc._isActive) {
                    isWin = false;
                }
            }

            if (isWin) {
                onWin?.Invoke();
                ResetGame();
            }
        }

        private void ResetGame() {
            foreach (Transform cell in Template.transform) {
                Destroy(cell.gameObject);
            }

            foreach (Transform part in Parts.transform) {
                part.GetComponent<Part>().Reset();
                foreach (Transform cell in part) {
                    Destroy(cell.gameObject);
                }
            }
        }

        private void OptimizeParts() {
            int count = 0;

            foreach (var part in _currentLevel.parts) { // delete empty rows
                for (int row = part.Count - 1; row >= 0; row--) {
                    var emptyRow = true;
                    foreach (var i in part[row]) {
                        if (i != 0) emptyRow = false;
                    }

                    if (emptyRow) {
                        _currentLevel.parts[count].Remove(part[row]);
                    }
                }
                count++;
            }
            
            count = 0;
            foreach (var part in _currentLevel.parts) { // delete empty rows
                List<int> rowFilling = new List<int>();
                var index = 0;
            
                for (int i = 0; i < part[0].Count; i++) {
                    bool colFilled = false;
                    foreach (var row in part) {
                        if (row[index] == 1) {
                            colFilled = true;
                        }
                    }

                    rowFilling.Add(colFilled ? 1 : 0);
                    index++;
                }
                count++;

                for (int i = rowFilling.Count - 1; i >= 0; i--) {
                    if (rowFilling[i] == 0) {
                        foreach (var row in part) {
                            row.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}