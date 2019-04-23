using System.Collections.Generic;
using System.Linq;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 节点
    /// </summary>
    public class Node<T> where T : new()
    {
//        private readonly List<Node<T>> _children;
//        private readonly T _data;

        public Node(T data)
        {
            Data = data;
            Children = new List<Node<T>>();
        }

        public T Data { get; set; }

        public List<Node<T>> Children { get; set; }

        public Node<T> AddChild(T data)
        {
            Children.Add(new Node<T>(data));

            return Children.Last();
        }
    }
}