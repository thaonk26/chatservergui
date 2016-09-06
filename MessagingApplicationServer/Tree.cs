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
        public Tree(int initial)
        {
            top = new Node(initial);
        }
        public void Add(int value)
        {
            //non-recursive add
            if(top == null) //tree is empty
            {
                //add item as the base node
                Node NewNode = new Node(value);
                top = NewNode;
                return;
            }
            Node currentNode = top;
            bool added = false;
            do
            {
                //traverse tree
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
            //recursive add
            AddRecursiveValue(ref top, value);
        }
        private void AddRecursiveValue(ref Node N, int value)
        {
            //private recursive search for where to add the new node
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
        public void Print(Node N,ref string newString)
        {
            //write out the tree in sorded order
            //recursive of print
            if(N == null) { N = top; }
            if(N.left != null)
            {
                Print(N.left, ref newString);
                newString = newString + N.value.ToString().PadLeft(3);
            }
            else
            {
                newString = newString + N.value.ToString().PadRight(3);
            }
            if(N.right != null) { Print(N.right, ref newString); }
        }
    }
}
