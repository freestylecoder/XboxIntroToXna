//#define Section1
//#define Section2
//#define Section3
//#define Section4
//#define Section5
//#define Section6

using System;
using System.Text;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Xbox360Intro {
	public class Game1 : Game {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

#if Section1 // Player variables
		Texture2D Player;
		Vector2 PlayerPosition;
#endif

#if Section2 // Bullet variables
		Texture2D Bullet;
		GamePadState LastGamePadState;
		Collection<Vector2> BulletLocations;
#endif

#if Section3 // Asteroid variables
		TimeSpan TimeToAsteroid;
		Collection<Asteroid> Asteroids;
		Collection<Texture2D> AsteroidTextures;
#endif

#if Section5 // Game State variables
		int Score;
		int Lives;
		SpriteFont ScoreFont;
		TimeSpan GameOverTime;
		GameStates CurerntState;
#endif

#if Section6 // Splash Variables
		Texture2D TnwOps;
		Texture2D Conference;
#endif

		public Game1() {
			graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
#if Section5 // Game State
			CurerntState = GameStates.Loading;
#endif
		}

		protected override void Initialize() {
#if Section5 // Game State
			Score = 0;
			Lives = 0;
#endif
			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch( GraphicsDevice );

#if Section1 // Player
			Player = this.Content.Load<Texture2D>( "Ship" );
			PlayerPosition =
				new Vector2(
					( GraphicsDevice.Viewport.Width / 2 ) - ( Player.Width / 2 ),
					GraphicsDevice.Viewport.Height - ( Player.Height )
				);
#endif

#if Section2 // Bullets
			Bullet = this.Content.Load<Texture2D>( "Bullet" );
			BulletLocations = new Collection<Vector2>();
			LastGamePadState = GamePad.GetState( PlayerIndex.One );
#endif

#if Section3 // Asteroids
			TimeToAsteroid = TimeSpan.Zero;
			Asteroids = new Collection<Asteroid>();
			AsteroidTextures = new Collection<Texture2D>();
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\blue-one" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\cool-rock" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\exploder" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\green-chunk" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\frozen" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\hot-rock" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\hunk-o-rock" ) );
			AsteroidTextures.Add( this.Content.Load<Texture2D>( "Asteroids\\magma" ) );
#endif

#if Section5 // Game State
			ScoreFont = this.Content.Load<SpriteFont>( "ScoreFont" );
			CurerntState = GameStates.Menu;
#endif

#if Section6 // Splash
			TnwOps = this.Content.Load<Texture2D>( "Logos\\TnwOps" );
			Conference = this.Content.Load<Texture2D>( "Logos\\Conference" );
			CurerntState = GameStates.Splash;
#endif
		}

		protected override void Update( GameTime gameTime ) {
			if( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
				this.Exit();

#if Section5 // Game State
			switch( CurerntState ) {
			    case GameStates.Menu:
			        if( GamePad.GetState( PlayerIndex.One ).Buttons.A == ButtonState.Pressed )
			            if( LastGamePadState.Buttons.A == ButtonState.Released ) {
			                Score = 0;
			                Lives = 3;
			                LastGamePadState = GamePad.GetState( PlayerIndex.One );
			                CurerntState = GameStates.Playing;
			            }
			        break;
			    case GameStates.GameOver:
			        if( GameOverTime < TimeSpan.Zero )
			            CurerntState = GameStates.Menu;

			        GameOverTime -= gameTime.ElapsedGameTime;
			        break;
			    case GameStates.Paused:
			        if( GamePad.GetState( PlayerIndex.One ).Buttons.Start == ButtonState.Pressed )
			            if( LastGamePadState.Buttons.Start == ButtonState.Released )
			                CurerntState = GameStates.Playing;
			        break;
			    case GameStates.Playing:
			        if( GamePad.GetState( PlayerIndex.One ).Buttons.Start == ButtonState.Pressed )
			            if( LastGamePadState.Buttons.Start == ButtonState.Released ) {
			                CurerntState = GameStates.Paused;
			                break;
			            }
#endif
#if Section1 // Player
			PlayerPosition += ( GamePad.GetState( PlayerIndex.One ).ThumbSticks.Left * new Vector2( 5, -5 ) );

			#region Keep On Screen
			PlayerPosition.X = MathHelper.Clamp( PlayerPosition.X, 0, GraphicsDevice.Viewport.Width - Player.Width );
			PlayerPosition.Y = MathHelper.Clamp( PlayerPosition.Y, 0, GraphicsDevice.Viewport.Height - Player.Height );
			#endregion
#endif

#if Section2 // Bullets
			#region Move
			for( int _lcv = 0; _lcv < BulletLocations.Count; ++_lcv ) {
				Vector2 _Bullet = BulletLocations[_lcv];
				_Bullet -= new Vector2( 0, 10 );
				if( ( _Bullet.Y + Bullet.Height ) > 0 ) {
					BulletLocations[_lcv] = _Bullet;
				} else {
					BulletLocations.RemoveAt( _lcv-- );
				}
			}
			#endregion

			#region Fire
			if( GamePad.GetState( PlayerIndex.One ).Buttons.A == ButtonState.Pressed )
				if( LastGamePadState.Buttons.A == ButtonState.Released )
					BulletLocations.Add( new Vector2( PlayerPosition.X + ( Player.Width / 2 ), PlayerPosition.Y ) );

			LastGamePadState = GamePad.GetState( PlayerIndex.One );
			#endregion
#endif

#if Section3 // Asteroid
			#region Move
			for( int _lcv = 0; _lcv < Asteroids.Count; ++_lcv ) {
				Asteroid _Asteroid = Asteroids[_lcv];
				_Asteroid.Move();
				if(
					( _Asteroid.Position.Y > GraphicsDevice.Viewport.Height ) ||
					( _Asteroid.Position.X > GraphicsDevice.Viewport.Width ) ||
					( _Asteroid.Position.X < ( AsteroidTextures[_Asteroid.TextureIndex].Width * -1 ) )
				) {
					Asteroids.RemoveAt( _lcv-- );
				} else {
					Asteroids[_lcv] = _Asteroid;
				}
			}
			#endregion

			#region Create
			if( TimeToAsteroid > TimeSpan.Zero ) {
				TimeToAsteroid -= gameTime.ElapsedGameTime;
			} else {
				TimeToAsteroid = new TimeSpan( 0, 0, 0, 0, 500 );
				Asteroids.Add( new Asteroid( AsteroidTextures ) );
			}
			#endregion
#endif

#if Section4 // Collision Detection
			Rectangle _PlayerRect =
			    new Rectangle(
			        Convert.ToInt32( PlayerPosition.X ),
			        Convert.ToInt32( PlayerPosition.Y ),
			        Player.Width,
			        Player.Height
			    );

			for( int _AsteroidIndex = 0; _AsteroidIndex < Asteroids.Count; ++_AsteroidIndex ) {
			    bool _DestroyAsteroid = false;
			    Asteroid _Asteroid = Asteroids[_AsteroidIndex];
			    Rectangle _AsteroidRect =
			        new Rectangle(
			            Convert.ToInt32( _Asteroid.Position.X ),
			            Convert.ToInt32( _Asteroid.Position.Y ),
			            AsteroidTextures[_Asteroid.TextureIndex].Width,
			            AsteroidTextures[_Asteroid.TextureIndex].Height
			        );

			    for( int _BulletIndex = 0; _BulletIndex < BulletLocations.Count; ++_BulletIndex ) {
			        Rectangle _BulletRect =
			            new Rectangle(
			                Convert.ToInt32( BulletLocations[_BulletIndex].X ),
			                Convert.ToInt32( BulletLocations[_BulletIndex].Y ),
			                Bullet.Width,
			                Bullet.Height
			            );

			        if( _AsteroidRect.Intersects( _BulletRect ) ) {
			            _DestroyAsteroid = true;
			            BulletLocations.RemoveAt( _BulletIndex );
			            break;
			        }
			    }

			    if( _DestroyAsteroid ) {
			        Asteroids.RemoveAt( _AsteroidIndex );
#endif
#if Section5 // Game State
			Score += 1000;
#endif
#if Section4 // Collision Detection
			    continue;
			} else {
			    if( _AsteroidRect.Intersects( _PlayerRect ) ) {
			        Asteroids.RemoveAt( _AsteroidIndex );
			        PlayerPosition =
			            new Vector2(
			                ( GraphicsDevice.Viewport.Width / 2 ) - ( Player.Width / 2 ),
			                GraphicsDevice.Viewport.Height - ( Player.Height / 2 )
			            );
#endif
#if Section5 // Game State
			if( --Lives == 0 ) {
			    GameOverTime = new TimeSpan( 0, 0, 5 );
			    CurerntState = GameStates.GameOver;
			}
#endif
#if Section4 // Collision Detection
					break;
				}
				}
			}

#endif
#if Section5 // Game State
			break;
			}
#endif

			base.Update( gameTime );
		}

		protected override void Draw( GameTime gameTime ) {
#if Section6 // Splash
			if( CurerntState == GameStates.Splash ) {
				GraphicsDevice.Clear( Color.White );
				spriteBatch.Begin();

				Vector2 TnwLocation =
			        new Vector2(
						( GraphicsDevice.Viewport.Width / 2 ) - ( TnwOps.Width / 2 ),
						( GraphicsDevice.Viewport.Height / 4 ) - ( TnwOps.Height / 2 )
					);

				Vector2 ConferenceLocation =
			        new Vector2(
						( GraphicsDevice.Viewport.Width / 2 ) - ( Conference.Width / 2 ),
						( 3 * ( GraphicsDevice.Viewport.Height / 4 ) ) - ( Conference.Height / 2 )
					);

				if( gameTime.TotalGameTime.TotalSeconds < 1 ) {
				} else if( gameTime.TotalGameTime.TotalSeconds < 2 ) {
					spriteBatch.Draw( TnwOps, TnwLocation, new Color( new Vector4( 1f, 1f, 1f, Convert.ToSingle( gameTime.TotalGameTime.TotalSeconds ) - 1f ) ) );
				} else if( gameTime.TotalGameTime.TotalSeconds < 3 ) {
					spriteBatch.Draw( TnwOps, TnwLocation, Color.White );
				} else if( gameTime.TotalGameTime.TotalSeconds < 4 ) {
					spriteBatch.Draw( TnwOps, TnwLocation, Color.White );
					spriteBatch.Draw( Conference, ConferenceLocation, new Color( new Vector4( 1f, 1f, 1f, Convert.ToSingle( gameTime.TotalGameTime.TotalSeconds ) - 3f ) ) );
				} else if( gameTime.TotalGameTime.TotalSeconds < 5 ) {
					spriteBatch.Draw( TnwOps, TnwLocation, Color.White );
					spriteBatch.Draw( Conference, ConferenceLocation, Color.White );
				} else {
					CurerntState = GameStates.Menu;
				}

				spriteBatch.End();
				base.Draw( gameTime );
				return;
			}
#endif
#if Section1 // Player
			GraphicsDevice.Clear( Color.Black );
            spriteBatch.Begin();
#else
			GraphicsDevice.Clear( Color.CornflowerBlue );
#endif

#if Section5 // Game State
			StringBuilder _Status = new StringBuilder();
			_Status.AppendLine( Score.ToString() );
			switch( CurerntState ) {
				case GameStates.Menu:
					_Status.Append( "Press A to Begin" );
					spriteBatch.DrawString( ScoreFont, _Status, Vector2.Zero, Color.Green );
					break;
				case GameStates.GameOver:
					_Status.Append( "Game Over" );
					spriteBatch.DrawString( ScoreFont, _Status, Vector2.Zero, Color.Red );
					break;
				case GameStates.Paused:
					_Status.Append( "Paused" );
					spriteBatch.DrawString( ScoreFont, _Status, Vector2.Zero, Color.Red );
					break;
				case GameStates.Playing:
					_Status.Append( Lives.ToString() );
					spriteBatch.DrawString( ScoreFont, _Status, Vector2.Zero, Color.White );
#endif
#if Section1 // Player
			spriteBatch.Draw( Player, PlayerPosition, Color.White );
#endif

#if Section2 // Bullets
			foreach( Vector2 _Bullet in BulletLocations )
				spriteBatch.Draw( Bullet, _Bullet, Color.White );
#endif

#if Section3 // Asteroids
			foreach( Asteroid _Asteroid in Asteroids )
			    spriteBatch.Draw( AsteroidTextures[_Asteroid.TextureIndex], _Asteroid.Position, Color.White );
#endif
#if Section5 // Game States
			break;
			}
#endif

#if Section1 // Player
            spriteBatch.End();
#endif
			base.Draw( gameTime );
		}
	}
}
