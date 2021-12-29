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
        /// ステート
        /// </summary>
        private readonly StateWait _stateWait = new StateWait();
        private readonly StateMove _stateMove = new StateMove();
        private readonly StateSleep _stateSleep = new StateSleep();
        private enum State
        {
            Wait,  // 待機
            Move,  // 動く
            Sleep, // 寝る
        }
        private StateBase GetState(State state)
        {
            switch (state)
            {
                case State.Wait:
                    return _stateWait;
                case State.Move:
                    return _stateMove;
                case State.Sleep:
                    return _stateSleep;
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
            _state = _stateWait;
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
        private void ChangeState(State nextState)
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
        
        // ----- wait -----
        private class StateWait : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;
            
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start wait");
                _countTime = 0.0f;
            }

            public override void OnUpdate(Enemy owner)
            {
                // wait -> move
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    owner.ChangeState(State.Move);
                }
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end wait");
            }
        }
        
        // ----- move -----
        private class StateMove : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;
            
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start move");
                _countTime = 0.0f;
            }

            public override void OnUpdate(Enemy owner)
            {
                // wait -> move
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    owner.ChangeState(State.Sleep);
                }
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end move");
            }
        }
        
        // ----- sleep -----
        private class StateSleep : StateBase
        {
            private const float ChangeTime = 3.0f;
            private float _countTime = 0.0f;
            
            public override void OnStart(Enemy owner)
            {
                Debug.Log("start sleep");
                _countTime = 0.0f;
            }

            public override void OnUpdate(Enemy owner)
            {
                // sleep -> wait
                _countTime += Time.deltaTime;
                if (_countTime > ChangeTime)
                {
                    owner.ChangeState(State.Wait);
                }
            }

            public override void OnEnd(Enemy owner)
            {
                Debug.Log("end sleep");
            }
        }
    }
}