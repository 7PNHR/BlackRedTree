using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBTree
{
    public class RedBlackTree<T>
        where T : IComparable
    {
        private RedBlackNode<T> _root;
        
        #region AddNode
        public void Add(T key)
        {
            if (_root == null)
                _root = new RedBlackNode<T>(key, color: Color.Black, position: Position.Root);
            else
                FindNewPlace(_root, key);
        }

        private void FindNewPlace(RedBlackNode<T> node, T key)
        {
            if (node.Key.CompareTo(key) <= 0)
                if (node.Right.IsFict)
                    AddNewNode(node, key, Position.Right);
                else FindNewPlace(node.Right, key);
            else if (node.Left.IsFict)
                AddNewNode(node, key, Position.Left);
            else FindNewPlace(node.Left, key);
        }

        private void AddNewNode(RedBlackNode<T> node, T key, Position pos)
        {
            if (pos == Position.Left)
            {
                node.Left = new RedBlackNode<T>(key, parent: node, color: Color.Red, position: Position.Left);
                CheckRed(node.Left);
            }
            else
            {
                node.Right = new RedBlackNode<T>(key, parent: node, color: Color.Red, position: Position.Right);
                CheckRed(node.Right);
            }
        }

        private void CheckRed(RedBlackNode<T> node)
        {
            if (node.Position == Position.Root) node.Color = Color.Black;
            //Это если надо просто покрасить корень
            else if (node.Color == Color.Red && node.Parent.Color == Color.Red)
                if (node.Uncle.Color == Color.Red)
                    SimpleRepaint(node.Parent.Parent);
                else RepaintAndRotateNodes(node);
        }
        
        #endregion
        
        #region Repaint
        
        private void SimpleRepaint(RedBlackNode<T> grandFather)
        {
            //Case 1 - просто перекраска бати и дяди
            grandFather.Left.Color = Color.Black;
            grandFather.Right.Color = Color.Black;
            grandFather.Color = Color.Red;
            CheckRed(grandFather);
        }
        
        private void RepaintAndRotateNodes(RedBlackNode<T> node)
        {
            //Case2 одна из красных левая, другая правая или наооборот
            if (node.Position != node.Parent.Position)
            {
                if (node.Position == Position.Right)
                {
                    LeftRotation(node);
                    node = node.Left;
                }
                else
                {
                    RightRotation(node);
                    node = node.Right;
                }
            }

            //Case 3 - обе красные либо левые либо правые
            node.Parent.Color = Color.Black;
            node.Parent.Parent.Color = Color.Red;
            if (node.Position == Position.Right)
                LeftRotation(node.Parent);
            else
                RightRotation(node.Parent);
        }
        #endregion
        
        #region Rotation
        private void RightRotation(RedBlackNode<T> newRoot)
        {
            var newRight = newRoot.Parent;
            if (newRight == _root)
            {
                _root = newRoot;
                _root.Parent = null;
                newRoot.Position = Position.Root;
            }
            else
            {
                var grand = newRight.Parent;
                newRoot.Parent = grand;
                newRoot.Position = newRight.Position;
                if (newRoot.Position == Position.Right) grand.Right = newRoot;
                else grand.Left = newRoot;
            }

            var node = newRoot.Right;
            newRoot.Right = newRight;
            newRoot.Right.Position = Position.Right;
            newRight.Parent = newRoot;
            newRight.Left = node;
            newRight.Left.Position = Position.Left;
            newRight.Right.Position = Position.Right;
            node.Parent = newRight;
        }
        
        private void LeftRotation(RedBlackNode<T> newRoot)
        {
            var newLeft = newRoot.Parent;
            if (newLeft == _root)
            {
                _root = newRoot;
                _root.Parent = null;
                newRoot.Position = Position.Root;
            }
            else
            {
                var grand = newLeft.Parent;
                newRoot.Parent = grand;
                newRoot.Position = newLeft.Position;
                if (newRoot.Position == Position.Left) grand.Left = newRoot;
                else grand.Right = newRoot;
            }

            var node = newRoot.Left;
            newRoot.Left = newLeft;
            newRoot.Left.Position = Position.Left;
            newLeft.Parent = newRoot;
            newLeft.Right = node;
            newLeft.Right.Position = Position.Right;
            newLeft.Left.Position = Position.Left;
            node.Parent = newLeft;
        }
        
        #endregion

        public void Delete(T key)
        {
            if(_root==null) return;
            var node = Find(key);
            if(node==null) return;
            if(node == _root && node.Left.IsFict && node.Right.IsFict) _root = null;
            else if(node.Color==Color.Red)
                DeleteRedNode(node);
            else DeleteBlackdNode(node);
        }
        
        private void Delete(RedBlackNode<T> node)
        {
            if (node == _root && node.Left.IsFict && node.Right.IsFict) _root = null;
            else if(node.Color==Color.Red)
                DeleteRedNode(node);
            else DeleteBlackdNode(node);
        }

        private void DeleteRedNode(RedBlackNode<T> node)
        {
            if (NodeIsLeaf(node))
                DeleteLeaf(node);
            else DeleteNodeWithTwoChilds(node);
        }
        
        private void DeleteLeaf(RedBlackNode<T> node)
        {
            if (node.Position == Position.Left)
                node.Parent.Left = new RedBlackNode<T>(node.Parent,Position.Left);
            else node.Parent.Right = new RedBlackNode<T>(node.Parent,Position.Right);
        }
        
        private void DeleteNodeWithTwoChilds(RedBlackNode<T> node)//У нас может быть либо лист красный либо нода с 2-мя поддеревьями
        {
            //Тут есть выбор между наибольшим левым и минимальным правым, мы выбираем правый путь - путь истины
            var newNode = Min(node.Right);
            var key = node.Key;
            node.Key = newNode.Key;
            newNode.Key = key;
            Delete(newNode);
        }        
        
        private bool NodeIsLeaf(RedBlackNode<T> node) => node.Left.IsFict && node.Right.IsFict;

        private bool BlackNodeHaveOneChild(RedBlackNode<T> node) =>
            node.Left.IsFict || node.Right.IsFict;
        
        private void DeleteBlackdNode(RedBlackNode<T> node)
        {
            if (NodeIsLeaf(node))
            {
                DeleteLeaf(node);
                BalanceTree(node.Parent);
            }
            else if (BlackNodeHaveOneChild(node))
                DeleteBlackNodeWithOnlyOneChild(node);
            else DeleteNodeWithTwoChilds(node);
        }
        private void DeleteBlackNodeWithOnlyOneChild(RedBlackNode<T> node)
        {
            if(node.Left.IsFict)
            {
                var rightNode = node.Right;
                node.Key = rightNode.Key;
                node.Right = new RedBlackNode<T>(node,Position.Right);
            }
            else
            {
                var leftNode = node.Left;
                node.Key = leftNode.Key;
                node.Left = new RedBlackNode<T>(node,Position.Left);
            }
        }

        private void BalanceTree(RedBlackNode<T> parent)
        {
            var deletedNode = !parent.Left.IsFict? parent.Right : parent.Left;
            if(deletedNode.Position==Position.Left)
                Balance(deletedNode);
            else Balance(deletedNode);
        }
        //Если мы удалили черную вершину в правом поддереве

        private void Balance(RedBlackNode<T> deletedNode) //теперь она нил
        {
            if (deletedNode.Parent.Parent == null) deletedNode.Brother.Color=Color.Red;
            else if (deletedNode.Parent.Color == Color.Red)
                RedBlackBalance(deletedNode);
            else if (deletedNode.Parent.Color == Color.Black && deletedNode.Brother.Color == Color.Red)
                BlackRedBalance(deletedNode);
            else BlackBlackBalance(deletedNode);
        }

        private void RedBlackBalance(RedBlackNode<T> deletedNode)//ситуации 1 и 2
        {
            var son = deletedNode.Brother;
            if (son.Left.Color == Color.Red && son.Position==Position.Left || 
                son.Right.Color == Color.Red && son.Position==Position.Right)//ситуация 2
            {
                deletedNode.Brother.Color = Color.Red;
                if (son.Position == Position.Left) son.Left.Color = Color.Black;
                else son.Right.Color = Color.Black;
                son.Parent.Color = Color.Black;
                if(son.Position==Position.Left) RightRotation(son);
                else LeftRotation(son);
            }
            else//ситуация 2
            {
                son.Parent.Color = Color.Black;
                deletedNode.Brother.Color = Color.Red;
            }
        }

        private void BlackRedBalance(RedBlackNode<T> deletedNode)//ситуации 3 и 4
        {
            var son = deletedNode.Brother;
            if (son.Position == Position.Left && son.Right.Left.Color == Color.Red
                || son.Position == Position.Right && son.Left.Right.Color == Color.Red)//ситуация 4
            {
                if (son.Position == Position.Left)
                {
                    son.Right.Left.Color = Color.Black;
                    LeftRotation(son.Right);
                    RightRotation(deletedNode.Brother);
                }
                else
                {
                    son.Left.Right.Color = Color.Black;
                    RightRotation(son.Left);
                    LeftRotation(deletedNode.Brother);
                }
            }
            else // ситуация 3
            {
                deletedNode.Brother.Color = Color.Black;
                if (son.Position == Position.Left)
                {
                    son.Right.Color = Color.Red;
                    RightRotation(son);
                }
                else
                {
                    son.Left.Color = Color.Red;
                    LeftRotation(son);
                }
            }
        }

        private void BlackBlackBalance(RedBlackNode<T> deletedNode)//ситуации 5 и 6
        {
            var son = deletedNode.Brother;
            if (son.Right.Color == Color.Red || son.Left.Color == Color.Red)//5
            {
                if (son.Right.Color == Color.Red)
                {
                    son.Right.Color = Color.Black;
                    LeftRotation(son.Right);
                }
                else
                {
                    son.Left.Color = Color.Black;
                    RightRotation(son.Left);
                }
                RightRotation(deletedNode.Brother);
            }
            else
            {
                deletedNode.Brother.Color = Color.Red;
                Balance(deletedNode.Parent);
            }
        }
        
        #region Min/Max

        public RedBlackNode<T> Min() => _root != null ? Min(_root) : _root;
        private RedBlackNode<T> Min(RedBlackNode<T> startNode) => startNode.Left.IsFict != true ? Min(startNode.Left) : startNode;
        
        public RedBlackNode<T> Max() => _root != null ? Max(_root) : _root;
        private RedBlackNode<T> Max(RedBlackNode<T> startNode) => startNode.Right.IsFict != true ? Max(startNode.Right) : startNode;

        #endregion

        #region Find

        #region Find

        public RedBlackNode<T> Find(T key) //Находит вершину, зная ключ
        {
            var node = _root;
            return node == null ? null : Find(node, key);
        }

        private RedBlackNode<T> Find(RedBlackNode<T> node,T key) =>
            node == null ? null 
            : node.Key.CompareTo(key) == 0 ? node 
            : Find(node.Key.CompareTo(key) < 0 ? node.Right 
                : node.Left, key);
        
        #endregion

        #region FindNext

        public RedBlackNode<T> FindNext(T key)
        {
            if (_root == null) return null;
            var current = Find(key);
            return current.Right.IsFict != true ? GetRightBranchMinKey(current) : MySecondMethod(current);
        }

        private RedBlackNode<T> GetRightBranchMinKey(RedBlackNode<T> node)
        {
            node = node.Right;
            return Min(node);
        }

        private RedBlackNode<T> MySecondMethod(RedBlackNode<T> node) //Доделать
        {
            while (node.Parent != null && node.Parent.Key.CompareTo(node.Key) < 0)
                node = node.Parent;
            return node.Parent;
        }

        #endregion

        #region FindPrev

        public RedBlackNode<T> FindPrev(T key)
        {
            if (_root == null) return null;
            var current = Find(key);
            return current.Left.IsFict != true ? GetLeftBranchMaxKey(current) : MyThirdMethod(current);
        }

        private RedBlackNode<T> GetLeftBranchMaxKey(RedBlackNode<T> node)
        {
            node = node.Left;
            return Max(node);
        }

        private RedBlackNode<T> MyThirdMethod(RedBlackNode<T> node) //Доделать
        {
            while (node.Parent != null && node.Parent.Key.CompareTo(node.Key) > 0)
                node = node.Parent;
            return node.Parent;
        }

        #endregion

        #endregion

        #region Print

        /// <summary>
        /// Печатает графическое представление дерева
        /// </summary>
        public void Print()
        {
            var str = ToString();
            if (str == null)
            {
                Console.WriteLine("Дерево пустое");
                return;
            }
            foreach (var e in str.Split('\n').Where(x => x != ""))
            {
                var line = "";
                var token = e.Split(' ');
                if (token[^1] == "Red")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    line = e.Substring(0, e.Length - 3);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    line = e.Substring(0, e.Length - 5);
                }

                Console.WriteLine(line);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Возвращает графическое представление дерева (Идея взята у компании Microsoft)
        /// </summary>
        public override string ToString()
        {
            if (_root == null) return null;
            var str = new StringBuilder(_root.Key + "  " + _root.Color + "\n");
            var stack = new Stack<Tuple<RedBlackNode<T>, bool>>();
            var isTwoBranches = true;
            if (_root.Right != null)
            {
                stack.Push(Tuple.Create(_root.Right, true));
                isTwoBranches = false;
            }

            if (_root.Left != null) stack.Push(Tuple.Create(_root.Left, isTwoBranches));
            while (stack.Count > 0)
            {
                isTwoBranches = true;
                var node = stack.Pop();
                if (node.Item1.Right != null)
                {
                    stack.Push(Tuple.Create(node.Item1.Right, true));
                    isTwoBranches = false;
                }

                if (node.Item1.Left != null) stack.Push(Tuple.Create(node.Item1.Left, isTwoBranches));
                str.Append(MakeString(node.Item1, node.Item2));
            }

            return str.ToString();
        }

        private string MakeString(RedBlackNode<T> node, bool isEnd)
        {
            var nodeParent = node.Parent.Parent;
            var previousNode = node.Parent;
            var line = "──" + (isEnd ? "└" : "├");
            while (nodeParent != null)
            {
                if (nodeParent.Left == null) line += "   ";
                else if (nodeParent.Right == null) line += "   ";
                else if (nodeParent.Left == previousNode)
                    line += "  |";
                else line += "   ";
                nodeParent = nodeParent.Parent;
                previousNode = previousNode.Parent;
            }

            var result = line.Reverse().Aggregate("", (current, e) => current + e);
            return result + (!node.IsFict ? node.Key.ToString() : "nil") + "  " + node.Color + "\n";
        }

        #endregion
        
    }
}// RedBLackNode<T> node

/*
 #region Right
        private void RightBalance(RedBlackNode<T> parent, RedBlackNode<T> son)
        {
            var leftGrandSon = son.Left;
            var rightGrandSon = son.Right;
            if (parent.Color == Color.Red)
                RightBalanceIfParentIsRed(parent, son, leftGrandSon, rightGrandSon);
            else
                RightBalanceIfParentIsBlack(parent, son, leftGrandSon, rightGrandSon);
        }

        private void RightBalanceIfParentIsRed(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (son.Color == Color.Black)
                RightBalanceIfSonIsBlack(parent,son,leftGrandSon,rightGrandSon);
            else
                RightBalanceIfSonIsRed(parent,son,leftGrandSon,rightGrandSon);
        }

        private void RightBalanceIfSonIsBlack(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (leftGrandSon.Color == Color.Black)
            {
                parent.Left.Color = Color.Black;
                son.Parent.Color = Color.Red;
            }
            else
            {
                son.Left.Color = Color.Black;
                son.Parent.Color = Color.Black;
                parent.Left.Color = Color.Red;
                RightRotation(son);
            }
        }
        
        private void RightBalanceIfSonIsRed(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if(rightGrandSon.Left.Color==Color.Black)
            {
                parent.Left.Color = Color.Black;
                son.Right.Color = Color.Red;
                RightRotation(son);
            }
            else
            {
                rightGrandSon.Left.Color = Color.Black;
                LeftRotation(rightGrandSon);
                RightRotation(parent.Left);
            }
        }
        
        private void RightBalanceIfParentIsBlack(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (rightGrandSon.Color == Color.Red)
            {
                son.Right.Color = Color.Black;
                LeftRotation(rightGrandSon);
                RightRotation(parent.Left);
            }
            else
            {
                parent.Left.Color = Color.Red;
                if (parent.Position == Position.Right)
                    RightBalance(parent.Parent, parent);
                else LeftBalance(parent.Parent, parent);
            }
        }
        
        #endregion
        
        #region Left
        private void LeftBalance(RedBlackNode<T> parent, RedBlackNode<T> son)
        {
            var leftGrandSon = son.Left;
            var rightGrandSon = son.Right;
            if (parent.Color == Color.Red)
                LeftBalanceIfParentIsRed(parent, son, leftGrandSon, rightGrandSon);
            else
                LeftBalanceIfParentIsBlack(parent, son, leftGrandSon, rightGrandSon);
        }
        
        private void LeftBalanceIfParentIsRed(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (son.Color == Color.Black)
                LeftBalanceIfSonIsBlack(parent,son,leftGrandSon,rightGrandSon);
            else
                LeftBalanceIfSonIsRed(parent,son,leftGrandSon,rightGrandSon);
        }

        private void LeftBalanceIfSonIsBlack(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (rightGrandSon.Color == Color.Black)
            {
                parent.Right.Color = Color.Red;
                son.Parent.Color = Color.Black;
            }
            else
            {
                son.Right.Color = Color.Black;
                son.Parent.Color = Color.Black;
                parent.Right.Color = Color.Black;
                LeftRotation(son);
            }
        }
        
        private void LeftBalanceIfSonIsRed(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if(leftGrandSon.Left.Color==Color.Black)
            {
                parent.Right.Color = Color.Black;
                son.Left.Color = Color.Red;
                LeftRotation(son);
            }
            else
            {
                leftGrandSon.Right.Color = Color.Black;
                RightRotation(leftGrandSon);
                LeftRotation(parent.Right);
            }
        }
        
        private void LeftBalanceIfParentIsBlack(RedBlackNode<T> parent, RedBlackNode<T> son, RedBlackNode<T> leftGrandSon,
            RedBlackNode<T> rightGrandSon)
        {
            if (leftGrandSon.Color == Color.Red)
            {
                son.Left.Color = Color.Black;
                RightRotation(leftGrandSon);
                LeftRotation(parent.Right);
            }
            else
            {
                parent.Right.Color = Color.Red;
                if (parent.Position == Position.Left)
                    LeftBalance(parent.Parent, parent);
                else RightBalance(parent.Parent, parent);
            }
        }
        #endregion

 */