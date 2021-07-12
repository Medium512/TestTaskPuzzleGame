using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TestTaskPuzzleGame {
    public class Part : MonoBehaviour {
        private bool _isDragged = false;
        private List<Cell> _cells = new List<Cell>();
        
        public Vector2 _startPosition;
        public Action checkWin;
        public Action<Transform> swapPart;
        
        public void Init(List<List<int>> part, Cell cell) {
            GetComponent<GridLayoutGroup>().constraintCount = part[0].Count;
            
            for (var i = 0; i < part.Count; i++) {
                for (var j = 0; j < part[i].Count; j++) {
                    var item = Instantiate(cell, transform);
                    item.Init(part[i][j] == 1);
                    if (part[i][j] == 1) {
                        item.onTap = OnTap;
                        _cells.Add(item);
                    }
                }
            }
        }

        public void SetPartColor(Color color) {
            foreach (var item in _cells) {
                item.SetColor(color);
            }
        }

        private void OnTap(bool value) {
            if (value) {
                swapPart?.Invoke(transform);
                transform.localScale = new Vector3(1, 1);
                _isDragged = true;
                foreach (var item in _cells) {
                    if (item._cellAviableToSet != null) {
                        item._cellAviableToSet._isCellSetted = false;
                    }
                }
            }
            else {
                _isDragged = false;
                if (CheckCells()) {
                    SetPart();
                }
                else {
                    transform.localScale = new Vector3(0.35f, 0.35f);
                    transform.DOMove(_startPosition, 0.3f);
                    foreach (var item in _cells) {
                        if (item._cellAviableToSet != null) {
                            item._cellAviableToSet = null;
                        }
                    }
                }
            }
        }

        private void Update() {
            if (_isDragged) {
                transform.position = Input.mousePosition;
            }
        }

        private bool CheckCells() {
            bool result = true;
            foreach (var item in _cells) {
                if (item._isSetAviable) {
                    if (item._cellAviableToSet == null) {
                        result = false;
                    }
                    else if (item._cellAviableToSet._isCellSetted) {
                        result = false;
                    }
                }
                else {
                    result = false;
                }
            }

            return result;
        }

        public void Reset() {
            _cells = new List<Cell>();
            transform.localScale = new Vector3(0.35f, 0.35f);
            transform.position = _startPosition;
        } 

        private void SetPart() {
            foreach (var item in _cells) {
                item.transform.DOMove(item._cellAviableToSet.transform.position, 0.1f);
                item._cellAviableToSet._isCellSetted = true;
            }
            
            checkWin?.Invoke();
        }
    }
}