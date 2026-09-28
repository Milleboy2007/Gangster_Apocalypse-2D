using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioJeu : MonoBehaviour
{
    // L'astuce magique pour y accéder depuis n'importe où
    public static AudioJeu instance; 

    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)] private float volumeEffets = 0.75f;

    [Header("Fichiers Audio")]
    public AudioClip sonTirer;
    public AudioClip sonFrapper;
    public AudioClip sonMarche;
    public AudioClip sonCollect;
    
    private AudioSource source;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        
        source.spatialBlend = 0f; 
    }

    public void JouerTir() => Jouer(sonTirer);
    public void JouerFrappe() => Jouer(sonFrapper);
    public void JouerMarche() => Jouer(sonMarche);
    public void JouerCollect() => Jouer(sonCollect);

    public void Jouer(AudioClip clip)
    {
        if (clip != null) 
        {
            source.PlayOneShot(clip, volumeEffets); 
        }
    }
}