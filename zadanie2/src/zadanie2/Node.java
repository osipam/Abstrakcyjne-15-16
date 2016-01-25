package zadanie2;

/**
 *
 * @author Mateusz Osipa
 *
 */

import java.util.ArrayList;
import java.util.Collection;

public class Node<T>  {
    
    private T value;
    private Node<T> parent;
    private Collection<Node<T>> children;
    public boolean visited;
    
    public Node(T value, Node<T> parent){
        this.value = value;
        this.parent = parent;
        children = new ArrayList<Node<T>>();
        this.visited = false;
    }
    
    public T getValue() {
        return value;
    }
    
    public Collection<Node<T>> getChildren() {
        return children;
    }
    
    public void addChild(Node<T> child){
        children.add(child);
    }
    
    public void setParent(Node<T> parent) {
        this.parent = parent;
    }
}
