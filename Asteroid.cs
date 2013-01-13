using System;
using Microsoft.Xna.Framework;

namespace Xbox360Intro {
	public struct Asteroid {
		public int TextureIndex {
			get;
			private set;
		}
		public Vector2 Position {
			get;
			private set;
		}
		private Vector2 Trajectory {
			get;
			set;
		}

		public Asteroid( System.Collections.ObjectModel.Collection<Microsoft.Xna.Framework.Graphics.Texture2D> p_Asteroids )
			: this() {
			Random _Random = new Random();
			TextureIndex = _Random.Next( 0, p_Asteroids.Count );
			Position =
				new Vector2(
					_Random.Next( 0, GraphicsDeviceManager.DefaultBackBufferWidth - p_Asteroids[TextureIndex].Width ),
					p_Asteroids[TextureIndex].Height * -1
				);

			Trajectory =
				new Vector2(
					_Random.Next( -5, 6 ),
					_Random.Next( 1, 6 )
				);
		}

		public void Move() {
			Position += Trajectory;
		}
	}
}