using System;
using System.Collections;
using UnityEngine;
using Character.Runtime;
using PinePie.SimpleJoystick;

/// <summary>
/// Comportement d'une main qui émerge de l'eau, attrape le joueur et le traîne vers le bas.
/// À placer sur le prefab de la main. Spawné par WaterHandSpawner.
///
/// Cycle de vie :
///   1. EMERGE  — la main monte depuis sous la surface jusqu'à sa hauteur cible
///   2. CHASE   — la main glisse horizontalement vers le joueur (sur la surface)
///   3. GRAB    — collision avec le joueur → il est verrouillé, doit spammer pour se libérer
///   4. DRAG    — si le joueur ne se libère pas à temps → tiré vers le bas → respawn
///   5. RETREAT — si le joueur se libère → la main redescend et se détruit
/// </summary>
public class WaterHandController : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Paramètres Inspector
    // ─────────────────────────────────────────
    [Header("Émergence")]
    [Tooltip("Hauteur au-dessus du point de spawn jusqu'où la main monte")]
    public float emergeHeight = 1.8f;
    [Tooltip("Vitesse de montée depuis le fond")]
    public float emergeSpeed = 3f;

    [Header("Poursuite")]
    [Tooltip("Vitesse horizontale de poursuite du joueur")]
    public float chaseSpeed = 4f;
    [Tooltip("Distance à laquelle la main considère avoir atteint le joueur")]
    public float grabRange = 0.6f;

    [Header("Capture")]
    [Tooltip("Temps max (secondes) que le joueur a pour se libérer")]
    public float escapeDuration = 2.5f;
    [Tooltip("Nombre de pressions joystick requises pour se libérer")]
    public int escapeInputsRequired = 12;
    [Tooltip("Vitesse à laquelle la main tire le joueur vers le bas si non libéré")]
    public float dragSpeed = 3f;
    [Tooltip("Profondeur en Y sous le spawn avant de déclencher le respawn")]
    public float dragDepth = 4f;

    [Header("Retraite")]
    [Tooltip("Vitesse de descente quand le joueur se libère")]
    public float retreatSpeed = 5f;

    // ─────────────────────────────────────────
    // Event pour le spawner
    // ─────────────────────────────────────────
    /// <summary>Déclenché juste avant que la main se détruise.</summary>
    public event Action OnHandDestroyed;

    // ─────────────────────────────────────────
    // État interne
    // ─────────────────────────────────────────
    private enum HandState { Emerge, Chase, Grab, Drag, Retreat }
    private HandState state = HandState.Emerge;

    private PlayerController player;
    private JoystickController joystick;
    private Vector3 spawnPos;
    private Vector3 emergeTargetPos;

    // Escape mechanic
    private float escapeTimer;
    private int   escapeInputCount;
    private Vector2 lastJoystickInput;

    private void Start()
    {
        spawnPos        = transform.position;
        emergeTargetPos = spawnPos + Vector3.up * emergeHeight;
    }

    /// <summary>Appelé par WaterHandSpawner après l'instantiation.</summary>
    public void Init(PlayerController target)
    {
        player   = target;
        // Récupérer le joystick depuis le PlayerController via réflexion field
        joystick = target.GetComponentInChildren<JoystickController>();
    }

    private void Update()
    {
        switch (state)
        {
            case HandState.Emerge:  UpdateEmerge();  break;
            case HandState.Chase:   UpdateChase();   break;
            case HandState.Grab:    UpdateGrab();    break;
            case HandState.Drag:    UpdateDrag();    break;
            case HandState.Retreat: UpdateRetreat(); break;
        }
    }

    // ─── 1. EMERGE ───────────────────────────────────────────────────
    private void UpdateEmerge()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, emergeTargetPos, emergeSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, emergeTargetPos) < 0.05f)
        {
            transform.position = emergeTargetPos;
            state = HandState.Chase;
        }
    }

    // ─── 2. CHASE ────────────────────────────────────────────────────
    private void UpdateChase()
    {
        if (player == null) { SelfDestroy(); return; }

        Vector3 target = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position, target, chaseSpeed * Time.deltaTime
        );

        // Rotation du mesh vers le joueur
        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Distance horizontale seulement
        float dist = new Vector2(
            transform.position.x - player.transform.position.x,
            transform.position.z - player.transform.position.z
        ).magnitude;

        if (dist <= grabRange)
            StartGrab();
    }

    // ─── 3. GRAB ─────────────────────────────────────────────────────
    private void StartGrab()
    {
        state            = HandState.Grab;
        escapeTimer      = escapeDuration;
        escapeInputCount = 0;
        lastJoystickInput = Vector2.zero;

        player.isMovementLocked = true;

        Debug.Log("[WaterHand] Joueur attrapé ! Spam joystick pour se libérer.");
    }

    private void UpdateGrab()
    {
        if (player == null) { SelfDestroy(); return; }

        // Coller la main au joueur
        transform.position = player.transform.position + Vector3.down * 0.3f;

        escapeTimer -= Time.deltaTime;

        // Lire l'input du joystick PinePie
        Vector2 currentInput = (joystick != null) ? joystick.InputDirection : Vector2.zero;

        // Détecter un changement de direction franc (spam)
        // On compte chaque fois que la magnitude passe d'un côté à l'autre de 0.7
        float currentMag = currentInput.magnitude;
        float lastMag    = lastJoystickInput.magnitude;

        bool wasActive  = lastMag > 0.7f;
        bool nowActive  = currentMag > 0.7f;

        // Alternance actif / inactif = 1 input compté
        if (!wasActive && nowActive)
        {
            escapeInputCount++;
            Debug.Log($"[WaterHand] Escape {escapeInputCount}/{escapeInputsRequired}");
        }

        lastJoystickInput = currentInput;

        // Afficher progression (optionnel - à brancher sur une UI)
        float escapeProgress = (float)escapeInputCount / escapeInputsRequired;

        if (escapeInputCount >= escapeInputsRequired)
        {
            EscapePlayer();
            return;
        }

        if (escapeTimer <= 0f)
            state = HandState.Drag;
    }

    // ─── 4. DRAG ─────────────────────────────────────────────────────
    private void UpdateDrag()
    {
        if (player == null) { SelfDestroy(); return; }

        Vector3 dragMove = Vector3.down * dragSpeed * Time.deltaTime;

        transform.position          += dragMove;
        player.transform.position   += dragMove;

        if (transform.position.y <= spawnPos.y - dragDepth)
        {
            player.isMovementLocked = false;
            player.Respawn();
            SelfDestroy();
        }
    }

    // ─── 5. RETREAT ──────────────────────────────────────────────────
    private void EscapePlayer()
    {
        player.isMovementLocked = false;
        state = HandState.Retreat;
        Debug.Log("[WaterHand] Joueur libéré !");
    }

    private void UpdateRetreat()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, spawnPos, retreatSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, spawnPos) < 0.05f)
            SelfDestroy();
    }

    // ─────────────────────────────────────────
    // Utilitaires
    // ─────────────────────────────────────────
    private void SelfDestroy()
    {
        if (player != null && (state == HandState.Grab || state == HandState.Drag))
            player.isMovementLocked = false;

        OnHandDestroyed?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (player != null && player.isMovementLocked)
            player.isMovementLocked = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.6f);
        Vector3 origin = Application.isPlaying ? spawnPos : transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.up * emergeHeight);
        Gizmos.DrawWireSphere(origin + Vector3.up * emergeHeight, 0.15f);

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, grabRange);

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawLine(origin, origin + Vector3.down * dragDepth);
    }
#endif
}