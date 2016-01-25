package zadanie1;

/**
 *
 * @author Mateusz Osipa

 *
 */
public class DistributedModuleTextFactory implements DistributedModuleFactory {

    private String text;

    public DistributedModuleTextFactory(String textToForFactory) {
        this.text = textToForFactory;
    }

    @Override
    public Data CreateData() {
        return new TextData(text);
    }

    @Override
    public Exporter CreateExporter() {
        return new TextExporter(text);
    }

    @Override
    public Importer CreateImporter() {
        return new TextImporter();
    }

}
