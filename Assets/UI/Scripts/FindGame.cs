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
    public Dropdown MapSelect;
    public Dropdown GameMode;
    public Toggle Passbot;


    public GameObject ArchitectModal;
    public Dropdown ArchitectMap;
    public Dropdown ArchGamemode;
    public Toggle ArchitectPassbot;


    public Camera camera;

	public Vector2 cardPosition = new Vector2(55, -70);
	public Vector2 widthHeight = new Vector2(90, 120);
	public Vector2 margin = new Vector2(10, 10);
	public int numColumns = 6;

    public Color selectedColor;
    public Color unselectedColor;

    private bool queued = false;
    private bool matchFound = false;
    private SocketIOComponent socket;

    private Dictionary<string, HeroCardData> selectedHeroes;
    private Dictionary<int, MapMetadata> mapSelectPosition; 
    private int minimumSelectedHeroes = 1;  
    private int maxHeroes = 4;

    //Card distance in pixels
    private int cardDist = 20;

    private JSONObject MatchData;

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        socket = GameManager.instance.getSocket();
        socket.On("match_found", OnMatch);
        selectedHeroes = new Dictionary<string, HeroCardData>();
        mapSelectPosition = new Dictionary<int, MapMetadata>();
    }
	
	public void OnBack() {
        socket.Off("match_found", OnMatch);
        SceneManager.LoadScene("MainMenu");
	}

    void OnDestroy() {
        socket.Off("match_found", OnMatch);
    }

    public void OnHeroes()
    {
        JSONObject data = new JSONObject();
        socket.Emit("get_heroes", data, HeroesSelect);
        socket.Emit("maps", data, ListMaps);
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


        }
    }

    private void ListMaps(JSONObject response)
    {
        Debug.Log(response.list[0]);
        if (response.list[0].GetField("status").n == 200)
        {
            MapSelect.ClearOptions();
            mapSelectPosition.Clear();
            List<MapMetadata> maps = JSONDecoder.DecodeMapMetadata(response.list[0].GetField("maps"));
            for (int i = 0; i < maps.Count; i++)
            {
                MapSelect.options.Add(new Dropdown.OptionData() {text = maps[i].Name});
                mapSelectPosition[i] = maps[i];
            }
            MapSelect.value = 1;
            MapSelect.value = 0;
        }
    }

    public void ChangeMap(int ddID)
    {
        maxHeroes = mapSelectPosition[ddID].HeroCapacity;
    }

    private void SetupHeroCards(JSONObject heroes)
    {
        List<HeroCardData> cards = JSONDecoder.DecodeHeroCards(heroes);
        int heroesAdded = 0;
        ClearDisplay();
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

    public void ClearDisplay()
    {
        foreach (Transform b in HeroScrollContent.transform)
        {
            removeContentFromDisplay(b.gameObject);
        }
    }

    private void removeContentFromDisplay(GameObject b)
    {
        Destroy(b);
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
                if (selectedHeroes.Count < maxHeroes)
                {
                selectedHeroes.Add(hero.UUID, hero);
                clickedCard.GetComponent<Image>().color = selectedColor;
            }
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
        rt.sizeDelta = ResizeToCurrentResolution(widthHeight); ;
    }

    private Vector2 ResizeToCurrentResolution(Vector2 vector)
    {
        RectTransform rt = (RectTransform)HeroScrollContent.transform;
        return new Vector2(vector.x * rt.lossyScale.x / 0.9f, vector.y * rt.lossyScale.y / 0.9f);
    }


    public void OnArchitect()
    {
        ArchitectModal.SetActive(true);
        socket.Emit("maps", new JSONObject(), OnMaps);
    }

    public void OnMaps(JSONObject response)
    {

        Debug.Log(response.list[0]);
        if (response.list[0].GetField("status").n == 200)
        {
            MapSelect.ClearOptions();
            mapSelectPosition.Clear();
            List<MapMetadata> maps = JSONDecoder.DecodeMapMetadata(response.list[0].GetField("maps"));
            for (int i = 0; i < maps.Count; i++)
            {
                ArchitectMap.options.Add(new Dropdown.OptionData() { text = maps[i].Name });
                mapSelectPosition[i] = maps[i];
            }
            MapSelect.value = 1;
            MapSelect.value = 0;
        }
    }



    public void OnArchitectConfirm()
    {
        JSONObject data = new JSONObject();
        data.AddField("map_id", mapSelectPosition[ArchitectMap.value].ID);
        socket.Emit("map", data, (x) => {
            Debug.Log(x);
            Debug.Log(x.type);
            MonsterPlacementManager.InitialMap = x[0]["map"];
            JSONObject root = new JSONObject();
            root.AddField("map_id", mapSelectPosition[ArchitectMap.value].ID);
            root.AddField("queue_with_passbot", ArchitectPassbot.isOn);
            switch (ArchGamemode.value)
            {
                case 0:
                    data.AddField("game_mode", "dm");
                    break;
                case 1:
                    data.AddField("game_mode", "obj");
                    break;
                default:
                    data.AddField("game_mode", "dm");
                    break;
            }
            MonsterPlacementManager.JSONRoot = root;
            SceneManager.LoadScene("ArchSetupScene");
        });

    }

    public void OnDequeue()
    {
        socket.Emit("dequeue", new JSONObject(), DequeueCallback);
    }

    public void DequeueCallback(JSONObject response)
    {
        if (response.list[0].GetField("status").n == 200)
        {
        QueueingModal.SetActive(false);
        queued = false;
    }
    }

    public void OnConfirmHero()
    {
        if (selectedHeroes.Count >= minimumSelectedHeroes)
        {
            JSONObject data = new JSONObject();
            if (Passbot.isOn)
            {
            data.AddField("queue_with_passbot", true);
            }
            JSONObject heroes = new JSONObject(JSONObject.Type.ARRAY);
            data.AddField("heroes", heroes);
            foreach (string  uuid in selectedHeroes.Keys)
            {
                heroes.Add(uuid);
            }
            //data.AddField("game_mode", "obj");
            switch (GameMode.value)
            {
                case 0:
                    data.AddField("game_mode", "dm");
                    break;
                case 1:
            data.AddField("game_mode", "obj");
                    break;
                default:
                    data.AddField("game_mode", "dm");
                    break;
            }
            if (MapSelect.options[MapSelect.value] != null)
            {
                data.AddField("map_id", mapSelectPosition[MapSelect.value].ID);
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
            QueueingModal.SetActive(true);
            queued = true;
        }
        else if (response.list[0].GetField("status").n == 422)
        {
            socket.Emit("dequeue", new JSONObject());
            socket.Emit("leave_match", new JSONObject(), AutoLeaveMatch);
        }
    }

    private void AutoLeaveMatch(JSONObject response)
    {
        Debug.Log(response.list[0]);
        if (response.list[0].GetField("status").n == 200)
        {
            OnConfirmHero();
        }
    }

    public void OnCancelHero()
    {
        selectedHeroes = new Dictionary<string, HeroCardData>();
        HeroConfirmButton.SetActive(false);
        HeroSelectModal.SetActive(false);
        ClearDisplay();
    }

    public static void OnMatch(SocketIOEvent e)
    {
        Debug.Log(e.data);
        GameManager.instance.getSocket().Off("match_found", OnMatch);
        MatchManager.SetInitialMatchState(e.data);
        GameManager.instance.InMatch = true;
        SceneManager.LoadScene("MatchScene");
    }


    public struct MapMetadata
    {
        public int ID;
        public int HeroCapacity;
        public string Name;
        public string CreatorID;
        public int x;
        public int y;

        public MapMetadata(int id, int cap, string name, string creator, int x, int y)
        {
            ID = id;
            HeroCapacity = cap;
            Name = name;
            CreatorID = creator;
            this.x = x;
            this.y = y;
        }
    }
}
