//-----------------------------------------------------------------------
// <copyright file="DirectionsExample.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Examples.Playground
{
	using Mapbox.Unity;
	using System;
	using UnityEngine;
	using UnityEngine.UI;
	using Mapbox.Json;
	using Mapbox.Directions;
	using Mapbox.Utils;
	using Mapbox.Utils.JsonConverters;
	using Mapbox.Geocoding;
	using System.Collections.Generic;

	public class DirectionOutput
	{
		public List<Route> routes { get; set; }
	}

	public class Route
	{
		public List<Leg> legs { get; set; }
	}

	public class Leg
	{
		public List<Step> steps { get; set; }
	}

	public class Step
	{
		public Maneuver maneuver { get; set; }
	}

	public class Maneuver
	{
		public int bearing_after { get; set; }
		public string type { get; set; }
		public string modifier { get; set; }
		public int bearing_before { get; set; }
		public List<double> Location { get; set; }
		public string instruction { get; set; }
	}

	/// <summary>
	/// Fetch directions JSON once start and end locations are provided.
	/// Example: Enter Start Location: San Francisco, Enter Destination: Los Angeles
	/// </summary>
	public class DirectionsExample : MonoBehaviour
	{
		// [SerializeField]
		// Text _resultsText;

		[SerializeField]
		ForwardGeocodeUserInput _startLocationGeocoder;

		[SerializeField]
		ForwardGeocodeUserInput _endLocationGeocoder;

		Directions _directions;

		Vector2d[] _coordinates;

		DirectionResource _directionResource;

		public List<string> instructions = new List<string>();
    	public List<string> modifiers = new List<string>();

		void Start()
		{
			_directions = MapboxAccess.Instance.Directions;
			_startLocationGeocoder.OnGeocoderResponse += StartLocationGeocoder_OnGeocoderResponse;
			_endLocationGeocoder.OnGeocoderResponse += EndLocationGeocoder_OnGeocoderResponse;

			_coordinates = new Vector2d[2];

			// Can we make routing profiles an enum?
			_directionResource = new DirectionResource(_coordinates, RoutingProfile.Driving);
			_directionResource.Steps = true;

			// Set navigation lists to the lists here
			GameObject directionDisplayObject = GameObject.Find("DirectionInstructions");
			DirectionDisplay directionDisplay = directionDisplayObject.GetComponent<DirectionDisplay>();

			directionDisplay.instructions = instructions;
			directionDisplay.modifiers = modifiers;
		}

		void OnDestroy()
		{
			if (_startLocationGeocoder != null)
			{
				_startLocationGeocoder.OnGeocoderResponse -= StartLocationGeocoder_OnGeocoderResponse;
			}

			if (_startLocationGeocoder != null)
			{
				_startLocationGeocoder.OnGeocoderResponse -= EndLocationGeocoder_OnGeocoderResponse;
			}
		}

		/// <summary>
		/// Start location geocoder responded, update start coordinates.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void StartLocationGeocoder_OnGeocoderResponse(ForwardGeocodeResponse response)
		{
			_coordinates[0] = _startLocationGeocoder.Coordinate;
			if (ShouldRoute())
			{
				Route();
			}
		}

		/// <summary>
		/// End location geocoder responded, update end coordinates.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void EndLocationGeocoder_OnGeocoderResponse(ForwardGeocodeResponse response)
		{
			_coordinates[1] = _endLocationGeocoder.Coordinate;
			if (ShouldRoute())
			{
				Route();
			}
		}

		/// <summary>
		/// Ensure both forward geocoders have a response, which grants access to their respective coordinates.
		/// </summary>
		/// <returns><c>true</c>, if both forward geocoders have a response, <c>false</c> otherwise.</returns>
		bool ShouldRoute()
		{
			return _startLocationGeocoder.HasResponse && _endLocationGeocoder.HasResponse;
		}

		/// <summary>
		/// Route 
		/// </summary>
		void Route()
		{
			_directionResource.Coordinates = _coordinates;
			_directions.Query(_directionResource, HandleDirectionsResponse);
		}
		/// <summary>
		/// Log directions response to UI.
		/// </summary>
		/// <param name="res">Res.</param>
		void HandleDirectionsResponse(DirectionsResponse res)
		{
			var data = JsonConvert.SerializeObject(res, Formatting.Indented, JsonConverters.Converters);
			// string sub = data.Substring(0, data.Length > 5000 ? 5000 : data.Length) + "\n. . . ";
			Debug.Log(data);
			Debug.Log(res);
			// _resultsText.text = sub;

			// Deserialize the JSON string to an object
			DirectionOutput parsedResponse = JsonConvert.DeserializeObject<DirectionOutput>(data);

			// Access each "maneuver" object
			foreach (var route in parsedResponse.routes)
			{
				foreach (var leg in route.legs)
				{
					foreach (var step in leg.steps)
					{
						// Access the "maneuver" object
						var maneuver = step.maneuver;

						// Access the properties of the "maneuver" object
						var bearingAfter = maneuver.bearing_after;
						var type = maneuver.type;
						var modifier = maneuver.modifier;
						var bearingBefore = maneuver.bearing_before;
						var location = maneuver.Location;
						var instruction = maneuver.instruction;

						// Add instruction to the list
						if (!string.IsNullOrEmpty(instruction))
						{
							instructions.Add(instruction);
						}
						else
						{
							instructions.Add(string.Empty);
						}

						// Add modifier to the list
						if (!string.IsNullOrEmpty(modifier))
						{
							modifiers.Add(modifier);
						}
						else
						{
							modifiers.Add(string.Empty);
						}

						// Do something with the maneuver properties
						Debug.Log("Bearing After: " + bearingAfter);
						Debug.Log("Type: " + type);
						Debug.Log("Modifier: " + modifier);
						Debug.Log("Bearing Before: " + bearingBefore);
						Debug.Log("Location: " + location[0] + ", " + location[1]);
						Debug.Log("Instruction: " + instruction);
					}
				}
			}
		}
	}
}
