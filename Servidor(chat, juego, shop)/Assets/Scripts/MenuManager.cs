using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager instance;

    public static MenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(MenuManager).Name);
                    instance = singleton.AddComponent<MenuManager>();
                }
            }
            return instance;
        }
    }

    public GameObject menuPanel;
    private bool isMenuVisible = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideMenu();
    }

    public void ToggleMenu()
    {
        if (isMenuVisible)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }

    private void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            isMenuVisible = true;
        }
    }

    private void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            isMenuVisible = false;
        }
    }
}
