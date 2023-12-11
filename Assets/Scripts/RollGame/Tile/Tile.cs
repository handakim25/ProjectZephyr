using UnityEngine;
using UnityEngine.EventSystems;

namespace Zephyr.RollGame.Tile
{
    // @Memo
    // 좌표를 설정하는 부분을 한 곳에서 관리하도록 해야한다.
    // TileMap에서도 설정하고 Tile에서도 설정하고 있으면 관리하기 힘들다.
    // 추후에 개선할 것

    /// <summary>
    /// Tile 움직이는 것을 구현. 추후에 Tile 움직이는 부분은 분리될 수 있다.
    /// </summary>
    // Check
    // https://stackoverflow.com/questions/41391708/how-to-detect-click-touch-events-on-ui-and-gameobjects
    // https://forum.unity.com/threads/onmousedown-with-new-input-system.955053/
    public class Tile : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public TileData TileData;
        public int x;
        public int y;

        public void Init(int x, int y, TileData tileData)
        {
            SetPos(x, y);
            TileData = tileData;
        }

        public void SetPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Tile의 움직임을 처리하기 위한 초기화
        /// </summary>
        /// <param name="camera">Main Camera</param>
        /// <param name="tileMap">Tile들이 속한 TileMap</param>
        /// <param name="setting">Game Settings</param>
        public static void InitTileSetting(Camera camera, TileMap tileMap, RollGameSetting setting)
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
        /// DragThreshold를 체크하기 위한 터치 시작 위치, Local이 아니라 World Position임에 유의
        /// </summary>
        private Vector2 _touchStartWorldPos;
        /// <summary>
        /// 현재 움직이는 방향, Horizontal, Vertical 중 하나
        /// </summary>
        private MoveDir _moveDir = MoveDir.None;
        /// <summary>
        /// 터치로 선택했을 때 Tile의 World Position, 움직임이 끝났을 때 원위치로 돌아오기 위해 저장
        /// </summary>
        private Vector2 _startTileWorldPos;
        /// <summary>
        /// Tile이 움직이고 있는지 여부, 움직임이 완료 되면 false가 된다.
        /// 움직임이 완료되지 않았을 때는 초기의 위치로 돌아온다.
        /// </summary>
        private bool _isMoving = false;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // Tile Select
            // 초기 위치를 저장하여 Drag 되었을 때 Threshold를 측정할 수 있게하고
            // 움직임이 끝났을 때 이동이 안 됬을 때 원위치로 돌아올 수 있게 한다.
            _touchStartWorldPos = s_mainCam.ScreenToWorldPoint(eventData.position);
            _moveDir = MoveDir.None;
            _startTileWorldPos = transform.position;
            _isMoving = TileData.CanMove;
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

        /// <summary>
        /// 초기 위치(_touchStartWorldPos)와 현재 위치(curCursorWorldPos)를 비교하여
        /// 움직임이 시작되었는지 판정
        /// </summary>
        /// <param name="curCursorWorldPos">현재 Cursor의 World Pos</param>
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

        /// <summary>
        /// Cursor에 맞춰서 이동을 처리
        /// </summary>
        /// <param name="curCursorWorldPos">현재 Cursor의 Wolrd 위치</param>
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

        /// <summary>
        /// Snap 여부를 판정
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Snap이 완료됬을 때 이동 처리
        /// </summary>
        private void MoveTile()
        {
            _isMoving = false;
            (int nextX, int nextY) = GetNextPos(_startTileWorldPos, x, y);

            if (s_tileMap.MoveToTile(nextX, nextY, gameObject))
            {
                // 자신의 데이터는 자기 자신이 Update
                SetPos(nextX, nextY);
            }
            else
            {
                // Return to original Pos
                transform.position = _startTileWorldPos;
            }
        }

        /// <summary>
        /// 현재 위치와 기준저을 기준점으로 TileMap에서 다음 위치를 계산
        /// </summary>
        /// <param name="originWorldPos">Tile 움직임이 시작된 위치</param>
        /// <param name="curX">현재 X값</param>
        /// <param name="curY">현재 Y값</param>
        /// <returns>가장 가까운 다음 TileMap에서의 위치</returns>
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
            // 드래그가 끝났을 때 Tile 이동이 없었으면 원래의 위치로 돌아온다.
            if(_isMoving)
            {
                transform.position = _startTileWorldPos;
            }
        }
    }

    /// <summary>
    /// Tile의 움직임 방향
    /// </summary>
    public enum MoveDir
    {
        None,
        Horizontal,
        Vertical,
    }
}