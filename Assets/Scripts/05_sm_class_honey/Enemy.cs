using System.Collections;
using UnityEngine;

namespace Sample05
{
    using StateBase = StateMachine<Enemy>.StateBase;
    
    /// <summary>
    /// Enemyクラス
    /// ハニーのために魚を採り続ける
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        /// <summary>
        /// ステージ管理クラス
        /// </summary>
        [SerializeField] public StageManager stageManager;

        /// <summary>
        /// ステート定義
        /// </summary>
        private enum StateType
        {
            MoveSea,  // 海へ移動
            Hunting,  // 魚採取
            MoveHome, // 家へ移動
            Eating,   // 食事
            Looking,  // 眺める
        }
        private StateMachine<Enemy> _stateMachine;

        private void Awake()
        {
            Debug.Log("start sample05!!");
        }
        
        private void Start()
        {
            // ステートマシン定義
            _stateMachine = new StateMachine<Enemy>(this);
            _stateMachine.Add<StateMoveSea>((int) StateType.MoveSea);
            _stateMachine.Add<StateHunting>((int) StateType.Hunting);
            _stateMachine.Add<StateMoveHome>((int) StateType.MoveHome);
            _stateMachine.Add<StateEating>((int) StateType.Eating);
            _stateMachine.Add<StateLooking>((int) StateType.Looking);
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
                Debug.Log("<color=aqua>**kanio** 魚採ってきます</color>");
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                var targetPosition = Owner.stageManager.seaTransform.position;
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
            }
        }

        // ----- hunting -----
        private class StateHunting : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=aqua>**kanio** 魚採ります</color>");
                // 採取スタート
                _isFinishHunting = false;
                MonoBehaviorHandler.StartStaticCoroutine(HuntCoroutine());
            }

            public override void OnUpdate()
            {
                // 採取が完了したら次のステートへ
                if (_isFinishHunting)
                {
                    StateMachine.ChangeState((int) StateType.MoveHome);
                }
            }

            public override void OnEnd()
            {
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
                Debug.Log("<color=aqua>**kanio** 魚採れたので帰ります</color>");
            }

            public override void OnUpdate()
            {
                var enemyPosition = Owner.transform.position;
                var targetPosition = Owner.stageManager.homeTransform.position;
                // 家へ到着したら次のステートへ
                if (Vector3.Distance(enemyPosition, targetPosition) < 0.5f)
                {
                    // ハニーが家にいた場合
                    if (Owner.stageManager.honey.IsWaitingHome())
                    {
                        // 子オブジェクト(魚)をプレゼントしてまた海へ出かける
                        Debug.Log("<color=yellow>**kanio** あげるよハニー</color>");
                        foreach (Transform child in Owner.transform)
                        {
                            Owner.stageManager.honey.ReceiveFish(child.gameObject);
                        }
                        StateMachine.ChangeState((int) StateType.Looking);
                        return;
                    }
                    // いなかったら自分で食べる
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
            }
        }

        // ----- eating -----
        private class StateEating : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=aqua>**kanio** 魚食べます</color>");
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
                    StateMachine.ChangeState((int) StateType.MoveSea);
                }
            }

            public override void OnEnd()
            {
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
        
        // ----- looking -----
        private class StateLooking : StateBase
        {
            public override void OnStart()
            {
                Debug.Log("<color=aqua>**kanio** 眺めます</color>");
                // 眺める開始
                _isFinishLooking = false;
                MonoBehaviorHandler.StartStaticCoroutine(LookCoroutine());
            }

            public override void OnUpdate()
            {
                // 眺めるのが完了したら次のステートへ
                if (_isFinishLooking)
                {
                    StateMachine.ChangeState((int) StateType.MoveSea);
                }
            }

            public override void OnEnd()
            {
            }
            
            // 眺めるのが完了しているか？
            private bool _isFinishLooking;
            
            // 眺めるコルーチン
            private IEnumerator LookCoroutine()
            {
                // 数秒待機
                yield return new WaitForSeconds(2.0f);
                // 眺める完了
                _isFinishLooking = true;
            }
        }
    }
}
