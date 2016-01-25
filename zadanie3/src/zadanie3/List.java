package zadanie3;

/**
 *
 * @author Mateusz Osipa
 *
 */
public class List {

    public static void main(String[] args) {
        TripleList<Integer> tripleList = new TripleList<>();
        int value1 = 1;
        int value2 = 2;
        int value3 = 3;
        int value4 = 4;
        int value5 = 5;
        tripleList.add(value1);
        tripleList.add(value2);
        tripleList.add(value3);
        tripleList.add(value4);
        tripleList.add(value5);

        System.out.println(tripleList.CheckLength());
    }

}
