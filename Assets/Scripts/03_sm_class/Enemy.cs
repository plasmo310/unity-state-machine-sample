using System;
using UnityEngine;

namespace Sample03
{
    using StateBase = StateMachine<Enemy>.StateBase;
    
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステート定義
        /// </summary>
        private enum StateType
        {
            Wait,  // 待機
            Move,  // 動く
            Sleep, // 寝る
        }
        private StateMachine<Enemy> _stateMachine;

        private void Start()
        {
            // ステートマシン定義
            _stateMachine = new StateMachine<Enemy>(this);
            _stateMachine.Add<StateWait>((int) StateType.Wait);
            _stateMachine.Add<StateMove>((int) StateType.Move);
            _stateMachine.Add<StateSleep>((int) StateType.Sleep);
            // ステート開始
            _stateMachine.OnStart((int) StateType.Wait);
        }

        private void Awake()
        {
            Debug.Log("start sample03!!");
        }

        private void Update()
        {
            // ステート更新
            _stateMachine.OnUpdate();
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
                    StateMachine.ChangeState((int) StateType.Move);
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
                    StateMachine.ChangeState((int) StateType.Sleep);
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
