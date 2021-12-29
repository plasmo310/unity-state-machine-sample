using System;
using UnityEngine;

namespace Sample04
{
    using StateBase = StateMachine<Enemy>.StateBase;
    
    /// <summary>
    /// Enemyクラス
    /// StateMachineを使用したステート遷移
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステート定義
        /// </summary>
        private enum EventType
        {
            Wait,  // 待機
            Move,  // 動く
            Sleep, // 寝る
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
            _stateMachine.AddTransition<StateSleep, StateWait>((int) EventType.Wait);
            _stateMachine.AddTransition<StateWait, StateMove>((int) EventType.Move);
            _stateMachine.AddTransition<StateMove, StateSleep>((int) EventType.Sleep);
            // ステート開始
            _stateMachine.Start<StateWait>();
        }

        private void Update()
        {
            // ステート更新
            _stateMachine.Update();
        }

        // 各ステート処理
        // ----- wait -----
        private class StateWait : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;

            public override void OnStart()
            {
                Debug.Log("start wait");
                _countTime = 0.0f;
            }

            public override void OnUpdate()
            {
                // wait -> move
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    StateMachine.ChangeState((int) EventType.Move);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end wait");
            }
        }
        
        // ----- move -----
        private class StateMove : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;
            
            public override void OnStart()
            {
                Debug.Log("start move");
                _countTime = 0.0f;
            }

            public override void OnUpdate()
            {
                // wait -> move
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    StateMachine.ChangeState((int) EventType.Sleep);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end move");
            }
        }
        
        // ----- sleep -----
        private class StateSleep : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;
            
            public override void OnStart()
            {
                Debug.Log("start sleep");
                _countTime = 0.0f;
            }

            public override void OnUpdate()
            {
                // sleep -> wait
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    StateMachine.ChangeState((int) EventType.Wait);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end sleep");
            }
        }
    }
}
