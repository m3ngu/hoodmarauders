using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Manhattanville.PieMenu
{
    public delegate void SimpleDelegate(Object sender);

    public class PieMenuNode
    {

        #region Fields
        public PieMenuNode parent;
        private List<PieMenuNode> children;
        private Texture2D icon;
        private string text;
        private SimpleDelegate onSelect;
        private AppState state;
        #endregion

        #region Constructors
        public PieMenuNode()
        {

        }
        public PieMenuNode(string text, Texture2D icon, SimpleDelegate onSelect)
        {
            this.text = text;
            this.icon = icon;
            this.onSelect = onSelect;
        }
        public PieMenuNode(string text, Texture2D icon, SimpleDelegate onSelect, AppState state)
        {
            this.text = text;
            this.icon = icon;
            this.onSelect = onSelect;
            this.state = state;
        }
        #endregion

        #region Methods
        public void Add(PieMenuNode newChild)
        {
            if (children == null)
                children = new List<PieMenuNode>();
            newChild.parent = this;
            children.Add(newChild);
        }

        
        public bool IsLeaf
        {
            get
            {
                if (children == null)
                    return true;
                return (children.Count == 0);
            }
        }

        public void Select()
        {
            if (OnSelect != null)
                OnSelect(this);
        }
        #endregion

        #region Getters and Setters
        
        public List<PieMenuNode> Children
        {
            get { return children; }
            set { children = value; }
        }

        public Texture2D Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public SimpleDelegate OnSelect
        {
            get { return onSelect; }
            set { onSelect = value; }
        }

        public AppState State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion
    }
}
