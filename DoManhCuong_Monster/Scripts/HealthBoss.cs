using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HealthBoss : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;
    public Slider healthSlider;  // Tham chiếu đến thanh trượt UI
    [SerializeField] int point = 100;
    bool wasCollected = false;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = health;

        if (healthSlider != null)
        {
            healthSlider.maxValue = health;  // Đặt giá trị tối đa cho thanh trượt
            healthSlider.value = currentHealth;  // Đặt giá trị ban đầu cho thanh trượt
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
        }


        if (health <= 0 && !wasCollected)
        {
            wasCollected = true;
            anim.SetBool("isDead", true);
            Debug.Log("Enemy Dead");
            rb.velocity = Vector2.zero;
            rb.simulated = false;  // Tắt physics khi chết
            Invoke(nameof(DestroyGameObject), 2f);
            FindObjectOfType<Gamesession>().addScore(point);
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;  // Cập nhật thanh máu UI
        }
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }

}
