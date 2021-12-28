using System;
using UnityEngine;

namespace Sample03
{
    using State = StateMachine<Enemy>.State;
    
    /// <summary>
    /// Enemyクラス
    /// StateMachineを使用したステート遷移
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        private enum StateType
        {
            Wait,  // 待機
            Move,  // 動く
            Sleep, // 寝る
        }
        private StateMachine<Enemy> _stateMachine;
        
        private void Awake()
        {
            Debug.Log("start sample03!!");
        }

        private void Start()
        {
            // StateMachine定義
            _stateMachine = new StateMachine<Enemy>(this);
            _stateMachine.AddTransition<StateSleep, StateWait>((int) StateType.Wait);
            _stateMachine.AddTransition<StateWait, StateMove>((int) StateType.Move);
            _stateMachine.AddTransition<StateMove, StateSleep>((int) StateType.Sleep);
            _stateMachine.Start<StateWait>();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        // ----- wait -----
        private class StateWait : State
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
                    StateMachine.ChangeState((int) StateType.Move);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end wait");
            }
        }
        
        // ----- move -----
        private class StateMove : State
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
                    StateMachine.ChangeState((int) StateType.Sleep);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end move");
            }
        }
        
        // ----- sleep -----
        private class StateSleep : State
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
                    StateMachine.ChangeState((int) StateType.Wait);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end sleep");
            }
        }
    }
}
