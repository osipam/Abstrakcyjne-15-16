package zadanie1;

/**
 *
 * @author Mateusz Osipa

 *
 */
public class TextExporter implements Exporter {

    private Data data;
    
    public Data ExportData() {
        if (data == null) {
            return new TextData("");
        }
        Data exportedData = data;
        data = null;
        return exportedData;
    }

    public TextExporter(String textToBeExported) {
        this.data = new TextData(textToBeExported);
    }

}
