using UnityEngine;
using UnityEngine.Audio;

public class AudioMezcla : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot interior;
    [Min(0f)] public float transicion = 1f;


    public void Normal () { if (normal != null) normal.TransitionTo(transicion); }
    public void Interior() {  if(interior != null) interior.TransitionTo(transicion);}
    void Start()
    {
        Normal();  
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Interior();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Normal();
        }
    }
}
