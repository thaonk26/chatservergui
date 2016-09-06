using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingApplicationServer
{
    class Tree
    {
        Node top;
        public Tree()
        {
            top = null;
        }
        //public Tree(int initial)
        //{
        //    top = new Node(initial);
        //}
        public void Add(int value)
        {
            if(top == null) //tree is empty
            {
                Node NewNode = new Node(value);
                top = NewNode;
                return;
            }
            Node currentNode = top;
            bool added = false;
            do
            {
                if(value < currentNode.value)
                {
                    if(currentNode.left == null)
                    {
                        Node newNode = new Node(value);
                        currentNode.left = newNode;
                        added = true;
                    }
                    else
                    {
                        currentNode = currentNode.left;
                    }
                }
                if (value >= currentNode.value)
                {
                    if(currentNode.right == null)
                    {
                        Node newNode = new Node(value);
                        currentNode.right = newNode;
                        added = true;
                    }else
                    {
                        currentNode = currentNode.right;
                    }
                }
            } while (!added);
        }
        public void AddRecursive(int value)
        {
            AddRecursiveValue(ref top, value);
        }
        private void AddRecursiveValue(ref Node N, int value)
        {
            if(N == null)
            {
                Node newNode = new Node(value);
                N = newNode;
                return;
            }
            if (value < N.value)
            {
                AddRecursiveValue(ref N.left, value);
                return;
            }
            if (value >= N.value)
            {
                AddRecursiveValue(ref N.right, value);
                return;
            }
        }
        public Node GetSuccessor(Node delNode)
        {
            Node successorParent = delNode;
            Node successor = delNode;
            Node current = delNode.right;

            while (current != null)
            {
                successorParent = current;
                successor = current;
                current = current.left;
            }
            if (successor != delNode.right)
            {
                successorParent.left = successor.right;
                successor.right = delNode.right;
            }
            return successor;
        }

        public bool Delete(int key)
        {
            Node current = top;
            Node parent = top;
            bool isLeftChild = true;
            while (current.value != key)
            {
                parent = current;
                if (key < current.value)
                {
                    isLeftChild = true;
                    current = current.left;
                }
                else
                {
                    isLeftChild = false;
                    current = current.right;
                }
                if (current == null)
                {
                    return false;
                }
            }

            if (current.left == null && current.right == null)
            {
                if (current == top)
                {
                    top = null;
                }
                else if (isLeftChild)
                {
                    parent.left = null;
                }
                else
                {
                    parent.right = null;
                }
            }
            else if (current.right == null)
            {
                if (current == top)
                {
                    top = current.left;
                }
                else if (isLeftChild)
                {
                    parent.left = current.left;
                }
                else
                {
                    parent.right = current.right;
                }
            }
            else if (current.left == null)
            {
                if (current == top)
                {
                    top = current.right;
                }
                else if (isLeftChild)
                {
                    parent.left = parent.right;
                }
                else
                {
                    parent.right = current.right;
                }
            }
            else
            {
                Node successor = GetSuccessor(current);
                if (current == top)
                {
                    top = successor;
                }
                else if (isLeftChild)
                {
                    parent.left = successor;
                }
                else
                {
                    parent.right = successor;
                }
                successor.left = current.left;
            }
            return true;
        }
    
  
    public void Print(Node N,ref string newString)
        {
            if(N == null) { N = top; }
            if(N.left != null)
            {
                Print(N.left, ref newString);
                newString = newString + N.value.ToString().PadLeft(2);
            }
            else
            {
                newString = newString + N.value.ToString().PadRight(3);
            }
            if(N.right != null) { Print(N.right, ref newString); }
        }
    }
}
