using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


// Adapted to be compatible with GoblinXNA from an XNA pie menu implementation
// by Catalin Zima  http://www.catalinzima.com/?page_id=15

namespace Manhattanville.PieMenu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PieMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private PieMenuNode    rootNode;
        private PieMenuNode    newMenuNode;
        private int            selectionIndex = -1;
        private float          radius = 150;
        private Transition     t;
        private Vector2        drawPosition;
        private SimpleDelegate hideDelegate;
        private SimpleDelegate newMenuDelegate;
        private SpriteFont     spriteFont;

        private SoundEffect whooshup;
        private String whooshupName = "whooshup";
        private SoundEffect whooshdown;
        private String whooshdownName = "whooshdown";


        #region Constructor and Base Methods

        public PieMenu(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = false;
            this.DrawOrder = 200;
            
            game.Components.Add(this);

            // TODO: Construct any child components here
            t = new Transition(Direction.Ascending, TransitionCurve.Linear, 0.3f);
            hideDelegate = new SimpleDelegate(this.OnHide);
            newMenuDelegate = new SimpleDelegate(this.NewMenu);

            ContentManager contentManager = new ContentManager(game.Services, @"Content\Sounds\");
            whooshup = contentManager.Load<SoundEffect>(whooshupName);
            whooshdown = contentManager.Load<SoundEffect>(whooshdownName);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            // Start off disabled/non-visible
            base.Initialize();
        }

        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Load any content
            
            // Create a new SpriteBatch, which can be used to draw textures. 
            spriteFont = Game.Content.Load<SpriteFont>("Fonts\\UIFont");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);

            // TODO: Add your drawing code here
            Vector2 center = drawPosition;

            float scale = t.CurrentPosition;
            //float scale = 1.0f;

            GoblinXNA.UI.UI2D.UI2DRenderer.DrawCircle((int)center.X, (int)center.Y, (int)(scale * (radius - 30.0f)), Color.White);
            GoblinXNA.UI.UI2D.UI2DRenderer.DrawCircle((int)center.X, (int)center.Y, (int)(scale * (radius + 30.0f)), Color.White);

            float currentAngle = MathHelper.PiOver2;

            float angleIncrement = MathHelper.TwoPi / rootNode.Children.Count;
            for (int i = 0; i < rootNode.Children.Count; i++)
            {
                Vector2 imagePos = center + scale * radius * new Vector2((float)Math.Cos(currentAngle), -(float)Math.Sin(currentAngle));

                //int imageSize = (int)(scale * 45.0f);
                int imageHeight = (int)(scale * 58.0f);
                int imageWidth = (int)(scale * 81.0f);
                Rectangle destinationRect = new Rectangle(
                    (int)imagePos.X - imageWidth,
                    (int)imagePos.Y - imageHeight,
                    2 * imageWidth,
                    2 * imageHeight);
                //Rectangle destinationRect = new Rectangle(
                //    (int)imagePos.X - imageSize,
                //    (int)imagePos.Y - imageSize,
                //    2 * imageSize,
                //    2 * imageSize);

                //GoblinXNA.UI.Notifier.AddMessage(destinationRect.ToString());

                Color drawColor = Color.White;
                float testCAngle = currentAngle;
                if (currentAngle <= 0)
                    currentAngle += MathHelper.TwoPi;

                if (i == selectionIndex)
                {
                    drawColor = Color.Red;
                    GoblinXNA.UI.UI2D.UI2DRenderer.WriteText  (
                        imagePos + new Vector2(-spriteFont.MeasureString(rootNode.Children[i].Text).X / 2, imageHeight),
                        //imagePos + new Vector2(-spriteFont.MeasureString(rootNode.Children[i].Text).X / 2, imageSize),
                        rootNode.Children[i].Text,
                        Color.White,
                        spriteFont,
                        GoblinXNA.GoblinEnums.HorizontalAlignment.Center,
                        GoblinXNA.GoblinEnums.VerticalAlignment.Center
                    ); 
                }

                GoblinXNA.UI.UI2D.UI2DRenderer.FillRectangle(destinationRect, rootNode.Children[i].Icon, drawColor);

                currentAngle -= angleIncrement;

            }

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            t.Update(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }
        
        #endregion

        #region Custom methods
        
        private void OnHide(Object sender)
        {
            //whooshdown.Play();
            this.Visible = false;
            this.Enabled = false;
            t.OnTransitionEnd = null;
        }
        
        private void NewMenu(Object sender)
        {
            whooshup.Play();
            rootNode = newMenuNode;
            this.selectionIndex = -1;
            t.Reset(Direction.Ascending);
            t.OnTransitionEnd = null;
        }

        private void ChangeTo(PieMenuNode newNode)
        {
            if (newNode == null)
            {
                whooshdown.Play();
                t.OnTransitionEnd = hideDelegate;
                t.Reset(Direction.Descending);
            }
            else
            {
                whooshdown.Play();
                t.OnTransitionEnd = newMenuDelegate;
                newMenuNode = newNode;
                t.Reset(Direction.Descending);
            }

        }

        public void Show(Vector2 position)
        {
            if (this.Visible) return;

            whooshup.Play();
            t.Reset(Direction.Ascending);
            t.OnTransitionEnd = null;
            this.Visible = true;
            this.Enabled = true;
            drawPosition = position;
        }
        
        public void Show(PieMenuNode rootNode, Vector2 position)
        {
            this.rootNode = rootNode;
            this.selectionIndex = -1;
            Show(position);
        }

        protected void ComputeSelected(Vector2 selectionVector)
        {
            selectionIndex = -1;
            /*
            if (selectionVector.Length() > 3.0f)
                return;
            
            if (selectionVector.Length() > 0.3f)
            {*/
                float angleDivision = 1.0f / rootNode.Children.Count;

                float angle = (float)Math.Atan2(-selectionVector.Y, selectionVector.X);
                
                if (angle < 0.0f)
                    angle += MathHelper.TwoPi; //now angle is between 0 and TwoPi

                angle /= MathHelper.TwoPi;
                angle = 1.0f - angle;

                float rotationBegins = 0.75f - angleDivision / 2.0f;

                if (angle <= rotationBegins)
                    angle += 1.0f;

                angle -= rotationBegins;

                selectionIndex = 0;
                
                while (selectionIndex * angleDivision < angle)
                {
                    selectionIndex++;
                }
                
                selectionIndex--;

            //}

        }

        public bool HandleInput(Vector2 selectionVector)
        {
            if (!(Visible && Enabled))
                return false;

            ComputeSelected(selectionVector);

            if (selectionIndex >= 0)
            {
                if (rootNode.Children[selectionIndex].IsLeaf)
                {
                    rootNode.Children[selectionIndex].Select();
                    Hide();
                }
                else
                    ChangeTo(rootNode.Children[selectionIndex]);
            }
            else
                ChangeTo(rootNode.parent);

            return true;
        }

        public void Hide()
        {
            if (!this.Visible) return;

            ChangeTo(null);
        }

        public void Back()
        {
            if (!this.Visible) return;

            ChangeTo(rootNode.parent);
        }

        #endregion

        #region Getters and Setters

        public PieMenuNode RootNode
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Vector2 Position
        {
            get { return drawPosition; }
            set { drawPosition = value; }
        }

        #endregion
    }
}