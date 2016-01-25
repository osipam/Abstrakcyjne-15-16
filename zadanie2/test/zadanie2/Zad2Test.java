package zadanie2;

import java.util.ArrayList;
import org.junit.Test;
import static org.junit.Assert.*;

public class Zad2Test {

    @Test
    public void enumeratorValidation() throws Exception {
        Tree checkTreeCollectionInstance = new Tree<Integer>(
                0, EnumeratorOrder.BREADTH_FIRST_SEARCH, new ArrayList<Integer>());
        assertTrue(checkTreeCollectionInstance.getListOfElements() instanceof ArrayList);
        Tree subtree = new Tree<Integer>(5, EnumeratorOrder.BREADTH_FIRST_SEARCH);
        subtree.add(1);
        subtree.add(2);
        Tree tree = new Tree<Integer>(7, EnumeratorOrder.BREADTH_FIRST_SEARCH, new ArrayList<Integer>());
        tree.add(subtree);
        tree.add(10);
        tree.add(15);
        Integer[] bfs = new Integer[]{7, 5, 10, 15, 1, 2};
        ArrayList<Integer> maybeBfs = (ArrayList) tree.getListOfElements();
        assertEquals(bfs.length, maybeBfs.size());
        for (int i = 0; i < bfs.length; i++) {
            assertEquals(bfs[i], maybeBfs.get(i));
        }
        tree.setOrder(EnumeratorOrder.DEPTH_FIRST_SEARCH);
        Integer[] dfs = new Integer[]{7, 5, 1, 2, 10, 15};
        ArrayList<Integer> maybeDfs = (ArrayList) tree.getListOfElements();
        assertEquals(dfs.length, maybeDfs.size());
        for (int i = 0; i < dfs.length; i++) {
            assertEquals(dfs[i], maybeDfs.get(i));
        }
    }

    @Test
    public void enumerateWithNoChildren() throws Exception {
        Tree tree = new Tree<Integer>(7, EnumeratorOrder.DEPTH_FIRST_SEARCH, new ArrayList<Integer>());
        assertEquals(7, ((ArrayList) tree.getListOfElements()).get(0));
        tree.setOrder(EnumeratorOrder.BREADTH_FIRST_SEARCH);
        assertEquals(7, ((ArrayList) tree.getListOfElements()).get(0));
        Tree subtree = new Tree<Integer>(5, EnumeratorOrder.BREADTH_FIRST_SEARCH);
        tree.add(subtree);
        assertEquals(5, ((ArrayList) tree.getListOfElements()).get(tree.getListOfElements().size() - 1));
        tree.setOrder(EnumeratorOrder.DEPTH_FIRST_SEARCH);
        assertEquals(5, ((ArrayList) tree.getListOfElements()).get(tree.getListOfElements().size() - 1));
    }

    @Test
    public void orderPropertyValidation() throws Exception {
        Tree subtree = new Tree<Integer>(5, EnumeratorOrder.DEPTH_FIRST_SEARCH);
        subtree.add(1);
        subtree.add(2);
        Tree tree = new Tree<Integer>(7, EnumeratorOrder.BREADTH_FIRST_SEARCH);
        tree.add(subtree);
        assertEquals(EnumeratorOrder.BREADTH_FIRST_SEARCH, subtree.getOrder());
        assertEquals(EnumeratorOrder.BREADTH_FIRST_SEARCH, tree.getChildren(5).getOrder());
        subtree.add(3);
        assertEquals(EnumeratorOrder.BREADTH_FIRST_SEARCH, tree.getChildren(3).getOrder());
        tree.setOrder(EnumeratorOrder.DEPTH_FIRST_SEARCH);
        subtree.add(4);
        assertEquals(EnumeratorOrder.DEPTH_FIRST_SEARCH, tree.getChildren(4).getOrder());
        try {
            tree.setOrder(123);
            fail("Unknown order type defined.");
        } catch (Exception e) {
        }
        tree.display();
    }
}
