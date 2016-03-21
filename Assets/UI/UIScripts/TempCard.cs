using System.Collections;

public class TempCard {

	public bool IsMonster { private set; get; }

	public string Name { private set; get; }

	public int Cost { private set; get; }

	public int Quantity { get; set; }

	public TempCard(string name, int cost, bool isMonster, int quantity) {
		Name = name;
		Cost = cost;
		IsMonster = isMonster;
		Quantity = quantity;
	}

}
