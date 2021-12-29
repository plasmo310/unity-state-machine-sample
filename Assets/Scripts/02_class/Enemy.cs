using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sample02
{
    /// <summary>
    /// Enemyクラス
    /// classを使用したステート遷移
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
        private readonly StateMoveSea _stateMoveSea = new StateMoveSea();
        private readonly StateHunting _stateHunting = new StateHunting();
        private readonly StateMoveHome _stateMoveHome = new StateMoveHome();
        private readonly StateEating _stateEating = new StateEating();
        private enum StateType
        {
            MoveSea,  // 海へ移動
            Hunting,  // 魚採取
            MoveHome, // 家へ移動
            Eating,   // 食事
        }
        private StateBase GetState(StateType state)
        {
            switch (state)
            {
                case StateType.MoveSea:
                    return _stateMoveSea;
                case StateType.Hunting:
                    return _stateHunting;
                case StateType.MoveHome:
                    return _stateMoveHome;
                case StateType.Eating:
                    return _stateEating;
                default:
                    Debug.Log("not state!!");
                    return null;
            }
        }
        private StateBase _state; // 現在のステート

        private void Awake()
        {
            Debug.Log("start sample02!!");
        }

        private void Start()
        {
            _state = _stateMoveSea;
            _state.OnStart(this);
        }
        
        private void Update()
        {
            _state.OnUpdate(this);
        }

        /// <summary>
        /// ステート切替処理
        /// </summary>
        /// <param name="nextState"></param>
        private void ChangeState(StateType nextState)
        {
            _state.OnEnd(this);
            _state = GetState(nextState);
            _state.OnStart(this);
        }

        // 各ステート処理
        // --- state基底クラス -----
        private class StateBase
        {
            public virtual void OnStart(Enemy owner) { }

            public virtual void OnUpdate(Enemy owner) { }

            public virtual void OnEnd(Enemy owner) { }
        }
        
        // ----- move sea -----
        private class StateMoveSea : StateBase
        {
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start move sea");
            }

            public override void OnUpdate(Enemy owner)
            {
                var enemyPosition = owner.transform.position;
                var targetPosition = owner.stageManager.seaTransform.position;
                // 海へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    owner.ChangeState(StateType.Hunting);
                    return;
                }
                // 海へ向かう
                owner.transform.position = Vector3.MoveTowards(
                    enemyPosition,
                    targetPosition, 
                    5.0f * Time.deltaTime);
                owner.transform.LookAt(targetPosition);
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end move sea");
            }
        }

        // ----- hunting -----
        private class StateHunting : StateBase
        {
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start hunting");
                // 採取スタート
                _isFinishHunting = false;
                MonoBehaviorHandler.StartStaticCoroutine(HuntCoroutine(owner));
            }

            public override void OnUpdate(Enemy owner)
            {
                // 採取が完了したら次のステートへ
                if (_isFinishHunting)
                {
                    owner.ChangeState(StateType.MoveHome);
                }
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end hunting");
            }
            
            // 採取が完了しているか？
            private bool _isFinishHunting;
            
            // 採取コルーチン
            private IEnumerator HuntCoroutine(Enemy owner)
            {
                // 狩猟中、数秒待機
                yield return new WaitForSeconds(2.0f);

                // 魚取得
                Instantiate(owner.stageManager.fishPrefab, owner.transform);
            
                // 狩猟完了
                _isFinishHunting = true;
            }
        }

        // ----- move home -----
        private class StateMoveHome : StateBase
        {
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start move home");
            }

            public override void OnUpdate(Enemy owner)
            {
                var enemyPosition = owner.transform.position;
                var targetPosition = owner.stageManager.homeTransform.position;
                // 家へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    owner.ChangeState(StateType.Eating);
                    return;
                }
                // 家へ向かう
                owner.transform.position = Vector3.MoveTowards(
                    enemyPosition,
                    targetPosition, 
                    5.0f * Time.deltaTime);
                owner.transform.LookAt(targetPosition);
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end move home");
            }
        }

        // ----- eating -----
        private class StateEating : StateBase
        {
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start eating");
                // 食事開始
                _isFinishEating = false;
                MonoBehaviorHandler.StartStaticCoroutine(EatCoroutine(owner));
            }

            public override void OnUpdate(Enemy owner)
            {
                // くるくる周る
                owner.transform.Rotate(Vector3.up * 500.0f * Time.deltaTime);
                
                // 食事が完了したら次のステートへ
                if (_isFinishEating)
                {
                    owner.ChangeState(StateType.MoveSea);
                }
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end eating");
            }
            
            // 食事が完了しているか？
            private bool _isFinishEating;
            
            // 食事コルーチン
            private IEnumerator EatCoroutine(Enemy owner)
            {
                // 食事中、数秒待機
                yield return new WaitForSeconds(5.0f);
                
                // 子オブジェクト(魚)を破棄
                foreach (Transform child in owner.transform)
                {
                    Destroy(child.gameObject);
                }
            
                // 食事完了
                _isFinishEating = true;
            }
        }
    }
}