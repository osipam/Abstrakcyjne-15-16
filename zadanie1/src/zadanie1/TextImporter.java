package zadanie1;

/**
 *
 * @author Mateusz Osipa
 *
 */
public class TextImporter implements Importer {

    private Data data;

    public void ImportData(Data data) {
        this.data = data;
    }

    public String ImportedText() {
        return this.data.Text();
    }

}
