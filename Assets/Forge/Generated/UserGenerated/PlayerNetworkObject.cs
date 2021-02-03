using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.45,0,0]")]
	public partial class PlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 3;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector2 _position;
		public event FieldEvent<Vector2> positionChanged;
		public InterpolateVector2 positionInterpolation = new InterpolateVector2() { LerpT = 0.45f, Enabled = true };
		public Vector2 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		[ForgeGeneratedField]
		private bool _isRunning;
		public event FieldEvent<bool> isRunningChanged;
		public Interpolated<bool> isRunningInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool isRunning
		{
			get { return _isRunning; }
			set
			{
				// Don't do anything if the value is the same
				if (_isRunning == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_isRunning = value;
				hasDirtyFields = true;
			}
		}

		public void SetisRunningDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_isRunning(ulong timestep)
		{
			if (isRunningChanged != null) isRunningChanged(_isRunning, timestep);
			if (fieldAltered != null) fieldAltered("isRunning", _isRunning, timestep);
		}
		[ForgeGeneratedField]
		private bool _grounded;
		public event FieldEvent<bool> groundedChanged;
		public Interpolated<bool> groundedInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool grounded
		{
			get { return _grounded; }
			set
			{
				// Don't do anything if the value is the same
				if (_grounded == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_grounded = value;
				hasDirtyFields = true;
			}
		}

		public void SetgroundedDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_grounded(ulong timestep)
		{
			if (groundedChanged != null) groundedChanged(_grounded, timestep);
			if (fieldAltered != null) fieldAltered("grounded", _grounded, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			isRunningInterpolation.current = isRunningInterpolation.target;
			groundedInterpolation.current = groundedInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _isRunning);
			UnityObjectMapper.Instance.MapBytes(data, _grounded);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector2>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_isRunning = UnityObjectMapper.Instance.Map<bool>(payload);
			isRunningInterpolation.current = _isRunning;
			isRunningInterpolation.target = _isRunning;
			RunChange_isRunning(timestep);
			_grounded = UnityObjectMapper.Instance.Map<bool>(payload);
			groundedInterpolation.current = _grounded;
			groundedInterpolation.target = _grounded;
			RunChange_grounded(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _isRunning);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _grounded);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector2>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector2>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (isRunningInterpolation.Enabled)
				{
					isRunningInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					isRunningInterpolation.Timestep = timestep;
				}
				else
				{
					_isRunning = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_isRunning(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (groundedInterpolation.Enabled)
				{
					groundedInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					groundedInterpolation.Timestep = timestep;
				}
				else
				{
					_grounded = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_grounded(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.UnityNear(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector2)positionInterpolation.Interpolate();
				//RunChange_position(positionInterpolation.Timestep);
			}
			if (isRunningInterpolation.Enabled && !isRunningInterpolation.current.UnityNear(isRunningInterpolation.target, 0.0015f))
			{
				_isRunning = (bool)isRunningInterpolation.Interpolate();
				//RunChange_isRunning(isRunningInterpolation.Timestep);
			}
			if (groundedInterpolation.Enabled && !groundedInterpolation.current.UnityNear(groundedInterpolation.target, 0.0015f))
			{
				_grounded = (bool)groundedInterpolation.Interpolate();
				//RunChange_grounded(groundedInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerNetworkObject() : base() { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
