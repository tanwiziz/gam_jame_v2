using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Value")]
    public int value = 1; // จำนวนเหรียญที่จะเพิ่มเมื่อเก็บ

    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจว่า Object ที่ชนชื่อว่า Player หรือมี tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // (ทางเลือก) แจ้งว่าผู้เล่นเก็บเหรียญได้
            Debug.Log($"Player collected coin worth {value}");

            // ทำลาย Coin ออกจาก Scene
            Destroy(gameObject);
        }
    }
}
