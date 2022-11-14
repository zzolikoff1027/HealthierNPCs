using System;
using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private int startingHp = 100;
    [SerializeField] private UnityEngine.UI.Slider hpBarSlider = null;
    [SerializeField] private ParticleSystem deathParticlePrefab = null;
    [SerializeField] private int currentHp;
    public float timePassed = 0f;
    public Material NPCMat;
    public Material DamageMat;
    public GameObject Object;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player" && timePassed > 1f)
        {
            Object.GetComponent<MeshRenderer>().material = DamageMat;
            TakeDamage(startingHp / 10);
            timePassed = 0f;
            StartCoroutine(damageOverTime());
        }
    }

    private void Start()
    {
        currentHp = startingHp;
        Object.GetComponent<MeshRenderer>().material = NPCMat;
    }

    internal void TakeDamage(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException("Invalid Damage amount specified: " + amount);

        currentHp -= amount;

        UpdateUI();

        if (currentHp <= 0)
            Die();
    }

    private void UpdateUI()
    {
        var currentHpPct = (float)currentHp / (float)startingHp;

        hpBarSlider.value = currentHpPct;
    }

    private void Die()
    {
        PlayDeathParticle();
        GameObject.Destroy(this.gameObject);
    }

    private void PlayDeathParticle()
    {
        var deathparticle = Instantiate(deathParticlePrefab, transform.position, deathParticlePrefab.transform.rotation);
        Destroy(deathparticle, 4f);
    }

    IEnumerator damageOverTime()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(startingHp / 20);
        }
        Object.GetComponent<MeshRenderer>().material = NPCMat;
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
        hpBarSlider.transform.LookAt(Camera.main.transform);
    }
}