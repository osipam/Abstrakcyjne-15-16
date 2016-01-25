package zadanie1;

/**
 *
 * @author Mateusz Osipa

 *
 */
public interface DistributedModuleFactory {

    public Data CreateData();

    public Exporter CreateExporter();

    public Importer CreateImporter();
    
}
