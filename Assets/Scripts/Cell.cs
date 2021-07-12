using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestTaskPuzzleGame {
    public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField] private Image BG;
        
        private Vector2 _gridPosition;

        public bool _isActive;
        public bool _isSetAviable = false;
        public bool _isCellSetted = false;
        public Cell _cellAviableToSet;
        public Action<bool> onTap;

        private Color _color;
        
        public void Init(bool isActive) {
            _isActive = isActive;
            BG.gameObject.SetActive(isActive);
            GetComponent<BoxCollider2D>().isTrigger = !isActive;
            _cellAviableToSet = null;
        }

        public void SetColor(Color color) {
            BG.color = color;
            _color = color;
        }

        public void OnPointerDown(PointerEventData eventData) {
            onTap?.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            onTap?.Invoke(false);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (name != "CellTemplate") {
                _isSetAviable = true;
                if (other.gameObject.name == "CellTemplate") {
                    if (!other.gameObject.GetComponent<Cell>()._isCellSetted) {
                        _cellAviableToSet = other.gameObject.GetComponent<Cell>();
                    }
                }
            }
        }
        
        private void OnCollisionExit2D(Collision2D other) {
            if (name != "CellTemplate") {
                _isSetAviable = false;
                
                if (other.gameObject.name == "CellTemplate") {
                    _cellAviableToSet = null;
                }
            }
        }
    }
}