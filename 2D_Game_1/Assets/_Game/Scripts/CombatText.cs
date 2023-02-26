using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
    [SerializeField] Text hpText;

    public void OnInit(float damage)
    {
        hpText.text = damage.ToString();
        Invoke(nameof(OnRespawn), 1f);
    }

    public void OnRespawn()
    {
        Destroy(gameObject);
    }
}
