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
    
    // *以下はSample05のみ使用
    [SerializeField] public Transform homeHoneyTransform; // 家ハニーの位置情報
    [SerializeField] public Transform walkTransform;      // 散歩ポイントの位置情報
    [SerializeField] public Sample05.Honey honey; // ハニー
}
