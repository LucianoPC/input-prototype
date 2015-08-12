using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class ControlMapEntry
		: ScriptableObject
	{
		#region Public Properties

		public InputControlData controlData
		{
			get { return _controlData; }
			set
			{
				_controlData = value;
				name = _controlData.name;
			}
		}

		// This is one entry for each control scheme (matching indices).
		public List< ControlBinding > bindings;

		[ NonSerialized ]
		public int controlIndex;
		
		public override string ToString ()
		{
			return string.Format( "({0}, bindings:{1})", controlData.name, bindings.Count );
		}

		#endregion

		#region Fields

		[ SerializeField ]
		private InputControlData _controlData;

		#endregion
	}
}