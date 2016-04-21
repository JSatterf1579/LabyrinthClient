using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FindGame : MonoBehaviour
{

    public GameObject QueueingModal;
    public GameObject HeroSelectModal;
    public GameObject HeroScrollContent;
    public GameObject CardPrefab;
    public GameObject HeroConfirmButton;

	public Vector2 cardPosition = new Vector2(55, -70);
	public Vector2 widthHeight = new Vector2(90, 120);
	public Vector2 margin = new Vector2(10, 10);
	public int numColumns = 6;

    public Color selectedColor;
    public Color unselectedColor;

    private bool queued = false;
    private SocketIOComponent socket;

    private Dictionary<string, HeroCardData> selectedHeroes;
    private int minimumSelectedHeroes = 1;  

    //Card distance in pixels
    private int cardDist = 20;

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        socket = GameManager.instance.getSocket();
        socket.On("match_found", OnMatch);
        selectedHeroes = new Dictionary<string, HeroCardData>();
    }
	
	public void OnBack() {
		SceneManager.LoadScene("MainMenu");
	}

    public void OnHeroes()
    {
        JSONObject data = new JSONObject();
        socket.Emit("get_heroes", data, HeroesSelect);
        //Debug.Log(data);
        //data.AddField("queue_with_passbot", true);
        //socket.Emit("queue_up_heroes", data, HeroesQueue);
    }

    private void HeroesSelect(JSONObject response)
    {
        Debug.Log(response.list[0]);
        if (response.list[0].GetField("status").n == 200)
        {
            HeroSelectModal.SetActive(true);
            SetupHeroCards(response.list[0]);
            //queued = true;
            //QueueingModal.SetActive(true);


        }
    }

    private void SetupHeroCards(JSONObject heroes)
    {
        List<HeroCardData> cards = JSONDecoder.DecodeHeroCards(heroes);
        int heroesAdded = 0;
        foreach (HeroCardData card in cards)
        {
            GameObject UICard = Instantiate(CardPrefab);
            UICard.transform.SetParent(HeroScrollContent.transform);
            //UICard.transform.localPosition= new Vector3(cardDist * (heroesAdded + 1) + UICard.transform.right.x * heroesAdded, cardDist, 0);
            RectTransform rt = (RectTransform)UICard.transform;
            calcRect(ref rt, heroesAdded);

            Transform title = rt.Find("Title");
            title.gameObject.GetComponent<Text>().text = card.Name;
            Transform description = rt.Find("Description");
            description.gameObject.GetComponent<Text>().text = "Level: " + card.level;
            UICard.GetComponent<HeroCardData>().init(card.Name, card.ownerID, card.UUID, card.health, card.attack, card.defense, card.vision, card.movement, card.actionPoints, card.Weapon, card.level);
            UICard.GetComponent<Button>().onClick.AddListener(delegate {toggleHeroCard(UICard);});
            heroesAdded++;
        }
        
    }

    private void toggleHeroCard(GameObject clickedCard)
    {
        HeroCardData hero = clickedCard.GetComponent<HeroCardData>();
        if (hero != null)
        {
            if (selectedHeroes.ContainsKey(hero.UUID))
            {
                selectedHeroes.Remove(hero.UUID);
                clickedCard.GetComponent<Image>().color = unselectedColor;
            }
            else
            {
                selectedHeroes.Add(hero.UUID, hero);
                clickedCard.GetComponent<Image>().color = selectedColor;
            }
        }
        if (selectedHeroes.Count >= minimumSelectedHeroes)
        {
            HeroConfirmButton.SetActive(true);
        }
        else
        {
            HeroConfirmButton.SetActive(false);
        }

        }

    private void calcRect(ref RectTransform rt, int i)
    {
		rt.anchorMin = new Vector2(0, 1);
		rt.anchorMax = new Vector2(0, 1);
		Vector2 temp = cardPosition + new Vector2((widthHeight.x + margin.x) * (i % numColumns), -(widthHeight.y + margin.y) * (int)(i / numColumns));
		rt.localPosition = new Vector3(temp.x, temp.y, 0);
		rt.sizeDelta = widthHeight;
    }

    public void OnArchitect()
    {
        throw new NotImplementedException();
    }

    public void OnDequeue()
    {
        QueueingModal.SetActive(false);
        queued = false;
    }

    public void OnConfirmHero()
    {
        if (selectedHeroes.Count >= minimumSelectedHeroes)
        {
            JSONObject data = new JSONObject();
            data.AddField("queue_with_passbot", true);
            JSONObject heroes = new JSONObject(JSONObject.Type.ARRAY);
            data.AddField("heroes", heroes);
            foreach (string  uuid in selectedHeroes.Keys)
            {
                heroes.Add(uuid);
            }
            socket.Emit("queue_up_heroes", data, HeroesQueue);
        }
    }

    private void HeroesQueue(JSONObject response)
    {
        Debug.Log(response.list[0]);
        if (response.list[0].GetField("status").n == 200)
        {
            HeroSelectModal.SetActive(false);
            selectedHeroes = new Dictionary<string, HeroCardData>();
            queued = true;
            QueueingModal.SetActive(true);
        }
    }

    public void OnCancelHero()
    {
        selectedHeroes = new Dictionary<string, HeroCardData>();
        HeroConfirmButton.SetActive(false);
        HeroSelectModal.SetActive(false);
    }


    

    

    private void OnMatch(SocketIOEvent e)
    {
        Debug.Log(e.data);
        SceneManager.LoadScene("MatchScene");
        MatchManager.SetInitialMatchState(e.data);
        GameManager.instance.InMatch = true;
    }
}
