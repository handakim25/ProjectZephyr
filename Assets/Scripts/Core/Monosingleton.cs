using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr
{
    /// <summary>
    /// Monbehaviour를 상속 받은 싱글톤 클래스
    /// 일반적으로 생성이 되어 있어야 한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance = null;
        private static bool appIsClosing = false;

        /// <summary>
        /// 싱글톤 인스턴스를 반환한다.
        /// 만약 인스턴스가 없다면, 씬에서 찾아서 할당한다.
        /// 만약 씬에도 없다면, 씬에 생성한다.
        /// </summary>
        public static T Instance
        {
            get
            {
                if(appIsClosing)
                {
                    return null;
                }
                if(s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                    if(s_instance == null)
                    {
                        Debug.LogWarning($"No instance of {typeof(T)}. Temporally create instance.");
                        GameObject go = new GameObject(typeof(T).ToString());
                        s_instance = go.AddComponent<T>();
                    }
                }

                return s_instance;
            }
        }


        /// <summary>
        /// Singleton을 유지하기 위해 Awake에서 null인지 체크하고, null이면 자기 자신을 할당한다.
        /// </summary>
        private void Awake()
        {
            if(s_instance == null)
            {
                s_instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// App이 종료될 때는 더 이상 싱글톤을 유지할 필요가 없다.
        /// </summary>
        private void OnApplicationQuit()
        {
            s_instance = null;
            appIsClosing = true;
        }
    }
}
