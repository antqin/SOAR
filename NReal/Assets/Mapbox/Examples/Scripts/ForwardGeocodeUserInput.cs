//-----------------------------------------------------------------------
// <copyright file="ForwardGeocodeUserInput.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Examples
{
	using Mapbox.Unity;
	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Mapbox.Geocoding;
	using Mapbox.Utils;

	[RequireComponent(typeof(InputField))]
	public class ForwardGeocodeUserInput : MonoBehaviour
	{
		public string enterString; // 


		InputField _inputField;

		ForwardGeocodeResource _resource;

		Vector2d _coordinate;
		public Vector2d Coordinate
		{
			get
			{
				return _coordinate;
			}
		}

		bool _hasResponse;
		public bool HasResponse
		{
			get
			{
				return _hasResponse;
			}
		}

		public ForwardGeocodeResponse Response { get; private set; }

		public event Action<ForwardGeocodeResponse> OnGeocoderResponse = delegate { };

		void Awake()
		{
			_inputField = GetComponent<InputField>();
			_inputField.onEndEdit.AddListener(HandleUserInput);
			_resource = new ForwardGeocodeResource(""); 
			Debug.Log("HI1"); 
		}

		void HandleUserInput(string searchString)
		{
			Debug.Log("HI2"); 
			_hasResponse = false;
			// searchString = "Cupertino, CA"; // 
			// searchString = "Cupertino, CA"; 
			// searchString = "Hi input here"; 
			_inputField.text = enterString; // 
			searchString = enterString; // 

			if (!string.IsNullOrEmpty(searchString))
			{
				Debug.Log("HI3"); 
				Debug.Log(searchString); 
				_resource.Query = searchString;
				MapboxAccess.Instance.Geocoder.Geocode(_resource, HandleGeocoderResponse);
			}
		}

		void HandleGeocoderResponse(ForwardGeocodeResponse res)
		{
			_hasResponse = true;
			if (null == res)
			{
				_inputField.text = "no geocode response";
			}
			else if (null != res.Features && res.Features.Count > 0)
			{
				var center = res.Features[0].Center;
				_coordinate = res.Features[0].Center;
			}
			Response = res;
			OnGeocoderResponse(res);
		}
	}
}
