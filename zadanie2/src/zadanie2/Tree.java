package zadanie2;

/**
 *
 * @author Mateusz Osipa
 *
 */

import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedList;
import java.util.Queue;

public class Tree<T> {

    private Node<T> root;
    private EnumeratorOrder enumeratorOrder;
    private Collection<T> values;
    private Integer iEnumeratorOrder;
    private Node<T> tempNode;

    public Tree(T value, Integer order, Collection<T> collection) {
        root = new Node<T>(value, null);
        this.iEnumeratorOrder = order;
        values = collection;
    }

    public Tree(T value, Integer order) {
        root = new Node<T>(value, null);
        this.iEnumeratorOrder = order;
    }

    public Tree() {
        root = new Node<T>(null, null);
    }

    public Node<T> getRoot() {
        return root;
    }

    public void getNode(T value, Node<T> node) {
        if (value.equals(node.getValue())) {
            tempNode = node;
        }
        if (tempNode != null) {
            return;
        }
        for (Node<T> child : node.getChildren()) {
            getNode(value, child);
        }
    }

    public Tree<T> getChildren(T value) {
        tempNode = null;
        getNode(value, root);
        if (tempNode == null) {
            return new Tree<T>();
        }
        Tree<T> tree = new Tree<T>(tempNode.getValue(), iEnumeratorOrder);
        for (Node<T> child : tempNode.getChildren()) {
            tree.add(child);
        }
        return tree;
    }
    
    public void display() {
        display(root);
    }

    public void display(Node<T> root) {
        System.out.print(root.getValue() + " | ");
        if (root.getChildren() == null) {
            return;
        }
        for (Node<T> n : root.getChildren()) {
            display(n);
        }
    }

    public void bfs(Node<T> root) {

        Queue<Node<T>> queue = new LinkedList<Node<T>>();

        if (root == null) {
            return;
        }

        root.visited = true;
        queue.add(root);
        while (!queue.isEmpty()) {
            Node<T> r = queue.remove();
            values.add(r.getValue());
            for (Node<T> n : r.getChildren()) {
                if (n.visited == false) {
                    queue.add(n);
                    n.visited = true;
                }
            }
        }
    }

    public void dfs(Node<T> root) {

        if (root == null) {
            return;
        }
        root.visited = true;
        values.add(root.getValue());
        if (root.getChildren() == null) {
            return;
        }
        for (Node<T> n : root.getChildren()) {
            if (n.visited == false) {
                dfs(n);
            }

        }
    }

    public void cleanVisitedState(Node<T> root) {

        if (root.getChildren() == null) {
            return;
        }
        for (Node<T> n : root.getChildren()) {
            n.visited = false;
            cleanVisitedState(n);
        }

    }

    public Collection<T> getListOfElements() {
        if (values == null) {
            values = new ArrayList<T>();
        }
        if (iEnumeratorOrder.equals(EnumeratorOrder.BREADTH_FIRST_SEARCH)) {
            values.removeAll(values);
            cleanVisitedState(root);
            bfs(root);
            return values;
        } else {
            values.removeAll(values);
            cleanVisitedState(root);
            dfs(root);
            return values;
        }
    }

    public void add(Tree<T> subtree) throws Exception {
        subtree.getRoot().setParent(root);
        subtree.setOrder(iEnumeratorOrder);
        root.addChild(subtree.getRoot());
    }

    public void add(Node<T> childNode) {
        root.addChild(childNode);
    }

    public void add(T value) {
        Node<T> child = new Node<T>(value, null);
        child.setParent(root);
        this.add(child);
    }

    public Integer getOrder() {
        return iEnumeratorOrder;
    }

    public void setOrder(Integer order) throws Exception {
        if (order.equals(EnumeratorOrder.BREADTH_FIRST_SEARCH)
                || order.equals(EnumeratorOrder.DEPTH_FIRST_SEARCH)) {
            this.iEnumeratorOrder = order;
        } else {
            throw new Exception();
        }
    }
    
}
