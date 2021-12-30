using System;
using System.Collections;
using UnityEngine;

namespace Sample04
{
    using StateBase = StateMachine<Enemy>.StateBase;
    
    /// <summary>
    /// Enemyクラス
    /// StateMachine(状態遷移版)
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステージ管理クラス
        /// </summary>
        [SerializeField] private StageManager stageManager;

        /// <summary>
        /// ステート定義
        /// </summary>
        private enum EventType
        {
            EatFinish,  // 食事終了
            ArriveSea,  // 海に到着
            HuntFinish, // 採取完了
            ArriveHome, // 家に到着
        }
        private StateMachine<Enemy> _stateMachine;
        
        private void Awake()
        {
            Debug.Log("start sample04!!");
        }

        private void Start()
        {
            // ステートマシン定義
            _stateMachine = new StateMachine<Enemy>(this);
            _stateMachine.AddTransition<StateEating, StateMoveSea>((int) EventType.EatFinish);
            _stateMachine.AddTransition<StateMoveSea, StateHunting>((int) EventType.ArriveSea);
            _stateMachine.AddTransition<StateHunting, StateMoveHome>((int) EventType.HuntFinish);
            _stateMachine.AddTransition<StateMoveHome, StateEating>((int) EventType.ArriveHome);
            // ステート開始
            _stateMachine.OnStart<StateMoveSea>();
        }

        private void Update()
        {
            // ステート更新
            _stateMachine.OnUpdate();
        }

        // 各ステート処理
        // ----- move sea -----
        private class StateMoveSea : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("start move sea");
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                var targetPosition = Owner.stageManager.seaTransform.position;
                // 海へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    StateMachine.DispatchEvent((int) EventType.ArriveSea);
                    return;
                }
                // 海へ向かう
                Owner.transform.position = Vector3.MoveTowards(
                    enemyPosition,
                    targetPosition, 
                    5.0f * Time.deltaTime);
                Owner.transform.LookAt(targetPosition);
            }

            public override void OnEnd()
            {
                Debug.Log("end move sea");
            }
        }

        // ----- hunting -----
        private class StateHunting : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("start hunting");
                // 採取スタート
                _isFinishHunting = false;
                MonoBehaviorHandler.StartStaticCoroutine(HuntCoroutine());
            }

            public override void OnUpdate()
            {
                // 採取が完了したら次のステートへ
                if (_isFinishHunting)
                {
                    StateMachine.DispatchEvent((int) EventType.HuntFinish);
                }
            }

            public override void OnEnd()
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
                Instantiate(Owner.stageManager.fishPrefab, Owner.transform);
            
                // 狩猟完了
                _isFinishHunting = true;
            }
        }

        // ----- move home -----
        private class StateMoveHome : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("start move home");
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                var targetPosition = Owner.stageManager.homeTransform.position;
                // 家へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    StateMachine.DispatchEvent((int) EventType.ArriveHome);
                    return;
                }
                // 家へ向かう
                Owner.transform.position = Vector3.MoveTowards(
                    enemyPosition,
                    targetPosition, 
                    5.0f * Time.deltaTime);
                Owner.transform.LookAt(targetPosition);
            }

            public override void OnEnd()
            {
                Debug.Log("end move home");
            }
        }

        // ----- eating -----
        private class StateEating : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("start eating");
                // 食事開始
                _isFinishEating = false;
                MonoBehaviorHandler.StartStaticCoroutine(EatCoroutine());
            }

            public override void OnUpdate()
            {
                // くるくる周る
                Owner.transform.Rotate(Vector3.up * 500.0f * Time.deltaTime);
                
                // 食事が完了したら次のステートへ
                if (_isFinishEating)
                {
                    StateMachine.DispatchEvent((int) EventType.EatFinish);
                }
            }

            public override void OnEnd()
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
                foreach (Transform child in Owner.transform)
                {
                    Destroy(child.gameObject);
                }
            
                // 食事完了
                _isFinishEating = true;
            }
        }
    }
}
