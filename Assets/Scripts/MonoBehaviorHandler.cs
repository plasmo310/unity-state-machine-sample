using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviourHandlerクラス
/// 非MonoBehaviorクラスはこの関数を使用する
/// </summary>
public class MonoBehaviorHandler : MonoBehaviour
{
    private static MonoBehaviorHandler _mInstance;
    private static MonoBehaviorHandler Instance
    {
        get
        {
            if (_mInstance == null)
            {
                var o = new GameObject("MonoBehaviorHandler");
                DontDestroyOnLoad(o);
                _mInstance = o.AddComponent<MonoBehaviorHandler>();
            }
            return _mInstance;
        }
    }

    public void OnDisable()
    {
        if(_mInstance)
            Destroy(_mInstance.gameObject);
    }

    /// <summary>
    /// コルーチン実行
    /// </summary>
    /// <param name="coroutine"></param>
    public static void StartStaticCoroutine(IEnumerator coroutine)
    {
        Instance.StartCoroutine(coroutine);
    }
}
