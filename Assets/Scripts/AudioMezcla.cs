using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioMixerSnapshot interior;
    public AudioMixerSnapshot exterior;
    [Min(0f)] public float transicion = 1f;


    public void Interior () { if (interior != null) interior.TransitionTo(transicion); }
    public void Exterior() {  if(exterior != null) exterior.TransitionTo(transicion);}
    void Start()
    {
        Interior();  
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Exterior();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Interior();
        }
    }
}
