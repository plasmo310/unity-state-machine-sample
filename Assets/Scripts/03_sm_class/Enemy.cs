using System;
using System.Collections;
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
            MoveSea,  // 海へ移動
            Hunting,  // 魚採取
            MoveHome, // 家へ移動
            Eating,   // 食事
        }
        private StateMachine<Enemy> _stateMachine;

        /// <summary>
        /// ステージ管理クラス
        /// </summary>
        [SerializeField] private StageManager stageManager;
        private Vector3 GetSeaPoint()
        {
            return stageManager.seaTransform.position;
        }
        
        private Vector3 GetHomePoint()
        {
            return stageManager.homeTransform.position;
        }

        private void Awake()
        {
            Debug.Log("start sample03!!");
        }
        
        private void Start()
        {
            // ステートマシン定義
            _stateMachine = new StateMachine<Enemy>(this);
            _stateMachine.Add<StateMoveSea>((int) StateType.MoveSea);
            _stateMachine.Add<StateHunting>((int) StateType.Hunting);
            _stateMachine.Add<StateMoveHome>((int) StateType.MoveHome);
            _stateMachine.Add<StateEating>((int) StateType.Eating);
            // ステート開始
            _stateMachine.OnStart((int) StateType.MoveSea);
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
                var targetPosition = Owner.GetSeaPoint();
                // 海へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    StateMachine.ChangeState((int) StateType.Hunting);
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
                Owner.HuntStart();
            }

            public override void OnUpdate()
            {
                // 採取が完了したら次のステートへ
                if (Owner.IsFinishHunting())
                {
                    StateMachine.ChangeState((int) StateType.MoveHome);
                }
            }

            public override void OnEnd()
            {
                Debug.Log("end hunting");
            }
        }
        
        /// <summary>
        /// 採取が完了しているか？
        /// </summary>
        private bool _isFinishHunting;
        private bool IsFinishHunting()
        {
            return _isFinishHunting;
        }

        /// <summary>
        /// 採取開始
        /// </summary>
        private void HuntStart()
        {
            _isFinishHunting = false;
            StartCoroutine(HuntCoroutine());
        }

        /// <summary>
        /// 採取コルーチン
        /// </summary>
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
        private class StateMoveHome : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("start move home");
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                var targetPosition = Owner.GetHomePoint();
                // 家へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    StateMachine.ChangeState((int) StateType.Eating);
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
                // 待機開始
                Owner.EatStart();
            }

            public override void OnUpdate()
            {
                // くるくる周る
                Owner.transform.Rotate(Vector3.up * 500.0f * Time.deltaTime);
                
                // 食事が完了したら次のステートへ
                if (Owner.IsFinishEating())
                {
                    StateMachine.ChangeState((int) StateType.MoveSea);
                }
                
            }

            public override void OnEnd()
            {
                Debug.Log("end eating");
            }
        }
        
        /// <summary>
        /// 食事が完了しているか？
        /// </summary>
        private bool _isFinishEating;
        private bool IsFinishEating()
        {
            return _isFinishEating;
        }
        
        /// <summary>
        /// 食事開始
        /// </summary>
        private void EatStart()
        {
            _isFinishEating = false;
            StartCoroutine(EatCoroutine());
        }

        /// <summary>
        /// 食事コルーチン
        /// </summary>
        private IEnumerator EatCoroutine()
        {
            // 食事中、数秒待機
            yield return new WaitForSeconds(5.0f);

            // 食事終了
            // 子オブジェクト(魚)を破棄
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            
            // 食事完了
            _isFinishEating = true;
        }
    }
}
