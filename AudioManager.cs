using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [SerializeField] private AudioClip gameBegin;
    [SerializeField] private AudioClip gameEnd;
    [SerializeField] private AudioClip moveSelf;
    [SerializeField] private AudioClip moveOpponent;
    [SerializeField] private AudioClip capture;
    [SerializeField] private AudioClip check;
    [SerializeField] private AudioClip castle;
    [SerializeField] private AudioClip promote;

    private AudioSource audioSource;
    private bool checkMated;
    private bool whiteMadeMove;
    private bool captured;
    private bool gaveCheck;
    private bool castled;
    private bool promoted;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        I = this;
    }

    void Start()
    {
        audioSource.PlayOneShot(gameBegin);
    }

    public void OnTurnPlayed()
    {
        audioSource.PlayOneShot(
            checkMated ? gameEnd :
            gaveCheck ? check :
            promoted ? promote :
            captured ? capture :
            castled ? castle :
            whiteMadeMove ? moveSelf :
            moveOpponent);

        checkMated = whiteMadeMove = captured = gaveCheck = castled = promoted = false;
    }
    
    public void OnCheckMated() => checkMated = true;
    
    public void OnMadeMove(bool whiteOrBlack) => whiteMadeMove = whiteOrBlack;

    public void OnCapture() => captured = true;

    public void OnGaveCheck() => gaveCheck = true;

    public void OnCastled() => castled = true;

    public void OnPromoted() => promoted = true;
}