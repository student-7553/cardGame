using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessTester : MonoBehaviour
{
	public List<int> processIds;

	void Start()
	{
		foreach (int processId in processIds)
		{
			CardHandler.current.playerCardTracker.ensureOneTimeProcessTracked(processId);
		}
	}
}
