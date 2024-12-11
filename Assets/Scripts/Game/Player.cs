using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Player : Photon.PunBehaviour, IPunObservable {
    [SerializeField] private GameObject playerInfoUI;
    [SerializeField] private GameObject burningEffect;
    [SerializeField] private float maxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float jumpStartSpeed = 15f;
    [SerializeField] private float maxFallSpeed = -12f;
    [SerializeField] private LayerMask whatIsGround;                  // A mask determining what is ground to the character
    [SerializeField] private float burningTime = 3f;
    [SerializeField] private int burningIterations = 10;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private BoxCollider2D standCollider;
    [SerializeField] private BoxCollider2D layCollider;
    [SerializeField] private GameObject stickmanContainer;
    [SerializeField] Rigidbody2D myRigidbody2D;

    public int PlayerId { get { return photonView.ownerId; } }
    public Team Team {
        get {
            if (photonView.owner.CustomProperties.ContainsKey(PlayerProperties.Team))
                return (Team)photonView.owner.CustomProperties[PlayerProperties.Team];
            else
                return Team.RED;
        }
        set {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
            playerProperties.Add(PlayerProperties.Team, value);
            photonView.owner.SetCustomProperties(playerProperties);
        }
    }
    public string PlayerName { get {
            return playerName;
        }
        set {
            playerName = value;
            playerInfo.SetPlayerName(playerName);
        }
    }
    public GameMode Mode { get; set; }
    public bool FriendlyFire { get; set; }
    public int Health {
        get {
            return health;
        }
        set {
            if (value <= 0 && health > 0)
            {
                sounds.PlayClip(sounds.ClipDie);
            }
            health = value;
            if (health > 100)
            {
                health = 100;
            }
            else if(health < 0) {                
                health = 0;
            }

            if (health > 0) {
                isDead = false;
                transform.tag = "Player";                
            }
            else {
                CanPickupItem = false;
                StartCoroutine(ResetCanPickupItem());
                isDead = true;
                transform.tag = "PlayerDead";
            }

            if (playerInfo != null)
            {
                playerInfo.SetHealth(health);
            }
        }
    }
    public int Armor {
        get { return armor; }
        set {          
            armor = value;
            if (armor > 100)
            {
                armor = 100;
            }
            else if (armor < 0)
            {
                armor = 0;
            }
            if (playerInfo != null)
            {
                playerInfo.SetArmor(armor);
            }
        }
    }
    public bool IsFacingRight {
        get {
            return isFacingRight;       
        }
        set {
            isFacingRight = value;
            if (isFacingRight)
            {
                float scaleX = Math.Abs(transform.localScale.x);
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
            }
            else {
                float scaleX = -Math.Abs(transform.localScale.x);
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
            }
        }

    }
    public bool IsInteractable {
        get { return isInteractable; }
        set {           
            isInteractable = value;
            if (isInteractable) {
                stickmanContainer.transform.localEulerAngles = new Vector3(0,0,0);
                standCollider.enabled = true;
                layCollider.enabled = false;
                myRigidbody2D.simulated = true;               
                if (playerInfo != null) {
                    playerInfo.gameObject.SetActive(true);
                }
            }
            else
            {
                DropFlagFromPlayer();
                inputController.HorizontalAxis = 0f;
                myRigidbody2D.velocity = new Vector2();
                stickmanContainer.transform.localEulerAngles = new Vector3(0,90,0);
                standCollider.enabled = false;
                layCollider.enabled = false;
                myRigidbody2D.simulated = false;
                if (playerInfo != null) {
                    playerInfo.gameObject.SetActive(false);
                }               
            }           
        }
    }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsCrouch { get { return crouch; } }
    public bool HasFlag {
        get {
            if (GameManagersHolder.Instance.GameMode != GameMode.CAPTURE_THE_FLAG)
                return false;

            Flag flag = Team == Team.RED ? GameManagersHolder.Instance.GameManagerCTF.BlueFlag : GameManagersHolder.Instance.GameManagerCTF.RedFlag;
            return flag.State == FlagState.ON_PLAYER && flag.CarryingPlayerId == this.PlayerId;
        }
    }  
    public bool CanPickupItem { get; set; }

    private int fullHealth = 100;
    private int fullArmor = 100;
    private string playerName;
    private Transform groundCheck;                  // A position marking where to check if the player is grounded.          
    private bool isGrounded;                        // Whether or not the player is grounded.         
    private SpriteRenderer sprRenderer;
    private PlayerSounds sounds;
    private bool isFacingRight = true;              // For determining which way the player is currently facing.
    private IInputController inputController;
    private IWeaponController weaponController;
    private int health;
    private int armor;   
    private PlayerInfoUI playerInfo;
    private int burningIterator = 0;
    private float burningTimer = 0f;
    private int burningDamage;
    private bool crouch = false;
    private float move = 0;
    private byte playerKilledEventCode = 0;
    private int burningFromPlayerId = 0;
    private bool isDead = false;
    private bool isInteractable;
    private bool destroyed = false;  

    private void Awake()
    {      
        Init();        
    }   

    private void Init()
    {                
        IsInteractable = true;       
        groundCheck = transform.Find("GroundCheck");
        sounds = GetComponent<PlayerSounds>();          
        sprRenderer = GetComponent<SpriteRenderer>();
        inputController = FindObjectOfType<InputController>();
        weaponController = GetComponent<WeaponController>();
        SetEventsListeners();
        CreatePlayerInfoUI();
        PlayerName = photonView.owner.NickName;
        Health = fullHealth;
        CanPickupItem = true;
        Armor = 0;              
    }

    private void CreatePlayerInfoUI() {
        playerInfo = Instantiate(playerInfoUI).GetComponent<PlayerInfoUI>();
        playerInfo.SetTarget(this.transform);
        SetupPlayerTeam();       
    }

    private void SetupPlayerTeam()
    {
        if (this.PlayerId == PhotonNetwork.player.ID)
        {
            playerInfo.SetPlayerColor(GameManagersHolder.Instance.GameType, this.Team);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
        else
        {
            playerInfo.SetOtherPlayerColor(GameManagersHolder.Instance.GameType, this.Team);
            if (GameManagersHolder.Instance.GameType == GameType.FFA)
            {
                gameObject.layer = LayerMask.NameToLayer("EnemyPlayer");
            }
            else
            {
                if (PhotonNetwork.player.CustomProperties.ContainsKey(PlayerProperties.Team)
                    && ((Team)PhotonNetwork.player.CustomProperties[PlayerProperties.Team]) == this.Team)
                {
                    gameObject.layer = LayerMask.NameToLayer("Player");
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("EnemyPlayer");
                }
            }
        }
    }

    private void OnDisable()
    {
        UnSetEventsListeners();
    }

    private void OnDestroy()
    {             
        destroyed = true;      
        if (playerInfo != null)
        {
            Destroy(playerInfo.gameObject);
        }       
    }

    private void Update()
    {
        CheckBurning();
    }

    private void FixedUpdate()
    {
        CheckGroud();
        ProcessInputs();
        RestricktFallSpeed();
        CheckFlip();
        CheckWalkSound();        
        CheckLayCollider();     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.tag == "Projectile")
        {
            CollisionWithProjectile(collision);
        }       
    }

    private void OnApplicationPause(bool pause)
    {
        if (destroyed)
            return;

        if (pause)
        {
            if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount > 1)
            {                           
                PhotonNetwork.SetMasterClient(PhotonNetwork.masterClient.GetNext());                                    
            }
        }
        if (photonView.isMine)
        {
            photonView.RPC("PlayerPause", PhotonTargets.Others, pause, PlayerId);
        }
        PhotonNetwork.SendOutgoingCommands();
    }

    protected virtual void SetEventsListeners() {       
        if (photonView.isMine)
        {
            inputController.ChangeWeapon += ChangeWeapon;
            inputController.Jump += Jump;
            if (PhotonNetwork.offlineMode == false) {
                PhotonNetwork.OnEventCall += OnPhotonEvent;
            }
            else {
                EventManager.PhotonOfflineEvent += OnPhotonEvent;
            }
        }
    }

    private void UnSetEventsListeners() {       
        if (photonView.isMine)
        {           
            inputController.ChangeWeapon -= ChangeWeapon;
            inputController.Jump -= Jump;
            if (PhotonNetwork.offlineMode == false) {
                PhotonNetwork.OnEventCall -= OnPhotonEvent;
            }
            else {
                EventManager.PhotonOfflineEvent -= OnPhotonEvent;
            }
        }
    }    

    private void CollisionWithProjectile(Collider2D collision) {
        var projectile = collision.GetComponent<ProjectileBase>();        
              
        switch (projectile.Type)
        {
            case ProjectileType.BULLET:
                if (projectile.PlayerId != this.PlayerId) {
                    if (IsProjectileDoDamage(projectile)) {
                        TakeDamage(projectile.damage, projectile.PlayerId);
                    }
                    projectile.ProjectileHitPlayer();
                }
                break;
            case ProjectileType.FLAME:
                if (projectile.PlayerId != this.PlayerId) {
                    if (IsProjectileDoDamage(projectile))
                    {
                        StartBorning(projectile.damage, projectile.PlayerId);
                    }
                    projectile.ProjectileHitPlayer();
                }
                break;
            case ProjectileType.ROCKET:
                if (projectile.PlayerId != this.PlayerId)
                {
                    if (IsProjectileDoDamage(projectile))
                    {
                        TakeDamage(projectile.damage, projectile.PlayerId);
                    }
                    projectile.ProjectileHitPlayer();
                }
                break;
            case ProjectileType.EXPLOSION:
                if (IsProjectileDoDamage(projectile))
                {
                    TakeDamage(projectile.damage, projectile.PlayerId);
                    projectile.ProjectileHitPlayer();
                }
                break;
            case ProjectileType.LASER:
                if (projectile.PlayerId != this.PlayerId)
                {
                    if (IsProjectileDoDamage(projectile))
                    {
                        TakeDamage(projectile.damage, projectile.PlayerId);
                    }
                    projectile.ProjectileHitPlayer();
                }
                break;
            case ProjectileType.MINE:
                var projectileMine = (ProjectileMine)projectile;

                if (!projectileMine.IsLanded && projectileMine.PlayerId == this.PlayerId)
                {
                    break;
                }

                projectile.ProjectileHitPlayer();
                break;
        }                
    }

    private bool IsProjectileDoDamage(ProjectileBase projectile) {
        if (photonView.isMine)
        {
            if (GameManagersHolder.Instance.GameType == GameType.TEAMS 
                && projectile.PlayerTeam == this.Team 
                && FriendlyFire == false)
            {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }

    private void OnItemPickup(CollectableItemType type, int value) {
        if (isDead)
            return;
        switch (type)
        {
            case CollectableItemType.AUTOMATON_BULLETS:
                weaponController.AddBullets(WeaponType.AUTOMATON, value);               
                if (weaponController.CurrentWeapon == WeaponType.PISTOL)
                {
                    weaponController.ChangeWeapon(WeaponType.AUTOMATON);
                }
                break;
            case CollectableItemType.FLAME_BULLETS:
                weaponController.AddBullets(WeaponType.FLAMETHROWER, value);              
                if (weaponController.CurrentWeapon == WeaponType.PISTOL)
                {
                    weaponController.ChangeWeapon(WeaponType.FLAMETHROWER);
                }
                break;
            case CollectableItemType.ROKET_BULLETS:
                weaponController.AddBullets(WeaponType.ROKET_LAUNCHER, value);               
                if (weaponController.CurrentWeapon == WeaponType.PISTOL)
                {
                    weaponController.ChangeWeapon(WeaponType.ROKET_LAUNCHER);
                }
                break;
            case CollectableItemType.LASER_BULLETS:
                weaponController.AddBullets(WeaponType.LASER, value);                
                if (weaponController.CurrentWeapon == WeaponType.PISTOL)
                {
                    weaponController.ChangeWeapon(WeaponType.LASER);
                }
                break;
            case CollectableItemType.MINES:
                weaponController.AddBullets(WeaponType.MINE, value);               
                if (weaponController.CurrentWeapon == WeaponType.PISTOL)
                {
                    weaponController.ChangeWeapon(WeaponType.MINE);
                }
                break;
            case CollectableItemType.HEALTH:
                try
                {
                    Health += value;
                }
                catch (Exception ex) {
                    print(ex.Message);
                    print(ex.StackTrace);
                }
                break;
            case CollectableItemType.ARMOR:                
                Armor += value;                                 
                break;
        }

    }   

    private void StartBorning(int damage,int damageFromID) {
        burningFromPlayerId = damageFromID;
        burningIterator = burningIterations;
        burningDamage = damage;       
    }

    private void CheckBurning() {
        if (burningIterator > 0)
        {
            burningEffect.SetActive(true);
            if (burningTimer > burningTime / burningIterations)
            {               
                if (photonView.isMine)
                {
                    TakeDamage(burningDamage, burningFromPlayerId);
                }
                burningTimer = 0f;
                burningIterator--;
            }
            burningTimer += Time.deltaTime;            
        }
        else{
            burningEffect.SetActive(false);
        }
    }

    private void CheckGroud() {
        isGrounded = false;        
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(0.52f, 0.2f), 0f, whatIsGround);
                
        if (colliders.Length > 0) 
        {
            isGrounded = true;
        }       
    }

    private void CheckWalkSound() {
        if (!GameOptions.Sound)
            walkSource.enabled = false;
        else if (isGrounded && myRigidbody2D.velocity.x != 0)
            walkSource.enabled = true;
        else
            walkSource.enabled = false;
    }

    private void ProcessInputs() {        
        if (isDead || !IsInteractable) return;
        if (PhotonNetwork.connected && !photonView.isMine) {
            return;
        }

        crouch = inputController.Crouch;
        move = inputController.HorizontalAxis;
        Move();                

        if (inputController.Fire) {
            Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
            weaponController.MakeShot(direction);
        }
    }

    private void RestricktFallSpeed() {      
        if (myRigidbody2D.velocity.y < maxFallSpeed) {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, maxFallSpeed);
        }
    }

    private void Move()
    {                
        if (crouch) {
            move = 0f;
        }
            
        myRigidbody2D.velocity = new Vector2(move * maxSpeed, myRigidbody2D.velocity.y);      
    }

    private void CheckFlip() {
        if (myRigidbody2D.velocity.x > 0)
        {
            IsFacingRight = true;
        }
        else if (myRigidbody2D.velocity.x < 0) {
            IsFacingRight = false;
        }        
    }

    private void CheckLayCollider() {
        if (isGrounded && crouch) {
            layCollider.enabled = true;
            standCollider.enabled = false;
        }
        else {
            layCollider.enabled = false;
            standCollider.enabled = true;
        }
    }

    private void Jump() {
        if (isDead || !IsInteractable) return;
        if (PhotonNetwork.connected  && !photonView.isMine)
        {
            return;
        }

        if (isGrounded && myRigidbody2D.velocity.y == 0)
        {           
            isGrounded = false;                   
            //if (myRigidbody2D.velocity.y == 0) {
            //    sounds.PlayClip(sounds.ClipJump);
            //}
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpStartSpeed);
        }
    }

    private void ChangeWeapon() {
        if (isDead || !IsInteractable) return;
        if (PhotonNetwork.connected && !photonView.isMine)
        {
            return;
        }
        weaponController.ChangeWeapon();
    }    

    private void TakeDamage(int damage, int damageFromID) {
        if (isDead || !IsInteractable) {
            return;
        }
         
        int armorDelta = armor - damage;
        if (armorDelta < 0)
        {
            Health -= (damage - Armor);
            Armor = 0;
        }
        else {
            Armor -= damage;
        }

        if (GameOptions.Vibro)
            Handheld.Vibrate();

        if (health == 0)        
            Die(damageFromID);        
        
    }

    private void Die(int damageFromID) {           
        myRigidbody2D.velocity = new Vector2(0, myRigidbody2D.velocity.y);
        if (damageFromID != this.PlayerId)
        {                   
            byte fragPlayerId = (byte)damageFromID;
            byte team = (byte)this.Team;
            byte[] eventContent = new byte[] { fragPlayerId, team };

            if (PhotonNetwork.offlineMode == false)
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerKilled, eventContent, true, options);
            }
            else {
                EventManager.OnPhotonOfflineEvent(PhotonEventCodes.PlayerKilled, eventContent, PhotonNetwork.player.ID);
            }
        }
        AddDeath();
        EventManager.OnPlayerDie(this);
        if (photonView.isMine && HasFlag) {
            DropFlagFromPlayer();
        }
    }
    
    public void SpawanPlayer(Vector3 position) {
        transform.position = position;
        burningTimer = 0;
        burningIterator = 0;
        Armor = 0;    
        Health = 100;             
        weaponController.ResetWeapon();
    }
   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
        if (stream.isWriting)
        {
            stream.SendNext(crouch);
            stream.SendNext(Health);
            stream.SendNext(Armor);
            stream.SendNext(IsFacingRight);
            stream.SendNext(burningIterator);            
        }
        else {
            crouch = (bool)stream.ReceiveNext();
            Health = (int)stream.ReceiveNext();
            Armor = (int)stream.ReceiveNext();
            IsFacingRight = (bool)stream.ReceiveNext();
            burningIterator = (int)stream.ReceiveNext();          
        }
    }

    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        ExitGames.Client.Photon.Hashtable props = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;
              
        if (player.ID == this.PlayerId && props.ContainsKey(PlayerProperties.Team)) {
            SetupPlayerTeam();
        }
    }   

    private void OnPhotonEvent(byte eventcode, object content, int senderid) {
        if (destroyed) {          
            return;
        }

        if (eventcode == PhotonEventCodes.PlayerKilled)
        {                        
            if (GameManagersHolder.Instance.GameType == GameType.FFA)
                OnPlayerKilledFFA(content);
            else if (GameManagersHolder.Instance.GameType == GameType.TEAMS)
                OnPlayerKilledTeams(content);
        }
        else if (eventcode == PhotonEventCodes.ItemPickup) {
            OnItemPickup(content);
        }
    }

    private void OnPlayerKilledFFA(object content) {
        byte[] eventContent = (byte[])content;        
        int fragPlayerId = (int)eventContent[0];

        if (fragPlayerId == PhotonNetwork.player.ID)
        {
            AddFrag();
        }
    }

    private void OnPlayerKilledTeams(object content) {
        byte[] eventContent = (byte[])content;       
        int fragPlayerId = (int)eventContent[0];
        Team killedPlayerTeam = (Team)eventContent[1];

        if (fragPlayerId == PhotonNetwork.player.ID && this.Team != killedPlayerTeam)
        {         
            AddFrag();
        }
    }

    private void OnItemPickup(object content)
    {
        byte[] eventContent = (byte[])content;
        int targetPlayerId = eventContent[0];
        if (targetPlayerId == this.PlayerId)
        {
            int value = eventContent[1];
            CollectableItemType itemType = (CollectableItemType)eventContent[2];
            OnItemPickup(itemType, value);
        }
    }

    [PunRPC]
    private void PlayerPause(bool pause, int playerId)
    {
        if (destroyed)
            return;
       
        if (this.PlayerId == playerId && PhotonNetwork.room != null)
        {
            if (pause)
            {                
                this.IsInteractable = false;                                   
            }
            else
            {                
                this.IsInteractable = true;               
            }
        }
    }  

    private void AddFrag() {
        int kills = (int)PhotonNetwork.player.CustomProperties[PlayerProperties.Kills];
        kills++;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.Kills, kills);
        PhotonNetwork.player.SetCustomProperties(playerProperties);       
    }

    private void AddDeath() {
        int deaths = (int)PhotonNetwork.player.CustomProperties[PlayerProperties.Death];
        deaths++;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.Death, deaths);
        PhotonNetwork.player.SetCustomProperties(playerProperties);
    }
  
    private void DropFlagFromPlayer() {
        if (GameManagersHolder.Instance.GameMode != GameMode.CAPTURE_THE_FLAG || !HasFlag)
            return;

        Vector2 dropPosition = new Vector2(Mathf.Ceil(transform.position.x) - 0.5f, Mathf.Ceil(transform.position.y) - 0.5f);
        Flag flag;
        if (Team == Team.RED)
            flag = GameManagersHolder.Instance.GameManagerCTF.BlueFlag;
        else
            flag = GameManagersHolder.Instance.GameManagerCTF.RedFlag;
        flag.PlayerDropFlag(dropPosition, PlayerId);
    }

    private IEnumerator ResetCanPickupItem() {    
        yield return new WaitForSeconds(2.5f);
        CanPickupItem = true;
    }

}


public interface IInputController {
    event System.Action ChangeWeapon;
    event System.Action Jump;
    float HorizontalAxis { get; set; }  
    bool Fire { get; set; }  
    bool Crouch { get; set; }    
}

public interface IWeaponController {
    WeaponType CurrentWeapon { get; set; }
    void ResetWeapon();
    void MakeShot(Vector2 direction);
    void ChangeWeapon();
    void ChangeWeapon(WeaponType type);
    void AddBullets(WeaponType type, int bulletsCount);
}
