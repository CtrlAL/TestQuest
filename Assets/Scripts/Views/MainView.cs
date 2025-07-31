using Services.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainView : MonoBehaviour
{
	[SerializeField] Button _switchButton;
	[Inject] ITabService _tabService;

	public void Start()
	{
		Debug.Log("in it");

		_switchButton.onClick.AddListener(() =>
		{
			_tabService.MoveNext();
		});
	}
}
