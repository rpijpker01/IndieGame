using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HighlightGameobject), typeof(DropLoot))]
public class LootableChest : MonoBehaviour
{
    private Animator _openChestAnimation;

    private void Start()
    {
        _openChestAnimation = GetComponent<Animator>();

        GameController.OnMouseLeftClickGameObject += OpenChest;
    }

    private void OpenChest(GameObject go)
    {
        if (this.gameObject != go) return;

        if (!_openChestAnimation.GetBool("isClicked"))
        {
            GetComponent<DropLoot>().DropItems(this.transform);
            _openChestAnimation.SetBool("isClicked", true);

            if (OlChapBehaviour.GetQuestProgression() == 5)
            {
                OlChapBehaviour.ContinuePorgression();
                ObjectiveText.SetObjectiveText("- Go back to the old man");
            }
        }
    }
}
