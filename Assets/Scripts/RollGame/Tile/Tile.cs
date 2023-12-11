using UnityEngine;
using UnityEngine.EventSystems;

namespace Zephyr.RollGame.Tile
{
    /// <summary>
    /// Tile 움직이는 것을 구현. 추후에 Tile 움직이는 부분은 분리될 수 있다.
    /// </summary>
    // Check
    // https://stackoverflow.com/questions/41391708/how-to-detect-click-touch-events-on-ui-and-gameobjects
    // https://forum.unity.com/threads/onmousedown-with-new-input-system.955053/
    public class Tile : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public int x;
        public int y;

        public void SetPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static void Init(Camera camera, TileMap tileMap, RollGameSetting setting)
        {
            s_mainCam = camera;
            s_tileMap = tileMap;
            s_setting = setting;
        }

        static Camera s_mainCam = null;
        static TileMap s_tileMap = null;
        static RollGameSetting s_setting = null;

        // Tile 처리
        // 1. 처음 이동의 경우 입력 위치를 감지해서 일정 거리 이상 움직였을 때 움직이는 것으로 판정
        // 2. 움직임이 시작됬을 때는 해당 위치로 이동
        // 3. 움직임을 취소할 수는 없다. 그러기 위해서는 한 번 터치를 떼야한다.

        /// <summary>
        /// DragThreshold를 체크하기 위한 터치 시작 위치
        /// </summary>
        private Vector2 _touchStartWorldPos;
        /// <summary>
        /// 현재 움직이는 방향
        /// </summary>
        private MoveDir _moveDir = MoveDir.None;
        /// <summary>
        /// 터치로 선택했을 때 Tile의 World Position
        /// </summary>
        private Vector2 _startTileWorldPos;
        /// <summary>
        /// Tile이 움직이고 있는지 여부, 움직임이 완료 되면 false가 된다.
        /// </summary>
        private bool _isMoving = false;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _touchStartWorldPos = s_mainCam.ScreenToWorldPoint(eventData.position);
            _moveDir = MoveDir.None;
            _startTileWorldPos = transform.position;
            _isMoving = true;
            Debug.Log($"OnPointerDown: {_touchStartWorldPos}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(_isMoving == false)
            {
                return;
            }

            // eventData.delta는 값이 정밀하지 않으므로 더 정확한 판정을 위해서 직접 delta를 계산
            Vector2 curCursorWorldPos = s_mainCam.ScreenToWorldPoint(eventData.position);
            UpdateMoveState(curCursorWorldPos);

            if (_moveDir != MoveDir.None)
            {
                Move(curCursorWorldPos);
                if(CheckSnap())
                {
                    MoveTile();
                }
            }
        }

        private void UpdateMoveState(Vector2 curCursorWorldPos)
        {
            if(_moveDir != MoveDir.None)
            {
                return;
            }

            Vector2 delta = curCursorWorldPos - _touchStartWorldPos;
            if (delta.magnitude > s_setting.TileMoveThreshold)
            {
                _moveDir = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? MoveDir.Horizontal : MoveDir.Vertical;
            }
        }

        private void Move(Vector2 curCursorWorldPos)
        {
            Vector2 newPos = _startTileWorldPos;
            if(_moveDir == MoveDir.Horizontal)
            {
                newPos.x = curCursorWorldPos.x;
            }
            else if(_moveDir == MoveDir.Vertical)
            {
                newPos.y = curCursorWorldPos.y;
            }
            transform.position = newPos;
        }

        private bool CheckSnap()
        {
            Vector2 curPos = transform.position;
            Vector2 delta = curPos - _startTileWorldPos;
            if(delta.magnitude > s_setting.TileSnapThreshold)
            {
                return true;
            }
            return false;
        }

        private void MoveTile()
        {
            _isMoving = false;
            (int nextX, int nextY) = GetNextPos(_startTileWorldPos, x, y);

            Debug.Log($"MoveTile: {x}, {y} -> {nextX}, {nextY}");
            var tile = s_tileMap.GetTile(x, y);
            Debug.Log($"TileMap tile : {tile}");

            if (s_tileMap.MoveToTile(nextX, nextY, gameObject))
            {
                SetPos(nextX, nextY);
            }
            else
            {
                // Return to original Pos
                Debug.Log($"Fail to move, return to original pos: {x}, {y}");
                transform.position = _startTileWorldPos;
            }
        }

        private (int, int) GetNextPos(Vector2 originWorldPos, int curX, int curY)
        {
            Vector2 delta = (Vector2)transform.position - originWorldPos;
            int nextX = curX;
            int nextY = curY;
            switch (_moveDir)
            {
                case MoveDir.Horizontal:
                    nextX += (delta.x > 0) ? 1 : -1;
                    break;
                case MoveDir.Vertical:
                    nextY += (delta.y > 0) ? 1 : -1;
                    break;
            }
            return (nextX, nextY);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(_isMoving)
            {
                transform.position = _startTileWorldPos;
            }
        }
    }

    public enum MoveDir
    {
        None,
        Horizontal,
        Vertical,
    }
}