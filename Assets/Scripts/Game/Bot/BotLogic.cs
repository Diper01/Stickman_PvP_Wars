using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms;

public class BotLogic : Photon.PunBehaviour {

    [SerializeField] PlayerBot playerBot;
    [SerializeField] BotInputController botInput;
    [SerializeField] Transform botPosition;
    [SerializeField] Rigidbody2D rBody2D;
    [SerializeField] BotItemsCollider itemsCollider;
    [SerializeField] EnemyDetection enemyDetection;
    [SerializeField] BotWeaponController weaponController; 

    private static System.Random rand = new System.Random();

    private BotState state = BotState.FollowPath;  
    private PathFinderFast pathFinder;
    private Map map;    
    private List<Vector2i> path;
    private int previousPointId;
    private int targetPointId;
    private float fitDeltaX = 0.15f;
    private float fitDeltaY = 0.15f;
    private int playerWidth = 1;
    private int playerHeight = 2;
    private short jumpHeight = 4;
    private Tile targetTile;
    private Vector2i newTargetPos;
    private bool waitingNewPath;
    private float nextPathTime = 1.5f;
    private float nodeReachedTime;  
    private float fightStateStartTime;
    private float fightStateLengthTime;
    private FightBotState fightState;
    private bool moveToFlagStand = false;
  
    #region UNITY CALLBACKS

    private void Start() {     
        map = FindObjectOfType<Map>();
        path = new List<Vector2i>();
        nodeReachedTime = Time.time;
        InitPathFinder();
        StartCoroutine(StartRandomMove());
    }

    private void OnEnable()
    {
        itemsCollider.OnTrigger += OnItemTrigger;
    }

    private void OnDisable()
    {
        itemsCollider.OnTrigger -= OnItemTrigger;
    }

    private void FixedUpdate()
    {
        if (!playerBot.IsInteractable || !PhotonNetwork.isMasterClient) return;

        DetectBotState();
        BotUpdate();
    }    

    void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            var start = path[0];

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(map.transform.position + new Vector3(start.x + 0.5f, start.y + 0.5f, -5.0f), 0.3f);

            for (var i = 1; i < path.Count; ++i)
            {
                var end = path[i];
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(map.transform.position + new Vector3(end.x + 0.5f, end.y + 0.5f, -5.0f), 0.3f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(map.transform.position + new Vector3(start.x + 0.5f, start.y + 0.5f, -5.0f),
                                map.transform.position + new Vector3(end.x + 0.5f, end.y + 0.5f, -5.0f));
                start = end;
            }
        }
    }


    #endregion

    private void InitPathFinder() {
        pathFinder = new PathFinderFast(map.ByteGrid);
        pathFinder.Formula = HeuristicFormula.Manhattan;
        pathFinder.Diagonals = false;
        pathFinder.HeavyDiagonals = false;
        pathFinder.HeuristicEstimate = 6;
        pathFinder.PunishChangeDirection = false;
        pathFinder.TieBreaker = false;
        pathFinder.SearchLimit = 1800;
        pathFinder.DebugProgress = false;
        pathFinder.DebugFoundPath = false;
    }

    private void OnItemTrigger(Vector2i tilePosition) {
        if (!PhotonNetwork.isMasterClient || moveToFlagStand)
            return;
      
        SetNewTarget(tilePosition);    
    }

    private void SetNewTarget(Vector2i newTargetPos) {
        waitingNewPath = true;
        this.newTargetPos = newTargetPos;
    }

    private IEnumerator StartRandomMove() {
        yield return new WaitForSeconds(1f);
        FindRandomMoveTarget();
    }

    private void FindRandomMoveTarget() {              
        if (GameManagersHolder.Instance.GameMode == GameMode.CAPTURE_THE_FLAG) {
            SetNewTarget(FindRandomTargetPositionCTF());
        }
        else {
            SetNewTarget(FindRandomTargetPosition());
        }
    }

    private Vector2i FindRandomTargetPosition() {     
        while (true)
        {
            Tile newTarget = map.MoveNodes[rand.Next(0, map.MoveNodes.Count)];
            if (newTarget != targetTile)
            {
                targetTile = newTarget;
                break;
            }
        }
        return new Vector2i(targetTile.X, targetTile.Y);
    }

    private Vector2i FindRandomTargetPositionCTF() {
        FlagStand botStand = GetBotStand();
        FlagStand opositTeamStand = GetOpositTeamStand();
        if (playerBot.HasFlag) {
            if (rand.Next(0, 2) != 0) {                
                moveToFlagStand = true;
                return new Vector2i(botStand.X, botStand.Y);
            }
        }
        else if (opositTeamStand.HasFlag) {
            if (rand.Next(0, 2) != 0) {              
                moveToFlagStand = true;
                return new Vector2i(opositTeamStand.X, opositTeamStand.Y);
            }
        }

        moveToFlagStand = false;
        return FindRandomTargetPosition();                
    }  

    private FlagStand GetBotStand() {
        if (playerBot.Team == Team.RED)
            return GameManagersHolder.Instance.GameManagerCTF.RedStand;
        else
            return GameManagersHolder.Instance.GameManagerCTF.BlueStand;
    }

    private FlagStand GetOpositTeamStand() {
        if (playerBot.Team == Team.RED)
            return GameManagersHolder.Instance.GameManagerCTF.BlueStand;
        else
            return GameManagersHolder.Instance.GameManagerCTF.RedStand;
    }

    private void FindPath(Vector2i targetPos)
    {
        Vector2i startPosition = FindBotTilePosition();
        SetFollowPath(pathFinder.FindPath(startPosition, targetPos, playerWidth, playerHeight, jumpHeight));
    }

    private void SetFollowPath(List<Vector2i> newPath)
    {
        botInput.ClearInput();
        path.Clear();
        if (newPath != null && newPath.Count > 1)
        {
            for (var i = newPath.Count - 1; i >= 0; --i)
            {
                path.Add(newPath[i]);
            }
            previousPointId = 0;
            targetPointId = 1;
        }
    }

    private void TrySetNewFollowPath()
    {
        if (IsBotStayOnGround()) {
            FindPath(newTargetPos);
            nodeReachedTime = Time.time;
            waitingNewPath = false;
        }
    }

    private Vector2i FindBotTilePosition()
    {
        int y = Mathf.FloorToInt(botPosition.position.y);
        int x = Mathf.FloorToInt(botPosition.position.x);
        return new Vector2i(x, y);
    }

    private bool IsBotStayOnGround() {
        Vector2i botTilePos = FindBotTilePosition();
        return map.IsTileOnGround(botTilePos.x, botTilePos.y) && rBody2D.velocity.y == 0;
    }

    private void BotFire() {
        if (weaponController.CurrentWeapon == WeaponType.MINE)
            weaponController.ChangeWeapon();
        else
        {            
            botInput.Fire = true;            
        }
    }

    #region STATE MACHINE

    private void DetectBotState() {
        if (IsBotStayOnGround() && (enemyDetection.EnemyForward || enemyDetection.EnemyBack))
        {
            if (state == BotState.FollowPath) {
                SetNewFightState();
                fightState = (FightBotState)rand.Next(0, 2);
                botInput.ClearInput();               
            }
            state = BotState.Fight;
        }
        else
        {
            if (state == BotState.Fight) {              
                TrySetNewFollowPath();
                botInput.ClearInput();
            }
            state = BotState.FollowPath;
        }
    }

    private void BotUpdate() {        
        switch (state)
        {
            case BotState.FollowPath:
                FollowPathUpdate();
                break;
            case BotState.Fight:
                FightUpdate();
                break;
        }
    }

    #endregion

    #region FOLLOW PATH

    private void FollowPathUpdate()
    {     
        if (Time.time - nodeReachedTime > nextPathTime) {
            nodeReachedTime = Time.time;          
            FindRandomMoveTarget();
        }

        if (waitingNewPath)
            TrySetNewFollowPath();

        if (path == null || path.Count == 0)
            return;

        FightInJump();

        Vector2 targetPosition = GetWorldRealPosition(path[targetPointId].x, path[targetPointId].y);
        Vector2 realPositionDelta = targetPosition - (Vector2)botPosition.position;

        Vector2i mapPositionDelta = new Vector2i(path[targetPointId].x - path[previousPointId].x, path[targetPointId].y - path[previousPointId].y);

        if (mapPositionDelta.x > 0) {
            if (mapPositionDelta.y > 0) {
                MoveRightUp(realPositionDelta);
            }
            else if (mapPositionDelta.y < 0) {
                MoveRightDown(realPositionDelta);
            }
            else {
                MoveRight(realPositionDelta);
            }
        }
        else if (mapPositionDelta.x < 0) {
            if (mapPositionDelta.y > 0) {
                MoveLeftUp(realPositionDelta);
            }
            else if (mapPositionDelta.y < 0) {
                MoveLeftDown(realPositionDelta);
            }
            else {
                MoveLeft(realPositionDelta);
            }
        }
        else {
            if (mapPositionDelta.y > 0) {
                MoveUp(realPositionDelta);
            }
            else if (mapPositionDelta.y < 0) {
                MoveDown(realPositionDelta);
            }
        }


    }

    private void MoveRightUp(Vector2 positionDelta) {
        bool isTargetOnGround = map.IsTileOnGround(path[targetPointId].x, path[targetPointId].y);

        if (positionDelta.x < fitDeltaX
            && ((!isTargetOnGround && positionDelta.y < fitDeltaY)
            || (isTargetOnGround && Mathf.Abs(positionDelta.y) < fitDeltaY)))
        {
            PointReached();
        }
        else {
            if (positionDelta.x > fitDeltaX)
                botInput.HorizontalAxis = 1f;
            else
                botInput.HorizontalAxis = 0f;

            if (positionDelta.y > fitDeltaY && map.IsTileOnGround(path[previousPointId].x, path[previousPointId].y) && !waitingNewPath)
                botInput.OnJump();
        }
    }

    private void MoveRightDown(Vector2 positionDelta) {
        if (positionDelta.x < fitDeltaX && positionDelta.y > -fitDeltaY)
            PointReached();
        else if (positionDelta.x > fitDeltaX)
            botInput.HorizontalAxis = 1f;
        else
            botInput.HorizontalAxis = 0f;
    }

    private void MoveRight(Vector2 positionDelta) {
        if (positionDelta.x < fitDeltaX && Mathf.Abs(positionDelta.y) < fitDeltaY)
            PointReached();
        else if (positionDelta.x > fitDeltaX)
            botInput.HorizontalAxis = 1f;
        else
            botInput.HorizontalAxis = 0f;
    }

    private void MoveLeftUp(Vector2 positionDelta)
    {
        bool isTargetOnGround = map.IsTileOnGround(path[targetPointId].x, path[targetPointId].y);

        if (positionDelta.x > -fitDeltaX
            && ((!isTargetOnGround && positionDelta.y < fitDeltaY)
            || (isTargetOnGround && Mathf.Abs(positionDelta.y) < fitDeltaY)))
        {
            PointReached();
        }
        else {
            if (positionDelta.x < -fitDeltaX)
                botInput.HorizontalAxis = -1f;
            else
                botInput.HorizontalAxis = 0f;

            if (positionDelta.y > fitDeltaY && map.IsTileOnGround(path[previousPointId].x, path[previousPointId].y) && !waitingNewPath)
                botInput.OnJump();
        }
    }

    private void MoveLeftDown(Vector2 positionDelta)
    {
        if (positionDelta.x > -fitDeltaX && positionDelta.y > -fitDeltaY)
            PointReached();
        else if (positionDelta.x < -fitDeltaX)
            botInput.HorizontalAxis = -1f;
        else
            botInput.HorizontalAxis = 0f;
    }

    private void MoveLeft(Vector2 positionDelta) {
        if (positionDelta.x > -fitDeltaX && Mathf.Abs(positionDelta.y) < fitDeltaY)
            PointReached();
        else if (positionDelta.x < -fitDeltaX)
            botInput.HorizontalAxis = -1f;
        else
            botInput.HorizontalAxis = 0f;
    }

    private void MoveUp(Vector2 positionDelta) {
        if (positionDelta.y < fitDeltaY)
            PointReached();
        else if (positionDelta.y > fitDeltaY && map.IsTileOnGround(path[previousPointId].x, path[previousPointId].y) && !waitingNewPath)
            botInput.OnJump();
    }

    private void MoveDown(Vector2 positionDelta) {
        if (positionDelta.y > -fitDeltaY)
            PointReached();
    }

    private void PointReached() {

        if (targetPointId == path.Count - 1) {
            path.Clear();
            botInput.HorizontalAxis = 0f;
            FindRandomMoveTarget();
        }
        else {
            previousPointId++;
            targetPointId++;
            Vector2i mapPositionDelta = new Vector2i(path[targetPointId].x - path[previousPointId].x, path[targetPointId].y - path[previousPointId].y);
            if (mapPositionDelta.x > 0)
                botInput.HorizontalAxis = 1f;
            else if (mapPositionDelta.x < 0)
                botInput.HorizontalAxis = -1f;
            else
                botInput.HorizontalAxis = 0f;
        }
        nodeReachedTime = Time.time;
    }

    private Vector2 GetWorldRealPosition(int mapX, int mapY)
    {
        float x = mapX + 0.5f;
        float y = mapY + 0.5f;
        return new Vector2(x, y);
    }

    private void FightInJump()
    {
        if (enemyDetection.EnemyForward)
        {
            BotFire();
        }
        else
        {
            botInput.Fire = false;
        }       
    }


    #endregion

    #region FIGHT

    private void FightUpdate()
    {
        botInput.ClearInput();
        if (enemyDetection.EnemyForward)
            Figth();
        else if (enemyDetection.EnemyBack)
        {
            if (enemyDetection.EnemyRight) {
                botInput.HorizontalAxis = 1f;
            }
            else if(enemyDetection.EnemyLeft) {
                botInput.HorizontalAxis = -1f;
            }
        }   
    }

    private void Figth() {
        if (Time.time - fightStateStartTime > fightStateLengthTime)
        {
            SetNewFightState();
        }
        else {
            switch (fightState)
            {
                case FightBotState.Stay:
                    BotFire();
                    break;
                case FightBotState.Crouch:
                    BotFire();
                    botInput.Crouch = true;
                    break;
                case FightBotState.Jump:
                    BotFire();
                    botInput.OnJump();
                    break;                
            }
        }
    }

    private void SetNewFightState() {
        fightState = (FightBotState)rand.Next(0, 3);
        fightStateStartTime = Time.time;
        fightStateLengthTime = UnityEngine.Random.Range(0.3f, 1.3f);
    }


    #endregion
}

public enum BotState {
    FollowPath,
    Fight
}

public enum FightBotState {
    Stay,
    Crouch,
    Jump
}