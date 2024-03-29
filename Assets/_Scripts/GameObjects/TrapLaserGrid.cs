﻿using FMODUnity;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
	[RequireComponent (typeof(Collider2D))]
	[RequireComponent (typeof(SpriteRenderer))]
	public class TrapLaserGrid : InGameObject
	{
		[EventRef]
		public string laserOnSound = "event:/laser_on";

        [EventRef]
		public string laserOffSound = "event:/laser_off";

	    [EventRef]
	    public string LaserCutSound;

		public override int Layer { get { return 1; } }

		public int Level { get; set; }

		public override bool IsDynamic { get { return true; } }

		protected bool armed = true;

		public override bool IsTraversableAt(GridPosition position)
		{
			return !armed;
		}

		public override void GameStart()
		{
			HackerInterface.Instance.OnTrapPowerChanged += OnTrapPowerChanged;
			Level = 3;
		}

		/// <summary>
		/// Sets the power of the trap and armed.
		/// </summary>
		/// <param name="newTrapPower">New trap power.</param>
		private void OnTrapPowerChanged(int newTrapPower)
		{
			if ((Level > newTrapPower) && !armed)
			{
				FMODUnity.RuntimeManager.PlayOneShot (laserOnSound, transform.position);
				GetComponent<SpriteRenderer>().enabled = true;
				armed = true;
			}
			else if ((Level <= newTrapPower) && armed)
			{
				FMODUnity.RuntimeManager.PlayOneShot (laserOffSound, transform.position);
				GetComponent<SpriteRenderer>().enabled = false;
				armed = false;
			}
		}

        [UnityMessage]
		public void OnTriggerEnter2D(Collider2D otherCollider)
		{
            if (otherCollider.tag == "Player" && armed)
            {
                RuntimeManager.PlayOneShot(LaserCutSound, transform.position);
                GameStateController.Instance.PlayerDied();
            }
		}

        [UnityMessage]
		public void OnTriggerStay2D( Collider2D otherCollider)
		{
            if (otherCollider.tag == "Player" && armed)
            {
                RuntimeManager.PlayOneShot(LaserCutSound, transform.position);
                GameStateController.Instance.PlayerDied();
            }
		}
	}
}