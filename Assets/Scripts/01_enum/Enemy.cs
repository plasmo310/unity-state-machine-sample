using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample01
{
    /// <summary>
    /// Enemyクラス
    /// enumを使用したステート遷移
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステージ管理クラス
        /// </summary>
        [SerializeField] private StageManager stageManager;
        
        /// <summary>
        /// ステート
        /// </summary>
        private enum StateType
        {
            MoveSea,  // 海へ移動
            Hunting,  // 魚採取
            MoveHome, // 家へ移動
            Eating,   // 食事
        }
        private StateType _state = StateType.MoveSea;     // 現在のステート
        private StateType _nextState = StateType.MoveSea; // 次のステート

        private void Awake()
        {
            Debug.Log("start sample01!!");
        }

        private void Start()
        {
            // 最初のステートを開始する
            MoveSeaStart();
        }

        private void Update()
        {
            // 現在のステートのUpdateを呼び出す
            switch (_state)
            {
                case StateType.MoveSea:
                    MoveSeaUpdate();
                    break;
                case StateType.Hunting:
                    HuntingUpdate();
                    break;
                case StateType.MoveHome:
                    MoveHomeUpdate();
                    break;
                case StateType.Eating:
                    EatingUpdate();
                    break;
            }

            // ステートが切り替わったら
            if (_state != _nextState)
            {
                // 終了処理を呼び出して
                switch (_state)
                {
                    case StateType.MoveSea:
                        MoveSeaEnd();
                        break;
                    case StateType.Hunting:
                        HuntingEnd();
                        break;
                    case StateType.MoveHome:
                        MoveHomeEnd();
                        break;
                    case StateType.Eating:
                        EatingEnd();
                        break;
                }

                // 次のステートに遷移する
                _state = _nextState;
                switch (_state)
                {
                    case StateType.MoveSea:
                        MoveSeaStart();
                        break;
                    case StateType.Hunting:
                        HuntingStart();
                        break;
                    case StateType.MoveHome:
                        MoveHomeStart();
                        break;
                    case StateType.Eating:
                        EatingStart();
                        break;
                }
            }
        }

        /// <summary>
        /// 遷移先のステート設定
        /// </summary>
        /// <param name="nextState">次のステート</param>
        private void ChangeState(StateType nextState)
        {
            _nextState = nextState;
        }

        // 各ステート処理
        // ----- move sea -----
        private void MoveSeaStart()
        {
            Debug.Log("start move sea");
        }

        private void MoveSeaUpdate()
        {
            var enemyPosition = transform.position;
            var targetPosition = stageManager.seaTransform.position;
            // 海へ到着したら次のステートへ
            if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
            {
                ChangeState(StateType.Hunting);
                return;
            }
            // 海へ向かう
            transform.position = Vector3.MoveTowards(
                enemyPosition,
                targetPosition, 
                5.0f * Time.deltaTime);
            transform.LookAt(targetPosition);
        }

        private void MoveSeaEnd()
        {
            Debug.Log("end move sea");
        }
        
        // ----- hunting -----
        private void HuntingStart()
        {
            Debug.Log("start hunting");
            // 採取スタート
            _isFinishHunting = false;
            StartCoroutine(HuntCoroutine());
        }

        private void HuntingUpdate()
        {
            // 採取が完了したら次のステートへ
            if (_isFinishHunting)
            {
                ChangeState(StateType.MoveHome);
            }
        }

        private void HuntingEnd()
        {
            Debug.Log("end hunting");
        }
        
        // 採取が完了しているか？
        private bool _isFinishHunting;
        
        // 採取コルーチン
        private IEnumerator HuntCoroutine()
        {
            // 狩猟中、数秒待機
            yield return new WaitForSeconds(2.0f);

            // 魚取得
            Instantiate(stageManager.fishPrefab, transform);
            
            // 狩猟完了
            _isFinishHunting = true;
        }
        
        // ----- move home -----
        private void MoveHomeStart()
        {
            Debug.Log("start move home");
        }

        private void MoveHomeUpdate()
        {
            var enemyPosition = transform.position;
            var targetPosition = stageManager.homeTransform.position;
            // 家へ到着したら次のステートへ
            if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
            {
                ChangeState(StateType.Eating);
                return;
            }
            // 家へ向かう
            transform.position = Vector3.MoveTowards(
                enemyPosition,
                targetPosition, 
                5.0f * Time.deltaTime);
            transform.LookAt(targetPosition);
        }

        private void MoveHomeEnd()
        {
            Debug.Log("end move home");
        }
        
        // ----- eating -----
        private void EatingStart()
        {
            Debug.Log("start eating");
            // 食事開始
            _isFinishEating = false;
            StartCoroutine(EatCoroutine());
        }

        private void EatingUpdate()
        {
            // くるくる周る
            transform.Rotate(Vector3.up * 500.0f * Time.deltaTime);
                
            // 食事が完了したら次のステートへ
            if (_isFinishEating)
            {
                ChangeState(StateType.MoveSea);
            }
        }

        private void EatingEnd()
        {
            Debug.Log("end eating");
        }
        
        // 食事が完了しているか？
        private bool _isFinishEating;
            
        // 食事コルーチン
        private IEnumerator EatCoroutine()
        {
            // 食事中、数秒待機
            yield return new WaitForSeconds(5.0f);
                
            // 子オブジェクト(魚)を破棄
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            // 食事完了
            _isFinishEating = true;
        }
    }
}