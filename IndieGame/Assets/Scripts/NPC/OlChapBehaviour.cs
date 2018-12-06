using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlChapBehaviour : MonoBehaviour
{
    private int _playerObjective = 0;

    private SoundPlayer _soundPlayer;
    private static OlChapBehaviour _olChapBehaviour;

    public static bool visitedMerchant = false;
    public static bool visitedBlacksmith = false;

    private bool _killPlayer = false;

    // Use this for initialization
    void Start()
    {
        GameController.OnMouseLeftClickGameObject += InteractWithOlChap;
        _soundPlayer = GetComponent<SoundPlayer>();

        _olChapBehaviour = this;
    }

    private void Update()
    {
        if (!_soundPlayer.GetAudioSources()[0].isPlaying || Input.GetKeyDown(KeyCode.Escape) || (GameController.player.transform.position - transform.position).magnitude > 4.9999f)
        {
            if (GameController.uiCanvas != null)
            {
                GameController.uiCanvas.CloseDialogBox();
                GameController.player.GetComponent<PlayerMovement>().enabled = true;
                GameController.player.GetComponent<PlayerAttacks>().enabled = true;

                if (_playerObjective >= 7 && _killPlayer && !_soundPlayer.GetAudioSources()[0].isPlaying)
                {
                    GameController.player.GetComponent<SoundPlayer>().PlayAudioClip(9);
                    _killPlayer = false;

                    //Show credits or whatever
                }
            }
        }
    }

    private void InteractWithOlChap(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            if (_playerObjective <= GameController.questProgress)
            {
                _playerObjective++;
                if (_playerObjective >= 5)
                    GameController.spawnKey = true;
            }
            DisplayQuestObjective();
            GameController.player.transform.LookAt(this.transform);
            GameController.player.GetComponent<PlayerMovement>().rotation.y = GameController.player.transform.rotation.eulerAngles.y;
        }
    }

    public void DisplayQuestObjective()
    {
        _soundPlayer.PlayAudioClip(_playerObjective - 1);
        if (_playerObjective > 7)
        {
            _killPlayer = true;
        }

        switch (_playerObjective)
        {
            case 1:
                GameController.uiCanvas.OpenDialogBox("Oh hello, yes you must be the new Forester, its good you’re here, the other forester… went missing… but enough about that, why don’t you head to the training ground for a little practice. We need to make sure you can defend yourself in the forest of course, so why don’t you try attacking the training dummies. You can use your sword to damage enemies up close & you can cast a spell to damage enemies from afar. Let’s see what you’ve got young one.");
                ObjectiveText.SetObjectiveText("- Find and attack the training dummies");
                break;
            case 2:
                GameController.uiCanvas.OpenDialogBox("Very good young one, you remind me of a younger version of myself hehehe, oh to be young again. Anyway it appears you’re more than capable of protecting yourself & that means that you can protect our little town too. Why don’t you try making your way into the forest now & showing me something you find there. Yes, you can find all sorts of useful things in there. Just don’t worry to much where they came from hehe oh.");
                ObjectiveText.SetObjectiveText("- Enter the portal");
                break;
            case 3:
                GameController.uiCanvas.OpenDialogBox("Ah you're back! I was beginning to get a little worried you wouldn’t return. It’s not that I doubt you, it’s just I’ve seen a lot of young adventurers walk into those forests but not many make it back. Say you look like you could use potion to restore your health or mana, you can take a look in the general store and buy all sorts of potions there. And if you’re looking for better equipment to take out with you, you should visit the blacksmith in town too. The blacksmith can make fine gear for you and you can even sell any weapons or armour you don’t want to him as well. Why don’t you make your way into town now and visit both stores. Hehe yes.");
                ObjectiveText.SetObjectiveText("- Talk to the merchant");
                break;
            case 4:
                GameController.uiCanvas.OpenDialogBox("Oh there you are, and am I glad to see you too. Those darn radishes are destroying the town’s crops. It just doesn’t make any sense why a Radish would want to destroy other plants, when it is a plant itself. Hmmm. Anyway now we really need your help, the town’s food supply is running short & I don’t think we’ll survive if the radishes destroy our crops again. We need you to teach those evil things a lesson. Go to the forest and make them pay I say! Also if you see any potatoes while you’re there do be a good boy and bring some back, OK? Oh I do love a good potato stew….. Why are you still here? C’mon on now there’s no time to waste, get going!");
                ObjectiveText.SetObjectiveText("- Enter the portal");
                break;
            case 5:
                GameController.uiCanvas.OpenDialogBox("Ah back again I see & I’ve not seen those Radishes around here either, great job teaching those Rascals a lesson. Hey, I wanted to tell you something actually. Word around town is that there is missing treasure somewhere deep within the forest. They say it’s a treasure chest that contains quite the pretty penny, worth a lot of gold indeed. Say why don’t you keep a look out for it, when you’re next in the forest? You never know when you might come across it, so just make sure to keep an eye out why don’t you.");
                ObjectiveText.SetObjectiveText("- Enter the portal");
                break;
            case 6:
                GameController.uiCanvas.OpenDialogBox("Ohhh heavens above! I can't get the warehouse door to open, I could have sworn I left the key pieces where I normally do, but they aren’t there! If I didn’t know any better I might suspect somebodies stolen them. Hmm but it could just be that I’m getting forgetful in my old age. Hmm. Could you take a look around for the key pieces? I’ll be sure to reward you if you find any! Thanks young one & good luck!");
                ObjectiveText.SetObjectiveText("- Enter the portal");
                break;
            case 7:
                GameController.uiCanvas.OpenDialogBox("Ah excellent! You found all the key pieces! I did say I’d reward you for that hehe, well funnily enough your reward is in the warehouse. Why don’t you be a good lad and open the door of the warehouse for me would you? You wouldn’t want to miss out on that reward ey, hehehe.");
                ObjectiveText.SetObjectiveText("- Go to the warhouse and open the door");
                break;
            case 8:
                GameController.uiCanvas.OpenDialogBox("Hehehe Well well well, you didn’t think I could keep someone as powerful as you in my town now would you? To tell you the truth young one, the other foresters never went missing, they all died by my hand! And your body will be a great addition to the collection hehehe. Now be a good lad young one & don’t make this difficult.");
                GameController.gameController.GetComponents<AudioSource>()[1].Stop();
                break;
            default:
                GameController.uiCanvas.OpenDialogBox("Insert hardcoded string here :-)");
                break;
        }
    }

    public static int GetQuestProgression()
    {
        return _olChapBehaviour._playerObjective;
    }

    public static void ContinuePorgression()
    {
        _olChapBehaviour._playerObjective++;
    }
}
