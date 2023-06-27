using System.Collections;
using UnityEngine;

public class AutoPortcullis : MonoBehaviour
{
    public GameObject player;
    public GameObject portcullis;
    public float portcullisSpeed = 0.2f;
    public float openHeight = 10f;
    public AudioSource audioSource;
    public AudioClip[] openSFX;
    public AudioClip[] closeSFX;
    public Vector2 openPitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 closePitchRange = new Vector2(0.95f, 1.05f);
    private bool portcullisOpen = false;
    private bool insideTrigger = false;
    private bool isMoving = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = portcullis.transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
    }

    void Update()
    {
        if (insideTrigger && !portcullisOpen && !isMoving)
        {
            OpenPortcullis();
        }
        else if (!insideTrigger && portcullisOpen && !isMoving)
        {
            ClosePortcullis();
        }
    }

    private void OpenPortcullis()
    {
        portcullisOpen = true;
        PlaySound(openSFX, openPitchRange);
        StartCoroutine(MovePortcullis(openPosition));
    }

    private void ClosePortcullis()
    {
        portcullisOpen = false;
        PlaySound(closeSFX, closePitchRange);
        StartCoroutine(MovePortcullis(closedPosition));
    }

    IEnumerator MovePortcullis(Vector3 targetPosition)
    {
        isMoving = true;
        var startPosition = portcullis.transform.position;
        for (float t = 0f; t < 1; t += Time.deltaTime * portcullisSpeed)
        {
            portcullis.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        portcullis.transform.position = targetPosition;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            insideTrigger = false;
        }
    }

    private void PlaySound(AudioClip[] sounds, Vector2 pitchRange)
    {
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
}
