using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ管理クラス
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] public Transform homeTransform; // 家の位置情報
    [SerializeField] public Transform seaTransform;  // 海の位置情報
    [SerializeField] public GameObject fishPrefab;   // 魚Prefab
}
