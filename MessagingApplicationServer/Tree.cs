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
            bool isleftChild = true;
            while (current.value != key)
            {
                parent = current;
                if (key < current.value)
                {
                    isleftChild = true;
                    current = current.left;
                }
                else
                {
                    isleftChild = false;
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
                else if (isleftChild)
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
                else if (isleftChild)
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
                else if (isleftChild)
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
                else if (isleftChild)
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
    
    //public void DeleteN(int N)
    //{
    //    Node deleteNode = new Node(N);
    //    DeleteNode(top, deleteNode);
    //}
    //private Node DeleteNode(Node top, Node deleteNode)
    //{
    //    if(top == null)
    //    {
    //        return top;
    //    }
    //    if(deleteNode.value < top.value)
    //    {
    //        top.left = DeleteNode(top.left, deleteNode);
    //    }
    //    if(deleteNode.value > top.value)
    //    {
    //        top.right = DeleteNode(top.right, deleteNode);
    //    }
    //    if(deleteNode.value == top.value)
    //    {
    //        if(top.left == null && top.right == null)
    //        {
    //            top = null;
    //            return top;
    //        }else if(top.left == null)
    //        {
    //            Node temporary = top;
    //            top = top.right;
    //            temporary = null;
    //        }else if(top.right == null)
    //        {
    //            Node temporary = top;
    //            top = top.left;
    //            temporary = null;
    //        }
    //    }
    //    return top;
    //}
    public void Print(Node N,ref string newString)
        {
            //write out the tree in sorded order
            //recursive of print
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
