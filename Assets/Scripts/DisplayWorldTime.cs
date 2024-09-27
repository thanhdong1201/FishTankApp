using UnityEngine;
using TMPro;
using System;

public class DisplayWorldTime : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI datetimeText;

	void Update()
	{
		if (WorldTimeAPI.Instance.IsTimeLodaed)
		{
			DateTime currentDateTime = WorldTimeAPI.Instance.GetCurrentDateTime();

			datetimeText.text = currentDateTime.ToString();
		}
	}
}
