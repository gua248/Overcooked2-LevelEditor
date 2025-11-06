using UnityEngine;

public class TransactionHitBuilder : HitBuilder<TransactionHitBuilder>
{
	private string transactionID = string.Empty;

	private string affiliation = string.Empty;

	private double revenue;

	private double tax;

	private double shipping;

	private string currencyCode = string.Empty;

	public string GetTransactionID()
	{
		return transactionID;
	}

	public TransactionHitBuilder SetTransactionID(string transactionID)
	{
		if (transactionID != null)
		{
			this.transactionID = transactionID;
		}
		return this;
	}

	public string GetAffiliation()
	{
		return affiliation;
	}

	public TransactionHitBuilder SetAffiliation(string affiliation)
	{
		if (affiliation != null)
		{
			this.affiliation = affiliation;
		}
		return this;
	}

	public double GetRevenue()
	{
		return revenue;
	}

	public TransactionHitBuilder SetRevenue(double revenue)
	{
		this.revenue = revenue;
		return this;
	}

	public double GetTax()
	{
		return tax;
	}

	public TransactionHitBuilder SetTax(double tax)
	{
		this.tax = tax;
		return this;
	}

	public double GetShipping()
	{
		return shipping;
	}

	public TransactionHitBuilder SetShipping(double shipping)
	{
		this.shipping = shipping;
		return this;
	}

	public string GetCurrencyCode()
	{
		return currencyCode;
	}

	public TransactionHitBuilder SetCurrencyCode(string currencyCode)
	{
		if (currencyCode != null)
		{
			this.currencyCode = currencyCode;
		}
		return this;
	}

	public override TransactionHitBuilder GetThis()
	{
		return this;
	}

	public override TransactionHitBuilder Validate()
	{
		if (string.IsNullOrEmpty(transactionID))
		{
			Debug.LogWarning("No transaction ID provided - Transaction hit cannot be sent.");
			return null;
		}
		if (string.IsNullOrEmpty(affiliation))
		{
			Debug.LogWarning("No affiliation provided - Transaction hit cannot be sent.");
			return null;
		}
		if (revenue == 0.0)
		{
			Debug.Log("Revenue in transaction hit is 0.");
		}
		if (tax == 0.0)
		{
			Debug.Log("Tax in transaction hit is 0.");
		}
		if (shipping == 0.0)
		{
			Debug.Log("Shipping in transaction hit is 0.");
		}
		return this;
	}
}
