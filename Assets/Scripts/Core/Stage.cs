using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr
{
    /// <summary>
    /// 게임의 스테이지를 나타내는 클래스.
    /// 추후에는 Json으로 직렬화될 예정
    /// </summary>
    public abstract class Stage : ScriptableObject
    {
        public string StageName;

#if UNITY_EDITOR
        [TextArea]
        public string EditorDescription;
#endif
    }
}
