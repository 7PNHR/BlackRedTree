using System;
using System.ComponentModel.Design;

namespace RBTree
{
    public enum Color
    {
        Red,
        Black
    }

    public enum Position
    {
        Right,
        Left,
        Root
    }
    public class RedBlackNode<T>
        where T : IComparable
    {
        public Position Position;
        public bool IsFict;
        public T Key;
        public RedBlackNode<T> Left, Right, Parent;
        public Color Color;
        public RedBlackNode<T> Uncle
        {
            get
            {
                var p = Parent;
                var g = p?.Parent;
                if (g == null) return null;
                return p.Position == Position.Left ? g.Right : g.Left;
            }
        }

        public RedBlackNode<T> Brother
        {
            get
            {
                var p = Parent;
                if (p == null) return null;
                return Position == Position.Left ? p.Right : p.Left;
            }
        }
        
        public RedBlackNode(T value,Position position, RedBlackNode<T> parent = null, Color color = Color.Red)//Это для не фиктивных kekw
        {
            Key = value;
            Parent = parent;
            Color = color;
            Position = position;
            Left = new RedBlackNode<T>(this,Position.Left);
            Right = new RedBlackNode<T>(this,Position.Right);
        }
        
        public RedBlackNode(RedBlackNode<T> parent,Position position) //Это для фиктивных lulw
        {
            Position = position;
            IsFict = true;
            Parent = parent;
            Color = Color.Black;
        }

        public override string ToString()
        { 
            return
                "Key = " + Key + "\n" +
                "Position = " + Position + "\n" +
                "IsFict = " + IsFict + "\n" +
                "Color = " + Color + "\n"+
                "Left key = " + (Left!=null ? Left.Key.ToString() : "null") + "\n"+
                "Right key = " + (Right!=null ? Right.Key.ToString() : "null") + "\n"+
                "Parent key = " + (Parent!=null ? Parent.Key.ToString() : "null") + "\n";
        }
    }
}