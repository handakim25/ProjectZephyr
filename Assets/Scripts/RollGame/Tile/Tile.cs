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
    public class Tile : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field : SerializeField] public TileData TileData { get; private set; }

        static Camera s_mainCam = null;

        // PointerEvnetData : ScreenSpace, 좌하단이 (0,0)인 좌표계
        // 이동 관련 처리할 때 Screen Space, World Space에 주의할 것
        public void OnPointerClick(PointerEventData eventData)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            Debug.Log($"OnPointerClick: {worldPos}");
        }

        private Vector3 _startWorldMousePos;
        private MoveDir _moveDir = MoveDir.None;
        public void OnBeginDrag(PointerEventData eventData)
        {
            s_mainCam = s_mainCam != null ? s_mainCam : Camera.main;
            _moveDir = MoveDir.None;
            _startWorldMousePos = s_mainCam.ScreenToWorldPoint(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Drag Threshold를 이용해도 되지만
            // Snap이 진행이 되었을 경우, 다시 Threshold를 측정해야 되므로
            // 직접 Threshold를 계산하고 적용한다.
            Vector3 curWorldMousePos = s_mainCam.ScreenToWorldPoint(eventData.position);
            if(_moveDir == MoveDir.None)
            {
                Vector3 deltaWorld = curWorldMousePos - _startWorldMousePos;
                if(deltaWorld.magnitude < 0.1f)
                {
                    return;
                }
                _moveDir = Mathf.Abs(deltaWorld.x) > Mathf.Abs(deltaWorld.y) ? MoveDir.Horizontal : MoveDir.Vertical;
            }

            if(_moveDir == MoveDir.Horizontal)
            {
                transform.position = new Vector3(curWorldMousePos.x, transform.position.y, transform.position.z);
            }
            else if(_moveDir == MoveDir.Vertical)
            {
                transform.position = new Vector3(transform.position.x, curWorldMousePos.y, transform.position.z);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
        }
    }

    public enum MoveDir
    {
        None,
        Horizontal,
        Vertical,
    }
}