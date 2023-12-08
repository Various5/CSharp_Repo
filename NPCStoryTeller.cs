using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class NPCStoryTeller : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text interactionPromptText;
    public AudioSource audioSource;
    public AudioClip storyAudio;
    public Camera playerCamera;
    public TextAsset storyTextFile;
    public Image dialogueBackground;

    private bool isPlayerNear = false;
    private bool isStoryPlaying = false;
    private string[] storyWords;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    void Start()
    {
        if (storyTextFile != null)
        {
            storyWords = storyTextFile.text.Split(new char[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogError("Story text file not assigned");
            storyWords = new string[0];
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!isStoryPlaying)
            {
                StartStory();
            }
            else
            {
                SkipStory();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true; // Set this to true
            interactionPromptText.text = "Press E to Interact"; // Show the prompt
                                                                // Do not deactivate the dialogue background here
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionPromptText.text = ""; // Hide the prompt
            if (isStoryPlaying)
            {
                SkipStory();
            }
            else
            {
                dialogueBackground.gameObject.SetActive(false); // Hide the background when the player leaves
            }
        }
    }

    void StartStory()
    {
        isStoryPlaying = true;
        audioSource.clip = storyAudio;
        audioSource.Play();

        dialogueBackground.gameObject.SetActive(true);
        // Save original camera position and rotation
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        // Focus camera on NPC face
        playerCamera.transform.LookAt(this.transform.position);

        // Start displaying story words
        StartCoroutine(DisplayStoryWords());
    }

    IEnumerator DisplayStoryWords()
    {
        dialogueText.text = "";
        float wordDisplayTime = storyAudio.length / storyWords.Length;

        foreach (string word in storyWords)
        {
            dialogueText.text += word + " ";
            yield return new WaitForSeconds(wordDisplayTime);
        }
    }

    void SkipStory()
    {
        StopAllCoroutines();
        isStoryPlaying = false;
        dialogueText.text = "";
        audioSource.Stop();
        
        // Restore original camera position and rotation
        playerCamera.transform.position = originalCameraPosition;
        playerCamera.transform.rotation = originalCameraRotation;
        dialogueBackground.gameObject.SetActive(false);

    }
}
