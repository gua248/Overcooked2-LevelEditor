using UnityEngine;

public class ItemHitBuilder : HitBuilder<ItemHitBuilder>
{
	private string transactionID = string.Empty;

	private string name = string.Empty;

	private string SKU = string.Empty;

	private double price;

	private string category = string.Empty;

	private long quantity;

	private string currencyCode = string.Empty;

	public string GetTransactionID()
	{
		return transactionID;
	}

	public ItemHitBuilder SetTransactionID(string transactionID)
	{
		if (transactionID != null)
		{
			this.transactionID = transactionID;
		}
		return this;
	}

	public string GetName()
	{
		return name;
	}

	public ItemHitBuilder SetName(string name)
	{
		if (name != null)
		{
			this.name = name;
		}
		return this;
	}

	public string GetSKU()
	{
		return name;
	}

	public ItemHitBuilder SetSKU(string SKU)
	{
		if (SKU != null)
		{
			this.SKU = SKU;
		}
		return this;
	}

	public double GetPrice()
	{
		return price;
	}

	public ItemHitBuilder SetPrice(double price)
	{
		this.price = price;
		return this;
	}

	public string GetCategory()
	{
		return category;
	}

	public ItemHitBuilder SetCategory(string category)
	{
		if (category != null)
		{
			this.category = category;
		}
		return this;
	}

	public long GetQuantity()
	{
		return quantity;
	}

	public ItemHitBuilder SetQuantity(long quantity)
	{
		this.quantity = quantity;
		return this;
	}

	public string GetCurrencyCode()
	{
		return currencyCode;
	}

	public ItemHitBuilder SetCurrencyCode(string currencyCode)
	{
		if (currencyCode != null)
		{
			this.currencyCode = currencyCode;
		}
		return this;
	}

	public override ItemHitBuilder GetThis()
	{
		return this;
	}

	public override ItemHitBuilder Validate()
	{
		if (string.IsNullOrEmpty(transactionID))
		{
			Debug.LogWarning("No transaction ID provided - Item hit cannot be sent.");
			return null;
		}
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogWarning("No name provided - Item hit cannot be sent.");
			return null;
		}
		if (string.IsNullOrEmpty(SKU))
		{
			Debug.LogWarning("No SKU provided - Item hit cannot be sent.");
			return null;
		}
		if (price == 0.0)
		{
			Debug.Log("Price in item hit is 0.");
		}
		if (quantity == 0)
		{
			Debug.Log("Quantity in item hit is 0.");
		}
		return this;
	}
}
