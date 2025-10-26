using UnityEngine;
using UnityEngine.SceneManagement; // ต้องเพิ่มบรรทัดนี้

public class MainMenuController : MonoBehaviour
{
    // ตั้งชื่อฉากเกมของคุณไว้ที่นี่ เช่น "GameScene"
    public string gameSceneName = "GameScene"; 

    public void StartGame()
    {
        // โหลดฉากเกม
        SceneManager.LoadScene(gameSceneName); 
    }

    public void QuitGame()
    {
        // คำสั่งออกจากเกม (จะทำงานเมื่อ Build เกมแล้วเท่านั้น)
        Application.Quit();
        // สำหรับการทดสอบใน Editor
        #if UNITY_EDITOR 
        Debug.Log("Game has been quit (Editor Mode)");
        #endif
    }
}